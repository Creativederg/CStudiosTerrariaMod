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

        private static bool HoldingElecta(Player player)
        {
            if (player == null || !player.active)
                return false;
            Item held = player.HeldItem;
            if (held == null || held.IsAir || held.ModItem == null)
                return false;
            return held.ModItem.Name == "ZaphielElectaOmega";
        }

        public override void UpdateUI(GameTime gameTime)
        {
            Player player = Main.LocalPlayer;
            bool holding = HoldingElecta(player);

            if (OpenModulesKey != null && OpenModulesKey.JustPressed)
            {
                if (Interface?.CurrentState != null)
                    Close();
                else if (holding)
                    Open();
            }

            Interface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int idx = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
            if (idx == -1)
                idx = layers.FindIndex(l => l.Name == "Vanilla: Resource Bars");
            if (idx == -1)
                return;

            layers.Insert(idx, new LegacyGameInterfaceLayer(
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
