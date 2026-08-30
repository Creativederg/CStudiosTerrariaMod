using CStudios.Content.Projectiles.Summon.Psybits;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Systems.ZaphielModules.Aerial
{
    public static class AerialHerrscherSystem
    {
        public static bool TryActivateForm(Player player)
        {
            if (player == null || player.whoAmI != Main.myPlayer)
                return false;

            var ctx = ZaphielModuleSystem.Resolve(player);
            if (!ctx.HerrscherDriveActive)
                return false;

            var ap = player.GetModPlayer<ZaphielAerialPlayer>();
            if (ap.FormActive || ap.FormCooldown > 0)
                return false;

            if (player.ownedProjectileCounts[ProjectileType<Psybits>()] < 1)
                return false;

            ap.StartForm(ctx);

            SoundEngine.PlaySound(SoundID.Item4, player.Center);
            CombatText.NewText(player.Hitbox, new Color(120, 210, 255), "Aerial Herrscher", true);

            for (int i = 0; i < 24; i++)
            {
                Dust d = Dust.NewDustPerfect(player.Center, DustID.Electric,
                    Main.rand.NextVector2Circular(7f, 7f), 80, new Color(100, 190, 255), 1.3f);
                d.noGravity = true;
            }

            return true;
        }
    }
}
