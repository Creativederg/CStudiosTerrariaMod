using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CStudios.Content.Systems.ZaphielModules.Aerial
{
    public class ZaphielAerialPlayer : ModPlayer
    {
        public bool FormActive;
        public int FormTimer;
        public int FormCooldown;
        public float Strain;

        public const int BaseFormTicks = 10 * 60;
        public const int BaseCooldownTicks = 24 * 60;

        public override void ResetEffects() { }

        public override void PostUpdate()
        {
            var ctx = ZaphielModuleSystem.Resolve(Player);

            if (FormTimer > 0)
            {
                FormTimer--;
                FormActive = true;
                Strain = MathHelper.Clamp(Strain + 0.004f, 0f, 1f);

                // Air control while formed
                float move = ctx.AerialMoveMul > 0f ? ctx.AerialMoveMul : 1.35f;
                Player.maxRunSpeed *= move;
                Player.moveSpeed += 0.12f * move;
                Player.slowFall = true;
                Player.noFallDmg = true;

                // Extra jump / hover
                if (Player.controlJump)
                {
                    Player.velocity.Y -= 0.35f * move;
                    if (Player.velocity.Y < -8.5f * move)
                        Player.velocity.Y = -8.5f * move;
                }

                if (FormTimer <= 0)
                    EndForm();
            }
            else
            {
                FormActive = false;
                if (Strain > 0f)
                    Strain = MathHelper.Clamp(Strain - 0.006f, 0f, 1f);
            }

            if (FormCooldown > 0)
                FormCooldown--;

            if (!ctx.HerrscherDriveActive && FormActive)
                EndForm();
        }

        public void StartForm(ZaphielShootContext ctx)
        {
            float durMul = ctx.AerialFormDurationMul > 0f ? ctx.AerialFormDurationMul : 1f;
            FormTimer = (int)(BaseFormTicks * durMul);
            FormActive = true;
            FormCooldown = BaseCooldownTicks;
        }

        public void EndForm()
        {
            FormActive = false;
            FormTimer = 0;
        }
    }
}
