using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModCocoonLock : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.Guidance;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(gold: 26);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.FinalityMode = true;
            ctx.CocoonLockActive = true;
            ctx.HomingMul *= 1.25f;
            ctx.MinionFireRangeMul *= 1.20f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "FinDesc",
                "Finality – Cocoon Lock\nBits and Origin slashes home on the locked target.")
            { OverrideColor = new Color(255, 220, 140) });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentNebula, 10)
                .AddIngredient(ItemID.SoulofSight, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
