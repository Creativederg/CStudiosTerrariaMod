using CStudios.Content.DamageClasses;
using CStudios.Content.Projectiles.Summon.Psybits;
using CStudios.Content.Systems.ZaphielModules;
using CStudios.Content.Systems.ZaphielModules.Finality;
using CStudios.Content.Systems.ZaphielModules.Score;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Players
{
    public enum ZaphielFantasy
    {
        None,
        AuthorityOfTheBits,
        AerialHerrscher,
        ScoreAndStigma,
        TimeFracture,
        Finality
    }

    public class ZaphielArmorPlayer : ModPlayer
    {
        public bool OmegaSet;
        public ZaphielFantasy ActiveFantasy = ZaphielFantasy.None;

        public bool AuthorityCommand;
        public bool AerialSovereign;
        public bool ResonantOverclock;
        public bool FractureMantle;
        public bool LastAuthority;

        private int _finalityPulse;
        private int _authorityCheck;

        public override void ResetEffects()
        {
            OmegaSet = false;
            ActiveFantasy = ZaphielFantasy.None;
            AuthorityCommand = false;
            AerialSovereign = false;
            ResonantOverclock = false;
            FractureMantle = false;
            LastAuthority = false;
        }

        public override void PostUpdateEquips()
        {
            if (!OmegaSet)
                return;

            ActiveFantasy = DetectFantasy();
            ApplyFantasyBonus();
        }

        public override void PostUpdate()
        {
            if (!OmegaSet)
                return;

            if (AuthorityCommand)
                MaintainExtraBits();

            if (LastAuthority)
                PulseFinalityBolts();

            if (ResonantOverclock)
            {
                var score = Player.GetModPlayer<ZaphielScorePlayer>();
                // small passive drip so a full bar still feels alive
                if (score.Score >= 99f)
                    score.AddScore(0.02f, ZaphielModuleSystem.Resolve(Player));
            }
        }

        private ZaphielFantasy DetectFantasy()
        {
            var mp = Player.GetModPlayer<ZaphielModulePlayer>();
            int authority = 0, aerial = 0, score = 0, fracture = 0, finality = 0;

            for (int i = 0; i < ZaphielModulePlayer.SlotCount; i++)
            {
                Item m = mp.GetModule(i);
                if (m?.ModItem == null)
                    continue;
                string name = m.ModItem.Name;

                if (name is "ModCommandLance" or "ModAbsoluteVector" or "ModAuthorityCore"
                        or "ModCascadeLink" or "ModThroneOfBits" or "ModSwarmPattern")
                    authority++;

                if (name is "ModSkybladeManifest" or "ModThreeDimensionalLock" or "ModHerrscherDrive"
                        or "ModFunnelOverflow" or "ModAerialSwarmProtocol")
                    aerial++;

                if (name is "ModRisingScoreEdge" or "ModStigmaResonance" or "ModFeedbackHeart"
                        or "ModDataBacklash" or "ModLivingGauge")
                    score++;

                if (name is "ModTimeLock" or "ModPermetAfterimage" or "ModFractureCore"
                        or "ModEchoRelay" or "ModPhantomBits")
                    fracture++;

                if (name is "ModFinalityEdge" or "ModCocoonLock" or "ModFinalityCore"
                        or "ModOriginRelay" or "ModCocoonBits")
                    finality++;
            }

            int best = 0;
            ZaphielFantasy result = ZaphielFantasy.None;
            if (authority > best) { best = authority; result = ZaphielFantasy.AuthorityOfTheBits; }
            if (aerial > best) { best = aerial; result = ZaphielFantasy.AerialHerrscher; }
            if (score > best) { best = score; result = ZaphielFantasy.ScoreAndStigma; }
            if (fracture > best) { best = fracture; result = ZaphielFantasy.TimeFracture; }
            if (finality > best) { best = finality; result = ZaphielFantasy.Finality; }
            return best >= 1 ? result : ZaphielFantasy.None;
        }

        private void ApplyFantasyBonus()
        {
            // Aerial = Providence baseline. Later fantasies add Calamity-tier bulk.
            switch (ActiveFantasy)
            {
                case ZaphielFantasy.AerialHerrscher:
                    AerialSovereign = true;
                    ApplyTier(def: 0, pk: 0.00f, crit: 0f, minions: 0, move: 0.15f);
                    Player.jumpSpeedBoost += 1.2f;
                    Player.noFallDmg = true;
                    break;

                case ZaphielFantasy.AuthorityOfTheBits:
                    AuthorityCommand = true;
                    ApplyTier(def: 4, pk: 0.08f, crit: 3f, minions: 2, move: 0.04f);
                    break;

                case ZaphielFantasy.ScoreAndStigma:
                    ResonantOverclock = true;
                    ApplyTier(def: 8, pk: 0.14f, crit: 8f, minions: 1, move: 0.06f);
                    Player.lifeRegen += 3;
                    Player.statManaMax2 += 20;
                    break;

                case ZaphielFantasy.TimeFracture:
                    FractureMantle = true;
                    ApplyTier(def: 12, pk: 0.20f, crit: 10f, minions: 2, move: 0.10f);
                    Player.endurance += 0.04f;
                    break;

                case ZaphielFantasy.Finality:
                    LastAuthority = true;
                    ApplyTier(def: 18, pk: 0.28f, crit: 12f, minions: 2, move: 0.12f);
                    Player.endurance += 0.08f;
                    Player.statLifeMax2 += 40;
                    break;

                default:
                    ApplyTier(def: 0, pk: 0.06f, crit: 4f, minions: 0, move: 0f);
                    break;
            }
        }

        private void ApplyTier(int def, float pk, float crit, int minions, float move)
        {
            Player.statDefense += def;
            Player.GetDamage<PsychokineticDamageClass>() += pk;
            Player.GetCritChance<PsychokineticDamageClass>() += crit;
            Player.maxMinions += minions;
            Player.moveSpeed += move;
        }

        private void MaintainExtraBits()
        {
            if (Player.whoAmI != Main.myPlayer)
                return;
            if (Player.ownedProjectileCounts[ProjectileType<Psybits>()] < 1)
                return;

            _authorityCheck++;
            if (_authorityCheck < 30)
                return;
            _authorityCheck = 0;

            int extras = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == Player.whoAmI && p.type == ProjectileType<Psybits>() && p.minionSlots <= 0.01f)
                    extras++;
            }

            int want = 2;
            for (int n = extras; n < want; n++)
            {
                int idx = Projectile.NewProjectile(
                    Player.GetSource_Misc("OmegaThrone"),
                    Player.Center, Vector2.Zero,
                    ProjectileType<Psybits>(),
                    Player.GetWeaponDamage(Player.HeldItem), 0f, Player.whoAmI,
                    0f, Main.rand.Next(0, 360), 20 + n);
                if (idx >= 0)
                {
                    Main.projectile[idx].minionSlots = 0f;
                    Main.projectile[idx].minion = true;
                }
            }
        }

        private void PulseFinalityBolts()
        {
            if (Player.whoAmI != Main.myPlayer)
                return;

            var fin = Player.GetModPlayer<ZaphielFinalityPlayer>();
            if (!fin.FinalityActive)
            {
                _finalityPulse = 0;
                return;
            }

            _finalityPulse++;
            if (_finalityPulse < 45)
                return;
            _finalityPulse = 0;

            int boltType = ProjectileType<PsybitUnchargedLaser>();
            if (ModLoader.TryGetMod("CalamityOverhaul", out Mod cwr)
                && cwr.TryFind("CyberTraceBeamProj", out ModProjectile beam))
                boltType = beam.Type;

            int fired = 0;
            int dmg = System.Math.Max(1, (int)(Player.GetWeaponDamage(Player.HeldItem) * 0.55f));
            for (int i = 0; i < Main.maxNPCs && fired < 3; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || !npc.CanBeChasedBy())
                    continue;
                if (Vector2.Distance(npc.Center, Player.Center) > 640f)
                    continue;

                Vector2 vel = (npc.Center - Player.Center).SafeNormalize(Vector2.UnitX) * 16f;
                int idx = Projectile.NewProjectile(
                    Player.GetSource_Misc("OmegaFinality"),
                    Player.Center, vel, boltType, dmg, 2f, Player.whoAmI,
                    ai0: Main.rand.Next(3));
                if (idx >= 0)
                    Main.projectile[idx].timeLeft = 32;
                fired++;
            }
        }
    }
}
