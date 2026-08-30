using CStudios.Content.Systems.ZaphielModules;
using CStudios.Content.Items.Modules.Zaphiel;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModFunnelOverflow : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.EnergyRelay;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.buyPrice(gold: 24);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.AerialMode = true;
            ctx.FunnelOverflowActive = true;

            ctx.FunnelFireRateMul *= 1.40f;
            ctx.BeamCountAdd += 1;
            ctx.MinionDamageMul *= 1.10f;
            ctx.MinionFireRangeMul *= 1.10f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "AerialDesc",
                "Aerial Herrscher Form – Funnel Overflow\n" +
                "While the Form is active, bits fire independently in aerial patterns and leave trails.\n" +
                "Increased funnel fire rate.")
            {
                OverrideColor = new Color(120, 200, 255)
            });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentVortex, 8)
                .AddIngredient(ItemID.SoulofFlight, 10)
                .AddIngredient(ItemID.SoulofFright, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
