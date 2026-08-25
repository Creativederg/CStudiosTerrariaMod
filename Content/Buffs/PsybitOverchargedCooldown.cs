using Terraria;
using Terraria.ModLoader;

namespace CStudios.Content.Buffs
{
    public class PsybitOverchargedCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
    }
}