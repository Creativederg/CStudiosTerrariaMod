using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CStudios.Content.DamageClasses;

namespace CStudios.Content.Projectiles.Summon.Psybits
{
    /// <summary>
    /// Short-lived wide slash / ribbon used by Skyblade Manifest during Aerial Herrscher Form.
    /// </summary>
    public class AerialSkybladeSlash : ModProjectile
    {
        public override string Texture => "CStudios/Content/Projectiles/Summon/Psybits/PsybitUnchargedLaser";

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = ModContent.GetInstance<PsychokineticDamageClass>();
            Projectile.penetrate = 6;
            Projectile.timeLeft = 22;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.scale = 1.15f + 0.25f * (float)System.Math.Sin(Projectile.timeLeft * 0.4f);
            Projectile.alpha = (int)MathHelper.Lerp(40f, 200f, 1f - Projectile.timeLeft / 22f);

            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric,
                    Projectile.velocity * 0.1f + Main.rand.NextVector2Circular(1.2f, 1.2f),
                    80, new Color(120, 210, 255), 1.1f);
                d.noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(140, 220, 255, 80) * (1f - Projectile.alpha / 255f);
        }
    }
}
