using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CStudios.Content.Systems
{
    public class CStudiosAudio : ModSystem
    {
        public static float bossVoiceVolume = 1f;
        public static float voiceVolume = 0.6f;

        #region Weapon Sound Effects
        public static readonly SoundStyle SFX_GundamLaser = new SoundStyle("CStudios/Sounds/SFX/GundamLaser")
        {
            Volume = 0.8f,   // adjust to taste
            PitchVariance = 0.1f
        };
        #endregion

        #region Miscellaneous Sound Effects
        public static readonly SoundStyle SFX_Trumpet = new($"{nameof(StarsAbove)}/Sounds/SFX/Trumpet")
        {
            PitchVariance = 0.3f,
            
        };
        public static readonly SoundStyle SFX_FFTransformation = new($"{nameof(StarsAbove)}/Sounds/SFX/FFTransformSFX")
        {
            Volume = 0.8f,
        };
        public static readonly SoundStyle SFX_SpeedrunEasterEgg = new($"{nameof(StarsAbove)}/Sounds/SFX/SpeedrunEasterEgg")
        {

        };
        public static readonly SoundStyle SFX_VoidExplosion = new($"{nameof(StarsAbove)}/Sounds/SFX/VoidExplosion")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_ThundercrashStart = new($"{nameof(StarsAbove)}/Sounds/SFX/ThundercrashStart")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_ThundercrashEnd = new($"{nameof(StarsAbove)}/Sounds/SFX/ThundercrashEnd")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_SilenceSquall1 = new($"{nameof(StarsAbove)}/Sounds/SFX/SilenceSquall1")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_SilenceSquall2 = new($"{nameof(StarsAbove)}/Sounds/SFX/SilenceSquall2")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_NovaBomb = new($"{nameof(StarsAbove)}/Sounds/SFX/NovaBomb")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_Needlestorm = new($"{nameof(StarsAbove)}/Sounds/SFX/Needlestorm")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_FireGoldenGun = new($"{nameof(StarsAbove)}/Sounds/SFX/FireGoldenGun")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_Laevateinn = new($"{nameof(StarsAbove)}/Sounds/SFX/Laevateinn")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_LamentClashLose = new($"{nameof(StarsAbove)}/Sounds/SFX/LimbusCoinLose")
        {

        };
        public static readonly SoundStyle SFX_LamentClashWin = new($"{nameof(StarsAbove)}/Sounds/SFX/LimbusCoinWin")
        {

        };
        public static readonly SoundStyle SFX_GuntriggerParryPrep = new($"{nameof(StarsAbove)}/Sounds/SFX/GuntriggerParryPrep")
        {

        };
        public static readonly SoundStyle SFX_GuntriggerParry = new($"{nameof(StarsAbove)}/Sounds/SFX/GuntriggerParry")
        {

        };
        public static readonly SoundStyle SFX_PrepDarkness = new($"{nameof(StarsAbove)}/Sounds/SFX/PrepDarkness")
        {

        };
        public static readonly SoundStyle SFX_TeleportPrep = new($"{nameof(StarsAbove)}/Sounds/SFX/TeleportPrep")
        {

        };
        public static readonly SoundStyle SFX_textsoundeffect = new($"{nameof(StarsAbove)}/Sounds/SFX/textsoundeffect")
        {

        };
        public static readonly SoundStyle SFX_textsoundeffect2 = new($"{nameof(StarsAbove)}/Sounds/SFX/textsoundeffect2")
        {

        };
        public static readonly SoundStyle SFX_textsoundeffect3 = new($"{nameof(StarsAbove)}/Sounds/SFX/textsoundeffect3")
        {

        };
        public static readonly SoundStyle SFX_prototokiaActive = new($"{nameof(StarsAbove)}/Sounds/SFX/prototokiaActive")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_TimeEffect = new($"{nameof(StarsAbove)}/Sounds/SFX/TimeEffect")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_TitanCast = new($"{nameof(StarsAbove)}/Sounds/SFX/TitanCast")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_TitanPrep = new($"{nameof(StarsAbove)}/Sounds/SFX/TitanPrep")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_TruesilverSlash = new($"{nameof(StarsAbove)}/Sounds/SFX/TruesilverSlash")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_Umbral = new($"{nameof(StarsAbove)}/Sounds/SFX/Umbral")
        {

        };
        public static readonly SoundStyle SFX_WhisperShot = new($"{nameof(StarsAbove)}/Sounds/SFX/WhisperShot")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_YunlaiSwing0 = new($"{nameof(StarsAbove)}/Sounds/SFX/YunlaiSwing0")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_YunlaiSwing1 = new($"{nameof(StarsAbove)}/Sounds/SFX/YunlaiSwing1")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_AlbionBlast = new($"{nameof(StarsAbove)}/Sounds/SFX/AlbionBlast")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_AmiyaSlash = new($"{nameof(StarsAbove)}/Sounds/SFX/AmiyaSlash")
        {

        };
        public static readonly SoundStyle SFX_AshenExecute = new($"{nameof(StarsAbove)}/Sounds/SFX/AshenExecute")
        {
            Volume = 0.5f
        };
        public static readonly SoundStyle SFX_AshenExecute1 = new($"{nameof(StarsAbove)}/Sounds/SFX/AshenExecute1")
        {
            Volume = 0.5f

        };
        public static readonly SoundStyle SFX_AshenExecute2 = new($"{nameof(StarsAbove)}/Sounds/SFX/AshenExecute2")
        {
            Volume = 0.5f

        };
        public static readonly SoundStyle SFX_AshenExecute3 = new($"{nameof(StarsAbove)}/Sounds/SFX/AshenExecute3")
        {
            Volume = 0.5f

        };
        public static readonly SoundStyle SFX_AshenExecute4 = new($"{nameof(StarsAbove)}/Sounds/SFX/AshenExecute4")
        {
            Volume = 0.5f

        };
        public static readonly SoundStyle SFX_BakaMitai = new($"{nameof(StarsAbove)}/Sounds/SFX/BakaMitai")
        {

        };
        public static readonly SoundStyle SFX_BlasterFire = new($"{nameof(StarsAbove)}/Sounds/SFX/BlasterFire")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_BlasterPrep = new($"{nameof(StarsAbove)}/Sounds/SFX/BlasterPrep")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_bowstring = new($"{nameof(StarsAbove)}/Sounds/SFX/bowstring")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_BuryTheLightPrep = new($"{nameof(StarsAbove)}/Sounds/SFX/BuryTheLightPrep")
        {

        };
        public static readonly SoundStyle SFX_CelestialConception = new($"{nameof(StarsAbove)}/Sounds/SFX/CelestialConception")
        {

        };
        public static readonly SoundStyle SFX_CounterFinish = new($"{nameof(StarsAbove)}/Sounds/SFX/CounterFinish")
        {

        };
        public static readonly SoundStyle SFX_CounterImpact = new($"{nameof(StarsAbove)}/Sounds/SFX/CounterImpact")
        {

        };
        public static readonly SoundStyle SFX_Death = new($"{nameof(StarsAbove)}/Sounds/SFX/Death")
        {

        };
        public static readonly SoundStyle SFX_DeathInFourActsFinish = new($"{nameof(StarsAbove)}/Sounds/SFX/DeathInFourActsFinish")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_DeathInFourActsReload = new($"{nameof(StarsAbove)}/Sounds/SFX/DeathInFourActsReload")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_DeathInFourActsShoot = new($"{nameof(StarsAbove)}/Sounds/SFX/DeathInFourActsShoot")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_electroSmack = new($"{nameof(StarsAbove)}/Sounds/SFX/electroSmack")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_EnterDarkness = new($"{nameof(StarsAbove)}/Sounds/SFX/EnterDarkness")
        {

        };
        public static readonly SoundStyle SFX_GardenOfAvalonActivated = new($"{nameof(StarsAbove)}/Sounds/SFX/GardenOfAvalonActivated")
        {

        };
        public static readonly SoundStyle SFX_GuardianDown = new($"{nameof(StarsAbove)}/Sounds/SFX/GuardianDown")
        {

        };
        public static readonly SoundStyle SFX_GunbladeImpact = new($"{nameof(StarsAbove)}/Sounds/SFX/GunbladeImpact")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_HolyStab = new($"{nameof(StarsAbove)}/Sounds/SFX/HolyStab")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_HuckleberryReload = new($"{nameof(StarsAbove)}/Sounds/SFX/HuckleberryReload")
        {

        };
        public static readonly SoundStyle SFX_HuckleberryShoot = new($"{nameof(StarsAbove)}/Sounds/SFX/HuckleberryShoot")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_HullwroughtBlast = new($"{nameof(StarsAbove)}/Sounds/SFX/HullwroughtBlast")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_HullwroughtLoad = new($"{nameof(StarsAbove)}/Sounds/SFX/HullwroughtLoad")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_iceCracking = new($"{nameof(StarsAbove)}/Sounds/SFX/iceCracking")
        {

        };
        public static readonly SoundStyle SFX_InugamiCharge = new($"{nameof(StarsAbove)}/Sounds/SFX/InugamiCharge")
        {

        };
        public static readonly SoundStyle SFX_izanagiEquipped = new($"{nameof(StarsAbove)}/Sounds/SFX/izanagiEquipped")
        {

        };
        public static readonly SoundStyle SFX_izanagiReload = new($"{nameof(StarsAbove)}/Sounds/SFX/izanagiReload")
        {

        };
        public static readonly SoundStyle SFX_izanagiReloadBuff = new($"{nameof(StarsAbove)}/Sounds/SFX/izanagiReloadBuff")
        {

        };
        public static readonly SoundStyle SFX_izanagiShoot = new($"{nameof(StarsAbove)}/Sounds/SFX/izanagiShoot")
        {
            PitchVariance = 0.1f,

        };
        public static readonly SoundStyle SFX_izanagiShootBuff = new($"{nameof(StarsAbove)}/Sounds/SFX/izanagiShootBuff")
        {

        };
        public static readonly SoundStyle SFX_LegendarySlash = new($"{nameof(StarsAbove)}/Sounds/SFX/LegendarySlash")
        {

        };
        public static readonly SoundStyle SFX_LimitBreakActive = new($"{nameof(StarsAbove)}/Sounds/SFX/LimitBreakActive")
        {

        };
        public static readonly SoundStyle SFX_LimitBreakCharge = new($"{nameof(StarsAbove)}/Sounds/SFX/LimitBreakCharge")
        {

        };
        public static readonly SoundStyle SFX_MuseFinish = new($"{nameof(StarsAbove)}/Sounds/SFX/MuseFinish")
        {

        };
        public static readonly SoundStyle SFX_MusePing = new($"{nameof(StarsAbove)}/Sounds/SFX/MusePing")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_outbreakShoot = new($"{nameof(StarsAbove)}/Sounds/SFX/outbreakShoot")
        {

        };
        public static readonly SoundStyle SFX_PhaseChange = new($"{nameof(StarsAbove)}/Sounds/SFX/PhaseChange")
        {

        };
        public static readonly SoundStyle SFX_RadGunFail = new($"{nameof(StarsAbove)}/Sounds/SFX/RadGunFail")
        {

        };
        public static readonly SoundStyle SFX_RadGunSuccess = new($"{nameof(StarsAbove)}/Sounds/SFX/RadGunSuccess")
        {

        };
        public static readonly SoundStyle SFX_ScytheImpact = new($"{nameof(StarsAbove)}/Sounds/SFX/ScytheImpact")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_skofnungSwing = new($"{nameof(StarsAbove)}/Sounds/SFX/skofnungSwing")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_spinConstant = new($"{nameof(StarsAbove)}/Sounds/SFX/spinConstant")
        {

        };
        public static readonly SoundStyle SFX_splat = new($"{nameof(StarsAbove)}/Sounds/SFX/splat")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_StarbitCollected = new($"{nameof(StarsAbove)}/Sounds/SFX/StarbitCollected")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_StarbitShoot = new($"{nameof(StarsAbove)}/Sounds/SFX/StarbitShoot")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_StarfarerChosen = new($"{nameof(StarsAbove)}/Sounds/SFX/StarfarerChosen")
        {

        };
        public static readonly SoundStyle SFX_SuistrumeFail = new($"{nameof(StarsAbove)}/Sounds/SFX/SuistrumeFail")
        {

        };
        public static readonly SoundStyle SFX_summoning = new($"{nameof(StarsAbove)}/Sounds/SFX/summoning")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_superReadySFX = new($"{nameof(StarsAbove)}/Sounds/SFX/superReadySFX")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_swordAttackFinish = new($"{nameof(StarsAbove)}/Sounds/SFX/swordAttackFinish")
        {

        };
        public static readonly SoundStyle SFX_SwordBreak = new($"{nameof(StarsAbove)}/Sounds/SFX/SwordBreak")
        {

        };
        public static readonly SoundStyle SFX_swordSpin = new($"{nameof(StarsAbove)}/Sounds/SFX/swordSpin")
        {
            PitchVariance = 0.1f,

        };
        public static readonly SoundStyle SFX_swordStab = new($"{nameof(StarsAbove)}/Sounds/SFX/swordStab")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_TakingDamage = new($"{nameof(StarsAbove)}/Sounds/SFX/TakingDamage")
        {

        };
        public static readonly SoundStyle SFX_TeleportFinisher = new($"{nameof(StarsAbove)}/Sounds/SFX/TeleportFinisher")
        {
            PitchVariance = 0.1f,
        };
        public static readonly SoundStyle SFX_WarriorStun = new($"{nameof(StarsAbove)}/Sounds/SFX/WarriorStun")
        {

        };
        #endregion

    }
}
