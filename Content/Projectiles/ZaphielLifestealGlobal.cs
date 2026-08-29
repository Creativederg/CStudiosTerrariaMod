using CStudios.Content.Systems.ZaphielModules;
using Terraria;
using Terraria.ModLoader;

namespace CStudios.Content.Projectiles
{
    public class ZaphielLifestealGlobal : GlobalProjectile
    {
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                return;

            Player player = Main.player[projectile.owner];
            if (!player.active || player.dead)
                return;

            // Only when holding Omega (or Apex if you want)
            if (player.HeldItem.ModItem is not Items.Weapons.Summon.ZaphielElectaOmega
                && player.HeldItem.ModItem is not Items.Weapons.Summon.ZaphielElectaApex)
                return;

            var ctx = ZaphielModuleSystem.Resolve(player);
            if (ctx.LifestealFraction <= 0f || damageDone <= 0)
                return;

            int heal = (int)(damageDone * ctx.LifestealFraction);
            if (heal < 1)
                heal = 1;

            player.statLife += heal;
            if (player.statLife > player.statLifeMax2)
                player.statLife = player.statLifeMax2;

            player.HealEffect(heal, true);
        }
    }
}