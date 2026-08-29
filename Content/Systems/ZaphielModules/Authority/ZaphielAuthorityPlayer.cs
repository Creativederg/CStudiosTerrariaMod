using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CStudios.Content.Systems.ZaphielModules.Authority
{
    public class ZaphielAuthorityPlayer : ModPlayer
    {
        public AuthorityPatternType CurrentPattern = AuthorityPatternType.None;
        public AuthorityPatternType CascadePattern = AuthorityPatternType.None;

        public int PatternTimer;
        public int CascadeTimer;
        public int PatternCooldown;
        public int CycleIndex;

        public int LockedTargetWhoAmI = -1;
        public Vector2 LockedWorldPosition;
        public float PatternFlash;

        public bool IsPatternActive => CurrentPattern != AuthorityPatternType.None && PatternTimer > 0;
        public bool IsCascadeActive => CascadePattern != AuthorityPatternType.None && CascadeTimer > 0;

        public override void ResetEffects() { }

        public override void PostUpdate()
        {
            if (PatternTimer > 0)
            {
                PatternTimer--;
                if (PatternTimer <= 0)
                    OnPrimaryPatternEnded();
            }

            if (CascadeTimer > 0)
            {
                CascadeTimer--;
                if (CascadeTimer <= 0)
                    CascadePattern = AuthorityPatternType.None;
            }

            if (PatternCooldown > 0)
                PatternCooldown--;

            if (PatternFlash > 0f)
                PatternFlash = MathHelper.Clamp(PatternFlash - 0.04f, 0f, 1f);

            var ctx = ZaphielModuleSystem.Resolve(Player);
            if (!ctx.AuthorityCoreActive && (IsPatternActive || IsCascadeActive))
                ForceCancelAllPatterns();
        }

        private void OnPrimaryPatternEnded()
        {
            var ctx = ZaphielModuleSystem.Resolve(Player);

            if (ctx.CascadeLinkActive && CurrentPattern != AuthorityPatternType.None)
            {
                AuthorityPatternType next = GetNextPattern(CurrentPattern);
                StartCascadePattern(next, ctx);
            }

            CurrentPattern = AuthorityPatternType.None;
            LockedTargetWhoAmI = -1;
        }

        public void StartCascadePattern(AuthorityPatternType type, ZaphielShootContext ctx)
        {
            CascadePattern = type;
            int baseDuration = GetBaseDuration(type);
            CascadeTimer = (int)(baseDuration * 0.45f * ctx.AuthorityPatternPowerMul);
            if (CascadeTimer < 30) CascadeTimer = 30;
        }

        public void ForceCancelAllPatterns()
        {
            CurrentPattern = AuthorityPatternType.None;
            CascadePattern = AuthorityPatternType.None;
            PatternTimer = 0;
            CascadeTimer = 0;
            LockedTargetWhoAmI = -1;
        }

        public static int GetBaseDuration(AuthorityPatternType type) => type switch
        {
            AuthorityPatternType.GiantLance => 4 * 60 + 30,
            AuthorityPatternType.BindingCage => 5 * 60,
            AuthorityPatternType.OrbitalBombardment => 5 * 60 + 30,
            AuthorityPatternType.AegisWall => 6 * 60,
            AuthorityPatternType.SpiralExecution => 3 * 60 + 30,
            _ => 4 * 60
        };

        public static AuthorityPatternType GetNextPattern(AuthorityPatternType current) => current switch
        {
            AuthorityPatternType.GiantLance => AuthorityPatternType.BindingCage,
            AuthorityPatternType.BindingCage => AuthorityPatternType.OrbitalBombardment,
            AuthorityPatternType.OrbitalBombardment => AuthorityPatternType.AegisWall,
            AuthorityPatternType.AegisWall => AuthorityPatternType.SpiralExecution,
            AuthorityPatternType.SpiralExecution => AuthorityPatternType.GiantLance,
            _ => AuthorityPatternType.GiantLance
        };

        public AuthorityPatternType GetPatternAtCycleIndex() => CycleIndex switch
        {
            0 => AuthorityPatternType.GiantLance,
            1 => AuthorityPatternType.BindingCage,
            2 => AuthorityPatternType.OrbitalBombardment,
            3 => AuthorityPatternType.AegisWall,
            4 => AuthorityPatternType.SpiralExecution,
            _ => AuthorityPatternType.GiantLance
        };

        public void AdvanceCycle() => CycleIndex = (CycleIndex + 1) % 5;
    }
}