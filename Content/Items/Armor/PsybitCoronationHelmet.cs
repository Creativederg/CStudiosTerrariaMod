using CStudios.Content.DamageClasses;
using CStudios.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class PsybitCoronationHelmet : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 22;
            Item.value = Item.sellPrice(platinum: 1);
            Item.rare = ItemRarityID.Red;
            Item.defense = 18;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<PsychokineticDamageClass>() += 0.8f;
            player.GetCritChance<PsychokineticDamageClass>() += 20f;
            player.statManaMax2 += 40;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<PsybitCoronationBreastplate>()
                && legs.type == ModContent.ItemType<PsybitCoronationLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            var ap = player.GetModPlayer<ZaphielArmorPlayer>();
            ap.OmegaSet = true;
            player.maxMinions += 5;

            player.setBonus = ap.ActiveFantasy switch
            {
                ZaphielFantasy.AuthorityOfTheBits =>
                    "Throne Command: Authority patterns are stronger and cheaper. +2 max minions while commanding.",
                ZaphielFantasy.AerialHerrscher =>
                    "Sky Sovereign: Aerial Herrscher Form lasts longer. Improved air control and funnel performance.",
                ZaphielFantasy.ScoreAndStigma =>
                    "Resonant Overclock: Score builds faster, stigma bonuses are stronger, data storms are milder.",
                ZaphielFantasy.RemoteSymphony =>
                    "Conductor's Mantle: Role switching is instant. Harmony bonuses increased.",
                ZaphielFantasy.FinalPermission =>
                    "Last Authority: Final Permission is more stable, lasts longer, and has reduced aftermath cost.",
                _ =>
                    "Adaptive Matrix: +10 max minions. Solid Psychokinetic power. Modules are more effective."
            };
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