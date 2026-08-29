using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CStudios.Content.Buffs;
using CStudios.Content.DamageClasses;
using CStudios.Content.NPCs;
using CStudios.Content.Items.Weapons.Summon;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Projectiles.Summon.Psybits
{
    // Ultimate beam — thicker, longer, stronger, applies Entropic Corruption
    // ai[1] = parent Psybits whoAmI
    public class PsybitMinionChargedBeam : ModProjectile
    {
        private const float MOVE_DISTANCE = 8f;
        private const float MAX_RANGE = 900f; // longer than normal minion beam

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
            Projectile.timeLeft = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Distance < 10f)
                return false;

            Projectile parent = Main.projectile[ParentIdentity];
            if (!parent.active)
                return false;

            Vector2 start = parent.Center + Projectile.velocity * MOVE_DISTANCE;
            Color beamColor = new Color(255, 50, 50);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            // Thicker than normal minion beam (scale 0.75 vs 0.35)
            DrawLaser(Main.spriteBatch, TextureAssets.Projectile[Projectile.type].Value,
                start, Projectile.velocity, 8, -MathHelper.PiOver2, 0.75f, MAX_RANGE, beamColor, (int)MOVE_DISTANCE);

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
                    new Rectangle(0, 26, 36, 26), i < transDist ? Color.Transparent : color, r,
                    new Vector2(18f, 13f), scale, 0, 0);
            }

            spriteBatch.Draw(texture, start + unit * (transDist - step) - Main.screenPosition,
                new Rectangle(0, 0, 36, 26), color, r, new Vector2(18f, 13f), scale, 0, 0);

            spriteBatch.Draw(texture, start + (Distance + step) * unit - Main.screenPosition,
                new Rectangle(0, 52, 36, 26), color, r, new Vector2(18f, 13f), scale, 0, 0);
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
                14, ref point); // wider than normal minion beam
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 6; // hits more often than normal minion beam

            int held = Main.player[Projectile.owner].HeldItem.type;
            bool applyEntropic =
                held == ItemType<ZaphielElectaResonator>()
                || held == ItemType<ZaphielElectaSurge>()
                || held == ItemType<ZaphielElectaApex>()
                || held == ItemType<ZaphielElectaOmega>();

            if (applyEntropic)
            {
                var global = target.GetGlobalNPC<CStudiosGlobalNPC>();
                if (!target.HasBuff(BuffType<EntropicCorruption>()))
                    global.EntropicStacks = 1;
                target.AddBuff(BuffType<EntropicCorruption>(), 5 * 60);
            }
        }

        public override void AI()
        {
            if (ParentIdentity < 0 || ParentIdentity >= Main.maxProjectiles)
            {
                Projectile.Kill();
                return;
            }

            Projectile parent = Main.projectile[ParentIdentity];
            Player owner = Main.player[Projectile.owner];

            // End immediately when ultimate expires or parent dies
            if (!parent.active || parent.type != ProjectileType<Psybits>()
                || parent.owner != Projectile.owner
                || !owner.HasBuff(BuffType<PsybitOvercharge>()))
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;
            Projectile.Center = parent.Center;

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
            Distance = Math.Min(dist, MAX_RANGE);

            // Extra particles while ultimate is active
            if (Main.rand.NextBool(2))
            {
                Vector2 end = parent.Center + Projectile.velocity * Distance;
                Dust d = Dust.NewDustPerfect(end, DustID.Electric,
                    Main.rand.NextVector2Circular(2f, 2f), 100, new Color(255, 40, 40), 1.1f);
                d.noGravity = true;
            }
        }

        public override bool ShouldUpdatePosition() => false;
    }
}