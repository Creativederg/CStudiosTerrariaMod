using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CStudios.Content.DamageClasses;
using CStudios.Content.Buffs;
using CStudios.Content.NPCs;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Projectiles.Summon.Psybits
{
    // Continuous beam fired by a Psybits minion
    // ai[1] = parent Psybits projectile whoAmI
    // localAI[1] = current target NPC whoAmI (-1 if none)
    public class PsybitMinionBeam : ModProjectile
    {
        private const float MOVE_DISTANCE = 8f;
        private const float MAX_RANGE = 600f;

        public float Distance
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public int ParentIdentity => (int)Projectile.ai[1];

        public override string Texture => "CStudios/Content/Projectiles/Summon/Psybits/PsybitLaser";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = GetInstance<PsychokineticDamageClass>();
            Projectile.hide = false;
            Projectile.timeLeft = 2;
            Projectile.minion = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Distance < 10f)
                return false;

            Projectile parent = Main.projectile[ParentIdentity];
            if (!parent.active)
                return false;

            Vector2 start = parent.Center + Projectile.velocity * MOVE_DISTANCE;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            DrawLaser(Main.spriteBatch, TextureAssets.Projectile[Projectile.type].Value,
                start, Projectile.velocity, 8, -MathHelper.PiOver2, 0.35f, MAX_RANGE, new Color(255, 50, 50), (int)MOVE_DISTANCE);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit,
            float step, float rotation, float scale, float maxDist, Color color, int transDist)
        {
            float r = unit.ToRotation() + rotation;

            for (float i = transDist; i <= Distance; i += step)
            {
                Vector2 origin = start + i * unit;
                spriteBatch.Draw(texture, origin - Main.screenPosition,
                    new Rectangle(0, 26, 36, 26), i < transDist ? Color.Transparent : color * 0.85f, r,
                    new Vector2(18f, 13f), scale, 0, 0);
            }

            spriteBatch.Draw(texture, start + unit * (transDist - step) - Main.screenPosition,
                new Rectangle(0, 0, 36, 26), color * 0.85f, r, new Vector2(18f, 13f), scale, 0, 0);

            spriteBatch.Draw(texture, start + (Distance + step) * unit - Main.screenPosition,
                new Rectangle(0, 52, 36, 26), color * 0.85f, r, new Vector2(18f, 13f), scale, 0, 0);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Distance < 10f)
                return false;

            Projectile parent = Main.projectile[ParentIdentity];
            if (!parent.active)
                return false;

            Vector2 start = parent.Center + Projectile.velocity * MOVE_DISTANCE;
            float point = 0f;

            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(), targetHitbox.Size(),
                start, start + Projectile.velocity * Distance,
                6, ref point);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Higher immunity so 11 beams don't melt everything instantly
            target.immune[Projectile.owner] = 10;

            var global = target.GetGlobalNPC<CStudiosGlobalNPC>();
            if (!target.HasBuff(BuffType<EntropicCorruption>()))
                global.EntropicStacks = 1;

            target.AddBuff(BuffType<EntropicCorruption>(), 5 * 60);
        }

        public override void AI()
        {
            // Validate parent minion
            if (ParentIdentity < 0 || ParentIdentity >= Main.maxProjectiles)
            {
                Projectile.Kill();
                return;
            }

            Projectile parent = Main.projectile[ParentIdentity];
            if (!parent.active || parent.type != ProjectileType<Psybits>() || parent.owner != Projectile.owner)
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;
            Projectile.Center = parent.Center;

            // Read target from parent (parent stores it in localAI[1])
            int targetId = (int)parent.localAI[1];
            if (targetId < 0 || targetId >= Main.maxNPCs)
            {
                Distance = 0f;
                return;
            }

            NPC target = Main.npc[targetId];
            if (!target.active || !target.CanBeChasedBy())
            {
                Distance = 0f;
                return;
            }

            Vector2 toTarget = target.Center - parent.Center;
            float dist = toTarget.Length();
            if (dist > MAX_RANGE || dist < 1f)
            {
                Distance = 0f;
                return;
            }

            Projectile.velocity = toTarget.SafeNormalize(Vector2.UnitX);
            SetLaserPosition(parent.Center, target.Center);
            SpawnDusts(parent.Center);
        }

        private void SetLaserPosition(Vector2 from, Vector2 to)
        {
            // Stretch to target (or max range), ignore tiles
            float maxDist = Vector2.Distance(from, to);
            Distance = Math.Min(maxDist, MAX_RANGE);
            if (Distance < MOVE_DISTANCE)
                Distance = MOVE_DISTANCE;
        }

        private void SpawnDusts(Vector2 from)
        {
            if (Distance < 10f)
                return;

            Vector2 end = from + Projectile.velocity * Distance;
            Color dustColor = new Color(255, 40, 40);

            // Tip
            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustPerfect(end, DustID.Electric,
                    Main.rand.NextVector2Circular(1.8f, 1.8f), 150, dustColor, 0.9f);
                d.noGravity = true;
                d.color = dustColor;
            }

            // Muzzle (minion body)
            if (Main.rand.NextBool(3))
            {
                Vector2 muzzleVel = Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(1f, 4f);
                Dust d = Dust.NewDustPerfect(from, DustID.Electric, muzzleVel, 150, dustColor, 0.8f);
                d.noGravity = true;
                d.color = dustColor;
            }

            // Sparse body particles along the beam
            if (Main.rand.NextBool(4))
            {
                float t = Main.rand.NextFloat(0.15f, 0.9f);
                Vector2 pos = from + Projectile.velocity * (Distance * t);
                Dust d = Dust.NewDustPerfect(pos, DustID.Electric, Vector2.Zero, 150, dustColor, 0.7f);
                d.noGravity = true;
                d.color = dustColor;
            }
        }

        public override bool ShouldUpdatePosition() => false;

        public override void CutTiles()
        {
            Projectile parent = Main.projectile[ParentIdentity];
            if (!parent.active)
                return;

            Vector2 start = parent.Center;
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(start, start + Projectile.velocity * Distance,
                (Projectile.width + 12) * Projectile.scale, DelegateMethods.CutTiles);
        }
    }
}