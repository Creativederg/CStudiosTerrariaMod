using CStudios.Content.Projectiles.Summon.Psybits;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Systems.ZaphielModules.Fracture
{
    public class ZaphielFracturePlayer : ModPlayer
    {
        public bool FractureActive;
        public int FractureTimer;
        public int FractureCooldown;
        public Vector2 LastEcho;
        private int _echoTick;

        public const int BaseFractureTicks = 8 * 60;
        public const int BaseCooldownTicks = 22 * 60;

        public override void ResetEffects() { }

        public override void PostUpdate()
        {
            var ctx = ZaphielModuleSystem.Resolve(Player);

            if (FractureTimer > 0)
            {
                FractureTimer--;
                FractureActive = true;

                Player.moveSpeed += 0.18f;
                Player.maxRunSpeed *= 1.20f;
                Player.jumpSpeedBoost += 0.8f;

                if (ctx.PermetAfterimageActive)
                {
                    if (Player.velocity.LengthSquared() > 2.5f)
                        LastEcho = Player.Center - Player.velocity.SafeNormalize(Vector2.Zero) * 48f;

                    _echoTick++;
                    if (Player.whoAmI == Main.myPlayer && _echoTick % 8 == 0)
                    {
                        Vector2 spawn = LastEcho != Vector2.Zero ? LastEcho : Player.Center;
                        int idx = Projectile.NewProjectile(
                            Player.GetSource_FromThis(),
                            spawn,
                            Player.velocity * -0.15f,
                            ProjectileType<PermetEcho>(),
                            0, 0f, Player.whoAmI);
                        if (idx >= 0)
                        {
                            Main.projectile[idx].spriteDirection = Player.direction;
                            Main.projectile[idx].scale = 1.05f;
                        }
                    }
                }

                if (FractureTimer <= 0)
                    EndFracture();
            }
            else
            {
                FractureActive = false;
                _echoTick = 0;
            }

            if (FractureCooldown > 0)
                FractureCooldown--;

            if (!ctx.FractureCoreActive && FractureActive)
                EndFracture();
        }

        public void StartFracture(ZaphielShootContext ctx)
        {
            float dur = ctx.FractureDurationMul > 0f ? ctx.FractureDurationMul : 1f;
            FractureTimer = (int)(BaseFractureTicks * dur);
            FractureActive = true;
            FractureCooldown = BaseCooldownTicks;
            LastEcho = Player.Center;
            _echoTick = 0;
        }

        public void EndFracture()
        {
            FractureActive = false;
            FractureTimer = 0;
        }
    }
}
