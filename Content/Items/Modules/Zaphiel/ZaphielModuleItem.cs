using CStudios.Content.Systems.ZaphielModules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CStudios.Content.Items.Modules.Zaphiel
{
    public enum ZaphielSlotCategory
    {
        Spearhead = 0,
        Guidance = 1,
        EnergyCore = 2,
        EnergyRelay = 3,
        Psybits = 4,
    }

    public abstract class ZaphielModuleItem : ModItem
    {
        public abstract ZaphielSlotCategory SlotCategory { get; }

        public abstract void Apply(ref ZaphielShootContext ctx);

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(gold: 5);
            //Item.rare = ItemRarityID.Yellow;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            string label = SlotCategory switch
            {
                ZaphielSlotCategory.Spearhead => "Spearhead",
                ZaphielSlotCategory.Guidance => "Guidance",
                ZaphielSlotCategory.EnergyCore => "Energy Core",
                ZaphielSlotCategory.EnergyRelay => "Energy Relay",
                ZaphielSlotCategory.Psybits => "Psybits",
                _ => SlotCategory.ToString()
            };

            tooltips.Add(new TooltipLine(Mod, "ZaphielSlot", $"Lance Matrix slot: {label}")
            {
                OverrideColor = SlotColor(SlotCategory)
            });
        }

        public static Color SlotColor(ZaphielSlotCategory cat) => cat switch
        {
            ZaphielSlotCategory.Spearhead => new Color(255, 160, 60),
            ZaphielSlotCategory.Guidance => new Color(0, 200, 255),
            ZaphielSlotCategory.EnergyCore => new Color(255, 220, 0),
            ZaphielSlotCategory.EnergyRelay => new Color(255, 80, 120),
            ZaphielSlotCategory.Psybits => new Color(180, 120, 255),
            _ => Color.White,
        };
    }
}