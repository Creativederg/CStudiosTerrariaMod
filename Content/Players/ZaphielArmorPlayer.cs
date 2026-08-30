using CStudios.Content.DamageClasses;
using CStudios.Content.Systems.ZaphielModules;
using Terraria;
using Terraria.ModLoader;

namespace CStudios.Content.Players
{
    public enum ZaphielFantasy
    {
        None,
        AuthorityOfTheBits,
        AerialHerrscher,
        ScoreAndStigma,
        RemoteSymphony,
        FinalPermission
    }

    public class ZaphielArmorPlayer : ModPlayer
    {
        public bool OmegaSet;
        public ZaphielFantasy ActiveFantasy = ZaphielFantasy.None;

        public bool AuthorityCommand;
        public bool AerialSovereign;
        public bool ResonantOverclock;
        public bool ConductorMantle;
        public bool LastAuthority;

        public override void ResetEffects()
        {
            OmegaSet = false;
            ActiveFantasy = ZaphielFantasy.None;

            AuthorityCommand = false;
            AerialSovereign = false;
            ResonantOverclock = false;
            ConductorMantle = false;
            LastAuthority = false;
        }

        public override void PostUpdateEquips()
        {
            if (!OmegaSet)
                return;

            ActiveFantasy = DetectFantasy();
            ApplyFantasyBonus();
        }

        private ZaphielFantasy DetectFantasy()
        {
            var mp = Player.GetModPlayer<ZaphielModulePlayer>();

            int authority = 0, aerial = 0, score = 0, symphony = 0, finalPerm = 0;

            for (int i = 0; i < ZaphielModulePlayer.SlotCount; i++)
            {
                Item m = mp.GetModule(i);
                if (m?.ModItem == null)
                    continue;

                string name = m.ModItem.Name;

                if (name is "ModCommandLance" or "ModAbsoluteVector" or "ModAuthorityCore"
                        or "ModCascadeLink" or "ModThroneOfBits")
                    authority++;

                if (name is "ModSkybladeManifest" or "ModThreeDimensionalLock" or "ModHerrscherDrive"
                        or "ModFunnelOverflow" or "ModAerialSwarmProtocol")
                    aerial++;

                if (name is "ModRisingScoreEdge" or "ModStigmaResonance" or "ModFeedbackHeart"
                        or "ModDataBacklash" or "ModLivingGauge")
                    score++;

                if (name is "ModConductorsBaton" or "ModRoleMatrix" or "ModSymphonyCore"
                        or "ModDynamicReassignment" or "ModOrchestraProtocol")
                    symphony++;

                if (name is "ModPermissionBlade" or "ModZeroPointLock" or "ModLastAuthority"
                        or "ModOverlimitCascade" or "ModUnifiedExistence")
                    finalPerm++;
            }

            int best = 0;
            ZaphielFantasy result = ZaphielFantasy.None;

            if (authority > best) { best = authority; result = ZaphielFantasy.AuthorityOfTheBits; }
            if (aerial > best) { best = aerial; result = ZaphielFantasy.AerialHerrscher; }
            if (score > best) { best = score; result = ZaphielFantasy.ScoreAndStigma; }
            if (symphony > best) { best = symphony; result = ZaphielFantasy.RemoteSymphony; }
            if (finalPerm > best) { best = finalPerm; result = ZaphielFantasy.FinalPermission; }

            return best >= 3 ? result : ZaphielFantasy.None;
        }

        private void ApplyFantasyBonus()
        {
            switch (ActiveFantasy)
            {
                case ZaphielFantasy.AuthorityOfTheBits:
                    AuthorityCommand = true;
                    Player.maxMinions += 2;
                    Player.GetDamage<PsychokineticDamageClass>() += 0.10f;
                    break;

                case ZaphielFantasy.AerialHerrscher:
                    AerialSovereign = true;
                    Player.moveSpeed += 0.15f;
                    Player.jumpSpeedBoost += 1.2f;
                    Player.GetDamage<PsychokineticDamageClass>() += 0.12f;
                    break;

                case ZaphielFantasy.ScoreAndStigma:
                    ResonantOverclock = true;
                    Player.GetCritChance<PsychokineticDamageClass>() += 8f;
                    Player.GetDamage<PsychokineticDamageClass>() += 0.08f;
                    break;

                case ZaphielFantasy.RemoteSymphony:
                    ConductorMantle = true;
                    Player.maxMinions += 1;
                    Player.GetDamage<PsychokineticDamageClass>() += 0.10f;
                    break;

                case ZaphielFantasy.FinalPermission:
                    LastAuthority = true;
                    Player.GetDamage<PsychokineticDamageClass>() += 0.18f;
                    Player.GetCritChance<PsychokineticDamageClass>() += 6f;
                    Player.endurance += 0.08f;
                    break;

                default:
                    Player.GetDamage<PsychokineticDamageClass>() += 0.12f;
                    Player.GetCritChance<PsychokineticDamageClass>() += 6f;
                    break;
            }
        }
    }
}