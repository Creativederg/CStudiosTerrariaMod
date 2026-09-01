using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModPermetAfterimage : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.EnergyRelay;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 24);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.FractureMode = true;
            ctx.PermetAfterimageActive = true;
            ctx.MinionMoveSpeedMul *= 1.20f;
            ctx.AerialMoveMul *= 1.10f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "FracDesc",
                "Time Fracture – Permet Afterimage\nDuring Fracture you leave Permet echoes. Bits can fire from the last echo.")
            { OverrideColor = new Color(180, 140, 255) });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentVortex, 8)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
