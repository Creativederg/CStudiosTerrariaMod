using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModOriginRelay : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.EnergyRelay;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(gold: 28);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.FinalityMode = true;
            ctx.OriginRelayActive = true;
            ctx.AttackSpeedMul *= 1.12f;
            ctx.FunnelFireRateMul *= 1.25f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "FinDesc",
                "Finality – Origin Relay\nDuring Finality, fire rate and slash count climb.")
            { OverrideColor = new Color(255, 220, 140) });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentVortex, 10)
                .AddIngredient(ItemID.SoulofMight, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
