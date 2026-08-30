using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModAerialSwarmProtocol : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.Psybits;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.buyPrice(gold: 28);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.AerialMode = true;
            ctx.AerialSwarmProtocolActive = true;

            ctx.AerialBitIndepMul *= 1.35f;
            ctx.MinionMoveSpeedMul *= 1.25f;
            ctx.MinionOrbitRadiusMul *= 1.30f;
            ctx.MinionDamageMul *= 1.12f;
            ctx.MinionAggressiveChase = true;
            ctx.MinionRandomOrbit = true;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "AerialDesc",
                "Aerial Herrscher Form – Aerial Swarm Protocol\n" +
                "Bits gain independent 3D movement and can dive / spiral only while the Form is active.\n" +
                "Larger orbit, faster chase.")
            {
                OverrideColor = new Color(120, 200, 255)
            });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentStardust, 10)
                .AddIngredient(ItemID.FragmentVortex, 8)
                .AddIngredient(ItemID.SoulofFlight, 12)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
