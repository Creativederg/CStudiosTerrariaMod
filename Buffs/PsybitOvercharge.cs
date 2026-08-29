using Terraria;
using Terraria.ModLoader;

namespace CStudios.Content.Buffs
{
    public class PsybitOvercharge : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false; // show the 10s timer
            Main.debuff[Type] = false;
        }
    }
}