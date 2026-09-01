using CStudios.Content.Projectiles.Summon.Psybits;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Systems.ZaphielModules.Score
{
    public class ScoreHitGlobal : GlobalProjectile
    {
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                return;

            Player player = Main.player[projectile.owner];
            if (player == null || !player.active)
                return;

            if (!IsScoreProjectile(projectile))
                return;

            var ctx = ZaphielModuleSystem.Resolve(player);
            if (!ctx.ScoreMode && !ctx.LivingGaugeActive && !ctx.RisingScoreEdgeActive && !ctx.FeedbackHeartActive)
                return;

            float amount = 2.4f;
            if (projectile.type == ProjectileType<PsybitPlayerBeam>())
                amount = 1.8f;
            else if (projectile.type == ProjectileType<PsybitMinionBeam>()
                  || projectile.type == ProjectileType<PsybitMinionChargedBeam>())
                amount = 1.1f;

            var sp = player.GetModPlayer<ZaphielScorePlayer>();
            sp.AddScore(amount, ctx);
            if (ctx.StigmaResonanceActive)
                sp.AddStigma(amount * 0.4f, ctx);
        }

        private static bool IsScoreProjectile(Projectile p)
        {
            int t = p.type;
            if (t == ProjectileType<PsybitPlayerBeam>()) return true;
            if (t == ProjectileType<PsybitMinionBeam>()) return true;
            if (t == ProjectileType<PsybitMinionChargedBeam>()) return true;
            if (t == ProjectileType<PsybitUnchargedLaser>()) return true;
            if (p.ModProjectile is AerialSkybladeSlash) return true;
            return false;
        }
    }
}