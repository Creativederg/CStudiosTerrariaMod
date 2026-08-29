using Microsoft.Xna.Framework;
using CStudios.Content.Buffs;
using CStudios.Content.DamageClasses;
using CStudios.Content.Projectiles.Summon.Psybits;
using CStudios.Content.Systems.ZaphielModules;
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
            Item.channel = true; // default single beam channels
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 18f;
            Item.mana = 6;
            Item.knockBack = 5f;
            Item.buffType = BuffType<PsybitDefensiveArray>();
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            var ctx = ZaphielModuleSystem.Resolve(player);
            damage *= ctx.DamageMul;
            damage *= ZaphielBossProgress.GetBossPowerMul(Item);
        }

        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            crit += ZaphielModuleSystem.Resolve(player).CritAdd;
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            var ctx = ZaphielModuleSystem.Resolve(player); // Omega only
            mult *= ctx.ManaCostMul;

            // Continuous beam: beam AI pays mana; avoid full cost every useTime
            if (player.channel && !ctx.MeleeMode && !ctx.TraceVolleyMode)
                mult = 0f;
        }

        public override float UseSpeedMultiplier(Player player)
        {
            return ZaphielModuleSystem.Resolve(player).AttackSpeedMul;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var prog = Item.GetGlobalItem<ZaphielBossProgress>();
            float mul = ZaphielBossProgress.GetBossPowerMul(Item);
            int pct = (int)((mul - 1f) * 100f);

            tooltips.Add(new TooltipLine(Mod, "OmegaBosses",
                $"Boss synchronizations: {prog.KilledBossTypes.Count} (+{pct}% damage)")
            {
                OverrideColor = new Color(255, 100, 140)
            });

            tooltips.Add(new TooltipLine(Mod, "OmegaModules",
                "Default: single beam. Splinter Beam module: SHPC-style volleys. Apex Edge: melee.")
            {
                OverrideColor = new Color(255, 120, 160)
            });
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

            if (ctx.MeleeMode)
            {
                Item.noUseGraphic = false;
                Item.channel = false;
                Item.useStyle = ItemUseStyleID.Swing;
            }
            else if (ctx.TraceVolleyMode)
            {
                // Splinter Beam: click volleys, no channel
                Item.noUseGraphic = true;
                Item.channel = false;
                Item.useStyle = ItemUseStyleID.Shoot;
            }
            else
            {
                // Default: hold for single continuous beam
                Item.noUseGraphic = true;
                Item.channel = true;
                Item.useStyle = ItemUseStyleID.Shoot;
            }

            if (player.whoAmI != Main.myPlayer)
                return;

            if (CStudios.UltimateKey != null
                && CStudios.UltimateKey.JustPressed
                && !player.HasBuff(BuffType<PsybitOvercharge>())
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
            // ===== RIGHT CLICK =====
            if (player.altFunctionUse == 2)
            {
                if (player.ownedProjectileCounts[ProjectileType<Psybits>()] < 1 && player.whoAmI == Main.myPlayer)
                {
                    for (int i = 0; i < 11; i++)
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

            // ===== LEFT CLICK =====
            var ctx = ZaphielModuleSystem.Resolve(player);
            int dmg = System.Math.Max(1, (int)(damage * ctx.DamageMul));
            Vector2 aim = velocity.SafeNormalize(Vector2.UnitX);

            // 1) Melee (Apex Edge)
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
                SoundEngine.PlaySound(SoundID.Item1, player.Center);
                return false;
            }

            // 2) Splinter Beam → SHPC-style cyber trace volleys
            if (ctx.TraceVolleyMode)
            {
                if (!TryShootCWRTraceBeams(player, source, aim, dmg, knockback, ctx))
                {
                    // Fallback without CWR
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

                SpawnGunVisual(player);
                return false;
            }

            // 3) Default → single continuous beam
            {
                int beamType = ProjectileType<PsybitPlayerBeam>();
                if (player.ownedProjectileCounts[beamType] < 1)
                {
                    Projectile.NewProjectile(source, player.Center, aim, beamType, dmg, knockback, player.whoAmI);
                    SoundEngine.PlaySound(SoundID.Item15, player.Center);
                }
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

            // Previous tier — must be listed
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