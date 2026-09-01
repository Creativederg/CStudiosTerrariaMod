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
                    "Throne Command: two extra bits that cost no slots.",
                ZaphielFantasy.AerialHerrscher =>
                    "Sky Sovereign: no fall damage, stronger air control.",
                ZaphielFantasy.ScoreAndStigma =>
                    "Resonant Overclock: score stays live at cap. +life regen.",
                ZaphielFantasy.TimeFracture =>
                    "Permet Mantle: echoes last; you move through the freeze.",
                ZaphielFantasy.Finality =>
                    "Last Herrscher: while Finality is active, Trace bolts pulse from you.",
                _ =>
                    "Adaptive Matrix: solid Psychokinetic power."
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