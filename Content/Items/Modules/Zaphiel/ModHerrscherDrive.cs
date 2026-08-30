using CStudios.Content.Systems.ZaphielModules;
using CStudios.Content.Items.Modules.Zaphiel;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModHerrscherDrive : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.EnergyCore;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.buyPrice(gold: 30);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.AerialMode = true;
            ctx.HerrscherDriveActive = true;

            ctx.AerialFormDurationMul *= 1.25f;
            ctx.AerialMoveMul *= 1.35f;
            ctx.DamageMul *= 1.10f;
            ctx.MinionDamageMul *= 1.12f;
            ctx.CritAdd += 4;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "AerialDesc",
                "Aerial Herrscher Form – Herrscher Drive\n" +
                "Ultimate key activates Aerial Herrscher Form: strong air control and empowered attacks.\n" +
                "Builds a strain gauge. Form lasts longer with this core.")
            {
                OverrideColor = new Color(120, 200, 255)
            });
            tooltips.Add(new TooltipLine(Mod, "AerialHint",
                "Press Ultimate while bits are active to enter the Form.")
            {
                OverrideColor = new Color(180, 230, 255)
            });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentVortex, 10)
                .AddIngredient(ItemID.FragmentNebula, 8)
                .AddIngredient(ItemID.LunarBar, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
