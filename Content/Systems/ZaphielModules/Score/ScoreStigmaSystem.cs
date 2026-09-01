using CStudios.Content.Projectiles.Summon.Psybits;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Systems.ZaphielModules.Score
{
    public static class ScoreStigmaSystem
    {
        public static bool TryBurst(Player player)
        {
            if (player == null || player.whoAmI != Main.myPlayer)
                return false;

            var ctx = ZaphielModuleSystem.Resolve(player);
            if (!ctx.FeedbackHeartActive)
                return false;

            var sp = player.GetModPlayer<ZaphielScorePlayer>();
            if (sp.BurstCooldown > 0 || sp.Score < 15f)
                return false;

            if (player.ownedProjectileCounts[ProjectileType<Psybits>()] < 1)
                return false;

            float dumped = sp.ConsumeForBurst(ctx);
            int dmg = System.Math.Max(1, (int)(player.GetWeaponDamage(player.HeldItem) * (1.2f + dumped / 50f)));

            for (int i = 0; i < 8; i++)
            {
                int idx = Projectile.NewProjectile(
                    player.GetSource_FromThis(),
                    player.Center,
                    Main.rand.NextVector2Circular(10f, 10f),
                    ProjectileType<PsybitUnchargedLaser>(),
                    dmg, 3f, player.whoAmI);
                if (idx >= 0)
                {
                    Main.projectile[idx].timeLeft = 24;
                    Main.projectile[idx].penetrate = -1;
                    Main.projectile[idx].scale = 1.4f;
                }
            }

            SoundEngine.PlaySound(SoundID.Item14, player.Center);
            CombatText.NewText(player.Hitbox, new Color(255, 210, 80), $"SCORE {dumped:0}", true);
            return true;
        }
    }
}