using CStudios.Content.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CStudios.Content.NPCs
{
    public class ZaphielBossKillNPC : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            if (!npc.boss && npc.realLife < 0)
                return;

            int playerIndex = npc.lastInteraction;
            if (playerIndex < 0 || playerIndex >= Main.maxPlayers)
                playerIndex = npc.target;
            if (playerIndex < 0 || playerIndex >= Main.maxPlayers)
                return;

            Player player = Main.player[playerIndex];
            if (!player.active)
                return;

            Item held = player.HeldItem;
            if (held?.ModItem is not ZaphielElectaApex && held?.ModItem is not ZaphielElectaOmega)
                return;

            var prog = held.GetGlobalItem<ZaphielBossProgress>();
            int key = npc.realLife >= 0 ? Main.npc[npc.realLife].type : npc.type;

            if (prog.KilledBossTypes.Add(key) && player.whoAmI == Main.myPlayer)
            {
                CombatText.NewText(player.getRect(), new Color(255, 80, 120),
                    $"Zaphiel Sync +1 ({prog.KilledBossTypes.Count})", true);
            }
        }
    }
}