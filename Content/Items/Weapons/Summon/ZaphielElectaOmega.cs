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
            Item.channel = true;
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
            mult *= ZaphielModuleSystem.Resolve(player).ManaCostMul;
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
                "Press K to open Lance Matrix (Spearhead / Guidance / Energy Core / Energy Relay / Psybits)")
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
            Item.channel = !ctx.MeleeMode;
            Item.noUseGraphic = !ctx.MeleeMode;

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
            // ----- Right click: summon / charged -----
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

            // ----- Left click: modules -----
            var ctx = ZaphielModuleSystem.Resolve(player);
            int dmg = System.Math.Max(1, (int)(damage * ctx.DamageMul));

            if (ctx.MeleeMode)
            {
                // Placeholder: use short-lived contact proj until you add ZaphielMeleeSwing
                Vector2 dir = velocity.SafeNormalize(Vector2.UnitX);
                int idx = Projectile.NewProjectile(source, player.Center + dir * 40f, dir,
                    ProjectileType<PsybitUnchargedLaser>(), dmg, knockback + 3f, player.whoAmI);
                if (idx >= 0)
                {
                    Main.projectile[idx].timeLeft = 12;
                    Main.projectile[idx].penetrate = -1;
                    Main.projectile[idx].extraUpdates = 0;
                    Main.projectile[idx].velocity *= 0.15f;
                }
                SoundEngine.PlaySound(SoundID.Item1, player.Center);
                return false;
            }

            if (ctx.ContinuousBeam && ctx.OverrideProjectileType == 0)
            {
                if (player.ownedProjectileCounts[ProjectileType<PsybitPlayerBeam>()] < 1)
                {
                    int idx = Projectile.NewProjectile(source, player.Center,
                        velocity.SafeNormalize(Vector2.UnitX),
                        ProjectileType<PsybitPlayerBeam>(), dmg, knockback, player.whoAmI);
                    if (idx >= 0)
                        Main.projectile[idx].penetrate += ctx.ExtraPierce;
                }
            }
            else
            {
                int projType = ctx.OverrideProjectileType > 0
                    ? ctx.OverrideProjectileType
                    : ProjectileType<PsybitUnchargedLaser>();

                int beams = System.Math.Max(1, 1 + ctx.BeamCountAdd);
                float spread = 0.07f * ctx.SpreadMul;
                Vector2 baseVel = velocity.SafeNormalize(Vector2.UnitX) * (14f * ctx.BeamSpeedMul);

                for (int i = 0; i < beams; i++)
                {
                    float off = beams > 1 ? (i - (beams - 1) / 2f) * spread : 0f;
                    Vector2 shot = baseVel.RotatedBy(off + Main.rand.NextFloat(-0.02f, 0.02f));
                    int idx = Projectile.NewProjectile(source, player.Center + shot * 2f, shot,
                        projType, dmg, knockback, player.whoAmI);
                    if (idx >= 0)
                    {
                        Main.projectile[idx].penetrate += ctx.ExtraPierce;
                        Main.projectile[idx].timeLeft = (int)(Main.projectile[idx].timeLeft * ctx.LifeMul);
                        Main.projectile[idx].ai[1] = ctx.HomingMul;
                    }
                }
                SoundEngine.PlaySound(SoundID.Item92, player.Center);
            }

            if (player.ownedProjectileCounts[ProjectileType<PsybitGunUncharged>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero,
                    ProjectileType<PsybitGunUncharged>(), 0, 0, player.whoAmI);
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ZaphielElectaApex>()
                .AddIngredient(ItemID.LunarBar, 15)
                .AddIngredient(ItemID.FragmentNebula, 20)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}