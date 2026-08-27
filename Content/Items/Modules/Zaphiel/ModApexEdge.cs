using CStudios.Content.Systems.ZaphielModules;
using Terraria.ID;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    /// <summary>Spearhead — energy blade; weapon + Psybits go melee.</summary>
    public class ModApexEdge : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.Spearhead;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.MeleeMode = true;
            ctx.ContinuousBeam = false;
            ctx.OverrideProjectileType = 0;

            ctx.DamageMul *= 1.15f;
            ctx.MeleeRangeMul = 1.25f;
            ctx.MeleeSizeMul = 1.2f;
            ctx.MeleeStrikeInterval = 14;

            // Minions close in and use melee strikes (Psybits.cs already reads these)
            ctx.MinionDamageMul *= 1.2f;
            ctx.MinionMoveSpeedMul *= 1.25f;
            ctx.MinionAggressiveChase = true;
        }
    }
}