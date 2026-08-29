using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModAuthorityCore : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.EnergyCore;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.AuthorityMode = true;
            ctx.AuthorityCoreActive = true;

            ctx.AuthorityPatternPowerMul *= 1.40f;
            ctx.AuthorityPatternCostMul *= 0.85f;

            ctx.DamageMul *= 1.12f;
            ctx.MinionDamageMul *= 1.18f;
            ctx.CritAdd += 5;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "AuthorityDesc",
                "Authority of the Bits – Authority Core\n" +
                "Unlocks ultimate bit patterns (Giant Lance, Binding Cage, Orbital Bombardment...).\n" +
                "Patterns are stronger and slightly cheaper to force.")
            {
                OverrideColor = new Color(180, 120, 255)
            });
            tooltips.Add(new TooltipLine(Mod, "AuthorityHint",
                "Press the Ultimate key while bits are active to cycle / force a pattern.")
            {
                OverrideColor = new Color(220, 180, 255)
            });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentNebula, 10)
                .AddIngredient(ItemID.FragmentStardust, 10)
                .AddIngredient(ItemID.LunarBar, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}