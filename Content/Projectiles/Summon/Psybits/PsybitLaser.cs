using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CStudios.Content.Buffs;
using CStudios.Content.NPCs;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Enums;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;
using Terraria.Graphics.Shaders;

namespace CStudios.Content.Projectiles.Summon.Psybits
{
    // The following laser shows a channeled ability, after charging up the laser will be fired
    // Using custom drawing, dust effects, and custom collision checks for tiles
    public class PsybitLaser : ModProjectile
    {
        // Use a different style for constant so it is very clear in code when a constant is used

        // The maximum charge value
        private const float MAX_CHARGE = 1f;
        //The distance charge particle from the player center
        private const float MOVE_DISTANCE = 88f;

        // How often the charged beam emits a pair of Calamity Overhaul SHPC (CyberTraceBeamProj) rounds.
        private const int ShpcBurstInterval = 10;
        // Perpendicular offset from beam centerline so shots leave the top and bottom edges.
        private const float ShpcEdgeOffset = 36f;
        // Wide initial fan so the pair visibly peels off the laser before curving back in.
        private const float ShpcPeelAngle = 0.42f;
        private const float ShpcSpeed = 16f;
        private const float ShpcDamageMul = 0.45f;
        // How hard flanking shots bend back toward the laser after the fan-out.
        private const float ShpcCurveLerp = 0.20f;
        private const float ShpcLookAhead = 110f;
        private const float ShpcAxisPull = 1.55f;

        // Identities of SHPC rounds this laser spawned, so we can steer them back toward the beam.
        private readonly List<int> _flankIdentities = new List<int>();

        // The actual distance is stored in the ai0 field
        // By making a property to handle this it makes our life easier, and the accessibility more readable
        public float Distance
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        // The actual charge value is stored in the localAI0 field
        public float Charge
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        // localAI[1] used as a fire timer for flanking SHPC rounds
        private float ShpcTimer
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }

