using System.Collections.Generic;
using CStudios.Content.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.Items.Armor
{
    public class ZaphielOmegaArmorTooltips : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemType<PsybitCoronationHelmet>()
                || entity.type == ItemType<PsybitCoronationBreastplate>()
                || entity.type == ItemType<PsybitCoronationLeggings>();
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            var ap = player.GetModPlayer<ZaphielArmorPlayer>();

            int helm = ItemType<PsybitCoronationHelmet>();
            int body = ItemType<PsybitCoronationBreastplate>();
            int legs = ItemType<PsybitCoronationLeggings>();

            string pieceStats;
            if (item.type == helm)
                pieceStats = "+80% Psychokinetic damage\n+20% Psychokinetic crit chance\n+40 maximum mana";
            else if (item.type == body)
                pieceStats = "+100% Psychokinetic damage\n+3 max minions\n+40 maximum life";
            else
                pieceStats = "+68% Psychokinetic damage\n+2 max minions\n+12% movement speed";

            tooltips.Add(new TooltipLine(Mod, "OmegaStats", pieceStats)
            {
                OverrideColor = new Color(220, 200, 255)
            });

            string matrix = ap.ActiveFantasy switch
            {
                ZaphielFantasy.AerialHerrscher =>
                    "Matrix: Sky Sovereign (Providence)\nBase Coronation stats. No fall damage, jump boost, +15% move.",
                ZaphielFantasy.AuthorityOfTheBits =>
                    "Matrix: Throne Command (Polterghast)\n+4 defense, +8% PK, +2 minions.\nTwo extra bits that cost no slots.",
                ZaphielFantasy.ScoreAndStigma =>
                    "Matrix: Resonant Overclock (DoG)\n+8 defense, +14% PK, +8% crit, +20 mana.\nScore stays live at cap.",
                ZaphielFantasy.TimeFracture =>
                    "Matrix: Permet Mantle (Yharon)\n+12 defense, +20% PK, +2 minions, +4% DR.",
                ZaphielFantasy.Finality =>
                    "Matrix: Last Herrscher (pre-SCal)\n+18 defense, +28% PK, +40 life, +8% DR.\nTrace bolts pulse during Finality.",
                _ => ap.OmegaSet
                    ? "Matrix: Adaptive — slot a fantasy core to lock a set personality."
                    : "Matrix: Wear the full set and install a fantasy core to adapt."
            };

            tooltips.Add(new TooltipLine(Mod, "OmegaMatrix", matrix)
            {
                OverrideColor = FantasyColor(ap.ActiveFantasy)
            });
        }

        private static Color FantasyColor(ZaphielFantasy f) => f switch
        {
            ZaphielFantasy.AuthorityOfTheBits => new Color(200, 160, 255),
            ZaphielFantasy.AerialHerrscher => new Color(120, 200, 255),
            ZaphielFantasy.ScoreAndStigma => new Color(255, 140, 90),
            ZaphielFantasy.TimeFracture => new Color(160, 220, 255),
            ZaphielFantasy.Finality => new Color(255, 220, 120),
            _ => new Color(200, 190, 220)
        };
    }
}
