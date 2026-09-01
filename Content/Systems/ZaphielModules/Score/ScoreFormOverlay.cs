using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
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

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int idx = layers.FindIndex(l => l.Name == "Vanilla: Resource Bars");
            if (idx == -1)
                idx = layers.Count;

            layers.Insert(idx, new LegacyGameInterfaceLayer(
                "CStudios: Score Bar",
                () => { DrawBar(); return true; },
                InterfaceScaleType.Game));
        }

        private void DrawBar()
        {
            if (Main.gameMenu || Main.dedServ)
                return;

            Player player = Main.LocalPlayer;
            if (player == null || !player.active || player.dead)
                return;

            var ctx = ZaphielModuleSystem.Resolve(player);
            var sp = player.GetModPlayer<ZaphielScorePlayer>();

            bool show = ctx.ScoreMode || ctx.FeedbackHeartActive
                || ctx.RisingScoreEdgeActive || ctx.LivingGaugeActive
                || sp.Score > 0f || sp.InStorm || sp.BurstCooldown > 0;
            if (!show)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            float target = MathHelper.Clamp(sp.Score01, 0f, 1f);
            _displayFill = MathHelper.Lerp(_displayFill, target, 0.18f);
            float gained = sp.Score - _lastScore;
            _lastScore = sp.Score;

            Vector2 barPos = player.Center - Main.screenPosition
                + new Vector2(-BarWidth * 0.5f, 42f);

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

            int fillW = System.Math.Max(1, (int)(BarWidth * _displayFill));
            spriteBatch.Draw(pixel, new Rectangle((int)barPos.X, (int)barPos.Y, fillW, BarHeight), fillColor);
            spriteBatch.Draw(pixel, new Rectangle((int)barPos.X, (int)barPos.Y, fillW, 2), Color.White * 0.4f);
            DrawSparks(spriteBatch, pixel);

            string label = $"{sp.Score:0}";
            Vector2 size = FontAssets.MouseText.Value.MeasureString(label) * 0.8f;
            ChatManager.DrawColorCodedStringWithShadow(
                spriteBatch, FontAssets.MouseText.Value, label,
                barPos + new Vector2(BarWidth * 0.5f - size.X * 0.5f, BarHeight + 2f),
                fillColor, 0f, Vector2.Zero, new Vector2(0.8f));

            Vector2 cdPos = barPos + new Vector2(0f, BarHeight + 14f);
            int cdH = 4;

            if (sp.InStorm)
            {
                float stormMax = 4f * 60f * (ctx.DataStormMildMul > 0.01f ? ctx.DataStormMildMul : 1f);
                float stormFill = MathHelper.Clamp(sp.StormTimer / stormMax, 0f, 1f);
                spriteBatch.Draw(pixel, new Rectangle((int)cdPos.X - 1, (int)cdPos.Y - 1, BarWidth + 2, cdH + 2), new Color(30, 8, 8, 220));
                spriteBatch.Draw(pixel, new Rectangle((int)cdPos.X, (int)cdPos.Y, BarWidth, cdH), new Color(70, 20, 16, 200));
                spriteBatch.Draw(pixel, new Rectangle((int)cdPos.X, (int)cdPos.Y, System.Math.Max(1, (int)(BarWidth * stormFill)), cdH), new Color(255, 90, 50));

                string storm = $"STORM {sp.StormTimer / 60 + 1}s";
                Vector2 ss = FontAssets.MouseText.Value.MeasureString(storm) * 0.75f;
                ChatManager.DrawColorCodedStringWithShadow(
                    spriteBatch, FontAssets.MouseText.Value, storm,
                    cdPos + new Vector2(BarWidth * 0.5f - ss.X * 0.5f, cdH + 1f),
                    new Color(255, 120, 80), 0f, Vector2.Zero, new Vector2(0.75f));
            }
            else if (sp.BurstCooldown > 0)
            {
                float cdFill = 1f - MathHelper.Clamp(sp.BurstCooldown / (float)ZaphielScorePlayer.BurstCooldownTicks, 0f, 1f);
                spriteBatch.Draw(pixel, new Rectangle((int)cdPos.X - 1, (int)cdPos.Y - 1, BarWidth + 2, cdH + 2), new Color(16, 16, 20, 220));
                spriteBatch.Draw(pixel, new Rectangle((int)cdPos.X, (int)cdPos.Y, BarWidth, cdH), new Color(40, 40, 48, 200));
                spriteBatch.Draw(pixel, new Rectangle((int)cdPos.X, (int)cdPos.Y, System.Math.Max(1, (int)(BarWidth * cdFill)), cdH), new Color(180, 170, 140));

                string cd = $"BURST {sp.BurstCooldown / 60 + 1}s";
                Vector2 cs = FontAssets.MouseText.Value.MeasureString(cd) * 0.75f;
                ChatManager.DrawColorCodedStringWithShadow(
                    spriteBatch, FontAssets.MouseText.Value, cd,
                    cdPos + new Vector2(BarWidth * 0.5f - cs.X * 0.5f, cdH + 1f),
                    new Color(200, 190, 160), 0f, Vector2.Zero, new Vector2(0.75f));
            }
        }

        private static Color GetFillColor(ZaphielScorePlayer sp, float fill, out Color glow)
        {
            if (sp.InStorm)
            {
                glow = new Color(255, 60, 30);
                return Color.Lerp(new Color(180, 40, 20), new Color(255, 140, 50),
                    0.5f + 0.5f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 14f));
            }
            Color low = new Color(160, 90, 30);
            Color mid = new Color(255, 190, 50);
            Color high = new Color(255, 245, 180);
            Color c = fill < 0.5f ? Color.Lerp(low, mid, fill / 0.5f) : Color.Lerp(mid, high, (fill - 0.5f) / 0.5f);
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
                    Pos = new Vector2(edgeX, y),
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
                if (s.Life >= s.MaxLife) _sparks.RemoveAt(i);
                else _sparks[i] = s;
            }
        }

        private void DrawSparks(SpriteBatch spriteBatch, Texture2D pixel)
        {
            for (int i = 0; i < _sparks.Count; i++)
            {
                float t = 1f - _sparks[i].Life / _sparks[i].MaxLife;
                int size = (int)System.Math.Max(1f, _sparks[i].Size * t);
                spriteBatch.Draw(pixel, new Rectangle((int)_sparks[i].Pos.X, (int)_sparks[i].Pos.Y, size, size), _sparks[i].Color * t);
            }
        }
    }
}