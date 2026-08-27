using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CStudios.Content.Items.Weapons.Summon
{
    public class ZaphielBossProgress : GlobalItem
    {
        public HashSet<int> KilledBossTypes = new();

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.ModItem is ZaphielElectaApex || entity.ModItem is ZaphielElectaOmega;
        }

        public override GlobalItem Clone(Item from, Item to)
        {
            var clone = (ZaphielBossProgress)base.Clone(from, to);
            clone.KilledBossTypes = new HashSet<int>(KilledBossTypes);
            return clone;
        }

        public override void SaveData(Item item, TagCompound tag)
        {
            if (!AppliesToEntity(item, true))
                return;
            tag["zaphielBosses"] = new List<int>(KilledBossTypes);
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            if (!AppliesToEntity(item, true))
                return;

            KilledBossTypes = new HashSet<int>();
            if (tag.ContainsKey("zaphielBosses"))
            {
                foreach (int id in tag.Get<List<int>>("zaphielBosses"))
                    KilledBossTypes.Add(id);
            }
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            writer.Write(KilledBossTypes.Count);
            foreach (int id in KilledBossTypes)
                writer.Write(id);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            KilledBossTypes = new HashSet<int>();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                KilledBossTypes.Add(reader.ReadInt32());
        }

        public static float GetBossPowerMul(Item item)
        {
            var prog = item.GetGlobalItem<ZaphielBossProgress>();
            return 1f + prog.KilledBossTypes.Count * 0.03f;
        }
    }
}