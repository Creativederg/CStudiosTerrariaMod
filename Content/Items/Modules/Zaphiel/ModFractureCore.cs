using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModFractureCore : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.EnergyCore;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 32);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.FractureMode = true;
            ctx.FractureCoreActive = true;
            ctx.FractureDurationMul *= 1.15f;
            ctx.DamageMul *= 1.08f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "FracDesc",
                "Time Fracture – Fracture Core\nUltimate snaps time. Enemies near you slow. You and the bits speed up.")
            { OverrideColor = new Color(180, 140, 255) });
            tooltips.Add(new TooltipLine(Mod, "FracHint",
                "Press Ultimate with bits summoned.")
            { OverrideColor = new Color(220, 200, 255) });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentNebula, 10)
                .AddIngredient(ItemID.FragmentStardust, 8)
                .AddIngredient(ItemID.LunarBar, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
