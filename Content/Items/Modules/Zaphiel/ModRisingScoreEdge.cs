using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModRisingScoreEdge : ZaphielModuleItem
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
            ctx.ScoreMode = true;
            ctx.RisingScoreEdgeActive = true;
            ctx.ScoreGainMul *= 1.25f;
            ctx.DamageMul *= 1.08f;
            ctx.AttackSpeedMul *= 1.06f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "ScoreDesc",
                "Score & Stigma – Rising Score Edge\n" +
                "Hits raise Score. At high Score the weapon hits harder and faster.")
            { OverrideColor = new Color(255, 210, 80) });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentSolar, 8)
                .AddIngredient(ItemID.SoulofMight, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
