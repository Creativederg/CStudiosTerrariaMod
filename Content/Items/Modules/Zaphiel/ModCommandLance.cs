using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModCommandLance : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.Spearhead;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.AuthorityMode = true;
            ctx.CommandLanceActive = true;

            ctx.DamageMul *= 0.65f;
            ctx.ContinuousBeam = false;
            ctx.MeleeMode = false;
            ctx.TraceVolleyMode = false;

            ctx.MinionDamageMul *= 1.25f;
            ctx.MinionAggressiveChase = true;
            ctx.MinionMoveSpeedMul *= 1.15f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "AuthorityDesc",
                "Authority of the Bits – Command Lance\n" +
                "Weapon damage greatly reduced. Every attack issues orders to Psybits.\n" +
                "Psybits gain significant damage, speed and aggression.")
            {
                OverrideColor = new Color(180, 120, 255)
            });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentStardust, 8)
                .AddIngredient(ItemID.FragmentNebula, 8)
                .AddIngredient(ItemID.SoulofMight, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}