using CStudios.Content.Projectiles.Summon.Psybits;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Systems.ZaphielModules.Finality
{
    public static class FinalitySystem
    {
        public static bool TryActivate(Player player)
        {
            if (player == null || player.whoAmI != Main.myPlayer)
                return false;

            var ctx = ZaphielModuleSystem.Resolve(player);
            if (!ctx.FinalityCoreActive)
                return false;

            var fp = player.GetModPlayer<ZaphielFinalityPlayer>();
            if (fp.FinalityActive || fp.FinalityCooldown > 0)
                return false;

            if (player.ownedProjectileCounts[ProjectileType<Psybits>()] < 1)
                return false;

            fp.StartFinality(ctx);
            SoundEngine.PlaySound(SoundID.Item119, player.Center);
            CombatText.NewText(player.Hitbox, new Color(255, 220, 140), "FINALITY", true);
            return true;
        }
    }
}
