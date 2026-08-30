using CStudios.Content.DamageClasses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class PsybitCoronationLeggings : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 80);
            Item.rare = ItemRarityID.Red;
            Item.defense = 16;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 2;
            player.moveSpeed += 0.12f;
            player.GetDamage<PsychokineticDamageClass>() += 0.68f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            if (ModLoader.TryGetMod("CalamityMod", out Mod cal))
            {
                if (cal.TryFind("DivineGeode", out ModItem geode))
                    recipe.AddIngredient(geode.Type, 8);
                if (cal.TryFind("UnholyEssence", out ModItem essence))
                    recipe.AddIngredient(essence.Type, 10);
                if (cal.TryFind("UelibloomBar", out ModItem ueli))
                    recipe.AddIngredient(ueli.Type, 6);
            }
            else
            {
                recipe.AddIngredient(ItemID.LunarBar, 8);
                recipe.AddIngredient(ItemID.FragmentNebula, 10);
                recipe.AddIngredient(ItemID.FragmentStardust, 10);
            }

            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}