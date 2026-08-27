using CStudios.Content.Items.Modules.Zaphiel;
using Terraria;

namespace CStudios.Content.Systems.ZaphielModules
{
    public static class ZaphielModuleSystem
    {
        public static ZaphielShootContext Resolve(Player player)
        {
            var ctx = ZaphielShootContext.Default;
            if (player == null)
                return ctx;

            var mp = player.GetModPlayer<ZaphielModulePlayer>();
            for (int i = 0; i < ZaphielModulePlayer.SlotCount; i++)
            {
                Item m = mp.GetModule(i);
                if (m?.ModItem is ZaphielModuleItem mod)
                    mod.Apply(ref ctx);
            }
            return ctx;
        }
    }
}