using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModStigmaResonance : ZaphielModuleItem
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
            ctx.ScoreMode = true;
            ctx.StigmaResonanceActive = true;
            ctx.StigmaBonusMul *= 1.30f;
            ctx.HomingMul *= 1.15f;
            ctx.CritAdd += 4;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "ScoreDesc",
                "Score & Stigma – Stigma Resonance\n" +
                "Marked enemies take bonus damage. Score on marked targets builds faster.")
            { OverrideColor = new Color(255, 210, 80) });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentNebula, 8)
                .AddIngredient(ItemID.SoulofSight, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
