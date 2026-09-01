using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using CStudios.Content.DamageClasses;
using static Terraria.ModLoader.ModContent;

namespace CStudios.Content.DamageClasses
{
    public class PsychokineticTooltipGlobal : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.DamageType != null
                && entity.DamageType.CountsAsClass(GetInstance<PsychokineticDamageClass>());
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            Color pink = new Color(255, 90, 170);
            Color white = new Color(255, 240, 250);
            float t = 0.5f + 0.5f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 4f);
            Color pulse = Color.Lerp(pink, white, t);

            for (int i = 0; i < tooltips.Count; i++)
            {
                TooltipLine line = tooltips[i];
                if (line.Mod != "Terraria")
                    continue;
                if (line.Name == "Damage" || line.Name == "CritChance" || line.Name == "Speed"
                    || line.Name == "Knockback")
                    line.OverrideColor = pulse;
            }
        }
    }
}
