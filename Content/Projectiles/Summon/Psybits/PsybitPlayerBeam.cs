using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CStudios.Content.Buffs;
using CStudios.Content.DamageClasses;
using CStudios.Content.Items.Weapons.Summon;
using CStudios.Content.NPCs;
using CStudios.Content.Systems.ZaphielModules;
using CStudios.Content.Systems.ZaphielModules.Score;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Projectiles.Summon.Psybits
{
    public class PsybitPlayerBeam : ModProjectile
    {
        private const float MAX_CHARGE = 1f;
        private const float MOVE_DISTANCE = 48f;
        private const float MAX_RANGE = 1800f;

        public float Distance { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public float Charge { get => Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!IsAtMaxCharge) return false;
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
            spriteBatch.Draw(texture, start + unit * (transDist - step) - Main.screenPosition,
                new Rectangle(0, 0, 36, 26), color, r, new Vector2(18f, 13f), scale, 0, 0);
            spriteBatch.Draw(texture, start + (Distance + step) * unit - Main.screenPosition,
                new Rectangle(0, 52, 36, 26), color, r, new Vector2(18f, 13f), scale, 0, 0);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!IsAtMaxCharge) return false;
            Player player = Main.player[Projectile.owner];
            Vector2 muzzle = player.MountedCenter + Projectile.velocity * MOVE_DISTANCE;
            float point = 0f;
            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(), targetHitbox.Size(),
                muzzle, muzzle + Projectile.velocity * Distance, 8, ref point);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];
            var ctx = ZaphielModuleSystem.Resolve(owner);
            if (ctx.ScoreMode || ctx.LivingGaugeActive || ctx.RisingScoreEdgeActive || ctx.FeedbackHeartActive)
                owner.GetModPlayer<ZaphielScorePlayer>().AddScore(0.45f, ctx);

            int held = owner.HeldItem.type;
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

            var ctx = ZaphielModuleSystem.Resolve(player);
            Projectile.localNPCHitCooldown = ZaphielBeamHitDelay.For(ctx);

            if (!player.channel)
            {
                Projectile.localAI[1] += 1f;
                if (Projectile.localAI[1] > 8f)
                {
                    Projectile.Kill();
                    return;
                }
            }
            else
                Projectile.localAI[1] = 0f;

            Projectile.localAI[2] += 1f;
            if (Projectile.owner == Main.myPlayer && Projectile.localAI[2] > 12f)
            {
                int manaCost = player.HeldItem.mana > 0 ? player.HeldItem.mana : 4;
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
            if (Projectile.velocity.LengthSquared() < 0.001f)
                Projectile.velocity = (Main.MouseWorld - player.MountedCenter).SafeNormalize(Vector2.UnitX);

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 diff = Main.MouseWorld - player.MountedCenter;
                diff.Normalize();
                Projectile.velocity = diff;
                Projectile.direction = Main.MouseWorld.X > player.MountedCenter.X ? 1 : -1;
                Projectile.netUpdate = true;
            }
            player.ChangeDir(Projectile.direction);
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.heldProj = Projectile.whoAmI;
            Distance = MAX_RANGE;
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
