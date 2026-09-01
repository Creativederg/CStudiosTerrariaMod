using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModCocoonBits : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.Psybits;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(gold: 32);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.FinalityMode = true;
            ctx.CocoonBitsActive = true;
            ctx.MinionDamageMul *= 1.15f;
            ctx.MinionMoveSpeedMul *= 1.20f;
            ctx.MaxBitsMul *= 1.0f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "FinDesc",
                "Finality – Cocoon Bits\nDuring Finality bits orbit tighter and fire charged beams.")
            { OverrideColor = new Color(255, 220, 140) });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentStardust, 12)
                .AddIngredient(ItemID.FragmentSolar, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
