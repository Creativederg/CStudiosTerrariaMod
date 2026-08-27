namespace CStudios.Content.Systems.ZaphielModules
{
    public struct ZaphielShootContext
    {
        // Weapon
        public float DamageMul;
        public float AttackSpeedMul;
        public float ManaCostMul;
        public int BeamCountAdd;
        public float SpreadMul;
        public float HomingMul;
        public float BeamSpeedMul;
        public int ExtraPierce;
        public float LifeMul;
        public bool ExplodeOnHit;
        public float ExplodeRadius;
        public bool ContinuousBeam;
        public int CritAdd;
        public int OverrideProjectileType;

        // Minions
        public float MinionDamageMul;
        public float MinionFireRangeMul;
        public float MinionMoveSpeedMul;
        public float MinionOrbitRadiusMul;
        public bool MinionUseChargedBeam;
        public bool MinionAggressiveChase;

        // Melee tip mode
        public bool MeleeMode;
        public float MeleeRangeMul;
        public float MeleeSizeMul;
        public int MeleeStrikeInterval;

        public static ZaphielShootContext Default => new()
        {
            DamageMul = 1f,
            AttackSpeedMul = 1f,
            ManaCostMul = 1f,
            BeamCountAdd = 0,
            SpreadMul = 1f,
            HomingMul = 1f,
            BeamSpeedMul = 1f,
            ExtraPierce = 0,
            LifeMul = 1f,
            ExplodeOnHit = false,
            ExplodeRadius = 0f,
            ContinuousBeam = false, // important: default is CWR volley / burst, not sustained beam
            CritAdd = 0,
            OverrideProjectileType = 0,

            MinionDamageMul = 1f,
            MinionFireRangeMul = 1f,
            MinionMoveSpeedMul = 1f,
            MinionOrbitRadiusMul = 1f,
            MinionUseChargedBeam = false,
            MinionAggressiveChase = false,

            MeleeMode = false,
            MeleeRangeMul = 1f,
            MeleeSizeMul = 1f,
            MeleeStrikeInterval = 20,
        };
    }
}