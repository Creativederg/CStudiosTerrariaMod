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
    public class ZaphielElectaCoil : ModItem
    {
        public const int MaxBits = 5;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.DamageType = GetInstance<PsychokineticDamageClass>();
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 12f;
            Item.mana = 5;
            Item.knockBack = 2f;
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
                int have = player.ownedProjectileCounts[ProjectileType<Psybits>()];
                int toSpawn = MaxBits - have;
                for (int i = 0; i < toSpawn; i++)
                {
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero,
                        ProjectileType<Psybits>(), player.GetWeaponDamage(Item), 0f, player.whoAmI,
                        0f, Main.rand.Next(0, 360), have + i);
                }
                player.AddBuff(Item.buffType, 2);
                SoundEngine.PlaySound(SoundID.Item46, player.Center);
                return false;
            }

            if (player.ownedProjectileCounts[ProjectileType<PsybitPlayerBeam>()] < 1)
            {
                Projectile.NewProjectile(source, player.Center, velocity.SafeNormalize(Vector2.UnitX),
                    ProjectileType<PsybitPlayerBeam>(), damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ZaphielElectaSpark>()
                .AddIngredient(ItemID.Wire, 30)
                .AddIngredient(ItemID.DemoniteBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient<ZaphielElectaSpark>()
                .AddIngredient(ItemID.Wire, 30)
                .AddIngredient(ItemID.CrimtaneBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
