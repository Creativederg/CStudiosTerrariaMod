using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModFinalityEdge : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.Spearhead;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(gold: 30);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.FinalityMode = true;
            ctx.FinalityEdgeActive = true;
            ctx.DamageMul *= 1.12f;
            ctx.BeamCountAdd += 1;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "FinDesc",
                "Finality – Finality Edge\nDuring Finality, left click fires a wider Origin slash fan.")
            { OverrideColor = new Color(255, 220, 140) });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentSolar, 10)
                .AddIngredient(ItemID.LunarBar, 6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
