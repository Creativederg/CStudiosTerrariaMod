using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CStudios.Content.Systems.ZaphielModules
{
    public class ZaphielModulePlayer : ModPlayer
    {
        public const int SlotCount = 5;
        public const int PresetCount = 5;

        public int ActivePreset;
        public Item[,] Presets = new Item[PresetCount, SlotCount];

        /// <summary>View of the active preset. Not a second copy.</summary>
        public Item[] Modules
        {
            get
            {
                var arr = new Item[SlotCount];
                for (int i = 0; i < SlotCount; i++)
                    arr[i] = GetSlot(ActivePreset, i);
                return arr;
            }
        }

        public override void Initialize()
        {
            ActivePreset = 0;
            for (int p = 0; p < PresetCount; p++)
                for (int i = 0; i < SlotCount; i++)
                    Presets[p, i] = new Item();
        }

        public Item GetSlot(int preset, int slot)
        {
            if (preset < 0 || preset >= PresetCount || slot < 0 || slot >= SlotCount)
                return new Item();
            Item m = Presets[preset, slot];
            return m ?? new Item();
        }

        public void SetSlot(int preset, int slot, Item item)
        {
            if (preset < 0 || preset >= PresetCount || slot < 0 || slot >= SlotCount)
                return;
            Presets[preset, slot] = item ?? new Item();
        }

        public Item GetModule(int slot)
        {
            Item m = GetSlot(ActivePreset, slot);
            return m.IsAir ? null : m;
        }

        public void SetActiveSlot(int slot, Item item) => SetSlot(ActivePreset, slot, item);

        public void SelectPreset(int preset)
        {
            if (preset < 0 || preset >= PresetCount)
                return;
            ActivePreset = preset;
        }

        public bool PresetHasItems(int preset)
        {
            for (int i = 0; i < SlotCount; i++)
            {
                Item m = GetSlot(preset, i);
                if (m != null && !m.IsAir)
                    return true;
            }
            return false;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["lanceActivePreset"] = ActivePreset;
            tag["lancePanelX"] = PanelX;
            tag["lancePanelY"] = PanelY;

            for (int p = 0; p < PresetCount; p++)
            {
                for (int i = 0; i < SlotCount; i++)
                {
                    Item m = GetSlot(p, i);
                    if (m != null && !m.IsAir)
                        tag[$"lancePreset{p}_{i}"] = m;
                }
            }
        }

        public float PanelX = -1f;
        public float PanelY = -1f;

        public override void LoadData(TagCompound tag)
        {
            ActivePreset = tag.ContainsKey("lanceActivePreset") ? tag.GetInt("lanceActivePreset") : 0;
            if (ActivePreset < 0 || ActivePreset >= PresetCount)
                ActivePreset = 0;

            if (tag.ContainsKey("lancePanelX"))
                PanelX = tag.GetFloat("lancePanelX");
            if (tag.ContainsKey("lancePanelY"))
                PanelY = tag.GetFloat("lancePanelY");

            for (int p = 0; p < PresetCount; p++)
            {
                for (int i = 0; i < SlotCount; i++)
                {
                    // migrate old single-bank save into preset 0
                    if (p == 0 && tag.ContainsKey($"lanceMod{i}") && !tag.ContainsKey($"lancePreset0_{i}"))
                        Presets[0, i] = tag.Get<Item>($"lanceMod{i}");
                    else
                        Presets[p, i] = tag.ContainsKey($"lancePreset{p}_{i}")
                            ? tag.Get<Item>($"lancePreset{p}_{i}")
                            : new Item();
                }
            }
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            var clone = (ZaphielModulePlayer)targetCopy;
            clone.ActivePreset = ActivePreset;
            for (int p = 0; p < PresetCount; p++)
                for (int i = 0; i < SlotCount; i++)
                    clone.Presets[p, i] = GetSlot(p, i).Clone();
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            var clone = (ZaphielModulePlayer)clientPlayer;
            if (clone.ActivePreset != ActivePreset)
            {
                SyncPlayer(-1, Main.myPlayer, false);
                return;
            }
            for (int i = 0; i < SlotCount; i++)
            {
                if (ItemIO.ToBase64(GetSlot(ActivePreset, i)) != ItemIO.ToBase64(clone.GetSlot(ActivePreset, i)))
                {
                    SyncPlayer(-1, Main.myPlayer, false);
                    return;
                }
            }
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)0);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)ActivePreset);
            for (int i = 0; i < SlotCount; i++)
                ItemIO.Send(GetSlot(ActivePreset, i), packet, writeStack: true, writeFavorite: false);
            packet.Send(toWho, fromWho);
        }
    }
}
