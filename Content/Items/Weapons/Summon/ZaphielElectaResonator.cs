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
    public class ZaphielElectaResonator : ModItem
    {
        public const int MaxBits = 7;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 48;
            Item.DamageType = GetInstance<PsychokineticDamageClass>();
            Item.width = 48;
            Item.height = 48;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 14f;
            Item.mana = 6;
            Item.knockBack = 3f;
            Item.buffType = BuffType<PsybitDefensiveArray>();
        }

        // Shoot / CanUseItem / AltFunctionUse same pattern as Coil with MaxBits = 7

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Entropic", "Attacks inflict Entropic Corruption")
            {
                OverrideColor = new Color(180, 80, 200)
            });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ZaphielElectaCoil>()
                .AddIngredient(ItemID.HellstoneBar, 12)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}