using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CStudios.Content.Projectiles; // namespace updated

namespace CStudios.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class RaiserWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Base stats (similar to Solar Wings). These get boosted by the feathers.
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(180, 9f, 2.5f);
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Cyan;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Count real minions
            int minionCount = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == player.whoAmI && p.minion && p.minionSlots > 0f)
                    minionCount++;
            }

            int desired = System.Math.Min(minionCount, 8);
            int current = player.ownedProjectileCounts[ModContent.ProjectileType<RaiserFeathers>()];

            // Only the owner handles spawning / despawning
            if (Main.myPlayer == player.whoAmI)
            {
                // Spawn missing feathers
                if (current < desired)
                {
                    for (int i = current; i < desired; i++)
                    {
                        Projectile.NewProjectile(
                            player.GetSource_Accessory(Item),
                            player.Center,
                            Vector2.Zero,
                            ModContent.ProjectileType<RaiserFeathers>(),
                            0, 0f,
                            player.whoAmI,
                            ai0: i
                        );
                    }
                }
                // Kill excess feathers when minions despawn
                else if (current > desired)
                {
                    int toKill = current - desired;
                    for (int i = 0; i < Main.maxProjectiles && toKill > 0; i++)
                    {
                        Projectile p = Main.projectile[i];
                        if (p.active && p.owner == player.whoAmI && p.type == ModContent.ProjectileType<RaiserFeathers>())
                        {
                            p.Kill();
                            toKill--;
                        }
                    }
                }
            }

            // Flight boosts based on current feather count
            float boost = current; // 0–8

            // Extra flight time (base 180 + 45 per feather → ~540 at 8)
            player.wingTimeMax = (int)(180 + boost * 45f);

            // Small ground speed bonus
            player.moveSpeed += boost * 0.04f;

            // Infinite flight at max
            if (current >= 8)
            {
                player.wingTime = player.wingTimeMax;
            }
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            int feathers = player.ownedProjectileCounts[ModContent.ProjectileType<RaiserFeathers>()];
            float mult = 1f + feathers * 0.05f;

            ascentWhenFalling = 0.85f * mult;
            ascentWhenRising = 0.15f * mult;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f * mult;
            constantAscend = 0.135f * mult;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            int feathers = player.ownedProjectileCounts[ModContent.ProjectileType<RaiserFeathers>()];
            speed = 9f + feathers * 0.6f;              // up to ~13.8 at 8
            acceleration *= 2.5f + feathers * 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofFlight, 20)
                .AddIngredient(ItemID.FragmentStardust, 15)
                .AddIngredient(ItemID.LunarBar, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}