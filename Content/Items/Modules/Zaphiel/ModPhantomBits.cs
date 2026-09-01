using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModPhantomBits : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.Psybits;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 28);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.FractureMode = true;
            ctx.PhantomBitsActive = true;
            ctx.MinionDamageMul *= 1.12f;
            ctx.MinionMoveSpeedMul *= 1.15f;
            ctx.FunnelFireRateMul *= 1.20f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "FracDesc",
                "Time Fracture – Phantom Bits\nDuring Fracture bits fire twice as often and ignore most knockback on themselves.")
            { OverrideColor = new Color(180, 140, 255) });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentStardust, 10)
                .AddIngredient(ItemID.FragmentVortex, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
