using CStudios.Content.Systems.ZaphielModules;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModEnergyCore : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.EnergyCore;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.DamageMul *= 1.18f;
            ctx.CritAdd += 6;
            ctx.MinionDamageMul *= 1.10f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofLight, 5)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.HallowedBar, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}