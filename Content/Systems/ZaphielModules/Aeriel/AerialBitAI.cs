using CStudios.Content.Projectiles.Summon.Psybits;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Systems.ZaphielModules.Aerial
{
    /// <summary>
    /// Overrides Psybit movement while Aerial Herrscher Form is active.
    /// Call from Psybits.AI before normal Movement.
    /// Returns true if movement was handled.
    /// </summary>
    public static class AerialBitAI
    {
        public static bool TryRunAerialAI(Projectile bit, Player owner, ZaphielShootContext ctx)
        {
            var ap = owner.GetModPlayer<ZaphielAerialPlayer>();
            if (!ap.FormActive)
                return false;

            if (!ctx.AerialSwarmProtocolActive && !ctx.ThreeDimensionalLockActive && !ctx.FunnelOverflowActive)
            {
                // Core alone still lifts bits into a high orbit
            }

            int bitIndex = (int)bit.ai[2];
            int total = System.Math.Max(1, owner.ownedProjectileCounts[ProjectileType<Psybits>()]);

            Vector2 target = GetTarget(owner, bit);
            float indep = ctx.AerialBitIndepMul > 0f ? ctx.AerialBitIndepMul : 1f;
            float speedMul = ctx.MinionMoveSpeedMul * indep;

            if (ctx.AerialSwarmProtocolActive)
                RunDiveSpiral(bit, owner, target, bitIndex, total, speedMul, ctx);
            else if (ctx.ThreeDimensionalLockActive)
                RunHighOrbit(bit, owner, target, bitIndex, total, speedMul, ctx);
            else
                RunHighOrbit(bit, owner, target, bitIndex, total, speedMul, ctx);

            return true;
        }

        private static Vector2 GetTarget(Player owner, Projectile bit)
        {
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC n = Main.npc[owner.MinionAttackTargetNPC];
                if (n.active && n.CanBeChasedBy())
                    return n.Center;
            }

            int last = (int)bit.localAI[1];
            if (last >= 0 && last < Main.maxNPCs && Main.npc[last].active)
                return Main.npc[last].Center;

            return owner.Center + new Vector2(owner.direction * 180f, -120f);
        }

        private static void RunHighOrbit(Projectile bit, Player owner, Vector2 target,
            int bitIndex, int total, float speedMul, ZaphielShootContext ctx)
        {
            float radius = (90f + bitIndex * 8f) * ctx.MinionOrbitRadiusMul;
            float height = -140f - (bitIndex % 4) * 18f;
            float angle = MathHelper.TwoPi * bitIndex / total + Main.GameUpdateCount * 0.03f;

            Vector2 desired = target + new Vector2(
                (float)System.Math.Cos(angle) * radius,
                height + (float)System.Math.Sin(angle) * radius * 0.35f);

            bit.velocity = (desired - bit.Center) * (0.16f * speedMul);
        }

        private static void RunDiveSpiral(Projectile bit, Player owner, Vector2 target,
            int bitIndex, int total, float speedMul, ZaphielShootContext ctx)
        {
            float t = (Main.GameUpdateCount + bitIndex * 12) % 180 / 180f;
            float radius = MathHelper.Lerp(160f, 28f, t) * ctx.MinionOrbitRadiusMul;
            float height = MathHelper.Lerp(-200f, 10f, t);
            float angle = MathHelper.TwoPi * bitIndex / total + t * MathHelper.TwoPi * 2f;

            Vector2 desired = target + new Vector2(
                (float)System.Math.Cos(angle) * radius,
                height);

            bit.velocity = (desired - bit.Center) * (0.20f * speedMul);
        }
    }
}
