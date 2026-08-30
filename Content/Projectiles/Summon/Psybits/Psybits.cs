using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System;
using CStudios.Content.Buffs;
using CStudios.Content.DamageClasses;
using CStudios.Content.Systems.ZaphielModules;
using CStudios.Content.Systems.ZaphielModules.Authority;
using CStudios.Content.Utilities;

namespace CStudios.Content.Projectiles.Summon.Psybits
{
    public class Psybits : ModProjectile
    {
        public ref float PsybitID => ref Projectile.ai[2];

        float damageBonus = 1f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 11;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.DamageType = GetInstance<PsychokineticDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
        }

        public override bool? CanCutTiles() => false;

        public override bool MinionContactDamage()
        {
            Player owner = Main.player[Projectile.owner];
            return ZaphielModuleSystem.Resolve(owner).MeleeMode;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            bool overcharged = owner.HasBuff(BuffType<PsybitOvercharge>());

            default(Content.Effects.RedTrail).Draw(Projectile);

            if (!overcharged)
            {
                Texture2D sheet = TextureAssets.Projectile[Type].Value;
                int frameHeight = sheet.Height / Main.projFrames[Type];
                Rectangle frame = new Rectangle(0, frameHeight * Projectile.frame, sheet.Width, frameHeight);
                Vector2 origin = frame.Size() / 2f;
                SpriteEffects effects = Projectile.spriteDirection == -1
                    ? SpriteEffects.FlipHorizontally
                    : SpriteEffects.None;
                Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                float alpha = (255 - Projectile.alpha) / 255f;

                Main.EntitySpriteDraw(sheet, drawPos, frame, lightColor * alpha,
                    Projectile.rotation, origin, Projectile.scale, effects, 0);
                return false;
            }

            Texture2D[] bits = new Texture2D[11];
            for (int i = 0; i < 11; i++)
                bits[i] = Request<Texture2D>($"CStudios/Content/Projectiles/Summon/Psybits/Psybit{i + 1}").Value;

            int[] map = { 3, 4, 5, 0, 1, 2, 6, 7, 8, 9, 10 };
            float spread = 6f;
            Vector2[] finalOffsets =
            {
                new Vector2(0f, -spread), new Vector2(0f, spread), new Vector2(spread, 0f),
                new Vector2(0f, -spread * 0.5f), new Vector2(0f, spread * 0.5f), new Vector2(-spread * 0.5f, 0f),
                new Vector2(spread * 0.35f, -spread * 0.35f), new Vector2(-spread * 0.35f, 0f),
                new Vector2(-spread * 0.35f, spread * 0.25f), new Vector2(0f, spread * 0.75f), new Vector2(0f, -spread * 0.75f),
            };
            Vector2[] startOffsets = new Vector2[11];
            for (int i = 0; i < 11; i++)
                startOffsets[i] = finalOffsets[i] * 5f;

            float assemble = MathHelper.Clamp(Projectile.localAI[0], 0f, 1f);
            float rotation = MathHelper.PiOver2;
            int targetId = (int)Projectile.localAI[1];
            if (targetId >= 0 && targetId < Main.maxNPCs && Main.npc[targetId].active)
            {
                Vector2 toTarget = Main.npc[targetId].Center - Projectile.Center;
                if (toTarget.LengthSquared() > 1f)
                    rotation = toTarget.ToRotation() + MathHelper.Pi;
            }

            SpriteEffects spriteEffects = Projectile.spriteDirection <= 0
                ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color drawColor = new Color(255, 90, 90) * ((255 - Projectile.alpha) / 255f);
            float scale = Projectile.scale * (1.1f + 0.04f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 8f));
            Vector2 center = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            for (int i = 0; i < 11; i++)
            {
                float layerDelay = i * 0.04f;
                float layerT = MathHelper.Clamp((assemble - layerDelay) / Math.Max(0.01f, 1f - layerDelay * 0.5f), 0f, 1f);
                layerT = EaseHelper.InOutQuad(layerT);

                Vector2 useOff = Vector2.Lerp(startOffsets[i], finalOffsets[i], layerT);
                Vector2 rotatedOffset = useOff.RotatedBy(rotation - MathHelper.Pi);
                Texture2D tex = bits[map[i]];

                Main.EntitySpriteDraw(tex, center + rotatedOffset, tex.Frame(), drawColor * layerT,
                    rotation, tex.Frame().Size() / 2f, scale, spriteEffects, 0);
            }

