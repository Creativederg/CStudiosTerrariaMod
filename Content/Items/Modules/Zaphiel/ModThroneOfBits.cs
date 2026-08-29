using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModThroneOfBits : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.Psybits;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.AuthorityMode = true;
            ctx.ThroneOfBitsActive = true;

            ctx.AuthorityBonusBits += 4;
            ctx.MaxBitsMul = System.Math.Max(ctx.MaxBitsMul, 1.35f);
            ctx.MinionSlotsPerBit = System.Math.Min(ctx.MinionSlotsPerBit, 0.85f);

            ctx.MinionDamageMul *= 1.20f;
            ctx.MinionFireRangeMul *= 1.15f;
            ctx.MinionAggressiveChase = true;
            ctx.AuthorityFormationIntegrity *= 1.25f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "AuthorityDesc",
                "Authority of the Bits – Throne of Bits\n" +
                "Greatly increases maximum Psybits and lets them act as one collective mind.\n" +
                "Bits become more efficient (lower slot cost) and hit harder while in formation.")
            {
                OverrideColor = new Color(180, 120, 255)
            });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentStardust, 12)
                .AddIngredient(ItemID.FragmentNebula, 6)
                .AddIngredient(ItemID.SoulofFlight, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}