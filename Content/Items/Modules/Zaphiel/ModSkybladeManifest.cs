using CStudios.Content.Systems.ZaphielModules;
using CStudios.Content.Items.Modules.Zaphiel;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public class ModSkybladeManifest : ZaphielModuleItem
    {
        public override ZaphielSlotCategory SlotCategory => ZaphielSlotCategory.Spearhead;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.buyPrice(gold: 25);
        }

        public override void Apply(ref ZaphielShootContext ctx)
        {
            ctx.AerialMode = true;
            ctx.SkybladeManifestActive = true;

            // Ground beam is weaker; aerial slashes / ribbons are the real weapon
            ctx.DamageMul *= 0.85f;
            ctx.MeleeMode = false;
            ctx.TraceVolleyMode = false;

            ctx.MinionDamageMul *= 1.15f;
            ctx.MinionMoveSpeedMul *= 1.20f;
            ctx.SpreadMul *= 1.15f;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(Mod, "AerialDesc",
                "Aerial Herrscher Form – Skyblade Manifest\n" +
                "Weapon becomes wide aerial slashes and beam ribbons while the Form is active.\n" +
                "Ground fire is weaker. Bits gain speed.")
            {
                OverrideColor = new Color(120, 200, 255)
            });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentVortex, 8)
                .AddIngredient(ItemID.SoulofFlight, 12)
                .AddIngredient(ItemID.Feather, 15)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
