using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using CStudios.Content.Projectiles.Summon.Psybits;

namespace CStudios.Content.Buffs
{
    public class PsybitDefensiveArray : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
             //Keep the buff alive as long as any Gundbit exists
            if (player.ownedProjectileCounts[ProjectileType<Psybits>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}