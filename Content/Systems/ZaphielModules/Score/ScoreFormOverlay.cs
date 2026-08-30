using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace CStudios.Content.Systems.ZaphielModules.Score
{
    public class ScoreFormOverlay : ModSystem
    {
        private const int BarWidth = 72;
        private const int BarHeight = 8;

        private float _displayFill;
        private float _lastScore;
        private float _pulse;
        private readonly List<BarSpark> _sparks = new();

        private struct BarSpark
        {
            public Vector2 Pos;
            public Vector2 Vel;
            public float Life;
            public float MaxLife;
            public Color Color;
            public float Size;
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (Main.gameMenu || Main.dedServ)
                return;

            Player player = Main.LocalPlayer;
            if (player == null || !player.active || player.dead)
                return;

            var ctx = ZaphielModuleSystem.Resolve(player);
            if (!ctx.ScoreMode && !ctx.FeedbackHeartActive)
                return;

            var sp = player.GetModPlayer<ZaphielScorePlayer>();
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            float target = MathHelper.Clamp(sp.Score01, 0f, 1f);
            _displayFill = MathHelper.Lerp(_displayFill, target, 0.18f);

            float gained = sp.Score - _lastScore;
            _lastScore = sp.Score;

            Vector2 center = player.Center - Main.screenPosition + new Vector2(0f, 42f);
            Vector2 barPos = center - new Vector2(BarWidth * 0.5f, 0f);

            Color fillColor = GetFillColor(sp, _displayFill, out Color glowColor);

            if (gained > 0.05f)
                SpawnFillSparks(barPos, _displayFill, fillColor, gained);

            UpdateSparks();

            _pulse += 0.12f;
            float pulse = 0.55f + 0.45f * (float)System.Math.Sin(_pulse);
            if (_displayFill > 0.8f || sp.InStorm)
            {
                int glowPad = 2 + (int)(pulse * 2f);
                spriteBatch.Draw(pixel,
                    new Rectangle((int)barPos.X - glowPad, (int)barPos.Y - glowPad,
                        BarWidth + glowPad * 2, BarHeight + glowPad * 2),
                    glowColor * (0.18f + 0.12f * pulse));
            }

            spriteBatch.Draw(pixel,
                new Rectangle((int)barPos.X - 1, (int)barPos.Y - 1, BarWidth + 2, BarHeight + 2),
                new Color(20, 16, 10, 220));

            spriteBatch.Draw(pixel,
                new Rectangle((int)barPos.X, (int)barPos.Y, BarWidth, BarHeight),
                new Color(40, 32, 16, 200));

            int fillW = (int)(BarWidth * _displayFill);
            if (fillW > 0)
            {
                spriteBatch.Draw(pixel,
                    new Rectangle((int)barPos.X, (int)barPos.Y, fillW, BarHeight),
                    fillColor);

                spriteBatch.Draw(pixel,
                    new Rectangle((int)barPos.X, (int)barPos.Y, fillW, 2),
                    Color.White * 0.40f);

                // moving sheen
                int sheenX = (int)barPos.X + (int)((_pulse * 7f) % System.Math.Max(1, fillW));
                spriteBatch.Draw(pixel,
                    new Rectangle(sheenX, (int)barPos.Y, 4, BarHeight),
                    Color.White * 0.25f);
            }

            DrawSparks(spriteBatch, pixel);

            string label;
            Color labelColor;
            if (sp.InStorm)
            {
                label = $"STORM {sp.StormTimer / 60 + 1}s";
                labelColor = new Color(255, 120, 80);
            }
            else if (sp.BurstCooldown > 0 && sp.Score < 15f)
            {
                label = $"BURST {sp.BurstCooldown / 60 + 1}s";
                labelColor = new Color(180, 170, 140);
            }
            else
            {
                label = $"{sp.Score:0}";
                labelColor = fillColor;
            }

            Vector2 size = FontAssets.MouseText.Value.MeasureString(label) * 0.8f;
            ChatManager.DrawColorCodedStringWithShadow(
                spriteBatch,
                FontAssets.MouseText.Value,
                label,
                barPos + new Vector2(BarWidth * 0.5f - size.X * 0.5f, BarHeight + 2f),
                labelColor,
                0f,
                Vector2.Zero,
                new Vector2(0.8f));
        }

        private static Color GetFillColor(ZaphielScorePlayer sp, float fill, out Color glow)
        {
            if (sp.InStorm)
            {
                float t = 0.5f + 0.5f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 14f);
                glow = new Color(255, 60, 30);
                return Color.Lerp(new Color(180, 40, 20), new Color(255, 140, 50), t);
            }

            Color low = new Color(160, 90, 30);
            Color mid = new Color(255, 190, 50);
            Color high = new Color(255, 245, 180);

            Color c;
            if (fill < 0.5f)
                c = Color.Lerp(low, mid, fill / 0.5f);
            else
                c = Color.Lerp(mid, high, (fill - 0.5f) / 0.5f);

            glow = c;
            return c;
        }

        private void SpawnFillSparks(Vector2 barPos, float fill, Color color, float gained)
        {
            int count = (int)MathHelper.Clamp(gained * 2f, 1f, 8f);
            float edgeX = barPos.X + BarWidth * fill;
            float y = barPos.Y + BarHeight * 0.5f;

            for (int i = 0; i < count; i++)
            {
                _sparks.Add(new BarSpark
                {
                    Pos = new Vector2(edgeX + Main.rand.NextFloat(-2f, 2f), y),
                    Vel = new Vector2(Main.rand.NextFloat(-0.4f, 1.2f), Main.rand.NextFloat(-2.2f, -0.6f)),
                    Life = 0f,
                    MaxLife = Main.rand.NextFloat(18f, 32f),
                    Color = color,
                    Size = Main.rand.NextFloat(1.5f, 3.2f)
                });
            }

            if (_sparks.Count > 40)
                _sparks.RemoveRange(0, _sparks.Count - 40);
        }

        private void UpdateSparks()
        {
            for (int i = _sparks.Count - 1; i >= 0; i--)
            {
                BarSpark s = _sparks[i];
                s.Life++;
                s.Pos += s.Vel;
                s.Vel.Y += 0.08f;
                s.Vel *= 0.96f;
                if (s.Life >= s.MaxLife)
                    _sparks.RemoveAt(i);
                else
                    _sparks[i] = s;
            }
        }

        private void DrawSparks(SpriteBatch spriteBatch, Texture2D pixel)
        {
            for (int i = 0; i < _sparks.Count; i++)
            {
                BarSpark s = _sparks[i];
                float t = 1f - s.Life / s.MaxLife;
                int size = (int)System.Math.Max(1f, s.Size * t);
                spriteBatch.Draw(pixel,
                    new Rectangle((int)s.Pos.X, (int)s.Pos.Y, size, size),
                    s.Color * t);
            }
        }
    }
}
