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

namespace CStudios.Content.Items.Weapons.Summon
{
    public class ZaphielElectaSpark : ModItem
    {
        public const int MaxBits = 3;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = GetInstance<PsychokineticDamageClass>();
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 10f;
            Item.mana = 4;
            Item.knockBack = 1.5f;
            Item.buffType = BuffType<PsybitDefensiveArray>();
        }

        private int GetEffectiveMaxBits(Player player)
        {
            var ctx = ZaphielModuleSystem.Resolve(player);
            // Use the weapon's own MaxBits const, or 11 for Apex/Omega
            int baseMax = MaxBits; // or 11
            int effective = (int)(baseMax * ctx.MaxBitsMul) + ctx.AuthorityBonusBits;
            return System.Math.Max(1, effective);
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                return player.ownedProjectileCounts[ProjectileType<Psybits>()] < GetEffectiveMaxBits(player);

            if (player.altFunctionUse == 2)
                return player.ownedProjectileCounts[ProjectileType<Psybits>()] < MaxBits;
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source,
            Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                int effectiveMax = GetEffectiveMaxBits(player);
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

            // Channel beam
            if (player.ownedProjectileCounts[ProjectileType<PsybitPlayerBeam>()] < 1)
            {
                Vector2 aim = velocity;
                if (aim.LengthSquared() < 0.001f)
                    aim = (Main.MouseWorld - player.MountedCenter).SafeNormalize(Vector2.UnitX);
                else
                    aim = aim.SafeNormalize(Vector2.UnitX);

                Projectile.NewProjectile(
                    source,
                    player.Center,
                    aim,
                    ProjectileType<PsybitPlayerBeam>(),
                    damage,
                    knockback,
                    player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.IronBar, 8) // or LeadBar
                .AddIngredient(ItemID.FallenStar, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}