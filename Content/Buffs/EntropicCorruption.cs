using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CStudios.Content.NPCs;

namespace CStudios.Content.Buffs
{
    public class EntropicCorruption : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            var global = npc.GetGlobalNPC<CStudiosGlobalNPC>();

            // Defense reduction (~2.5% per stack)
            float defenseMult = 1f - (0.025f * global.EntropicStacks);
            npc.defense = (int)(npc.defense * defenseMult);

            // Attack power reduction is handled via damage dealt modifiers if desired
            // (see GlobalNPC if you want to lower contact damage)
        }

        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            var global = npc.GetGlobalNPC<CStudiosGlobalNPC>();
            if (global.EntropicStacks < CStudiosGlobalNPC.MaxStacks)
                global.EntropicStacks++;

            npc.buffTime[buffIndex] = time;
            return false;
        }
    }
}