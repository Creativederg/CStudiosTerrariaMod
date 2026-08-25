using Terraria;
using Terraria.ModLoader;
using CStudios.Content.Buffs;

namespace CStudios.Content.NPCs
{
    public class CStudiosGlobalNPC : GlobalNPC
    {
        public int EntropicStacks = 0;
        public const int MaxStacks = 12;

        public override bool InstancePerEntity => true;

        public override void ResetEffects(NPC npc)
        {
            // Stacks decay slowly when the buff is not present
            if (!npc.HasBuff(ModContent.BuffType<EntropicCorruption>()))
            {
                if (EntropicStacks > 0 && Main.GameUpdateCount % 30 == 0)
                    EntropicStacks--;
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (EntropicStacks > 0 && npc.HasBuff(ModContent.BuffType<EntropicCorruption>()))
            {
                int dot = 4 + EntropicStacks * 3;
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= dot * 2;
                damage = dot;
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // Optional: extra damage from your own psychokinetic projectiles
        }
    }
}