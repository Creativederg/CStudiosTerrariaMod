using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Projectiles
{
    public class RaiserFeathers : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = false;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = false;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.minion = false;
            Projectile.minionSlots = 0f;
            Projectile.netImportant = true;
            Projectile.damage = 0;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            // Kill if wings are gone or owner is dead
            if (!owner.active || owner.dead || !owner.HasItem(ModContent.ItemType<Items.Accessories.RaiserWings>()))
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;

            int index = (int)Projectile.ai[0];          // 0–7
            int order = index + 1;                       // 1–8 (for size / angle)

            // ===== SIZE =====
            // 1 = smallest (~0.55), 8 = largest (~1.35)
            float scale = 0.55f + (order - 1) * 0.1f;
            Projectile.scale = scale;

            // ===== SIDE & LAYERING (illusion of two wings) =====
            // First 4 go on the left wing, last 4 on the right wing
            bool isLeftWing = index < 4;
            int layer = index % 4;                       // 0–3 depth inside each wing

            int dir = owner.direction;

            // Base offset behind the player
            float baseX = -dir * 18f;
            float baseY = -12f;

            // Spread the two wings outward
            float wingSpread = isLeftWing ? -1f : 1f;
            float xOffset = baseX + wingSpread * (22f + layer * 9f);
            float yOffset = baseY - layer * 11f + (isLeftWing ? -4f : 4f); // slight vertical stagger

            Vector2 targetPos = owner.Center + new Vector2(xOffset, yOffset);

            // Smooth follow
            Vector2 toTarget = targetPos - Projectile.Center;
            float dist = toTarget.Length();

            if (dist > 3f)
            {
                toTarget.Normalize();
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, toTarget * (dist * 0.18f + 3.5f), 0.22f);
            }
            else
            {
                Projectile.velocity *= 0.75f;
            }

            // ===== SLIGHT ANGLE =====
            // Bigger ones angle more (looks more wing-like)
            float angleAmount = (order - 1) * 0.045f;   // ~0° to ~18°
            float finalRotation = wingSpread * angleAmount;

            // Slight extra tilt so they fan out
            finalRotation += wingSpread * layer * 0.03f;

            Projectile.rotation = finalRotation;
            Projectile.spriteDirection = dir;
        }

        public override bool? CanDamage() => false;
        public override bool MinionContactDamage() => false;
    }
}