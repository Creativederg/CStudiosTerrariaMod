using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace CStudios.Content.Systems.ZaphielModules.Aerial
{
    public class AerialFormOverlay : ModSystem
    {
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (Main.gameMenu || Main.dedServ)
                return;

            Player player = Main.LocalPlayer;
            if (player == null || !player.active || player.dead)
                return;

            var aerial = player.GetModPlayer<ZaphielAerialPlayer>();

            string text;
            Color color;

            if (aerial.FormActive && aerial.FormTimer > 0)
            {
                text = $"HERRSCHER  {aerial.FormTimer / 60 + 1}s";
                color = new Color(120, 210, 255);
            }
            else if (aerial.FormCooldown > 0)
            {
                text = $"FORM  {aerial.FormCooldown / 60 + 1}s";
                color = new Color(180, 190, 220);
            }
            else
                return;

            Vector2 size = FontAssets.MouseText.Value.MeasureString(text);
            Vector2 pos = player.Center - Main.screenPosition + new Vector2(-size.X * 0.5f, 40f);

            ChatManager.DrawColorCodedStringWithShadow(
                spriteBatch,
                FontAssets.MouseText.Value,
                text,
                pos,
                color,
                0f,
                Vector2.Zero,
                Vector2.One);
        }
    }
}