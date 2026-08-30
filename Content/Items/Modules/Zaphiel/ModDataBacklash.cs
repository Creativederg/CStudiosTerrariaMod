using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModDataBacklash : ZaphielModuleItem
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
            ctx.ScoreMode = true;
            ctx.DataBacklashActive = true;
            ctx.DataStormMildMul *= 0.65f;
            ctx.MinionDamageMul *= 1.08f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "ScoreDesc",
                "Score & Stigma – Data Backlash\n" +
                "Data Storm after a burst is milder. Bits keep firing through most of the storm.")
            { OverrideColor = new Color(255, 210, 80) });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentVortex, 8)
                .AddIngredient(ItemID.SoulofFright, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
