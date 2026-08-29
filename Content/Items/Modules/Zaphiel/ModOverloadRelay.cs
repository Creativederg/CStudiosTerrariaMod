using CStudios.Content.Systems.ZaphielModules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    /// <summary>
    /// Energy Relay — large damage boost, wider spread, extra beams (Splinter / CWR volleys).
    /// </summary>
    public class ModOverloadRelay : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.EnergyRelay;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            // Do not touch MeleeMode / ContinuousBeam / TraceVolleyMode (tip mode stays on Spearhead)

            ctx.DamageMul *= 1.45f;     // massive damage
            ctx.SpreadMul *= 1.75f;     // much wider spread
            ctx.BeamCountAdd += 3;      // +3 beams on volleys (3 → 6 with Splinter/CWR)

            ctx.ManaCostMul *= 1.25f;   // tradeoff so it isn't free
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                //.AddIngredient(ItemID.SoulofFury, 5)
                .AddIngredient(ItemID.HallowedBar, 8)
                .AddIngredient(ItemID.Wire, 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}