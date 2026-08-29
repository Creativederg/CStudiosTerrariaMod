using CStudios.Content.Systems.ZaphielModules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    /// <summary>
    /// Spearhead — switches left click from single beam to SHPC-style cyber trace volleys.
    /// </summary>
    public class ModSplinterBeam : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.Spearhead;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.MeleeMode = false;
            ctx.ContinuousBeam = false;
            ctx.TraceVolleyMode = true;
            ctx.OverrideProjectileType = 0;

            ctx.DamageMul *= 1.08f;
            ctx.SpreadMul *= 1f;
            ctx.BeamCountAdd += 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofLight, 3)
                .AddIngredient(ItemID.SoulofNight, 3)
                .AddIngredient(ItemID.HallowedBar, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}