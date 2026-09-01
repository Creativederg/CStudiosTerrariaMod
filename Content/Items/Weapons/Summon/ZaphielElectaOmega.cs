using Microsoft.Xna.Framework;
using CStudios.Content.Buffs;
using CStudios.Content.DamageClasses;
using CStudios.Content.Projectiles.Summon.Psybits;
using CStudios.Content.Systems.ZaphielModules;
using CStudios.Content.Systems.ZaphielModules.Authority;
using CStudios.Content.Systems.ZaphielModules.Aerial;
using CStudios.Content.Systems.ZaphielModules.Score;
using CStudios.Content.Systems.ZaphielModules.Fracture;
using CStudios.Content.Systems.ZaphielModules.Finality;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System.Collections.Generic;

namespace CStudios.Content.Items.Weapons.Summon
{
    public class ZaphielElectaOmega : ModItem
    {
        public const int MaxBits = 11;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 320;
            Item.DamageType = GetInstance<PsychokineticDamageClass>();
            Item.width = 54;
            Item.height = 54;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.buyPrice(platinum: 2);
            Item.rare = ItemRarityID.Red;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 18f;
            Item.mana = 6;
            Item.knockBack = 5f;
            Item.buffType = BuffType<PsybitDefensiveArray>();
        }

        private int GetEffectiveMaxBits(Player player)
        {
            var ctx = ZaphielModuleSystem.Resolve(player);
            int effective = (int)(MaxBits * ctx.MaxBitsMul) + ctx.AuthorityBonusBits;
            return System.Math.Max(1, effective);
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            var ctx = ZaphielModuleSystem.Resolve(player);
            damage *= ctx.DamageMul;
            damage *= ZaphielBossProgress.GetBossPowerMul(Item);
            if (ctx.RisingScoreEdgeActive || ctx.ScoreMode)
                damage *= player.GetModPlayer<ZaphielScorePlayer>().DamageFromScore();
        }

        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            crit += ZaphielModuleSystem.Resolve(player).CritAdd;
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            var ctx = ZaphielModuleSystem.Resolve(player);
            mult *= ctx.ManaCostMul;
            if (player.channel && !ctx.MeleeMode && !ctx.TraceVolleyMode)
                mult = 0f;
        }

        public override float UseSpeedMultiplier(Player player)
        {
            var ctx = ZaphielModuleSystem.Resolve(player);
            float speed = ctx.AttackSpeedMul;
            if (ctx.RisingScoreEdgeActive)
                speed *= 1f + player.GetModPlayer<ZaphielScorePlayer>().Score01 * 0.20f;
            return speed;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var prog = Item.GetGlobalItem<ZaphielBossProgress>();
            float mul = ZaphielBossProgress.GetBossPowerMul(Item);
            int pct = (int)((mul - 1f) * 100f);

            tooltips.Add(new TooltipLine(Mod, "OmegaBosses",
                $"Boss synchronizations: {prog.KilledBossTypes.Count} (+{pct}% damage)")
            { OverrideColor = new Color(255, 100, 140) });

            tooltips.Add(new TooltipLine(Mod, "OmegaModules",
                "Ultimate: Authority / Aerial / Score / Fracture / Finality / Overcharge.")
            { OverrideColor = new Color(255, 120, 160) });

            Player player = Main.LocalPlayer;
            if (player == null || !player.active)
                return;

            var ctx = ZaphielModuleSystem.Resolve(player);
            var aerial = player.GetModPlayer<ZaphielAerialPlayer>();
            var score = player.GetModPlayer<ZaphielScorePlayer>();
            var frac = player.GetModPlayer<ZaphielFracturePlayer>();
            var fin = player.GetModPlayer<ZaphielFinalityPlayer>();

            if (aerial.FormActive)
                tooltips.Add(new TooltipLine(Mod, "HerrscherForm", $"Herrscher Form: {aerial.FormTimer / 60 + 1}s")
                { OverrideColor = new Color(120, 210, 255) });
            else if (aerial.FormCooldown > 0)
                tooltips.Add(new TooltipLine(Mod, "HerrscherCD", $"Herrscher Form ready in {aerial.FormCooldown / 60 + 1}s")
                { OverrideColor = new Color(180, 180, 220) });

            if (ctx.FeedbackHeartActive || ctx.ScoreMode)
                tooltips.Add(new TooltipLine(Mod, "ScoreTip", $"Score: {score.Score:0}/100")
                { OverrideColor = new Color(255, 210, 80) });

            if (frac.FractureActive)
                tooltips.Add(new TooltipLine(Mod, "FracOn", $"Time Fracture: {frac.FractureTimer / 60 + 1}s")
                { OverrideColor = new Color(180, 140, 255) });
            else if (frac.FractureCooldown > 0)
                tooltips.Add(new TooltipLine(Mod, "FracCd", $"Fracture ready in {frac.FractureCooldown / 60 + 1}s")
                { OverrideColor = new Color(140, 130, 170) });

            if (fin.FinalityActive)
                tooltips.Add(new TooltipLine(Mod, "FinOn", $"Finality: {fin.FinalityTimer / 60 + 1}s")
                { OverrideColor = new Color(255, 220, 140) });
            else if (fin.FinalityCooldown > 0)
                tooltips.Add(new TooltipLine(Mod, "FinCd", $"Finality ready in {fin.FinalityCooldown / 60 + 1}s")
                { OverrideColor = new Color(160, 140, 110) });
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.ownedProjectileCounts[ProjectileType<Psybits>()] < 1)
                    return true;
                return !player.HasBuff(BuffType<PsybitLaserCooldown>());
            }
            return true;
        }

        public override void HoldItem(Player player)
        {
            var ctx = ZaphielModuleSystem.Resolve(player);
            var aerial = player.GetModPlayer<ZaphielAerialPlayer>();
            var fin = player.GetModPlayer<ZaphielFinalityPlayer>();
            bool skybladeForm = ctx.SkybladeManifestActive && aerial.FormActive;
            bool finalitySlash = ctx.FinalityEdgeActive && fin.FinalityActive;

            if (ctx.MeleeMode)
            {
                Item.noUseGraphic = false;
                Item.channel = false;
                Item.useStyle = ItemUseStyleID.Swing;
            }
            else if (ctx.TraceVolleyMode || skybladeForm || finalitySlash)
            {
                Item.noUseGraphic = true;
                Item.channel = false;
                Item.useStyle = ItemUseStyleID.Shoot;
            }
            else
            {
                Item.noUseGraphic = true;
                Item.channel = true;
                Item.useStyle = ItemUseStyleID.Shoot;
            }

            if (player.whoAmI != Main.myPlayer)
                return;

            if (CStudios.UltimateKey == null || !CStudios.UltimateKey.JustPressed)
                return;

            if (ctx.AuthorityCoreActive)
            {
                AuthorityPatternSystem.TryActivatePattern(player);
                return;
            }
            if (ctx.HerrscherDriveActive)
            {
                AerialHerrscherSystem.TryActivateForm(player);
                return;
            }
            if (ctx.FeedbackHeartActive)
            {
                ScoreStigmaSystem.TryBurst(player);
                return;
            }
            if (ctx.FractureCoreActive)
            {
                FractureSystem.TryActivate(player);
                return;
            }
            if (ctx.FinalityCoreActive)
            {
                FinalitySystem.TryActivate(player);
                return;
            }

            if (!player.HasBuff(BuffType<PsybitOvercharge>())
                && !player.HasBuff(BuffType<PsybitOverchargedCooldown>())
                && player.ownedProjectileCounts[ProjectileType<Psybits>()] > 0)
            {
                player.AddBuff(BuffType<PsybitOvercharge>(), 12 * 60);
                player.AddBuff(BuffType<PsybitOverchargedCooldown>(), 90 * 60);
                SoundEngine.PlaySound(SoundID.Item113, player.Center);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source,
            Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                int effectiveMax = GetEffectiveMaxBits(player);

                if (player.ownedProjectileCounts[ProjectileType<Psybits>()] < 1 && player.whoAmI == Main.myPlayer)
                {
                    for (int i = 0; i < effectiveMax; i++)
                    {
                        Projectile.NewProjectile(
                            player.GetSource_ItemUse(Item),
                            player.Center, Vector2.Zero,
                            ProjectileType<Psybits>(),
                            player.GetWeaponDamage(Item), 0f, player.whoAmI,
                            0f, Main.rand.Next(0, 360), i);
                    }
                    player.AddBuff(Item.buffType, 2);
                    SoundEngine.PlaySound(SoundID.Item46, player.Center);
                    return false;
                }

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (!p.active || p.owner != player.whoAmI)
                        continue;
                    if (p.type == ProjectileType<Psybits>()
                        || p.type == ProjectileType<PsybitMinionBeam>()
                        || p.type == ProjectileType<PsybitMinionChargedBeam>())
                        p.Kill();
                }

                player.ClearBuff(BuffType<PsybitDefensiveArray>());
                player.AddBuff(BuffType<PsybitBeamAttack>(), 3 * 60);
                player.AddBuff(BuffType<PsybitLaserCooldown>(), 24 * 60);

                Projectile.NewProjectile(
                    player.GetSource_ItemUse(Item),
                    player.position, Vector2.Zero,
                    ProjectileType<PsybitGunCharged>(),
                    damage, 0f, player.whoAmI);

                SoundEngine.PlaySound(SoundID.DD2_DefenseTowerSpawn, player.Center);
                return false;
            }

            var ctx = ZaphielModuleSystem.Resolve(player);
            int dmg = System.Math.Max(1, (int)(damage * ctx.DamageMul));
            Vector2 aim = velocity.SafeNormalize(Vector2.UnitX);

            var aerial = player.GetModPlayer<ZaphielAerialPlayer>();
            var fin = player.GetModPlayer<ZaphielFinalityPlayer>();
            if (ctx.SkybladeManifestActive && aerial.FormActive)
            {
                int slashes = System.Math.Max(3, 3 + ctx.BeamCountAdd);
                float arc = 0.55f * ctx.SpreadMul;
                float speed = 26f * (ctx.BeamSpeedMul > 0f ? ctx.BeamSpeedMul : 1f);

                for (int i = 0; i < slashes; i++)
                {
                    float t = slashes == 1 ? 0f : (i / (float)(slashes - 1) - 0.5f);
                    int idx = Projectile.NewProjectile(
                        source, player.Center + aim * 28f,
                        aim.RotatedBy(t * arc) * speed,
                        ProjectileType<AerialSkybladeSlash>(),
                        dmg, knockback, player.whoAmI);
                    if (idx >= 0)
                    {
                        Main.projectile[idx].timeLeft = 40;
                        Main.projectile[idx].extraUpdates = 2;
                        Main.projectile[idx].scale = 1.2f + 0.15f * System.Math.Abs(t);
                        Main.projectile[idx].penetrate += ctx.ExtraPierce;
                    }
                }

                int ribbons = ctx.FunnelOverflowActive ? 3 : 2;
                for (int i = 0; i < ribbons; i++)
                {
                    float off = (i - (ribbons - 1) * 0.5f) * 0.18f;
                    int idx = Projectile.NewProjectile(
                        source, player.Center + aim * 20f,
                        aim.RotatedBy(off) * speed * 0.9f,
                        ProjectileType<PsybitUnchargedLaser>(),
                        System.Math.Max(1, (int)(dmg * 0.75f)),
                        knockback * 0.6f, player.whoAmI);
                    if (idx >= 0)
                    {
                        Main.projectile[idx].timeLeft = 48;
                        Main.projectile[idx].extraUpdates = 2;
                        Main.projectile[idx].penetrate = -1;
                        Main.projectile[idx].scale = 1.35f;
                    }
                }

                if (ctx.ScoreMode)
                    player.GetModPlayer<ZaphielScorePlayer>().AddScore(1.5f, ctx);

                SoundEngine.PlaySound(SoundID.Item71, player.Center);
                SpawnGunVisual(player);
                return false;
            }

            if (ctx.FinalityEdgeActive && fin.FinalityActive)
            {
                int slashes = System.Math.Max(4, 4 + ctx.BeamCountAdd);
                if (ctx.OriginRelayActive)
                    slashes += 2;

                float arc = 0.70f * ctx.SpreadMul;
                float speed = 24f * (ctx.BeamSpeedMul > 0f ? ctx.BeamSpeedMul : 1f);

                for (int i = 0; i < slashes; i++)
                {
                    float t = slashes == 1 ? 0f : (i / (float)(slashes - 1) - 0.5f);
                    int idx = Projectile.NewProjectile(
                        source, player.Center + aim * 28f,
                        aim.RotatedBy(t * arc) * speed,
                        ProjectileType<FinalitySlash>(),
                        dmg, knockback, player.whoAmI);
                    if (idx >= 0)
                    {
                        Main.projectile[idx].scale = 1.35f + 0.1f * System.Math.Abs(t);
                        Main.projectile[idx].penetrate += ctx.ExtraPierce;
                    }
                }

                int turretCap = System.Math.Max(1, player.maxMinions / 2);
                if (player.velocity.LengthSquared() > 2.5f
                    && player.ownedProjectileCounts[ProjectileType<FinalityTurret>()] < turretCap)
                {
                    Vector2 spot = player.Center
                        + new Vector2(Main.rand.NextFloat(-70f, 70f), Main.rand.NextFloat(-90f, -40f));
                    int tIdx = Projectile.NewProjectile(
                        source, spot, Vector2.Zero,
                        ProjectileType<FinalityTurret>(),
                        0, 0f, player.whoAmI,
                        ai0: Main.rand.Next(11));
                    if (tIdx >= 0)
                        Main.projectile[tIdx].minionSlots = 0f;
                }

                SoundEngine.PlaySound(SoundID.Item71, player.Center);
                SpawnGunVisual(player);
                return false;
            }

            if (ctx.MeleeMode)
            {
                int idx = Projectile.NewProjectile(source, player.Center + aim * 48f, aim,
                    ProjectileType<PsybitUnchargedLaser>(), dmg, knockback + 3f, player.whoAmI);
                if (idx >= 0)
                {
                    Main.projectile[idx].timeLeft = 14;
                    Main.projectile[idx].penetrate = -1;
                    Main.projectile[idx].extraUpdates = 0;
                    Main.projectile[idx].velocity *= 0.12f;
                    Main.projectile[idx].scale = ctx.MeleeSizeMul > 0f ? ctx.MeleeSizeMul : 1f;
                }
                if (ctx.ScoreMode)
                    player.GetModPlayer<ZaphielScorePlayer>().AddScore(2.0f, ctx);
                SoundEngine.PlaySound(SoundID.Item1, player.Center);
                return false;
            }

            if (ctx.TraceVolleyMode)
            {
                if (!TryShootCWRTraceBeams(player, source, aim, dmg, knockback, ctx))
                {
                    int beams = System.Math.Max(1, 3 + ctx.BeamCountAdd);
                    float spread = 0.08f * ctx.SpreadMul;
                    Vector2 baseVel = aim * (14f * ctx.BeamSpeedMul);

                    for (int i = 0; i < beams; i++)
                    {
                        float off = beams > 1 ? (i - (beams - 1) / 2f) * spread : 0f;
                        Vector2 shot = baseVel.RotatedBy(off + Main.rand.NextFloat(-0.03f, 0.03f));
                        Projectile.NewProjectile(source, player.Center + shot.SafeNormalize(aim) * 28f, shot,
                            ProjectileType<PsybitUnchargedLaser>(), dmg, knockback, player.whoAmI);
                    }
                    SoundEngine.PlaySound(SoundID.Item92, player.Center);
                }

                if (ctx.ScoreMode || ctx.RisingScoreEdgeActive || ctx.FeedbackHeartActive || ctx.LivingGaugeActive)
                    player.GetModPlayer<ZaphielScorePlayer>().AddScore(3.5f, ctx);

                SpawnGunVisual(player);
                return false;
            }

            {
                int beamType = ProjectileType<PsybitPlayerBeam>();
                if (player.ownedProjectileCounts[beamType] < 1)
                {
                    Projectile.NewProjectile(source, player.Center, aim, beamType, dmg, knockback, player.whoAmI);
                    SoundEngine.PlaySound(SoundID.Item15, player.Center);
                }
                if (ctx.ScoreMode)
                    player.GetModPlayer<ZaphielScorePlayer>().AddScore(0.35f, ctx);
                SpawnGunVisual(player);
                return false;
            }
        }

        private static bool TryShootCWRTraceBeams(Player player, IEntitySource source, Vector2 aim,
            int damage, float knockback, ZaphielShootContext ctx)
        {
            if (!ModLoader.TryGetMod("CalamityOverhaul", out Mod cwr))
                return false;
            if (!cwr.TryFind("CyberTraceBeamProj", out ModProjectile beamMod))
                return false;

            int beamType = beamMod.Type;
            SoundEngine.PlaySound(SoundID.Item92, player.Center);

            int beams = System.Math.Max(1, 3 + ctx.BeamCountAdd);
            float spreadAngle = 0.08f * System.Math.Max(ctx.SpreadMul, 0f);
            Vector2 baseVel = aim * (14f * ctx.BeamSpeedMul);
            Vector2 spawnPos = player.Center + new Vector2(aim.X * 20f, -12f);

            for (int i = 0; i < beams; i++)
            {
                float spreadOffset = beams > 1 ? (i - (beams - 1) / 2f) * spreadAngle : 0f;
                float randomOffset = spreadAngle > 0f ? Main.rand.NextFloat(-0.03f, 0.03f) : 0f;
                Vector2 shotVel = baseVel.RotatedBy(spreadOffset + randomOffset);

                int idx = Projectile.NewProjectile(
                    source,
                    spawnPos + shotVel.SafeNormalize(Vector2.UnitX) * 28f,
                    shotVel,
                    beamType,
                    damage,
                    knockback,
                    player.whoAmI,
                    ai0: Main.rand.Next(3));

                if (idx >= 0 && idx < Main.maxProjectiles)
                {
                    Main.projectile[idx].ai[1] = ctx.HomingMul > 0f ? ctx.HomingMul : 1f;
                    Main.projectile[idx].penetrate += ctx.ExtraPierce;
                }
            }

            return true;
        }

        private void SpawnGunVisual(Player player)
        {
            if (player.ownedProjectileCounts[ProjectileType<PsybitGunUncharged>()] < 1)
            {
                Projectile.NewProjectile(
                    player.GetSource_ItemUse(Item),
                    player.Center, Vector2.Zero,
                    ProjectileType<PsybitGunUncharged>(),
                    0, 0, player.whoAmI);
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<ZaphielElectaApex>(), 1);

            if (ModLoader.TryGetMod("CalamityMod", out Mod cal))
            {
                if (cal.TryFind("DivineGeode", out ModItem geode))
                    recipe.AddIngredient(geode.Type, 15);
                if (cal.TryFind("UnholyEssence", out ModItem essence))
                    recipe.AddIngredient(essence.Type, 20);
                if (cal.TryFind("UelibloomBar", out ModItem ueli))
                    recipe.AddIngredient(ueli.Type, 10);
                if (cal.TryFind("CosmicAnvil", out ModTile cosmicAnvil))
                    recipe.AddTile(cosmicAnvil.Type);
                else
                    recipe.AddTile(TileID.LunarCraftingStation);
            }
            else
            {
                recipe.AddIngredient(ItemID.LunarBar, 15);
                recipe.AddIngredient(ItemID.FragmentNebula, 20);
                recipe.AddTile(TileID.LunarCraftingStation);
            }

            recipe.Register();
        }
    }
}
