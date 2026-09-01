using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModFinalityCore : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.EnergyCore;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(gold: 40);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.FinalityMode = true;
            ctx.FinalityCoreActive = true;
            ctx.FinalityDurationMul *= 1.10f;
            ctx.DamageMul *= 1.10f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "FinDesc",
                "Finality – Finality Core\nUltimate opens Herrscher of Finality. You and the bits overclock.")
            { OverrideColor = new Color(255, 220, 140) });
            tooltips.Add(new TooltipLine(Mod, "FinHint",
                "Press Ultimate with bits summoned.")
            { OverrideColor = new Color(255, 240, 200) });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 12)
                .AddIngredient(ItemID.FragmentSolar, 8)
                .AddIngredient(ItemID.FragmentStardust, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
