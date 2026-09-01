using CStudios.Content.Projectiles.Summon.Psybits;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Systems.ZaphielModules.Fracture
{
    public static class FractureSystem
    {
        public static bool TryActivate(Player player)
        {
            if (player == null || player.whoAmI != Main.myPlayer)
                return false;

            var ctx = ZaphielModuleSystem.Resolve(player);
            if (!ctx.FractureCoreActive)
                return false;

            var fp = player.GetModPlayer<ZaphielFracturePlayer>();
            if (fp.FractureActive || fp.FractureCooldown > 0)
                return false;

            if (player.ownedProjectileCounts[ProjectileType<Psybits>()] < 1)
                return false;

            fp.StartFracture(ctx);

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || !npc.CanBeChasedBy())
                    continue;
                if (Vector2.Distance(npc.Center, player.Center) > 700f)
                    continue;
                npc.AddBuff(BuffID.Slow, fp.FractureTimer);
                npc.AddBuff(BuffID.ShadowFlame, 60);
            }

            SoundEngine.PlaySound(SoundID.Item29, player.Center);
            CombatText.NewText(player.Hitbox, new Color(180, 140, 255), "TIME FRACTURE", true);
            return true;
        }
    }
}
