
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using System;
using Terraria.ModLoader;
using Terraria.Audio;
using CStudios.Content.Buffs;
using CStudios.Content.DamageClasses;
using CStudios.Content.Projectiles.Summon.Psybits;

namespace CStudios.Content.Projectiles.Summon.Psybits
{

    public class Psybits : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 11;
            //ProjectileID.Sets.Homing[Projectile.type] = true;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;        //The recording mode
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

            //ProjectileType<TakodachiRound>();
        }
        public ref float PsybitID => ref Projectile.ai[2];//As there are 11 Psybits, some act differently to keep things unique

        float damageBonus = 1f;
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            default(Content.Effects.RedTrail).Draw(Projectile);
            return true;
        }
        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return false;
        }

        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            if (!CheckActive(owner))
            {
                return;
            }
            // In AI(), after CheckActive:
            if (owner.HasBuff(BuffType<PsybitOvercharge>()))
            {
                // Visual: brighter / different frame, still same movement
                Projectile.alpha = 0;
                if (Main.rand.NextBool(2))
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric,
                        Main.rand.NextVector2Circular(2f, 2f), 100, new Color(255, 40, 40), 1.2f);
                    d.noGravity = true;
                }
            }

            Projectile.frame = (int)PsybitID;
            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
            Visuals();
            Projectile.alpha -= 3;
            if (Projectile.scale < 0)
            {
                Projectile.alpha = 255;
            }
            Projectile.scale += 0.02f;
            Projectile.scale = Math.Clamp(Projectile.scale, -1, 1);
            // Continuous beam while we have a target in range
            if (foundTarget && distanceFromTarget < 500f)
            {
                bool overcharged = owner.HasBuff(BuffType<PsybitOvercharge>());
                int beamType = overcharged
                    ? ProjectileType<PsybitMinionChargedBeam>()
                    : ProjectileType<PsybitMinionBeam>();

                bool hasBeam = false;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.owner == Projectile.owner
                        && (int)p.ai[1] == Projectile.whoAmI
                        && (p.type == ProjectileType<PsybitMinionBeam>()
                            || p.type == ProjectileType<PsybitMinionChargedBeam>()))
                    {
                        // Wrong mode still active (e.g. ultimate just started/ended) → replace
                        if (p.type != beamType)
                            p.Kill();
                        else
                            hasBeam = true;
                        break;
                    }
                }

                if (!hasBeam && Projectile.owner == Main.myPlayer)
                {
                    int index = Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center, Vector2.Zero,
                        beamType,
                        (int)(Projectile.damage * (overcharged ? 2.5f : damageBonus)), // stronger in ultimate
                        0f, owner.whoAmI,
                        0f, Projectile.whoAmI);
                    Main.projectile[index].originalDamage = Projectile.damage;
                }
            }
            if (Projectile.scale >= 1f)
            {
                Projectile.ai[0]++;
            }
            else
            {
                Projectile.ai[0] = 0;

            }
            if (owner.HasBuff(BuffType<PsybitBeamAttack>()))
            {
                if (Projectile.alpha <= 200)
                {
                    for (int d = 0; d < 5; d++)
                    {
                        int dustIndex = Dust.NewDust(Projectile.Center, 0, 0, DustID.GemSapphire, 0f + Main.rand.Next(-1, 1), 0f + Main.rand.Next(-1, 1), 0, default, 2f);
                        Main.dust[dustIndex].noGravity = true;
                    }
                }
                Projectile.alpha = 255;
                Projectile.ai[0] = Main.rand.Next(-20, 20);
                Projectile.ai[1] = Main.rand.Next(0, 360);
            }
            else
            {

            }
        }


        // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                return false;
            }

            if (owner.HasBuff(BuffType<PsybitDefensiveArray>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            Vector2 idlePosition = owner.Center;
            idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

            // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
            // The index is projectile.minionPos
            float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
            idlePosition.X += minionPositionOffsetX; // Go behind the player

            // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

            // Teleport to player if distance is too big
            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                // and then set netUpdate to true
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            // If your minion is flying, you want to do this independently of any conditions
            float overlapVelocity = 0.04f;

            // Fix overlap with other minions
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];

                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.X += overlapVelocity;
                    }

                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.Y += overlapVelocity;
                    }
                }
            }
        }

        private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
        {
            distanceFromTarget = 1200f;
            targetCenter = Projectile.Center;
            foundTarget = false;
            int targetIndex = -1;

            // 1) Player right-click target
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy())
                {
                    float between = Vector2.Distance(npc.Center, Projectile.Center);
                    if (between < 2000f)
                    {
                        distanceFromTarget = between;
                        targetCenter = npc.Center;
                        foundTarget = true;
                        targetIndex = npc.whoAmI;
                    }
                }
            }

            // 2) Marked enemies
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

            // 3) Stick to last target after mark expires
            if (!foundTarget && Projectile.localAI[1] >= 0)
            {
                int last = (int)Projectile.localAI[1];
                if (last >= 0 && last < Main.maxNPCs)
                {
                    NPC npc = Main.npc[last];
                    if (npc.active && npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        if (between < 1500f)
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                            targetIndex = npc.whoAmI;
                        }
                    }
                }
            }

            // 4) Fallback: any nearby enemy
            if (!foundTarget)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.CanBeChasedBy())
                        continue;

                    float between = Vector2.Distance(npc.Center, Projectile.Center);
                    if (between < 800f && between < distanceFromTarget)
                    {
                        distanceFromTarget = between;
                        targetCenter = npc.Center;
                        foundTarget = true;
                        targetIndex = npc.whoAmI;
                    }
                }
            }

            Projectile.localAI[1] = foundTarget ? targetIndex : -1;

            // friendly needs to be set to true so the minion can deal contact damage
            // friendly needs to be set to false so it doesn't damage things like target dummies while idling
            // Both things depend on if it has a target or not, so it's just one assignment here
            // You don't need this assignment if your minion is shooting things instead of dealing contact damage
            //Projectile.friendly = foundTarget;
        }

        private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition)
        {
            // Default movement parameters (here for attacking)
            float speed = 16f;
            float inertia = 12f;

            if (foundTarget)
            {
                // Minion has a target: attack (here, fly towards the enemy)
                if (distanceFromTarget > 300f)
                {
                    Vector2 direction = targetCenter - Projectile.Center;
                    direction.Normalize();
                    direction *= speed;

                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + direction) / inertia;
                }
                else
                {
                    //Orbit the enemy
                    if (PsybitID <= 6)
                    {
                        //Factors for calculations
                        double deg = Projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                        double rad = deg * (Math.PI / 180); //Convert degrees to radians
                        double dist = 100 + PsybitID * 15; //Distance away from the target
                        Vector2 adjustedPosition = targetCenter;

                        /*Position the player based on where the player is, the Sin/Cos of the angle times the /
						/distance for the desired distance away from the player minus the projectile's width   /
						/and height divided by two so the center of the projectile is at the right place.     */
                        if (PsybitID <= 3)
                        {
                            Projectile.position.Y = adjustedPosition.Y - (int)(Math.Sin(rad) * dist / 2) - Projectile.height / 2;

                        }
                        else
                        {
                            Projectile.position.Y = adjustedPosition.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;

                        }
                        Projectile.position.X = adjustedPosition.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;

                        //Increase the counter/angle in degrees by 1 point, you can change the rate here too, but the orbit may look choppy depending on the value
                        if (PsybitID <= 2)
                        {
                            Projectile.ai[1] -= 0.9f;

                        }
                        else
                        {
                            Projectile.ai[1] += 0.9f;

                        }

                        Projectile.rotation = Vector2.Normalize(targetCenter - Projectile.Center).ToRotation() + MathHelper.ToRadians(90f);
                    }
                    else
                    {
                        //Factors for calculations
                        double deg = Projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                        double rad = deg * (Math.PI / 180); //Convert degrees to radians
                        double dist = 100 + PsybitID * 10; //Distance away from the target
                        Vector2 adjustedPosition = targetCenter;

                        /*Position the player based on where the player is, the Sin/Cos of the angle times the /
						/distance for the desired distance away from the player minus the projectile's width   /
						/and height divided by two so the center of the projectile is at the right place.     */
                        if (PsybitID >= 9)
                        {
                            Projectile.position.X = adjustedPosition.X - (int)(Math.Cos(rad) * dist / 2) - Projectile.width / 2;

                        }
                        else
                        {
                            Projectile.position.X = adjustedPosition.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;

                        }
                        Projectile.position.Y = adjustedPosition.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;

                        //Increase the counter/angle in degrees by 1 point, you can change the rate here too, but the orbit may look choppy depending on the value
                        if (PsybitID >= 10)
                        {
                            Projectile.ai[1] += 0.9f;

                        }
                        else
                        {
                            Projectile.ai[1] -= 0.9f;

                        }

                        Projectile.rotation = Vector2.Normalize(targetCenter - Projectile.Center).ToRotation() + MathHelper.ToRadians(90f);
                    }


                }
            }
            else
            {

                //Projectile.alpha = 170;
                //Factors for calculations
                double deg = Projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / 180); //Convert degrees to radians
                double dist = 100; //Distance away from the target
                Vector2 adjustedPosition = Main.player[Projectile.owner].Center;

                /*Position the player based on where the player is, the Sin/Cos of the angle times the /
				/distance for the desired distance away from the player minus the projectile's width   /
				/and height divided by two so the center of the projectile is at the right place.     */
                if (PsybitID >= 9)
                {
                    Projectile.position.X = adjustedPosition.X - (int)(Math.Cos(rad) * dist / 2) - Projectile.width / 2;

                }
                else
                {
                    Projectile.position.X = adjustedPosition.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;

                }
                if (PsybitID <= 3)
                {
                    Projectile.position.Y = adjustedPosition.Y - (int)(Math.Sin(rad) * dist / 2) - Projectile.height / 2;

                }
                else
                {
                    Projectile.position.Y = adjustedPosition.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;

                }

                //Increase the counter/angle in degrees by 1 point, you can change the rate here too, but the orbit may look choppy depending on the value
                if (PsybitID % 2 == 0)
                {
                    Projectile.ai[1] += 0.9f;
                }
                else
                {
                    Projectile.ai[1] -= 0.9f;

                }

                Projectile.rotation = Vector2.Normalize(Main.MouseWorld - Projectile.Center).ToRotation() + MathHelper.ToRadians(90f);
            }
        }

        private void Visuals()
        {
            //Projectile.rotation = Projectile.velocity.X * 0.05f;

            //Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            // This is a simple "loop through all frames from top to bottom" animation
            int frameSpeed = 5;


            if (Projectile.velocity.X > 0f)
            {
                Projectile.spriteDirection = Projectile.direction = -1;
            }
            else if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = Projectile.direction = 1;
            }

            // Some visuals here
            //Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
    }
}

