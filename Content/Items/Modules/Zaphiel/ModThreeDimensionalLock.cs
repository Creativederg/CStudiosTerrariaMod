using CStudios.Content.Systems.ZaphielModules;
using CStudios.Content.Items.Modules.Zaphiel;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModThreeDimensionalLock : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.Guidance;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.buyPrice(gold: 22);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.AerialMode = true;
            ctx.ThreeDimensionalLockActive = true;

            ctx.HomingMul *= 1.45f;
            ctx.MinionFireRangeMul *= 1.25f;
            ctx.MinionOrbitRadiusMul *= 1.20f;
            ctx.MinionAggressiveChase = true;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "AerialDesc",
                "Aerial Herrscher Form – Three-Dimensional Lock\n" +
                "Bits path in full 3D. They prefer attacks from above and below.\n" +
                "Greatly improved homing and fire range.")
            {
                OverrideColor = new Color(120, 200, 255)
            });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentVortex, 6)
                .AddIngredient(ItemID.SoulofSight, 12)
                .AddIngredient(ItemID.SoulofFlight, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
