using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ZaphielModuleRecipeSystem : ModSystem
    {
        public override void AddRecipes()
        {
            Bind("ModSkybladeManifest", ZaphielRecipeTier.Aerial);
            Bind("ModThreeDimensionalLock", ZaphielRecipeTier.Aerial);
            Bind("ModHerrscherDrive", ZaphielRecipeTier.Aerial);
            Bind("ModFunnelOverflow", ZaphielRecipeTier.Aerial);
            Bind("ModAerialSwarmProtocol", ZaphielRecipeTier.Aerial);

            Bind("ModCommandLance", ZaphielRecipeTier.Authority);
            Bind("ModAbsoluteVector", ZaphielRecipeTier.Authority);
            Bind("ModAuthorityCore", ZaphielRecipeTier.Authority);
            Bind("ModCascadeLink", ZaphielRecipeTier.Authority);
            Bind("ModThroneOfBits", ZaphielRecipeTier.Authority);
            Bind("ModSwarmPattern", ZaphielRecipeTier.Authority);

            Bind("ModRisingScoreEdge", ZaphielRecipeTier.Score);
            Bind("ModStigmaResonance", ZaphielRecipeTier.Score);
            Bind("ModFeedbackHeart", ZaphielRecipeTier.Score);
            Bind("ModDataBacklash", ZaphielRecipeTier.Score);
            Bind("ModLivingGauge", ZaphielRecipeTier.Score);

            Bind("ModTimeLock", ZaphielRecipeTier.Fracture);
            Bind("ModPermetAfterimage", ZaphielRecipeTier.Fracture);
            Bind("ModFractureCore", ZaphielRecipeTier.Fracture);
            Bind("ModEchoRelay", ZaphielRecipeTier.Fracture);
            Bind("ModPhantomBits", ZaphielRecipeTier.Fracture);

            Bind("ModFinalityEdge", ZaphielRecipeTier.Finality);
            Bind("ModCocoonLock", ZaphielRecipeTier.Finality);
            Bind("ModFinalityCore", ZaphielRecipeTier.Finality);
            Bind("ModOriginRelay", ZaphielRecipeTier.Finality);
            Bind("ModCocoonBits", ZaphielRecipeTier.Finality);
        }

        private static void Bind(string className, ZaphielRecipeTier tier)
        {
            if (!ModContent.TryFind("CStudios/" + className, out ModItem item))
                return;
            ZaphielModuleRecipes.AddTierRecipe(item, tier);
        }

        public override void PostAddRecipes()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out _))
                return;

            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe r = Main.recipe[i];
                if (r.Disabled)
                    continue;
                if (!IsZaphielModule(r.createItem.type))
                    continue;

                bool vanillaFallback =
                    r.HasIngredient(ItemID.LunarBar)
                    || r.HasIngredient(ItemID.FragmentVortex)
                    || r.HasIngredient(ItemID.FragmentNebula)
                    || r.HasIngredient(ItemID.FragmentStardust)
                    || r.HasIngredient(ItemID.FragmentSolar);

                if (vanillaFallback)
                    r.DisableRecipe();
            }
        }

        private static bool IsZaphielModule(int type)
        {
            Item item = ContentSamples.ItemsByType.TryGetValue(type, out Item sample) ? sample : null;
            return item?.ModItem is ZaphielModuleItem;
        }
    }
}
