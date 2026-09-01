using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace CStudios.Content.Systems.ZaphielModules.Finality
{
    public class FinalityOverlay : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int idx = layers.FindIndex(l => l.Name == "Vanilla: Resource Bars");
            if (idx == -1)
                idx = layers.Count;

            layers.Insert(idx, new LegacyGameInterfaceLayer(
                "CStudios: Finality Timer",
                () => { Draw(); return true; },
                InterfaceScaleType.Game));
        }

        private static void Draw()
        {
            if (Main.gameMenu || Main.dedServ)
                return;

            Player player = Main.LocalPlayer;
            if (player == null || !player.active || player.dead)
                return;

            var fp = player.GetModPlayer<ZaphielFinalityPlayer>();
            string text;
            Color col;
            if (fp.FinalityActive)
            {
                text = $"FINALITY  {fp.FinalityTimer / 60 + 1}s";
                col = new Color(255, 220, 140);
            }
            else if (fp.FinalityCooldown > 0)
            {
                text = $"FINALITY  {fp.FinalityCooldown / 60 + 1}s";
                col = new Color(160, 140, 110);
            }
            else
                return;

            Vector2 size = FontAssets.MouseText.Value.MeasureString(text);
            Vector2 pos = player.Center - Main.screenPosition + new Vector2(-size.X * 0.5f, 84f);
            ChatManager.DrawColorCodedStringWithShadow(
                Main.spriteBatch, FontAssets.MouseText.Value, text,
                pos, col, 0f, Vector2.Zero, Vector2.One);
        }
    }
}
