using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModCascadeLink : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.EnergyRelay;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.AuthorityMode = true;
            ctx.CascadeLinkActive = true;

            ctx.AuthorityPatternPowerMul *= 1.15f;
            ctx.BeamCountAdd += 1;
            ctx.MinionDamageMul *= 1.10f;
            ctx.SpreadMul *= 1.10f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "AuthorityDesc",
                "Authority of the Bits – Cascade Link\n" +
                "After an ultimate pattern finishes, a second smaller pattern triggers automatically.\n" +
                "Increases overall pattern power and bit output.")
            {
                OverrideColor = new Color(180, 120, 255)
            });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentNebula, 8)
                .AddIngredient(ItemID.Wire, 40)
                .AddIngredient(ItemID.SoulofFright, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}