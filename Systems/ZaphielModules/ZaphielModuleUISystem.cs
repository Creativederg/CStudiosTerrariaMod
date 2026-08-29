using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace CStudios.Content.Systems.ZaphielModules
{
    public class ZaphielModuleUISystem : ModSystem
    {
        internal static UserInterface Interface;
        internal static ZaphielModuleUI UI;
        public static ModKeybind OpenModulesKey;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                UI = new ZaphielModuleUI();
                UI.Activate();
                Interface = new UserInterface();
            }

            OpenModulesKey = KeybindLoader.RegisterKeybind(Mod, "Zaphiel Lance Modules", "K");
        }

        public override void Unload()
        {
            UI = null;
            Interface = null;
            OpenModulesKey = null;
        }

        public static void Open() => Interface?.SetState(UI);
        public static void Close() => Interface?.SetState(null);

        public static void Toggle()
        {
            if (Interface?.CurrentState == null)
                Open();
            else
                Close();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (OpenModulesKey != null && OpenModulesKey.JustPressed)
                Toggle();

            Interface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int idx = layers.FindIndex(l => l.Name == "Vanilla: Resource Bars");
            if (idx == -1)
                return;

            layers.Insert(idx + 1, new LegacyGameInterfaceLayer(
                "CStudios: Lance Matrix",
                () =>
                {
                    Interface?.Draw(Main.spriteBatch, new GameTime());
                    return true;
                },
                InterfaceScaleType.UI));
        }
    }
}