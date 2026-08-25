
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
 
using CStudios.Content.Buffs;
using CStudios.Content.Projectiles.Generics;
using CStudios.Content.Systems;
using CStudios.Content.Projectiles.Summon.Psybits;
using CStudios.Content.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Projectiles.Summon.Psybits
{
    public class PsybitGunCharged : PsybitGun
    {
        public override string Texture => "CStudios/Content/Projectiles/Summon/Psybit/PsybitGunUncharged";
        public override string TextureFlash => "CStudios/Content/Projectiles/Summon/Psybit/PsybitGunUnchargedFlash";
        //Use the extra recoil/reload code.
        public override bool UseRecoil => false;
        //The dust that appears from the barrel after shooting.
        public override int SmokeDustID => DustID.Smoke;

        //The dust that fires from the barrel after shooting.
        public override int FlashDustID => DustID.Electric;
        //The distance the gun's muzzle is relative to the player. Remember this also is influenced by base distance.
        public override int MuzzleDistance => 70;
        //The distance the gun is relative to the player.
        public override float BaseDistance => 48;
        public override int StartingState => 2;
        public override bool KillOnIdle => false;
        public override int ScreenShakeTime => 95;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 180;
            Projectile.height = 64;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        float TransformationProgress = 0;
        float TransitionSpeed = 0.1f;

        float Psybit1Transition = 0f;
        float Psybit2Transition = 0f;
        float Psybit3Transition = 0f;
        float Psybit4Transition = 0f;
        float Psybit5Transition = 0f;
        float Psybit6Transition = 0f;
        float Psybit7Transition = 0f;
        float Psybit8Transition = 0f;
        float Psybit9Transition = 0f;
        float Psybit10Transition = 0f;
        float Psybit11Transition = 0f;

        public override void PostAI()
        {
            Player projOwner = Main.player[Projectile.owner];

            TransformationProgress += 0.02f;

            if (TransformationProgress >= 1f && projOwner.whoAmI == Main.myPlayer && projOwner.ownedProjectileCounts[ProjectileType<PsybitLaser>()] < 1)
            {
                for (int d = 0; d < 15; d++)
                {
                    int dustIndex = Dust.NewDust(Projectile.Center, 0, 0, DustID.MinecartSpark, 0f + Main.rand.Next(-2, 2), 0f + Main.rand.Next(-2, 2), 0, default, 1f);
                    Main.dust[dustIndex].noGravity = true;
                }
                int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ProjectileType<PsybitLaser>(), Projectile.damage, 0f, projOwner.whoAmI);
                SoundEngine.PlaySound(CStudiosAudio.SFX_GundamLaser, Projectile.Center);

            }
            if (TransformationProgress >= 1f)
            {

            }

            if (!projOwner.HasBuff(BuffType<PsybitBeamAttack>()))
            {
                for (int d = 0; d < 15; d++)
                {
                    int dustIndex = Dust.NewDust(Projectile.Center, 0, 0, DustID.GemSapphire, 0f + Main.rand.Next(-2, 2), 0f + Main.rand.Next(-2, 2), 0, default, 2f);
                    Main.dust[dustIndex].noGravity = true;
                }
                Projectile.Kill();
            }

            Psybit1Transition = MathHelper.Clamp(Psybit1Transition, 0f, 1f);
            Psybit2Transition = MathHelper.Clamp(Psybit2Transition, 0f, 1f);
            Psybit3Transition = MathHelper.Clamp(Psybit3Transition, 0f, 1f);
            Psybit4Transition = MathHelper.Clamp(Psybit4Transition, 0f, 1f);
            Psybit5Transition = MathHelper.Clamp(Psybit5Transition, 0f, 1f);
            Psybit6Transition = MathHelper.Clamp(Psybit6Transition, 0f, 1f);
            Psybit7Transition = MathHelper.Clamp(Psybit7Transition, 0f, 1f);
            Psybit8Transition = MathHelper.Clamp(Psybit8Transition, 0f, 1f);
            Psybit9Transition = MathHelper.Clamp(Psybit9Transition, 0f, 1f);
            Psybit10Transition = MathHelper.Clamp(Psybit10Transition, 0f, 1f);
            Psybit11Transition = MathHelper.Clamp(Psybit11Transition, 0f, 1f);

        }
        public override bool PreAI()
        {
            return true;
        }
        public override void PostDraw(Color lightColor)
        {
            // This is where we specify which way to flip the sprite. If the projectile is moving to the left, then flip it vertically.
            SpriteEffects spriteEffects = Projectile.spriteDirection <= 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Getting texture of projectile
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Texture2D texturePsybit1 = (Texture2D)Request<Texture2D>("CStudios/Content/Projectiles/Summon/Psybit/Psybit4");
            Texture2D texturePsybit2 = (Texture2D)Request<Texture2D>("CStudios/Content/Projectiles/Summon/Psybit/Psybit5");
            Texture2D texturePsybit3 = (Texture2D)Request<Texture2D>("CStudios/Content/Projectiles/Summon/Psybit/Psybit6");
            Texture2D texturePsybit4 = (Texture2D)Request<Texture2D>("CStudios/Content/Projectiles/Summon/Psybit/Psybit1");
            Texture2D texturePsybit5 = (Texture2D)Request<Texture2D>("CStudios/Content/Projectiles/Summon/Psybit/Psybit2");
            Texture2D texturePsybit6 = (Texture2D)Request<Texture2D>("CStudios/Content/Projectiles/Summon/Psybit/Psybit3");
            Texture2D texturePsybit7 = (Texture2D)Request<Texture2D>("CStudios/Content/Projectiles/Summon/Psybit/Psybit7");
            Texture2D texturePsybit8 = (Texture2D)Request<Texture2D>("CStudios/Content/Projectiles/Summon/Psybit/Psybit8");
            Texture2D texturePsybit9 = (Texture2D)Request<Texture2D>("CStudios/Content/Projectiles/Summon/Psybit/Psybit9");
            Texture2D texturePsybit10 = (Texture2D)Request<Texture2D>("CStudios/Content/Projectiles/Summon/Psybit/Psybit10");
            Texture2D texturePsybit11 = (Texture2D)Request<Texture2D>("CStudios/Content/Projectiles/Summon/Psybit/Psybit11");

            // Get the currently selected frame on the texture.
            Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);

            Vector2 origin = sourceRectangle.Size() / 2f;

            // Applying lighting and draw our projectile
            Color drawColor = Projectile.GetAlpha(lightColor);
            if (TransformationProgress >= 0.01)
            {
                Psybit1Transition += TransitionSpeed;
                Main.EntitySpriteDraw(texturePsybit1,
                    Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY + MathHelper.Lerp(25, 0, EaseHelper.InOutQuad(Psybit1Transition))),
                    sourceRectangle, lightColor * Psybit1Transition, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            if (TransformationProgress >= 0.09)
            {
                Psybit2Transition += TransitionSpeed;
                Main.EntitySpriteDraw(texturePsybit2,
                   Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY + MathHelper.Lerp(-25, 0, EaseHelper.InOutQuad(Psybit2Transition))),
                   sourceRectangle, lightColor * Psybit2Transition, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            if (TransformationProgress >= 0.19)
            {
                Psybit3Transition += TransitionSpeed;
                Main.EntitySpriteDraw(texturePsybit3,
                   Projectile.Center - Main.screenPosition + new Vector2(0f + MathHelper.Lerp(25, 0, EaseHelper.InOutQuad(Psybit3Transition)), Projectile.gfxOffY),
                   sourceRectangle, lightColor * Psybit3Transition, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            if (TransformationProgress >= 0.29)
            {
                Psybit4Transition += TransitionSpeed;
                Main.EntitySpriteDraw(texturePsybit4,
                   Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY + MathHelper.Lerp(25, 0, EaseHelper.InOutQuad(Psybit4Transition))),
                   sourceRectangle, lightColor * Psybit4Transition, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            if (TransformationProgress >= 0.39)
            {
                Psybit5Transition += TransitionSpeed;
                Main.EntitySpriteDraw(texturePsybit5,
                   Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY + MathHelper.Lerp(-25, 0, EaseHelper.InOutQuad(Psybit5Transition))),
                   sourceRectangle, lightColor * Psybit5Transition, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            if (TransformationProgress >= 0.49)
            {
                Psybit6Transition += TransitionSpeed;
                Main.EntitySpriteDraw(texturePsybit6,
                   Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY + MathHelper.Lerp(-25, 0, EaseHelper.InOutQuad(Psybit6Transition))),
                   sourceRectangle, lightColor * Psybit6Transition, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            if (TransformationProgress >= 0.59)
            {
                Psybit7Transition += TransitionSpeed;
                Main.EntitySpriteDraw(texturePsybit7,
                   Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY + MathHelper.Lerp(-25, 0, EaseHelper.InOutQuad(Psybit7Transition))),
                   sourceRectangle, lightColor * Psybit7Transition, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            if (TransformationProgress >= 0.69)
            {
                Psybit8Transition += TransitionSpeed;
                Main.EntitySpriteDraw(texturePsybit8,
                   Projectile.Center - Main.screenPosition + new Vector2(0f + MathHelper.Lerp(-25, 0, EaseHelper.InOutQuad(Psybit8Transition)), Projectile.gfxOffY),
                   sourceRectangle, lightColor * Psybit8Transition, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            if (TransformationProgress >= 0.79)
            {
                Psybit9Transition += TransitionSpeed;
                Main.EntitySpriteDraw(texturePsybit9,
                   Projectile.Center - Main.screenPosition + new Vector2(0f + MathHelper.Lerp(-25, 0, EaseHelper.InOutQuad(Psybit9Transition)), Projectile.gfxOffY),
                   sourceRectangle, lightColor * Psybit9Transition, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            if (TransformationProgress >= 0.89)
            {
                Psybit10Transition += TransitionSpeed;
                Main.EntitySpriteDraw(texturePsybit10,
                   Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY + MathHelper.Lerp(25, 0, EaseHelper.InOutQuad(Psybit10Transition))),
                   sourceRectangle, lightColor * Psybit10Transition, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            if (TransformationProgress >= 0.99)
            {
                Psybit11Transition += TransitionSpeed;
                Main.EntitySpriteDraw(texturePsybit11,
                   Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY + MathHelper.Lerp(25, 0, EaseHelper.InOutQuad(Psybit11Transition))),
                   sourceRectangle, lightColor * Psybit11Transition, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }

            base.PostDraw(lightColor);
        }
        //For posterity, the draw code of this gun is going to have each part of the upgraded gun seperate and they will draw in with a white flash.
        public override void OnKill(int timeLeft)
        {


        }

    }
}
