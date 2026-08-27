using Terraria.ModLoader;

namespace CStudios.Content.Systems
{
    public static class CrossMod
    {
        public static bool CalamityLoaded => ModLoader.HasMod("CalamityMod");
        public static bool OverhaulLoaded => ModLoader.HasMod("CalamityOverhaul");

        /// <summary>
        /// Example: resolve a Calamity item type by internal name.
        /// Returns 0 if not found.
        /// </summary>
        public static int FindCalamityItem(string internalName)
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity))
                return 0;

            if (calamity.TryFind(internalName, out ModItem item))
                return item.Type;

            return 0;
        }

        /// <summary>
        /// Example ModCall into Calamity (only if they expose that call).
        /// </summary>
        public static object CallCalamity(params object[] args)
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity))
                return null;

            return calamity.Call(args);
        }

        /// <summary>
        /// Same pattern for Calamity Overhaul.
        /// </summary>
        public static object CallOverhaul(params object[] args)
        {
            if (!ModLoader.TryGetMod("CalamityOverhaul", out Mod cwr))
                return null;

            return cwr.Call(args);
        }
    }
}