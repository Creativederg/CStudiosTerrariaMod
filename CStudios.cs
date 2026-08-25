using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using CStudios.Content.Items.Weapons.Summon;

namespace CStudios
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class CStudios : Mod
	{
        public override void Load()
        {
            //PsychokineticArray.OverchargeKey = KeybindLoader.RegisterKeybind(this, "Psybit Overcharge", "Q");
        }

        public override void Unload()
        {
            
        }
    }
}
