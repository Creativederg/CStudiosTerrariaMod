using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public enum ZaphielRecipeTier
    {
        Aerial,     // post-Providence
        Authority,  // Polterghast
        Score,      // Devourer of Gods
        Fracture,   // Yharon
        Finality    // post-Yharon / Exo, before SCal
    }

    public static class ZaphielModuleRecipes
    {
        public static void AddTierRecipe(ModItem item, ZaphielRecipeTier tier)
        {
            Recipe recipe = item.CreateRecipe();

            if (ModLoader.TryGetMod("CalamityMod", out Mod cal))
            {
                switch (tier)
                {
                    case ZaphielRecipeTier.Aerial:
                        Add(cal, recipe, "DivineGeode", 6);
                        Add(cal, recipe, "UnholyEssence", 8);
                        Add(cal, recipe, "UelibloomBar", 4);
                        break;

                    case ZaphielRecipeTier.Authority:
                        Add(cal, recipe, "RuinousSoul", 4);
                        Add(cal, recipe, "DivineGeode", 4);
                        Add(cal, recipe, "Bloodstone", 8);
                        break;

                    case ZaphielRecipeTier.Score:
                        Add(cal, recipe, "CosmiliteBar", 6);
                        Add(cal, recipe, "RuinousSoul", 3);
                        Add(cal, recipe, "NightmareFuel", 6);
                        break;

                    case ZaphielRecipeTier.Fracture:
                        Add(cal, recipe, "YharonSoulFragment", 4);
                        Add(cal, recipe, "CosmiliteBar", 4);
                        Add(cal, recipe, "AuricBar", 2);
                        break;

                    case ZaphielRecipeTier.Finality:
                        Add(cal, recipe, "AuricBar", 6);
                        Add(cal, recipe, "ExoPrism", 4);
                        Add(cal, recipe, "YharonSoulFragment", 3);
                        break;
                }

                recipe.AddTile(TileID.LunarCraftingStation);
                if (cal.TryFind("CosmicAnvil", out ModTile cosmic))
                    recipe.AddTile(cosmic.Type);
            }
            else
            {
                recipe.AddIngredient(ItemID.LunarBar, 8);
                recipe.AddIngredient(ItemID.FragmentStardust, 8);
                recipe.AddIngredient(ItemID.FragmentNebula, 8);
                recipe.AddTile(TileID.LunarCraftingStation);
            }

            recipe.Register();
        }

        private static void Add(Mod cal, Recipe recipe, string name, int stack)
        {
            if (cal.TryFind(name, out ModItem item))
                recipe.AddIngredient(item.Type, stack);
        }
    }
}
