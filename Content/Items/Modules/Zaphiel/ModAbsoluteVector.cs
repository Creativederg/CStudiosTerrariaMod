using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModAbsoluteVector : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.Guidance;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.AuthorityMode = true;
            ctx.AbsoluteVectorActive = true;

            ctx.AuthorityFormationIntegrity = 1.75f;
            ctx.MinionOrbitRadiusMul *= 0.85f;
            ctx.MinionMoveSpeedMul *= 1.10f;
            ctx.HomingMul *= 1.35f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "AuthorityDesc",
                "Authority of the Bits – Absolute Vector\n" +
                "Psybits lock into rigid geometric formations.\n" +
                "Formations hold shape even under heavy pressure and high speed.")
            {
                OverrideColor = new Color(180, 120, 255)
            });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentStardust, 6)
                .AddIngredient(ItemID.SoulofSight, 12)
                .AddIngredient(ItemID.Lens, 5)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}