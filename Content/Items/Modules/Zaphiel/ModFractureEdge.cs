using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModFractureEdge : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.Spearhead;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 25);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.FractureMode = true;
            ctx.FractureEdgeActive = true;
            ctx.DamageMul *= 1.10f;
            ctx.AttackSpeedMul *= 1.08f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "FracDesc",
                "Time Fracture – Fracture Edge\nHits during Fracture cut harder and briefly slow the target.")
            { OverrideColor = new Color(180, 140, 255) });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentSolar, 8)
                .AddIngredient(ItemID.SoulofFright, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
