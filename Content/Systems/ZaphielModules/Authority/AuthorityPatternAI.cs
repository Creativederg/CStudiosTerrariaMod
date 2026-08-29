using CStudios.Content.Projectiles.Summon.Psybits;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Systems.ZaphielModules.Authority
{
    public static class AuthorityPatternAI
    {
        public static bool TryRunPatternAI(Projectile bit, Player owner, ZaphielShootContext ctx)
        {
            var ap = owner.GetModPlayer<ZaphielAuthorityPlayer>();

            AuthorityPatternType pattern = ap.IsPatternActive ? ap.CurrentPattern
                                         : ap.IsCascadeActive ? ap.CascadePattern
                                         : AuthorityPatternType.None;

            if (pattern == AuthorityPatternType.None)
                return false;

            float power = ctx.AuthorityPatternPowerMul;
            if (ap.IsCascadeActive && !ap.IsPatternActive)
                power *= 0.60f;

            int bitIndex = (int)bit.ai[2];
            int totalBits = System.Math.Max(1, owner.ownedProjectileCounts[ProjectileType<Psybits>()]);
            Vector2 targetPos = GetTargetPosition(ap, owner);

            switch (pattern)
            {
                case AuthorityPatternType.GiantLance:
                    RunGiantLance(bit, owner, targetPos, bitIndex, totalBits, power, ctx);
                    break;
                case AuthorityPatternType.BindingCage:
                    RunBindingCage(bit, owner, targetPos, bitIndex, totalBits, power, ctx);
                    break;
                case AuthorityPatternType.OrbitalBombardment:
                    RunOrbitalBombardment(bit, owner, targetPos, bitIndex, totalBits, power, ctx);
                    break;
                case AuthorityPatternType.AegisWall:
                    RunAegisWall(bit, owner, targetPos, bitIndex, totalBits, power, ctx);
                    break;
                case AuthorityPatternType.SpiralExecution:
                    RunSpiralExecution(bit, owner, targetPos, bitIndex, totalBits, power, ctx, ap);
                    break;
            }

            return true;
        }

        private static Vector2 GetTargetPosition(ZaphielAuthorityPlayer ap, Player owner)
        {
            if (ap.LockedTargetWhoAmI >= 0 && ap.LockedTargetWhoAmI < Main.maxNPCs)
            {
                NPC npc = Main.npc[ap.LockedTargetWhoAmI];
                if (npc.active)
                    return npc.Center;
            }
            return ap.LockedWorldPosition;
        }

        private static void RunGiantLance(Projectile bit, Player owner, Vector2 targetPos,
            int bitIndex, int totalBits, float power, ZaphielShootContext ctx)
        {
            Vector2 dir = (targetPos - owner.Center).SafeNormalize(Vector2.UnitX);
            float spacing = 28f * ctx.AuthorityFormationIntegrity;
            float startOffset = 60f;
            Vector2 desired = owner.Center + dir * (startOffset + bitIndex * spacing);
            bit.velocity = (desired - bit.Center) * 0.18f;
            bit.friendly = true;
        }

        private static void RunBindingCage(Projectile bit, Player owner, Vector2 targetPos,
            int bitIndex, int totalBits, float power, ZaphielShootContext ctx)
        {
            float radius = 110f * ctx.AuthorityFormationIntegrity;
            float angle = MathHelper.TwoPi * bitIndex / totalBits
                        + (float)Main.GameUpdateCount * 0.045f;
            Vector2 desired = targetPos + new Vector2(
                (float)System.Math.Cos(angle) * radius,
                (float)System.Math.Sin(angle) * radius);
            bit.velocity = (desired - bit.Center) * 0.22f;
        }

        private static void RunOrbitalBombardment(Projectile bit, Player owner, Vector2 targetPos,
            int bitIndex, int totalBits, float power, ZaphielShootContext ctx)
        {
            float height = 220f + (bitIndex % 3) * 25f;
            float angle = MathHelper.TwoPi * bitIndex / totalBits
                        + (float)Main.GameUpdateCount * 0.02f;
            Vector2 desired = targetPos + new Vector2(
                (float)System.Math.Cos(angle) * 90f,
                -height);
            bit.velocity = (desired - bit.Center) * 0.15f;

            // Optional: spawn downward beams here using your existing laser projectile
        }

        private static void RunAegisWall(Projectile bit, Player owner, Vector2 targetPos,
            int bitIndex, int totalBits, float power, ZaphielShootContext ctx)
        {
            Vector2 dir = (targetPos - owner.Center).SafeNormalize(Vector2.UnitX);
            Vector2 perp = dir.RotatedBy(MathHelper.PiOver2);
            float width = 28f * ctx.AuthorityFormationIntegrity;
            float centerOffset = 70f;
            float t = bitIndex - (totalBits - 1) * 0.5f;
            Vector2 desired = owner.Center + dir * centerOffset + perp * (t * width);
            bit.velocity = (desired - bit.Center) * 0.20f;
        }

        private static void RunSpiralExecution(Projectile bit, Player owner, Vector2 targetPos,
            int bitIndex, int totalBits, float power, ZaphielShootContext ctx, ZaphielAuthorityPlayer ap)
        {
            float progress = 1f - (ap.PatternTimer / (float)ZaphielAuthorityPlayer.GetBaseDuration(AuthorityPatternType.SpiralExecution));
            progress = MathHelper.Clamp(progress, 0f, 1f);

            float radius = MathHelper.Lerp(160f, 12f, progress) * ctx.AuthorityFormationIntegrity;
            float angle = MathHelper.TwoPi * bitIndex / totalBits
                        + progress * MathHelper.TwoPi * 2.5f
                        + (float)Main.GameUpdateCount * 0.03f;

            Vector2 desired = targetPos + new Vector2(
                (float)System.Math.Cos(angle) * radius,
                (float)System.Math.Sin(angle) * radius);
            bit.velocity = (desired - bit.Center) * 0.25f;
        }
    }
}