            int coreIndex = Utils.Clamp((int)PsybitID, 0, 10);
            Main.EntitySpriteDraw(bits[coreIndex], center, bits[coreIndex].Frame(),
                new Color(255, 40, 40, 0) * (0.45f * assemble),
                rotation, bits[coreIndex].Frame().Size() / 2f, scale * 1.25f, spriteEffects, 0);

            return false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            if (!CheckActive(owner))
                return;

            Projectile.frame = (int)PsybitID;
            ZaphielShootContext ctx = ZaphielModuleSystem.Resolve(owner);

            Projectile.minionSlots = ctx.MinionSlotsPerBit > 0f ? ctx.MinionSlotsPerBit : 1f;

            if (owner.HasBuff(BuffType<PsybitOvercharge>()))
            {
                Projectile.localAI[0] += 0.035f;
                if (Projectile.localAI[0] > 1f)
                    Projectile.localAI[0] = 1f;
            }
            else
                Projectile.localAI[0] = 0f;

            bool inPattern = AuthorityPatternAI.TryRunPatternAI(Projectile, owner, ctx);

            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SearchForTargets(owner, ctx, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);

            // Prefer the Authority locked target while a pattern is running
            if (inPattern)
            {
                var ap = owner.GetModPlayer<ZaphielAuthorityPlayer>();
                if (ap.LockedTargetWhoAmI >= 0 && ap.LockedTargetWhoAmI < Main.maxNPCs)
                {
                    NPC npc = Main.npc[ap.LockedTargetWhoAmI];
                    if (npc.active && npc.CanBeChasedBy())
                    {
                        foundTarget = true;
                        targetCenter = npc.Center;
                        distanceFromTarget = Vector2.Distance(Projectile.Center, targetCenter);
                        Projectile.localAI[1] = npc.whoAmI;
                    }
                }
                else if (ap.LockedWorldPosition != Vector2.Zero)
                {
                    foundTarget = true;
                    targetCenter = ap.LockedWorldPosition;
                    distanceFromTarget = Vector2.Distance(Projectile.Center, targetCenter);
                }
            }
            else
            {
                Movement(owner, ctx, foundTarget, distanceFromTarget, targetCenter,
                    distanceToIdlePosition, vectorToIdlePosition);
            }

            Visuals();
            HandleOffense(owner, ctx, foundTarget, distanceFromTarget, targetCenter);

            Projectile.alpha -= 3;
            if (Projectile.scale < 0)
                Projectile.alpha = 255;

            Projectile.scale += 0.02f;
            Projectile.scale = Math.Clamp(Projectile.scale, -1f, 1f);

            if (Projectile.scale >= 1f)
                Projectile.ai[0]++;
            else
                Projectile.ai[0] = 0;

            if (owner.HasBuff(BuffType<PsybitBeamAttack>()))
            {
                if (Projectile.alpha <= 200)
                {
                    for (int d = 0; d < 5; d++)
                    {
                        int dustIndex = Dust.NewDust(Projectile.Center, 0, 0, DustID.GemSapphire,
                            Main.rand.Next(-1, 1), Main.rand.Next(-1, 1), 0, default, 2f);
                        Main.dust[dustIndex].noGravity = true;
                    }
                }
                Projectile.alpha = 255;
                Projectile.ai[0] = Main.rand.Next(-20, 20);
                Projectile.scale = -0.5f;
                Projectile.ai[1] = Main.rand.Next(0, 360);
            }

