using Microsoft.Xna.Framework;
using CStudios.Content.Buffs;
using CStudios.Content.Projectiles.Summon.Psybits;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CStudios.Content.DamageClasses;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Items.Weapons.Summon
{
    public class ZaphielElectaApex : ModItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.DamageType = GetInstance<PsychokineticDamageClass>();
            Item.width = 52;
            Item.height = 52;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.LightRed;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 35f;
            Item.mana = 4;
            Item.buffType = BuffType<PsybitDefensiveArray>();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                // Always allow the first summon. For the charged shot, respect the original cooldowns.
                if (player.ownedProjectileCounts[ProjectileType<Psybits>()] < 1)
                    return true;

                return !player.HasBuff(BuffType<PsybitLaserCooldown>());
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // ===== RIGHT CLICK =====
            if (player.altFunctionUse == 2)
            {
                // First right-click: summon all minions
                if (player.ownedProjectileCounts[ProjectileType<Psybits>()] < 1 && player.whoAmI == Main.myPlayer)
                {
                    for (int i = 0; i < 11; i++)
                    {
                        Projectile.NewProjectile(
                            player.GetSource_ItemUse(player.HeldItem),
                            player.Center.X, player.Center.Y,
                            0f, 0f,
                            ProjectileType<Psybits>(),
                            player.GetWeaponDamage(Item),
                            0f,
                            player.whoAmI,
                            0f,
                            Main.rand.Next(0, 360),
                            i);
                    }

                    // Long-duration buff so they persist after switching weapons
                    player.AddBuff(Item.buffType, 2);

                    SoundEngine.PlaySound(SoundID.Item46, player.Center);
                    return false;
                }

                // Subsequent right-clicks: charged beam (original behavior)
                player.AddBuff(BuffType<PsybitBeamAttack>(), 3 * 60);
                player.AddBuff(BuffType<PsybitLaserCooldown>(), 30 * 60);

                Projectile.NewProjectile(
                    player.GetSource_ItemUse(player.HeldItem),
                    player.position.X, player.position.Y,
                    0f, 0f,
                    ProjectileType<PsybitGunCharged>(),
                    damage, 0f, player.whoAmI, 0f);

                SoundEngine.PlaySound(SoundID.DD2_DefenseTowerSpawn, player.Center);
                return false;
            }

            // ===== LEFT CLICK / CHANNEL: Tagging bullet =====
            Projectile.NewProjectile(
                player.GetSource_ItemUse(player.HeldItem),
                player.Center.X, player.Center.Y,
                0f, 0f,
                ProjectileType<PsybitGunUncharged>(),
                0, 0, player.whoAmI, 0f);

            SoundEngine.PlaySound(SoundID.Item125, player.Center);

            Projectile.NewProjectile(
                source,
                position.X, position.Y,
                velocity.X, velocity.Y,
                ProjectileType<PsybitUnchargedLaser>(),
                damage, 0f, player.whoAmI, 0f);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.Nanites, 50)
                .AddIngredient(ItemID.FragmentNebula, 10)
                .AddIngredient(ItemID.MartianConduitPlating, 20)
                //.AddIngredient(ItemType<EssenceOfFirepower>())
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}