using CStudios.Content.Projectiles.Summon.Psybits;
using CStudios.Content.Systems.ZaphielModules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    /// <summary>Spearhead — sustained energy beam from the tip.</summary>
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
            // Exclusive tip mode
            ctx.MeleeMode = false;
            ctx.ContinuousBeam = true;

            // Prefer your visible continuous beam (reliable).
            // Optional: try CWR prism laser first, then fall back.
            if (ModLoader.TryGetMod("CalamityOverhaul", out Mod cwr)
                && cwr.TryFind("CyberPrismLaserProj", out ModProjectile laser))
            {
                ctx.OverrideProjectileType = laser.Type;
            }
            else
            {
                ctx.OverrideProjectileType = ProjectileType<PsybitPlayerBeam>();
            }

            ctx.DamageMul *= 1.08f;
            ctx.ManaCostMul *= 1.1f;
        }
    }
}