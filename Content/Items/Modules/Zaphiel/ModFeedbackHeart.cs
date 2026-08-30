using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModFeedbackHeart : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.EnergyCore;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 30);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.ScoreMode = true;
            ctx.FeedbackHeartActive = true;
            ctx.ScoreGainMul *= 1.15f;
            ctx.DamageMul *= 1.10f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "ScoreDesc",
                "Score & Stigma – Feedback Heart\n" +
                "Ultimate dumps Score into a burst. High Score = bigger burst, then a short Data Storm.")
            { OverrideColor = new Color(255, 210, 80) });
            tooltips.Add(new TooltipLine(Mod, "ScoreHint",
                "Press Ultimate while bits are active to detonate Score.")
            { OverrideColor = new Color(255, 240, 160) });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentNebula, 10)
                .AddIngredient(ItemID.FragmentSolar, 8)
                .AddIngredient(ItemID.LunarBar, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
