using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CStudios.Content.Systems.ZaphielModules.Finality
{
    public class ZaphielFinalityPlayer : ModPlayer
    {
        public bool FinalityActive;
        public int FinalityTimer;
        public int FinalityCooldown;

        public const int BaseFinalityTicks = 12 * 60;
        public const int BaseCooldownTicks = 30 * 60;

        public override void ResetEffects() { }

        public override void PostUpdate()
        {
            var ctx = ZaphielModuleSystem.Resolve(Player);

            if (FinalityTimer > 0)
            {
                FinalityTimer--;
                FinalityActive = true;
                Player.moveSpeed += 0.22f;
                Player.maxRunSpeed *= 1.25f;
                Player.jumpSpeedBoost += 1.1f;
                Player.noFallDmg = true;

                if (FinalityTimer <= 0)
                    EndFinality();
            }
            else
                FinalityActive = false;

            if (FinalityCooldown > 0)
                FinalityCooldown--;

            if (!ctx.FinalityCoreActive && FinalityActive)
                EndFinality();
        }

        public void StartFinality(ZaphielShootContext ctx)
        {
            float dur = ctx.FinalityDurationMul > 0f ? ctx.FinalityDurationMul : 1f;
            FinalityTimer = (int)(BaseFinalityTicks * dur);
            FinalityActive = true;
            FinalityCooldown = BaseCooldownTicks;
        }

        public void EndFinality()
        {
            FinalityActive = false;
            FinalityTimer = 0;
        }
    }
}
