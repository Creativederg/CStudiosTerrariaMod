using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CStudios.Content.Systems.ZaphielModules.Score
{
    public class ZaphielScorePlayer : ModPlayer
    {
        public float Score;
        public float Stigma;
        public int StormTimer;
        public int BurstCooldown;

        public const float ScoreMax = 100f;
        public const int BurstCooldownTicks = 18 * 60;

        public bool InStorm => StormTimer > 0;
        public float Score01 => MathHelper.Clamp(Score / ScoreMax, 0f, 1f);

        public override void ResetEffects() { }

        public override void PostUpdate()
        {
            var ctx = ZaphielModuleSystem.Resolve(Player);

            if (!ctx.ScoreMode && !ctx.FeedbackHeartActive)
            {
                Score = MathHelper.Clamp(Score - 0.15f, 0f, ScoreMax);
            }
            else
            {
                // Slow natural decay so you have to keep hitting
                Score = MathHelper.Clamp(Score - 0.04f, 0f, ScoreMax);
            }

            if (StormTimer > 0)
                StormTimer--;
            if (BurstCooldown > 0)
                BurstCooldown--;

            if (Stigma > 0f)
                Stigma = MathHelper.Clamp(Stigma - 0.08f, 0f, 100f);
        }

        public void AddScore(float amount, ZaphielShootContext ctx)
        {
            float mul = ctx.ScoreGainMul > 0f ? ctx.ScoreGainMul : 1f;
            Score = MathHelper.Clamp(Score + amount * mul, 0f, ScoreMax);
        }

        public void AddStigma(float amount, ZaphielShootContext ctx)
        {
            float mul = ctx.StigmaBonusMul > 0f ? ctx.StigmaBonusMul : 1f;
            Stigma = MathHelper.Clamp(Stigma + amount * mul, 0f, 100f);
        }

        public float DamageFromScore()
        {
            return 1f + Score01 * 0.45f;
        }

        public float ConsumeForBurst(ZaphielShootContext ctx)
        {
            float dumped = Score;
            Score = 0f;
            BurstCooldown = BurstCooldownTicks;

            float stormMul = ctx.DataStormMildMul > 0f ? ctx.DataStormMildMul : 1f;
            StormTimer = (int)(4 * 60 * stormMul);
            return dumped;
        }
    }
}
