using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CStudios.Content.DamageClasses;
using CStudios.Content.Systems.ZaphielModules;

namespace CStudios.Content.Projectiles.Summon.Psybits
{
    public class FinalitySlash : ModProjectile
    {
        public override string Texture => "CStudios/Content/Projectiles/Summon/Psybits/PsybitUnchargedLaser";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = ModContent.GetInstance<PsychokineticDamageClass>();
            Projectile.penetrate = 8;
            Projectile.timeLeft = 72;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.alpha = (int)MathHelper.Lerp(10f, 180f, 1f - Projectile.timeLeft / 72f);

            HomeTowardTarget();

            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame,
                    Projectile.velocity * 0.05f, 80, new Color(255, 220, 140), 1.2f);
                d.noGravity = true;
            }
        }

        private void HomeTowardTarget()
        {
            Player owner = Main.player[Projectile.owner];
            float homing = 1f;
            if (owner != null && owner.active)
            {
                var ctx = ZaphielModuleSystem.Resolve(owner);
                if (ctx.HomingMul > 0f)
                    homing = ctx.HomingMul;
            }

            NPC target = null;
            if (owner != null && owner.HasMinionAttackTargetNPC)
            {
                NPC n = Main.npc[owner.MinionAttackTargetNPC];
                if (n.CanBeChasedBy())
                    target = n;
            }

            if (target == null)
            {
                float best = 900f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || !npc.CanBeChasedBy())
                        continue;
                    float d = Vector2.Distance(npc.Center, Projectile.Center);
                    if (d < best)
                    {
                        best = d;
                        target = npc;
                    }
                }
            }

            if (target == null)
                return;

            float speed = Projectile.velocity.Length();
            if (speed < 1f)
                speed = 24f;

            Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Projectile.velocity) * speed;
            float turn = 0.12f * homing;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, MathHelper.Clamp(turn, 0.04f, 0.28f));
            if (Projectile.velocity.Length() > 0.1f)
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * speed;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Explode(target.Center);
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.penetrate <= 0)
                Explode(Projectile.Center);
        }

        private void Explode(Vector2 at)
        {
            if (Projectile.localAI[0] > 0f)
                return;
            Projectile.localAI[0] = 1f;

            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.45f, Pitch = 0.35f }, at);

            for (int i = 0; i < 18; i++)
            {
                Dust d = Dust.NewDustPerfect(at, DustID.GoldFlame,
                    Main.rand.NextVector2Circular(6f, 6f), 80, new Color(255, 220, 140), 1.6f);
                d.noGravity = true;
            }

            float radius = 96f;
            int boom = System.Math.Max(1, (int)(Projectile.damage * 0.55f));
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || !npc.CanBeChasedBy())
                    continue;
                if (Vector2.Distance(npc.Center, at) > radius)
                    continue;

                NPC.HitInfo info = new NPC.HitInfo
                {
                    Damage = boom,
                    Knockback = 3f,
                    HitDirection = npc.Center.X >= at.X ? 1 : -1,
                    DamageType = Projectile.DamageType
                };
                npc.StrikeNPC(info);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(Content.Effects.GoldTrail).Draw(Projectile);
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 230, 160, 80) * (1f - Projectile.alpha / 255f);
        }
    }
}