            if (owner.HasBuff(BuffType<PsybitOvercharge>()) && Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric,
                    Main.rand.NextVector2Circular(2f, 2f), 100, new Color(255, 40, 40), 1.1f);
                d.noGravity = true;
            }
        }

        private void HandleOffense(Player owner, ZaphielShootContext ctx, bool foundTarget,
            float distanceFromTarget, Vector2 targetCenter)
        {
            if (owner.HasBuff(BuffType<PsybitBeamAttack>()))
                return;

            if (ctx.MeleeMode)
            {
                ClearLinkedBeams(owner);
                if (!foundTarget)
                    return;

                float meleeRange = 100f * ctx.MeleeRangeMul;
                if (distanceFromTarget > meleeRange)
                    return;

                int interval = Math.Max(8, ctx.MeleeStrikeInterval);
                if (Projectile.ai[0] < interval)
                    return;
                Projectile.ai[0] = 0;

                if (Projectile.owner == Main.myPlayer)
                {
                    int dmg = Math.Max(1, (int)(Projectile.damage * ctx.MinionDamageMul * ctx.DamageMul * damageBonus));
                    Vector2 toTarget = (targetCenter - Projectile.Center).SafeNormalize(Vector2.UnitX);
                    int idx = Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(), Projectile.Center, toTarget,
                        ProjectileType<PsybitUnchargedLaser>(), dmg, 2f, owner.whoAmI);
                    if (idx >= 0)
                    {
                        Main.projectile[idx].timeLeft = 10;
                        Main.projectile[idx].penetrate = -1;
                        Main.projectile[idx].velocity *= 0.2f;
                    }
                }
                return;
            }

            if (ctx.MinionVolleyShot)
            {
                ClearLinkedBeams(owner);

                float maxRange = 900f * ctx.MinionFireRangeMul;
                if (!foundTarget || distanceFromTarget >= maxRange)
                    return;

                int interval = 22;
                if (Projectile.ai[0] < interval)
                    return;
                Projectile.ai[0] = 0;

                if (Projectile.owner != Main.myPlayer)
                    return;

                int dmg = Math.Max(1, (int)(Projectile.damage * ctx.MinionDamageMul * ctx.DamageMul * damageBonus));
                Vector2 shot = (targetCenter - Projectile.Center).SafeNormalize(Vector2.UnitX) * 13f;

                int projType = ProjectileType<PsybitUnchargedLaser>();
                if (ModLoader.TryGetMod("CalamityOverhaul", out Mod cwr)
                    && cwr.TryFind("CyberTraceBeamProj", out ModProjectile beam))
                    projType = beam.Type;

                int idx = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center, shot,
                    projType, dmg, 1.5f, owner.whoAmI,
                    ai0: Main.rand.Next(3));

                if (idx >= 0 && idx < Main.maxProjectiles)
                {
                    Main.projectile[idx].ai[1] = ctx.HomingMul > 0f ? ctx.HomingMul : 1f;
                    Main.projectile[idx].penetrate += ctx.ExtraPierce;
                    Main.projectile[idx].originalDamage = Projectile.damage;
                }
                return;
            }

            float beamRange = 900f * ctx.MinionFireRangeMul;
            if (!foundTarget || distanceFromTarget >= beamRange)
                return;

            bool overcharged = owner.HasBuff(BuffType<PsybitOvercharge>());
            bool forceCharged = overcharged || ctx.MinionUseChargedBeam;
            int beamType = forceCharged
                ? ProjectileType<PsybitMinionChargedBeam>()
                : ProjectileType<PsybitMinionBeam>();

            if (!overcharged && ctx.OverrideProjectileType > 0)
                beamType = ctx.OverrideProjectileType;

            bool hasBeam = false;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.owner != Projectile.owner)
                    continue;
                if ((int)p.ai[1] != Projectile.whoAmI)
                    continue;

                if (p.type == ProjectileType<PsybitMinionBeam>()
                    || p.type == ProjectileType<PsybitMinionChargedBeam>()
                    || (ctx.OverrideProjectileType > 0 && p.type == ctx.OverrideProjectileType))
                {
                    if (p.type != beamType)
                        p.Kill();
                    else
                        hasBeam = true;
                    break;
                }
            }

            if (!hasBeam && Projectile.owner == Main.myPlayer)
            {
                float mult = (overcharged ? 2.5f : damageBonus) * ctx.MinionDamageMul * ctx.DamageMul;
                int index = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center, Vector2.Zero,
                    beamType,
                    Math.Max(1, (int)(Projectile.damage * mult)),
                    0f, owner.whoAmI,
                    0f, Projectile.whoAmI);
                if (index >= 0)
                {
                    Main.projectile[index].originalDamage = Projectile.damage;
                    Main.projectile[index].penetrate += ctx.ExtraPierce;
                }
            }
        }

        private void ClearLinkedBeams(Player owner)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.owner != owner.whoAmI)
                    continue;
                if ((int)p.ai[1] != Projectile.whoAmI)
                    continue;
                if (p.type == ProjectileType<PsybitMinionBeam>()
                    || p.type == ProjectileType<PsybitMinionChargedBeam>())
                    p.Kill();
            }
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
                return false;
            if (owner.HasBuff(BuffType<PsybitDefensiveArray>()))
                Projectile.timeLeft = 2;
            return true;
        }

        private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            Vector2 idlePosition = owner.Center;
            idlePosition.Y -= 48f;
            idlePosition.X += (10 + Projectile.minionPos * 40) * -owner.direction;

            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            float overlapVelocity = 0.04f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];
                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner
                    && Math.Abs(Projectile.position.X - other.position.X)
                     + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X) Projectile.velocity.X -= overlapVelocity;
                    else Projectile.velocity.X += overlapVelocity;
                    if (Projectile.position.Y < other.position.Y) Projectile.velocity.Y -= overlapVelocity;
                    else Projectile.velocity.Y += overlapVelocity;
                }
            }
        }

        private void SearchForTargets(Player owner, ZaphielShootContext ctx,
            out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
        {
            float baseRange = ctx.MinionAggressiveChase ? 1400f : 1200f;
            distanceFromTarget = baseRange;
            targetCenter = Projectile.Center;
            foundTarget = false;
            int targetIndex = -1;
            float rangeMul = ctx.MinionFireRangeMul;

            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy())
                {
                    float between = Vector2.Distance(npc.Center, Projectile.Center);
                    if (between < 2000f * rangeMul)
                    {
                        distanceFromTarget = between;
                        targetCenter = npc.Center;
                        foundTarget = true;
                        targetIndex = npc.whoAmI;
                    }
                }
            }

            if (!foundTarget)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.CanBeChasedBy() || !npc.HasBuff(BuffType<PsybitMarked>()))
                        continue;
                    float between = Vector2.Distance(npc.Center, Projectile.Center);
                    if (between < distanceFromTarget)
                    {
                        distanceFromTarget = between;
                        targetCenter = npc.Center;
                        foundTarget = true;
                        targetIndex = npc.whoAmI;
                    }
                }
            }

            if (!foundTarget && Projectile.localAI[1] >= 0)
            {
                int last = (int)Projectile.localAI[1];
                if (last >= 0 && last < Main.maxNPCs)
                {
                    NPC npc = Main.npc[last];
                    if (npc.active && npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        float leash = (ctx.MinionAggressiveChase ? 2000f : 1500f) * rangeMul;
                        if (between < leash)
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                            targetIndex = npc.whoAmI;
                        }
                    }
                }
            }

            if (!foundTarget)
            {
                float fallback = 800f * rangeMul;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.CanBeChasedBy())
                        continue;
                    float between = Vector2.Distance(npc.Center, Projectile.Center);
                    if (between < fallback && between < distanceFromTarget)
                    {
                        distanceFromTarget = between;
                        targetCenter = npc.Center;
                        foundTarget = true;
                        targetIndex = npc.whoAmI;
                    }
                }
            }

            Projectile.localAI[1] = foundTarget ? targetIndex : -1;
        }

        private void Movement(Player owner, ZaphielShootContext ctx, bool foundTarget,
            float distanceFromTarget, Vector2 targetCenter,
            float distanceToIdlePosition, Vector2 vectorToIdlePosition)
        {
            bool overcharged = owner.HasBuff(BuffType<PsybitOvercharge>());
            float moveMul = ctx.MinionMoveSpeedMul;
            float orbitMul = ctx.MinionOrbitRadiusMul;

            if (foundTarget && ctx.MinionRandomOrbit)
            {
                float engage = ctx.MeleeMode ? 90f : 320f;

                if (distanceFromTarget > engage)
                {
                    float speed = 18f * moveMul;
                    float inertia = 12f;
                    Vector2 direction = (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + direction) / inertia;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
                else
                {
                    if (Projectile.localAI[2] <= 0f)
                    {
                        float radius = (70f + Main.rand.NextFloat(50f, 160f)) * orbitMul;
                        float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                        Vector2 goal = targetCenter + angle.ToRotationVector2() * radius;
                        float speed = 15f * moveMul;
                        Projectile.velocity = (goal - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;
                        Projectile.localAI[2] = Main.rand.Next(18, 45);
                    }
                    else
                    {
                        Projectile.localAI[2] -= 1f;
                        Vector2 pull = (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero) * 0.4f;
                        Projectile.velocity += pull;
                        float maxSpd = 17f * moveMul;
                        if (Projectile.velocity.Length() > maxSpd)
                            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * maxSpd;
                    }

                    if (Projectile.velocity.LengthSquared() > 0.1f)
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
                return;
            }

            float engageDistance = ctx.MeleeMode ? 90f : 300f;

            if (foundTarget)
            {
                if (distanceFromTarget > engageDistance)
                {
                    float speed = (ctx.MeleeMode ? 20f : 16f) * moveMul;
                    float inertia = 12f;
                    Vector2 direction = (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + direction) / inertia;
                    Projectile.rotation = Vector2.Normalize(targetCenter - Projectile.Center).ToRotation()
                        + MathHelper.ToRadians(90f);
                }
                else if (PsybitID <= 6)
                {
                    double deg = Projectile.ai[1];
                    double rad = deg * (Math.PI / 180);
                    double dist = (100 + PsybitID * 15) * orbitMul;
                    Vector2 adjustedPosition = targetCenter;

                    if (PsybitID <= 3)
                        Projectile.position.Y = adjustedPosition.Y - (int)(Math.Sin(rad) * dist / 2) - Projectile.height / 2;
                    else
                        Projectile.position.Y = adjustedPosition.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;

                    Projectile.position.X = adjustedPosition.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;

                    if (PsybitID <= 2) Projectile.ai[1] -= 0.9f;
                    else Projectile.ai[1] += 0.9f;

                    Projectile.rotation = Vector2.Normalize(targetCenter - Projectile.Center).ToRotation()
                        + MathHelper.ToRadians(90f);
                }
                else
                {
                    double deg = Projectile.ai[1];
                    double rad = deg * (Math.PI / 180);
                    double dist = (100 + PsybitID * 10) * orbitMul;
                    Vector2 adjustedPosition = targetCenter;

                    if (PsybitID >= 9)
                        Projectile.position.X = adjustedPosition.X - (int)(Math.Cos(rad) * dist / 2) - Projectile.width / 2;
                    else
                        Projectile.position.X = adjustedPosition.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;

                    Projectile.position.Y = adjustedPosition.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;

                    if (PsybitID >= 10) Projectile.ai[1] += 0.9f;
                    else Projectile.ai[1] -= 0.9f;

                    Projectile.rotation = Vector2.Normalize(targetCenter - Projectile.Center).ToRotation()
                        + MathHelper.ToRadians(90f);
                }
            }
            else
            {
                double deg = Projectile.ai[1];
                double rad = deg * (Math.PI / 180);
                double dist = 100 * orbitMul;
                Vector2 adjustedPosition = owner.Center;

                if (PsybitID >= 9)
                    Projectile.position.X = adjustedPosition.X - (int)(Math.Cos(rad) * dist / 2) - Projectile.width / 2;
                else
                    Projectile.position.X = adjustedPosition.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;

                if (PsybitID <= 3)
                    Projectile.position.Y = adjustedPosition.Y - (int)(Math.Sin(rad) * dist / 2) - Projectile.height / 2;
                else
                    Projectile.position.Y = adjustedPosition.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;

                if ((int)PsybitID % 2 == 0) Projectile.ai[1] += 0.9f;
                else Projectile.ai[1] -= 0.9f;

                if (overcharged)
                    Projectile.rotation = MathHelper.PiOver2;
                else
                    Projectile.rotation = Vector2.Normalize(Main.MouseWorld - Projectile.Center).ToRotation()
                        + MathHelper.ToRadians(90f);
            }
        }

        private void Visuals()
        {
            if (Projectile.velocity.X > 0f)
                Projectile.spriteDirection = Projectile.direction = -1;
            else if (Projectile.velocity.X < 0f)
                Projectile.spriteDirection = Projectile.direction = 1;
        }
    }
}