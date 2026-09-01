using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using System.Collections.Generic;

namespace CStudios.Content.Systems.ZaphielModules.Fracture
{
    public class FractureOverlay : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int idx = layers.FindIndex(l => l.Name == "Vanilla: Resource Bars");
            if (idx == -1)
                idx = layers.Count;

            layers.Insert(idx, new LegacyGameInterfaceLayer(
                "CStudios: Fracture Timer",
                () =>
                {
                    Draw();
                    return true;
                },
                InterfaceScaleType.Game));
        }

        private static void Draw()
        {
            if (Main.gameMenu || Main.dedServ)
                return;

            Player player = Main.LocalPlayer;
            if (player == null || !player.active || player.dead)
                return;

            var fp = player.GetModPlayer<ZaphielFracturePlayer>();
            string text;
            Color col;

            if (fp.FractureActive)
            {
                text = $"FRACTURE  {fp.FractureTimer / 60 + 1}s";
                col = new Color(180, 140, 255);
            }
            else if (fp.FractureCooldown > 0)
            {
                text = $"FRACTURE  {fp.FractureCooldown / 60 + 1}s";
                col = new Color(140, 130, 170);
            }
            else
                return;

            Vector2 size = FontAssets.MouseText.Value.MeasureString(text);
            Vector2 pos = player.Center - Main.screenPosition + new Vector2(-size.X * 0.5f, 70f);
            ChatManager.DrawColorCodedStringWithShadow(
                Main.spriteBatch, FontAssets.MouseText.Value, text,
                pos, col, 0f, Vector2.Zero, Vector2.One);
        }
    }
}
