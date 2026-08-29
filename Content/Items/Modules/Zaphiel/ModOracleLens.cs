using CStudios.Content.Systems.ZaphielModules;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    /// <summary>
    /// Guidance — stronger homing, mild life steal, bonus damage.
    /// </summary>
    public class ModOracleLens : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.Guidance;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(gold: 8);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.HomingMul *= 2.0f;       // stronger tracking (read by CWR ai[1] / your projs)
            ctx.DamageMul *= 1.12f;      // +12% damage
            ctx.LifestealFraction += 0.04f; // 4% of damage as heal (stacks additively if you add more later)
            ctx.SpreadMul *= 0.85f;      // slightly tighter shots
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofSight, 8)
                .AddIngredient(ItemID.HallowedBar, 6)
                .AddIngredient(ItemID.Lens, 3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}