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
        public bool TraceVolleyMode;
        public int CritAdd;
        public int OverrideProjectileType;
        public float LifestealFraction;

        // Minions
        public float MinionDamageMul;
        public float MinionFireRangeMul;
        public float MinionMoveSpeedMul;
        public float MinionOrbitRadiusMul;
        public bool MinionUseChargedBeam;
        public bool MinionAggressiveChase;
        public bool MinionVolleyShot;
        public bool MinionRandomOrbit;

        // Melee
        public bool MeleeMode;
        public float MeleeRangeMul;
        public float MeleeSizeMul;
        public int MeleeStrikeInterval;

        // Shared new
        public float MaxBitsMul;
        public float MinionSlotsPerBit;

        // Authority of the Bits
        public bool AuthorityMode;
        public bool CommandLanceActive;
        public bool AbsoluteVectorActive;
        public bool AuthorityCoreActive;
        public bool CascadeLinkActive;
        public bool ThroneOfBitsActive;
        public float AuthorityPatternPowerMul;
        public float AuthorityPatternCostMul;
        public int AuthorityBonusBits;
        public float AuthorityFormationIntegrity;

        // Aerial Herrscher Form
        public bool AerialMode;
        public bool SkybladeManifestActive;
        public bool ThreeDimensionalLockActive;
        public bool HerrscherDriveActive;
        public bool FunnelOverflowActive;
        public bool AerialSwarmProtocolActive;

        public float AerialFormDurationMul;
        public float AerialMoveMul;
        public float FunnelFireRateMul;
        public float AerialBitIndepMul;

        // Score & Stigma
        public bool ScoreMode;
        public bool RisingScoreEdgeActive;
        public bool StigmaResonanceActive;
        public bool FeedbackHeartActive;
        public bool DataBacklashActive;
        public bool LivingGaugeActive;
        public float ScoreGainMul;      // default 1f
        public float StigmaBonusMul;    // default 1f
        public float DataStormMildMul;  // default 1f


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
            ContinuousBeam = false,
            TraceVolleyMode = false,
            CritAdd = 0,
            OverrideProjectileType = 0,
            LifestealFraction = 0f,

            MinionDamageMul = 1f,
            MinionFireRangeMul = 1f,
            MinionMoveSpeedMul = 1f,
            MinionOrbitRadiusMul = 1f,
            MinionUseChargedBeam = false,
            MinionAggressiveChase = false,
            MinionVolleyShot = false,
            MinionRandomOrbit = false,

            MeleeMode = false,
            MeleeRangeMul = 1f,
            MeleeSizeMul = 1f,
            MeleeStrikeInterval = 20,

            MaxBitsMul = 1f,
            MinionSlotsPerBit = 1f,

            AuthorityMode = false,
            CommandLanceActive = false,
            AbsoluteVectorActive = false,
            AuthorityCoreActive = false,
            CascadeLinkActive = false,
            ThroneOfBitsActive = false,
            AuthorityPatternPowerMul = 1f,
            AuthorityPatternCostMul = 1f,
            AuthorityBonusBits = 0,
            AuthorityFormationIntegrity = 1f,

            AerialMode = false,
            SkybladeManifestActive = false,
            ThreeDimensionalLockActive = false,
            HerrscherDriveActive = false,
            FunnelOverflowActive = false,
            AerialSwarmProtocolActive = false,

            AerialFormDurationMul = 1f,
            AerialMoveMul = 1f,
            FunnelFireRateMul = 1f,
            AerialBitIndepMul = 1f,

            ScoreMode = false,
            RisingScoreEdgeActive = false,
            StigmaResonanceActive = false,
            FeedbackHeartActive = false,
            DataBacklashActive = false,
            LivingGaugeActive = false,
            ScoreGainMul = 1f,
            StigmaBonusMul = 1f,
            DataStormMildMul = 1f,
        };
    }
}