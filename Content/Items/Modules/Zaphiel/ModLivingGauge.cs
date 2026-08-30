using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModLivingGauge : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.Psybits;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 28);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.ScoreMode = true;
            ctx.LivingGaugeActive = true;
            ctx.ScoreGainMul *= 1.20f;
            ctx.MinionDamageMul *= 1.10f;
            ctx.MinionFireRangeMul *= 1.10f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "ScoreDesc",
                "Score & Stigma – Living Gauge\n" +
                "Bits generate Score on hit. Their damage scales with current Score.")
            { OverrideColor = new Color(255, 210, 80) });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentStardust, 10)
                .AddIngredient(ItemID.FragmentSolar, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
