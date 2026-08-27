using System;
using Terraria.ModLoader;

namespace CStudios
{
    public class CStudios : Mod
    {
        public static ModKeybind UltimateKey;

        /// <summary>Cached refs after Load. Null if missing (shouldn't happen with modReferences).</summary>
        public static Mod Calamity { get; private set; }
        public static Mod CalamityOverhaul { get; private set; }

        public override void Load()
        {
            UltimateKey = KeybindLoader.RegisterKeybind(this, "Unleash Ultimate", "Q");

            // Safe lookups (modReferences already forces both to be present)
            ModLoader.TryGetMod("CalamityMod", out Mod calamity);
            ModLoader.TryGetMod("CalamityOverhaul", out Mod cwr);

            Calamity = calamity;
            CalamityOverhaul = cwr;

            // Optional hard fail with a clear message
            if (Calamity == null || CalamityOverhaul == null)
            {
                throw new Exception(
                    "CStudios requires both Calamity Mod (CalamityMod) and " +
                    "Calamity Overhaul (CalamityOverhaul) to be enabled.");
            }

            Logger.Info("CStudios loaded with Calamity + Calamity Overhaul.");
        }

        public override void Unload()
        {
            UltimateKey = null;
            Calamity = null;
            CalamityOverhaul = null;
        }
    }
}