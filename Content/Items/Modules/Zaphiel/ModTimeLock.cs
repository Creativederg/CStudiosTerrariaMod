using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModTimeLock : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.Guidance;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 22);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.FractureMode = true;
            ctx.TimeLockActive = true;
            ctx.HomingMul *= 1.20f;
            ctx.MinionFireRangeMul *= 1.15f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "FracDesc",
                "Time Fracture – Time Lock\nFracture focuses the nearest marked target. Bits home harder.")
            { OverrideColor = new Color(180, 140, 255) });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentNebula, 8)
                .AddIngredient(ItemID.SoulofSight, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
