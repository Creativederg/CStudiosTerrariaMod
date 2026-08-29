using CStudios.Content.Projectiles.Summon.Psybits;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Systems.ZaphielModules.Authority
{
    public static class AuthorityPatternSystem
    {
        public const int GlobalCooldownTicks = 10 * 60;

        public static bool TryActivatePattern(Player player)
        {
            if (player == null || player.whoAmI != Main.myPlayer)
                return false;

            var ctx = ZaphielModuleSystem.Resolve(player);
            if (!ctx.AuthorityCoreActive)
                return false;

            var ap = player.GetModPlayer<ZaphielAuthorityPlayer>();

            if (ap.IsPatternActive || ap.PatternCooldown > 0)
                return false;

            if (player.ownedProjectileCounts[ProjectileType<Psybits>()] < 1)
                return false;

            AuthorityPatternType pattern = ap.GetPatternAtCycleIndex();
            ap.AdvanceCycle();
            ResolveTarget(player, ap);

            int duration = (int)(ZaphielAuthorityPlayer.GetBaseDuration(pattern) * ctx.AuthorityPatternPowerMul);

            ap.CurrentPattern = pattern;
            ap.PatternTimer = duration;
            ap.PatternCooldown = (int)(GlobalCooldownTicks * ctx.AuthorityPatternCostMul);
            ap.PatternFlash = 1f;

            SoundEngine.PlaySound(SoundID.Item113, player.Center);
            CombatText.NewText(player.Hitbox, new Color(200, 140, 255), GetPatternDisplayName(pattern), true);

            for (int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustPerfect(player.Center, DustID.Electric,
                    Main.rand.NextVector2Circular(6f, 6f), 100, new Color(180, 100, 255), 1.4f);
                d.noGravity = true;
            }

            return true;
        }

        private static void ResolveTarget(Player player, ZaphielAuthorityPlayer ap)
        {
            if (player.HasMinionAttackTargetNPC)
            {
                int idx = player.MinionAttackTargetNPC;
                if (idx >= 0 && idx < Main.maxNPCs && Main.npc[idx].active && !Main.npc[idx].friendly)
                {
                    ap.LockedTargetWhoAmI = idx;
                    ap.LockedWorldPosition = Main.npc[idx].Center;
                    return;
                }
            }

            float bestDist = 900f;
            int best = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.life <= 0 || !npc.CanBeChasedBy())
                    continue;

                float dist = Vector2.Distance(player.Center, npc.Center);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = i;
                }
            }

            if (best >= 0)
            {
                ap.LockedTargetWhoAmI = best;
                ap.LockedWorldPosition = Main.npc[best].Center;
            }
            else
            {
                ap.LockedTargetWhoAmI = -1;
                ap.LockedWorldPosition = player.Center + player.DirectionTo(Main.MouseWorld) * 320f;
            }
        }

        public static string GetPatternDisplayName(AuthorityPatternType type) => type switch
        {
            AuthorityPatternType.GiantLance => "Giant Lance",
            AuthorityPatternType.BindingCage => "Binding Cage",
            AuthorityPatternType.OrbitalBombardment => "Orbital Bombardment",
            AuthorityPatternType.AegisWall => "Aegis Wall",
            AuthorityPatternType.SpiralExecution => "Spiral Execution",
            _ => "Authority"
        };
    }
}