using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CStudios.Content.Systems.ZaphielModules
{
    public class ZaphielModulePlayer : ModPlayer
    {
        public const int SlotCount = 5;
        public Item[] Modules = new Item[SlotCount];

        public override void Initialize()
        {
            for (int i = 0; i < SlotCount; i++)
                Modules[i] = new Item();
        }

        public Item GetModule(int slot)
        {
            if (slot < 0 || slot >= SlotCount)
                return null;
            Item m = Modules[slot];
            return m == null || m.IsAir ? null : m;
        }

        public override void SaveData(TagCompound tag)
        {
            for (int i = 0; i < SlotCount; i++)
            {
                if (Modules[i] != null && !Modules[i].IsAir)
                    tag[$"lanceMod{i}"] = Modules[i];
            }
        }

        public override void LoadData(TagCompound tag)
        {
            for (int i = 0; i < SlotCount; i++)
            {
                Modules[i] = tag.ContainsKey($"lanceMod{i}")
                    ? tag.Get<Item>($"lanceMod{i}")
                    : new Item();
            }
        }

        // --- Must override BOTH or NEITHER ---

        public override void CopyClientState(ModPlayer targetCopy)
        {
            var clone = (ZaphielModulePlayer)targetCopy;
            for (int i = 0; i < SlotCount; i++)
                clone.Modules[i] = Modules[i].Clone();
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            var clone = (ZaphielModulePlayer)clientPlayer;
            for (int i = 0; i < SlotCount; i++)
            {
                if (ItemIO.ToBase64(Modules[i]) != ItemIO.ToBase64(clone.Modules[i]))
                {
                    // Something changed — full sync via SyncPlayer is fine for 5 slots
                    SyncPlayer(-1, Main.myPlayer, false);
                    return;
                }
            }
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)0); // packet id: Zaphiel modules
            packet.Write((byte)Player.whoAmI);
            for (int i = 0; i < SlotCount; i++)
                ItemIO.Send(Modules[i], packet, writeStack: true, writeFavorite: false);
            packet.Send(toWho, fromWho);
        }
    }
}