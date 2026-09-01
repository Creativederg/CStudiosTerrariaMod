using CStudios.Content.DamageClasses;
using CStudios.Content.Systems.ZaphielModules;
using CStudios.Content.Systems.ZaphielModules.Finality;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Projectiles.Summon.Psybits
{
    public class FinalityTurret : ModProjectile
    {
        public override string Texture => "CStudios/Content/Projectiles/Summon/Psybits/PsybitGunCharged";

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 54;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 12 * 60 + 30;
            Projectile.minion = false;
            Projectile.minionSlots = 0f;
            Projectile.DamageType = GetInstance<PsychokineticDamageClass>();
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner == null || !owner.active || owner.dead)
            {
                Projectile.Kill();
                return;
            }

            var fin = owner.GetModPlayer<ZaphielFinalityPlayer>();
            if (!fin.FinalityActive)
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 8;
            Projectile.velocity *= 0.94f;
            Projectile.position.Y += (float)System.Math.Sin(Main.GameUpdateCount * 0.08f + Projectile.whoAmI) * 0.35f;

            NPC target = FindTarget(700f);
            Projectile.localAI[1] = target != null ? target.whoAmI : -1;
            if (target != null)
            {
                Vector2 to = target.Center - Projectile.Center;
                Projectile.rotation = to.ToRotation();
                Projectile.spriteDirection = to.X < 0f ? -1 : 1;
            }

            var ctx = ZaphielModuleSystem.Resolve(owner);
            if (Projectile.owner == Main.myPlayer && target != null)
            {
                EnsureChargedBeam(owner, ctx);

                Projectile.localAI[0] += 1f;
                int interval = ctx.OriginRelayActive ? 14 : 18;
                if (Projectile.localAI[0] >= interval)
                {
                    Projectile.localAI[0] = 0f;
                    FireCWRVolley(owner, ctx, target);
                }
            }
        }

        private NPC FindTarget(float range)
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC n = Main.npc[owner.MinionAttackTargetNPC];
                if (n.CanBeChasedBy() && Vector2.Distance(n.Center, Projectile.Center) < range * 1.4f)
                    return n;
            }

            NPC best = null;
            float bestD = range;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || !npc.CanBeChasedBy())
                    continue;
                float d = Vector2.Distance(npc.Center, Projectile.Center);
                if (d < bestD)
                {
                    bestD = d;
                    best = npc;
                }
            }
            return best;
        }

        private void EnsureChargedBeam(Player owner, ZaphielShootContext ctx)
        {
            int beamType = ProjectileType<PsybitMinionChargedBeam>();
            bool has = false;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.owner != Projectile.owner)
                    continue;
                if (p.type == beamType && (int)p.ai[1] == Projectile.whoAmI)
                {
                    has = true;
                    break;
                }
            }

            if (has)
                return;

            int dmg = System.Math.Max(1, (int)(owner.GetWeaponDamage(owner.HeldItem) * 0.70f * ctx.DamageMul));
            int idx = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center, Vector2.Zero,
                beamType, dmg, 0f, owner.whoAmI,
                0f, Projectile.whoAmI);
            if (idx >= 0)
            {
                Main.projectile[idx].originalDamage = dmg;
                Main.projectile[idx].minion = false;
                Main.projectile[idx].minionSlots = 0f;
            }
            SoundEngine.PlaySound(SoundID.Item15 with { Volume = 0.35f, Pitch = 0.2f }, Projectile.Center);
        }


        private void FireCWRVolley(Player owner, ZaphielShootContext ctx, NPC target)
        {
            Vector2 aim = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
            int dmg = System.Math.Max(1, (int)(owner.GetWeaponDamage(owner.HeldItem) * 0.28f * ctx.DamageMul));

            int projType = ProjectileType<PsybitUnchargedLaser>();
            bool cwr = false;
            if (ModLoader.TryGetMod("CalamityOverhaul", out Mod cwrMod)
                && cwrMod.TryFind("CyberTraceBeamProj", out ModProjectile beam))
            {
                projType = beam.Type;
                cwr = true;
            }

            int shots = 3;
            for (int i = 0; i < shots; i++)
            {
                float spread = (i - (shots - 1) * 0.5f) * 0.10f;
                Vector2 vel = aim.RotatedBy(spread) * (14f + Main.rand.NextFloat(0f, 2f));
                int idx = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center, vel, projType,
                    dmg, 1.2f, owner.whoAmI,
                    ai0: cwr ? Main.rand.Next(3) : 0);
                if (idx >= 0 && idx < Main.maxProjectiles)
                {
                    Main.projectile[idx].timeLeft = 28;
                    Main.projectile[idx].scale = 0.95f;
                    if (cwr)
                        Main.projectile[idx].ai[1] = ctx.HomingMul > 0f ? ctx.HomingMul : 1f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 230, 160, 180);
        }
    }
}
