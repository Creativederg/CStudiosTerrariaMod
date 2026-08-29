using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CStudios.Content.Buffs;
using CStudios.Content.DamageClasses;
using CStudios.Content.Items.Weapons.Summon;
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
    // Continuous beam fired while channeling the weapon (left click)
    public class PsybitPlayerBeam : ModProjectile
    {
        private const float MAX_CHARGE = 1f;
        private const float MOVE_DISTANCE = 48f;
        private const float MAX_RANGE = 1800f;

        public float Distance
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public float Charge
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        public bool IsAtMaxCharge => Charge >= MAX_CHARGE;

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
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!IsAtMaxCharge)
                return false;

            Player player = Main.player[Projectile.owner];
            Vector2 muzzle = player.MountedCenter + Projectile.velocity * MOVE_DISTANCE;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            DrawLaser(Main.spriteBatch, TextureAssets.Projectile[Projectile.type].Value,
                muzzle, Projectile.velocity, 10, -MathHelper.PiOver2, 0.45f, MAX_RANGE, new Color(255, 60, 60), (int)MOVE_DISTANCE);

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

            // Tail
            spriteBatch.Draw(texture, start + unit * (transDist - step) - Main.screenPosition,
                new Rectangle(0, 0, 36, 26), color, r, new Vector2(18f, 13f), scale, 0, 0);

            // Head
            spriteBatch.Draw(texture, start + (Distance + step) * unit - Main.screenPosition,
                new Rectangle(0, 52, 36, 26), color, r, new Vector2(18f, 13f), scale, 0, 0);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!IsAtMaxCharge)
                return false;

            Player player = Main.player[Projectile.owner];
            Vector2 muzzle = player.MountedCenter + Projectile.velocity * MOVE_DISTANCE;
            float point = 0f;

            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(), targetHitbox.Size(),
                muzzle, muzzle + Projectile.velocity * Distance,
                8, ref point);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 6;

            // Entropic Corruption
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

            target.AddBuff(BuffType<PsybitMarked>(), 180);

            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(target.Center, DustID.Electric,
                    Main.rand.NextVector2Circular(3f, 3f), 150, default, 1.2f);
                d.noGravity = true;
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            bool validWeapon =
                player.HeldItem.type == ItemType<ZaphielElectaSpark>()
                || player.HeldItem.type == ItemType<ZaphielElectaCoil>()
                || player.HeldItem.type == ItemType<ZaphielElectaResonator>()
                || player.HeldItem.type == ItemType<ZaphielElectaSurge>()
                || player.HeldItem.type == ItemType<ZaphielElectaApex>()
                || player.HeldItem.type == ItemType<ZaphielElectaOmega>();

            if (!player.active || player.dead || !validWeapon)
            {
                Projectile.Kill();
                return;
            }

            // Grace: channel can lag 1–2 frames when you press the button
            if (!player.channel)
            {
                Projectile.localAI[1] += 1f;
                if (Projectile.localAI[1] > 8f) // ~8 frames without channel → stop
                {
                    Projectile.Kill();
                    return;
                }
            }
            else
            {
                Projectile.localAI[1] = 0f;
            }

            // Mana while held (skip the first ~12 frames so the beam can appear)
            Projectile.localAI[2] += 1f;
            if (Projectile.owner == Main.myPlayer && Projectile.localAI[2] > 12f)
            {
                int manaCost = player.HeldItem.mana > 0 ? player.HeldItem.mana : 4;
                // Drain every 8 frames
                if ((int)Projectile.localAI[2] % 8 == 0)
                {
                    if (!player.CheckMana(player.HeldItem, manaCost, pay: true))
                    {
                        player.channel = false;
                        Projectile.Kill();
                        return;
                    }
                }
            }

            Projectile.timeLeft = 2;
            Charge = MAX_CHARGE;

            // Ensure a facing direction if velocity was zero
            if (Projectile.velocity.LengthSquared() < 0.001f)
                Projectile.velocity = (Main.MouseWorld - player.MountedCenter).SafeNormalize(Vector2.UnitX);

            UpdatePlayer(player);
            SetLaserPosition(player);
            SpawnDusts(player);
            CastLights();
        }

        private void UpdatePlayer(Player player)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 diff = Main.MouseWorld - player.MountedCenter;
                diff.Normalize();
                Projectile.velocity = diff;
                Projectile.direction = Main.MouseWorld.X > player.MountedCenter.X ? 1 : -1;
                Projectile.netUpdate = true;
            }

            int dir = Projectile.direction;
            player.ChangeDir(dir);
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.heldProj = Projectile.whoAmI;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full,
                (player.Center - (player.MountedCenter + Projectile.velocity * MOVE_DISTANCE)).ToRotation() + MathHelper.PiOver2);
        }

        private void SetLaserPosition(Player player)
        {
            // Always full range — passes through all tiles
            Distance = MAX_RANGE;
        }

        private void SpawnDusts(Player player)
        {
            Vector2 muzzle = player.MountedCenter + Projectile.velocity * MOVE_DISTANCE;
            Vector2 end = muzzle + Projectile.velocity * Math.Max(0f, Distance - MOVE_DISTANCE);

            int dustID = DustID.RainbowMk2;   // or DustID.RedTorch / DustID.CrimsonTorch for more red
            Color dustColor = new Color(255, 40, 40);

            // Tip sparks
            for (int i = 0; i < 2; i++)
            {
                float num1 = Projectile.velocity.ToRotation() + (Main.rand.NextBool() ? -1f : 1f) * MathHelper.PiOver2;
                float num2 = Main.rand.NextFloat(0.8f, 1.6f);
                Vector2 dustVel = new Vector2((float)Math.Cos(num1) * num2, (float)Math.Sin(num1) * num2);
                Dust dust = Dust.NewDustPerfect(end, DustID.Electric, dustVel, 0, dustColor, 1.2f);
                dust.noGravity = true;
                dust.color = dustColor;
            }

            // Muzzle burst (similar to charged)
            for (int d = 0; d < 6; d++)
            {
                Vector2 perturbedSpeed = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(25));
                float scale = Main.rand.NextFloat(2f, 10f);
                perturbedSpeed *= scale;
                int dustIndex = Dust.NewDust(muzzle, 0, 0, dustID, perturbedSpeed.X, perturbedSpeed.Y, 150, dustColor, 1.4f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].color = dustColor;
            }

            // Side jets
            for (int d = 0; d < 4; d++)
            {
                Vector2 side = Projectile.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextBool() ? 70 : -70));
                side *= Main.rand.NextFloat(2f, 6f);
                int dustIndex = Dust.NewDust(muzzle, 0, 0, dustID, side.X, side.Y, 150, dustColor, 0.9f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].color = dustColor;
            }

            // Occasional beam body sparkles
            if (Main.rand.NextBool(2))
            {
                float t = Main.rand.NextFloat(0.1f, 0.95f);
                Vector2 pos = muzzle + Projectile.velocity * (Distance * t);
                Dust d = Dust.NewDustPerfect(pos, DustID.Electric, Projectile.velocity * 0.2f, 150, dustColor, 0.8f);
                d.noGravity = true;
                d.color = dustColor;
            }
        }

        private void CastLights()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 muzzle = player.MountedCenter + Projectile.velocity * MOVE_DISTANCE;
            DelegateMethods.v3_1 = new Vector3(1f, 0.25f, 0.25f);
            Utils.PlotTileLine(muzzle, muzzle + Projectile.velocity * (Distance - MOVE_DISTANCE), 22, DelegateMethods.CastLight);
        }

        public override bool ShouldUpdatePosition() => false;

        public override void CutTiles()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 muzzle = player.MountedCenter + Projectile.velocity * MOVE_DISTANCE;
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(muzzle, muzzle + Projectile.velocity * Distance,
                (Projectile.width + 16) * Projectile.scale, DelegateMethods.CutTiles);
        }
    }
}