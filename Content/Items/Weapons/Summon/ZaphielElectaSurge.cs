using Microsoft.Xna.Framework;
using CStudios.Content.Buffs;
using CStudios.Content.DamageClasses;
using CStudios.Content.Projectiles.Summon.Psybits;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Items.Weapons.Summon
{
    public class ZaphielElectaSurge : ModItem
    {
        public const int MaxBits = 9;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 72;
            Item.DamageType = GetInstance<PsychokineticDamageClass>();
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.buyPrice(gold: 12);
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 16f;
            Item.mana = 6;
            Item.knockBack = 3.5f;
            Item.buffType = BuffType<PsybitDefensiveArray>();
        }

        public override void HoldItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return;

            // Ultimate unlocked at this tier
            if (CStudios.UltimateKey != null
                && CStudios.UltimateKey.JustPressed
                && !player.HasBuff(BuffType<PsybitOvercharge>())
                && !player.HasBuff(BuffType<PsybitOverchargedCooldown>())
                && player.ownedProjectileCounts[ProjectileType<Psybits>()] > 0)
            {
                player.AddBuff(BuffType<PsybitOvercharge>(), 8 * 60);      // slightly shorter than Omega
                player.AddBuff(BuffType<PsybitOverchargedCooldown>(), 150 * 60); // longer CD than Omega
                SoundEngine.PlaySound(SoundID.Item113, player.Center);
            }
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Entropic", "Attacks inflict Entropic Corruption")
            {
                OverrideColor = new Color(180, 80, 200)
            });
            tooltips.Add(new TooltipLine(Mod, "Ult", "Press Ultimate key to overcharge Psybits briefly")
            {
                OverrideColor = new Color(255, 100, 120)
            });
        }

        // Shoot same pattern, MaxBits = 9

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ZaphielElectaResonator>()
                .AddIngredient(ItemID.ChlorophyteBar, 12)
                .AddIngredient(ItemID.SoulofFright, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}