        // Are we at max charge? With c#6 you can simply use => which indicates this is a get only property
        public bool IsAtMaxCharge => Charge == MAX_CHARGE;

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hide = false;
            Projectile.timeLeft = 900;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // We start drawing the laser if we have charged up
            if (IsAtMaxCharge)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                //ArmorShaderData data = GameShaders.Armor.GetSecondaryShader((byte)GameShaders.Armor.GetShaderIdFromItemId(ItemID.StardustDye), Main.LocalPlayer);
                //data.Apply(null);
                DrawLaser(Main.spriteBatch, (Texture2D)TextureAssets.Projectile[Projectile.type], Main.player[Projectile.owner].Center,
                    Projectile.velocity, 10, Projectile.damage, -1.57f, 1f, 1000f, Color.White, (int)MOVE_DISTANCE);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            }
            return false;
        }

        // The core function of drawing a laser
        public void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit, float step, int damage, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default, int transDist = 50)
        {
            float r = unit.ToRotation() + rotation;

            // Draws the laser 'body'
            for (float i = transDist; i <= Distance; i += step)
            {
                Color c = Color.White;
                var origin = start + i * unit;
                spriteBatch.Draw(texture, origin - Main.screenPosition,
                    //36 is the width of the laser!
                    new Rectangle(0, 26, 36, 26), i < transDist ? Color.Transparent : c, r,
                    new Vector2(36 * .5f, 26 * .5f), scale, 0, 0);
            }

            // Draws the laser 'tail'
            spriteBatch.Draw(texture, start + unit * (transDist - step) - Main.screenPosition,
                new Rectangle(0, 0, 36, 26), Color.White, r, new Vector2(36 * .5f, 26 * .5f), scale, 0, 0);

            // Draws the laser 'head'
            spriteBatch.Draw(texture, start + (Distance + step) * unit - Main.screenPosition,
                new Rectangle(0, 52, 36, 26), Color.White, r, new Vector2(36 * .5f, 26 * .5f), scale, 0, 0);
        }

        // Change the way of collision check of the projectile
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // We can only collide if we are at max charge, which is when the laser is actually fired
            if (!IsAtMaxCharge) return false;

            Player player = Main.player[Projectile.owner];
            Vector2 unit = Projectile.velocity;
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.MountedCenter,
                player.MountedCenter + unit * Distance, 22, ref point);
        }

        // Set custom immunity time on hitting an NPC
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 4;
            var global = target.GetGlobalNPC<CStudiosGlobalNPC>();
            if (!target.HasBuff(BuffType<EntropicCorruption>()))
                global.EntropicStacks = 1;

            target.AddBuff(BuffType<EntropicCorruption>(), 5 * 60);
            float dustAmount = 4f;
            float randomConstant = MathHelper.ToRadians(Main.rand.Next(0, 360));
            for (int i = 0; i < dustAmount; i++)
            {
                Vector2 spinningpoint5 = Vector2.UnitX * 0f;
                spinningpoint5 += -Vector2.UnitY.RotatedBy(i * ((float)Math.PI * 2f / dustAmount)) * new Vector2(35f, 1f);
                spinningpoint5 = spinningpoint5.RotatedBy(target.velocity.ToRotation() + randomConstant);
                int dust = Dust.NewDust(target.Center, 0, 0, DustID.Electric);
                Main.dust[dust].scale = 1.5f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position = target.Center + spinningpoint5;
                Main.dust[dust].velocity = target.velocity * 0f + spinningpoint5.SafeNormalize(Vector2.UnitY) * 3f;
            }
            for (int i = 0; i < dustAmount; i++)
            {
                Vector2 spinningpoint5 = Vector2.UnitX * 0f;
                spinningpoint5 += -Vector2.UnitY.RotatedBy(i * ((float)Math.PI * 2f / dustAmount)) * new Vector2(35f, 1f);
                spinningpoint5 = spinningpoint5.RotatedBy(target.velocity.ToRotation() + randomConstant + MathHelper.ToRadians(90));
                int dust = Dust.NewDust(target.Center, 0, 0, DustID.Electric);
                Main.dust[dust].scale = 1.5f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position = target.Center + spinningpoint5;
                Main.dust[dust].velocity = target.velocity * 0f + spinningpoint5.SafeNormalize(Vector2.UnitY) * 3f;
            }
        }

        // The AI of the projectile
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.position = player.Center + Projectile.velocity * MOVE_DISTANCE;
            Projectile.timeLeft = 2;

            // By separating large AI into methods it becomes very easy to see the flow of the AI in a broader sense
            // First we update player variables that are needed to channel the laser
            // Then we run our charging laser logic
            // If we are fully charged, we proceed to update the laser's position
            // Finally we spawn some effects like dusts and light

            UpdatePlayer(player);
            ChargeLaser(player);

            // If laser is not charged yet, stop the AI here.
            if (Charge < MAX_CHARGE) return;

            SetLaserPosition(player);
            SpawnDusts(player);
            CastLights();
            FireShpcFlankingRounds(player);
            SteerFlankingRounds(player);
        }

        /// <summary>
        /// While Omega's standard charged ultimate laser is live, emit Calamity Overhaul
        /// SHPC CyberTraceBeamProj rounds from the top and bottom edges of the beam.
        /// No-ops if CalamityOverhaul is not loaded.
        /// </summary>
        private void FireShpcFlankingRounds(Player player)
        {
            if (Projectile.owner != Main.myPlayer)
                return;

            if (!ModLoader.TryGetMod("CalamityOverhaul", out Mod cwr))
                return;

            if (!cwr.TryFind("CyberTraceBeamProj", out ModProjectile beamMod))
                return;

            ShpcTimer++;
            if (ShpcTimer < ShpcBurstInterval)
                return;
            ShpcTimer = 0f;

            Vector2 beamDir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Vector2 perp = beamDir.RotatedBy(MathHelper.PiOver2);

            Vector2 playerToMouse = Main.MouseWorld - player.Center;
            if (playerToMouse.LengthSquared() < 0.001f)
                playerToMouse = beamDir;
            playerToMouse.Normalize();
            Vector2 muzzle = player.Center + playerToMouse * 76f;

            int dmg = Math.Max(1, (int)(Projectile.damage * ShpcDamageMul));
            int beamType = beamMod.Type;

            // Extra jitter so successive pairs don't stack on the same arc.
            float fanJitter = Main.rand.NextFloat(-0.06f, 0.06f);

            // Top edge, then bottom edge of the laser — wide outward fan.
            for (int side = -1; side <= 1; side += 2)
            {
                Vector2 spawn = muzzle + perp * (ShpcEdgeOffset * side);
                float peel = (ShpcPeelAngle + fanJitter) * side;
                Vector2 vel = beamDir.RotatedBy(peel) * ShpcSpeed;

                int idx = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawn,
                    vel,
                    beamType,
                    dmg,
                    Projectile.knockBack,
                    player.whoAmI,
                    ai0: Main.rand.Next(3));

                if (idx >= 0 && idx < Main.maxProjectiles)
                {
                    // Kill CWR enemy-homing so our beam-curve isn't fighting it.
                    Main.projectile[idx].ai[1] = 0f;
                    Main.projectile[idx].DamageType = Projectile.DamageType;
                    _flankIdentities.Add(Main.projectile[idx].identity);
                }
            }
        }

        /// <summary>
        /// After the initial fan-out, bend each flanking SHPC round back toward a point
        /// ahead on the ultimate laser so they wrap in around the beam.
        /// </summary>
        private void SteerFlankingRounds(Player player)
        {
            if (_flankIdentities.Count == 0)
                return;

            Vector2 origin = player.MountedCenter;
            Vector2 beamDir = Projectile.velocity.SafeNormalize(Vector2.UnitX);

            for (int i = _flankIdentities.Count - 1; i >= 0; i--)
            {
                int identity = _flankIdentities[i];
                Projectile shot = null;
                for (int p = 0; p < Main.maxProjectiles; p++)
                {
                    Projectile cand = Main.projectile[p];
                    if (cand.active && cand.identity == identity && cand.owner == Projectile.owner)
                    {
                        shot = cand;
                        break;
                    }
                }

                if (shot == null)
                {
                    _flankIdentities.RemoveAt(i);
                    continue;
                }

                Vector2 toShot = shot.Center - origin;
                float along = Vector2.Dot(toShot, beamDir);
                if (along < 40f)
                    along = 40f;

                Vector2 onBeam = origin + beamDir * along;
                Vector2 ahead = origin + beamDir * (along + ShpcLookAhead);
                Vector2 toAxis = onBeam - shot.Center;
                Vector2 desired = (ahead - shot.Center) + toAxis * ShpcAxisPull;

                if (desired.LengthSquared() < 0.001f)
                    desired = beamDir;
                desired = desired.SafeNormalize(beamDir) * ShpcSpeed;

                shot.velocity = Vector2.Lerp(shot.velocity, desired, ShpcCurveLerp);
                shot.rotation = shot.velocity.ToRotation();
            }
        }

        private void SpawnDusts(Player player)
        {
            Player projOwner = Main.player[Projectile.owner];

            Vector2 unit = Projectile.velocity * -1;
            Vector2 dustPos = player.MountedCenter + Projectile.velocity * Distance;

            for (int i = 0; i < 2; ++i)
            {
                float num1 = Projectile.velocity.ToRotation() + (Main.rand.Next(2) == 1 ? -1.0f : 1.0f) * 1.57f;
                float num2 = (float)(Main.rand.NextDouble() * 0.8f + 1.0f);
                Vector2 dustVel = new Vector2((float)Math.Cos(num1) * num2, (float)Math.Sin(num1) * num2);
                Dust dust = Main.dust[Dust.NewDust(dustPos, 0, 0, DustID.Electric, dustVel.X, dustVel.Y)];
                dust.noGravity = true;
                dust.scale = 1.2f;

            }
            int dustID = DustID.RainbowMk2;
            Color dustColor = Color.SkyBlue;
            Vector2 playerToMouse = Main.MouseWorld - projOwner.Center;
            playerToMouse = Vector2.Normalize(playerToMouse);
            Vector2 MuzzlePosition = new Vector2(projOwner.Center.X, projOwner.Center.Y - 6) + Vector2.Normalize(playerToMouse) * 76;
            for (int d = 0; d < 10; d++)
            {
                Vector2 perturbedSpeed = Vector2.Normalize(Main.MouseWorld - projOwner.Center).RotatedByRandom(MathHelper.ToRadians(23));
                float scale = 22f - Main.rand.NextFloat() * 21f;
                perturbedSpeed *= scale;
                int dustIndex = Dust.NewDust(MuzzlePosition, 0, 0, dustID, perturbedSpeed.X, perturbedSpeed.Y, 150, default, 1.7f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].color = dustColor;
            }
            for (int d = 0; d < 30; d++)
            {
                Vector2 perturbedSpeed = Vector2.Normalize(Main.MouseWorld - projOwner.Center).RotatedByRandom(MathHelper.ToRadians(120));
                float scale = 4f - Main.rand.NextFloat() * 3f;
                perturbedSpeed *= -scale;
                int dustIndex = Dust.NewDust(MuzzlePosition, 0, 0, dustID, perturbedSpeed.X, perturbedSpeed.Y, 150, default, 0.7f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].color = dustColor;
            }
            for (int d = 0; d < 10; d++)
            {
                Vector2 perturbedSpeed = Vector2.Normalize(Main.MouseWorld - projOwner.Center).RotatedBy(MathHelper.ToRadians(50));
                float scale = 8f - Main.rand.NextFloat() * 3f;
                perturbedSpeed *= -scale;
                int dustIndex = Dust.NewDust(MuzzlePosition, 0, 0, dustID, perturbedSpeed.X, perturbedSpeed.Y, 150, default, 0.8f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].color = dustColor;
            }
            for (int d = 0; d < 10; d++)
            {
                Vector2 perturbedSpeed = Vector2.Normalize(Main.MouseWorld - projOwner.Center).RotatedBy(MathHelper.ToRadians(-50));
                float scale = 8f - Main.rand.NextFloat() * 3f;
                perturbedSpeed *= -scale;
                int dustIndex = Dust.NewDust(MuzzlePosition, 0, 0, dustID, perturbedSpeed.X, perturbedSpeed.Y, 150, default, 0.8f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].color = dustColor;
            }
            for (int d = 0; d < 20; d++)
            {
                Vector2 perturbedSpeed = Vector2.Normalize(Main.MouseWorld - projOwner.Center).RotatedByRandom(MathHelper.ToRadians(2));
                float scale = 102f - Main.rand.NextFloat() * 101f;
                perturbedSpeed *= scale;
                int dustIndex = Dust.NewDust(MuzzlePosition, 0, 0, dustID, perturbedSpeed.X, perturbedSpeed.Y, 150, default, 2f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].color = dustColor;
            }
            for (int d = 0; d < 10; d++)
            {
                Vector2 perturbedSpeed = Vector2.Normalize(Main.MouseWorld - projOwner.Center).RotatedBy(MathHelper.ToRadians(80));
                float scale = 8f - Main.rand.NextFloat() * 1f;
                perturbedSpeed *= scale;
                int dustIndex = Dust.NewDust(MuzzlePosition, 0, 0, dustID, perturbedSpeed.X, perturbedSpeed.Y, 150, default, 1f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].color = dustColor;

            }
            for (int d = 0; d < 10; d++)
            {
                Vector2 perturbedSpeed = Vector2.Normalize(Main.MouseWorld - projOwner.Center).RotatedBy(MathHelper.ToRadians(-80));
                float scale = 8f - Main.rand.NextFloat() * 1f;
                perturbedSpeed *= scale;
                int dustIndex = Dust.NewDust(MuzzlePosition, 0, 0, dustID, perturbedSpeed.X, perturbedSpeed.Y, 150, default, 1f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].color = dustColor;
            }
            float rotation = (float)Math.Atan2(Main.player[Projectile.owner].MountedCenter.Y - Main.MouseWorld.Y, Main.player[Projectile.owner].MountedCenter.X - Main.MouseWorld.X);//Aim towards mouse



        }

        /*
		 * Sets the end of the laser position based on where it collides with something
		 */
        private void SetLaserPosition(Player player)
        {
            Player projOwner = Main.player[Projectile.owner];

            Vector2 playerToMouse = Main.MouseWorld - projOwner.Center;
            playerToMouse = Vector2.Normalize(playerToMouse);
            Vector2 MuzzlePosition = projOwner.Center + Vector2.Normalize(playerToMouse) * 76;

            for (Distance = MOVE_DISTANCE; Distance <= 2200f; Distance += 5f)
            {
                var start = MuzzlePosition + Projectile.velocity * Distance;
                if (!Collision.CanHit(MuzzlePosition, 1, 1, start, 1, 1))
                {
                    //Distance -= 5f;
                    //break;
                }
            }
        }

        private void ChargeLaser(Player player)
        {
            // Kill the projectile if the player stops channeling
            if (!player.HasBuff(BuffType<PsybitBeamAttack>()))
            {
                Projectile.Kill();
            }
            else
            {
                Vector2 offset = Projectile.velocity;
                offset *= MOVE_DISTANCE;
                Vector2 pos = player.MountedCenter + offset - new Vector2(10, 10);
                if (Charge < MAX_CHARGE)
                {
                    Charge++;
                }
                int chargeFact = (int)(Charge / 20f);
                Vector2 dustVelocity = Vector2.UnitX * 18f;
                dustVelocity = dustVelocity.RotatedBy(Projectile.rotation - 1.57f);
                Vector2 spawnPos = Projectile.Center + dustVelocity;
                /*for (int k = 0; k < chargeFact + 1; k++)
				{
					Vector2 spawn = spawnPos + ((float)Main.rand.NextDouble() * 6.28f).ToRotationVector2() * (12f - chargeFact * 2);
					Dust dust = Main.dust[Dust.NewDust(pos, 20, 20, 226, projectile.velocity.X / 2f, projectile.velocity.Y / 2f)];
					dust.velocity = Vector2.Normalize(spawnPos - spawn) * 1.5f * (10f - chargeFact * 2f) / 10f;
					dust.noGravity = true;
					dust.scale = Main.rand.Next(10, 20) * 0.05f;
				}*/
            }
        }

        private void UpdatePlayer(Player player)
        {
            // Multiplayer support here, only run this code if the client running it is the owner of the projectile
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 diff = Main.MouseWorld - player.MountedCenter;
                diff.Normalize();
                Projectile.velocity = diff;
                Projectile.direction = Main.MouseWorld.X > player.MountedCenter.X ? 1 : -1;
                Projectile.netUpdate = true;
            }
            int dir = Projectile.direction;
            player.ChangeDir(dir); // Set player direction to where we are shooting
                                   //player.heldProj = projectile.whoAmI; // Update player's held projectile

            player.itemTime = 2; // Set item time to 2 frames while we are used
            player.itemAnimation = 2; // Set item animation time to 2 frames while we are used
            //player.itemTime = 35; // Set item time to 2 frames while we are used
            //player.itemAnimation = 35; // Set item animation time to 2 frames while we are used
            //player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * dir, Projectile.velocity.X * dir); // Set the item rotation to where we are shooting

            Player projOwner = Main.player[Projectile.owner];
            projOwner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (projOwner.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
        }

        private void CastLights()
        {
            // Cast a light along the line of the laser
            DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * (Distance - MOVE_DISTANCE), 26, DelegateMethods.CastLight);
        }

        public override bool ShouldUpdatePosition() => false;

        /*
		 * Update CutTiles so the laser will cut tiles (like grass)
		 */
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * Distance, (Projectile.width + 16) * Projectile.scale, DelegateMethods.CutTiles);
        }
    }
}
