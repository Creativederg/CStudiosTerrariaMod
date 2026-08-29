using CStudios.Content.Systems.ZaphielModules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    /// <summary>
    /// Psybits — minions fire a single volley bolt (not continuous beams)
    /// and fly randomly around the target.
    /// </summary>
    public class ModSwarmPattern : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.Psybits;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.MinionVolleyShot = true;
            ctx.MinionRandomOrbit = true;

            ctx.MinionMoveSpeedMul *= 1.2f;
            ctx.MinionFireRangeMul *= 1.1f;
            ctx.MinionAggressiveChase = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofFlight, 6)
                .AddIngredient(ItemID.HallowedBar, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}