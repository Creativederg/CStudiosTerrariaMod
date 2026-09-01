using CStudios.Content.DamageClasses;
using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Projectiles.Summon.Psybits
{
    public class PermetEcho : ModProjectile
    {
        public override string Texture => "CStudios/Content/Projectiles/Summon/Psybits/Psybits";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
        }

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 28;
            Projectile.alpha = 40;
            Projectile.DamageType = GetInstance<PsychokineticDamageClass>();
        }

        public override void AI()
        {
            Projectile.frame = (int)MathHelper.Clamp(Projectile.ai[0], 0f, 10f);
            Projectile.velocity *= 0.90f;
            Projectile.alpha += 8;
            if (Projectile.ai[1] != 0f)
                Projectile.rotation = Projectile.ai[1];

            // Fire once, shortly after appearing
            if (Projectile.owner == Main.myPlayer && Projectile.localAI[0] == 0f && Projectile.timeLeft <= 20)
            {
                Projectile.localAI[0] = 1f;
                FireVolley();
            }

            if (Projectile.alpha >= 250)
                Projectile.Kill();
        }

        private void FireVolley()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner == null || !owner.active)
                return;

            var ctx = ZaphielModuleSystem.Resolve(owner);

            NPC target = null;
            float best = 700f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || !npc.CanBeChasedBy())
                    continue;
                float d = Vector2.Distance(npc.Center, Projectile.Center);
                if (d < best)
                {
                    best = d;
                    target = npc;
                }
            }

            if (target == null)
                return;

            Vector2 aim = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
            int shots = ctx.PhantomBitsActive ? 7 : 5;
            int dmg = System.Math.Max(1, (int)(owner.GetWeaponDamage(owner.HeldItem) * 0.22f * ctx.MinionDamageMul));

            int projType = ProjectileType<PsybitUnchargedLaser>();
            bool cwrBeam = false;
            if (ModLoader.TryGetMod("CalamityOverhaul", out Mod cwr)
                && cwr.TryFind("CyberTraceBeamProj", out ModProjectile beam))
            {
                projType = beam.Type;
                cwrBeam = true;
            }

            for (int i = 0; i < shots; i++)
            {
                float spread = (i - (shots - 1) * 0.5f) * 0.18f;
                Vector2 vel = aim.RotatedBy(spread) * (11f + Main.rand.NextFloat(0f, 3f));
                int idx = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    vel,
                    projType,
                    dmg, 1.2f, owner.whoAmI,
                    ai0: cwrBeam ? Main.rand.Next(3) : 0);
                if (idx >= 0 && idx < Main.maxProjectiles)
                {
                    Main.projectile[idx].timeLeft = 22;
                    Main.projectile[idx].penetrate = 2;
                    Main.projectile[idx].scale = 0.85f;
                    if (cwrBeam)
                        Main.projectile[idx].ai[1] = ctx.HomingMul > 0f ? ctx.HomingMul : 1f;
                }
            }

            SoundEngine.PlaySound(SoundID.Item12 with { Volume = 0.35f, Pitch = 0.4f }, Projectile.Center);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(170, 130, 255, 70) * ((255 - Projectile.alpha) / 255f);
        }
    }
}
