using CStudios.Content.Items.Modules.Zaphiel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace CStudios.Content.Systems.ZaphielModules
{
    public class ZaphielModuleUI : UIState
    {
        private UIPanel _panel;
        private readonly LanceItemSlot[] _slots = new LanceItemSlot[ZaphielModulePlayer.SlotCount];
        private readonly UIText[] _presetButtons = new UIText[ZaphielModulePlayer.PresetCount];

        private bool _dragging;
        private Vector2 _dragOffset;
        private bool _appliedSavedPos;

        private static readonly string[] Labels =
        {
            "Spearhead", "Guidance", "Energy Core", "Energy Relay", "Psybits"
        };

        public override void OnInitialize()
        {
            _panel = new UIPanel();
            _panel.Width.Set(360f, 0f);
            _panel.Height.Set(268f, 0f);
            _panel.Left.Set(0f, 0.38f);
            _panel.Top.Set(0f, 0.28f);
            _panel.BackgroundColor = new Color(28, 18, 40) * 0.94f;
            _panel.OnLeftMouseDown += BeginDrag;
            _panel.OnLeftMouseUp += EndDrag;
            Append(_panel);

            var title = new UIText("Lance Matrix", 0.9f, true);
            title.HAlign = 0.5f;
            title.Top.Set(6f, 0f);
            _panel.Append(title);

            for (int i = 0; i < ZaphielModulePlayer.SlotCount; i++)
            {
                int index = i;

                var slot = new LanceItemSlot(54f);
                slot.Left.Set(22f + i * 64f, 0f);
                slot.Top.Set(42f, 0f);
                slot.OnLeftClick += (_, _) => ClickSlot(index);
                _slots[i] = slot;
                _panel.Append(slot);

                var lab = new UIText(Labels[i], 0.62f);
                lab.Left.Set(16f + i * 64f, 0f);
                lab.Top.Set(102f, 0f);
                lab.TextColor = ZaphielModuleItem.SlotColor((ZaphielSlotCategory)i);
                _panel.Append(lab);
            }

            var presetTitle = new UIText("Presets — click to install that bank", 0.7f);
            presetTitle.HAlign = 0.5f;
            presetTitle.Top.Set(138f, 0f);
            _panel.Append(presetTitle);

            for (int p = 0; p < ZaphielModulePlayer.PresetCount; p++)
            {
                int preset = p;
                var btn = new UIText($"[ P{p + 1} ]", 0.85f);
                btn.Left.Set(28f + p * 64f, 0f);
                btn.Top.Set(162f, 0f);
                btn.OnLeftClick += (_, _) =>
                {
                    Main.LocalPlayer.GetModPlayer<ZaphielModulePlayer>().SelectPreset(preset);
                    SoundEngine.PlaySound(SoundID.MenuTick);
                };
                _presetButtons[p] = btn;
                _panel.Append(btn);
            }

            var hint = new UIText("Hover a module for its tooltip", 0.72f);
            hint.HAlign = 0.5f;
            hint.Top.Set(198f, 0f);
            _panel.Append(hint);

            var close = new UIText("[Close]", 0.85f);
            close.HAlign = 0.5f;
            close.Top.Set(222f, 0f);
            close.OnLeftClick += (_, _) => ZaphielModuleUISystem.Close();
            _panel.Append(close);
        }

        private void BeginDrag(UIMouseEvent evt, UIElement listeningElement)
        {
            if (listeningElement != _panel)
                return;

            // Don't start a drag when clicking a slot / button
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] != null && _slots[i].IsMouseHovering)
                    return;
            }
            for (int i = 0; i < _presetButtons.Length; i++)
            {
                if (_presetButtons[i] != null && _presetButtons[i].IsMouseHovering)
                    return;
            }

            CalculatedStyle dims = _panel.GetDimensions();
            _dragOffset = new Vector2(Main.mouseX - dims.X, Main.mouseY - dims.Y);
            _dragging = true;
        }

        private void EndDrag(UIMouseEvent evt, UIElement listeningElement)
        {
            _dragging = false;
            var mp = Main.LocalPlayer.GetModPlayer<ZaphielModulePlayer>();
            CalculatedStyle dims = _panel.GetDimensions();
            mp.PanelX = dims.X;
            mp.PanelY = dims.Y;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Main.LocalPlayer == null)
                return;

            var mp = Main.LocalPlayer.GetModPlayer<ZaphielModulePlayer>();

            if (!_appliedSavedPos && mp.PanelX >= 0f && mp.PanelY >= 0f)
            {
                _panel.Left.Set(mp.PanelX, 0f);
                _panel.Top.Set(mp.PanelY, 0f);
                _panel.Recalculate();
                _appliedSavedPos = true;
            }

            if (_dragging)
            {
                _panel.Left.Set(Main.mouseX - _dragOffset.X, 0f);
                _panel.Top.Set(Main.mouseY - _dragOffset.Y, 0f);
                _panel.Recalculate();
            }

            // Only block world clicks while the cursor is on this panel
            if (_panel.IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;

            for (int i = 0; i < ZaphielModulePlayer.SlotCount; i++)
                _slots[i].SetItem(mp.GetSlot(mp.ActivePreset, i));

            for (int p = 0; p < ZaphielModulePlayer.PresetCount; p++)
            {
                bool active = mp.ActivePreset == p;
                bool filled = mp.PresetHasItems(p);
                if (active)
                    _presetButtons[p].TextColor = new Color(255, 230, 140);
                else if (filled)
                    _presetButtons[p].TextColor = new Color(220, 190, 255);
                else
                    _presetButtons[p].TextColor = new Color(120, 110, 130);
            }
        }

        private static void ClickSlot(int index)
        {
            var mp = Main.LocalPlayer.GetModPlayer<ZaphielModulePlayer>();
            Item cursor = Main.mouseItem;

            if (cursor != null && !cursor.IsAir && cursor.ModItem is ZaphielModuleItem mod)
            {
                if ((int)mod.SlotCategory != index)
                {
                    Main.NewText($"That module belongs in the {Labels[(int)mod.SlotCategory]} slot.", Color.OrangeRed);
                    return;
                }

                Item existing = mp.GetSlot(mp.ActivePreset, index).Clone();
                mp.SetActiveSlot(index, cursor.Clone());
                cursor.TurnToAir();
                Main.mouseItem = new Item();

                if (existing != null && !existing.IsAir)
                    Main.mouseItem = existing;
                return;
            }

            Item slotted = mp.GetSlot(mp.ActivePreset, index);
            if (slotted != null && !slotted.IsAir)
            {
                if (Main.mouseItem == null || Main.mouseItem.IsAir)
                {
                    Main.mouseItem = slotted.Clone();
                    mp.SetActiveSlot(index, new Item());
                }
            }
        }
    }

    public class LanceItemSlot : UIElement
    {
        private Item _item = new Item();
        private readonly float _size;

        public LanceItemSlot(float size)
        {
            _size = size;
            Width.Set(size, 0f);
            Height.Set(size, 0f);
        }

        public void SetItem(Item item) => _item = item ?? new Item();

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dims = GetDimensions();
            Color back = IsMouseHovering ? new Color(90, 60, 120) : new Color(45, 35, 60);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, dims.ToRectangle(), back * 0.95f);

            if (_item != null && !_item.IsAir)
            {
                Main.instance.LoadItem(_item.type);
                Texture2D tex = TextureAssets.Item[_item.type].Value;
                float scale = _size * 0.7f / System.Math.Max(tex.Width, tex.Height);
                spriteBatch.Draw(tex, dims.Center(), null, Color.White, 0f, tex.Size() / 2f, scale, SpriteEffects.None, 0f);

                if (IsMouseHovering)
                {
                    Main.HoverItem = _item.Clone();
                    Main.hoverItemName = _item.Name;
                }
            }
            else if (IsMouseHovering)
            {
                string empty = "Empty slot";
                ChatManager.DrawColorCodedStringWithShadow(
                    spriteBatch, FontAssets.MouseText.Value, empty,
                    new Vector2(Main.mouseX + 16, Main.mouseY + 16),
                    Color.Gray, 0f, Vector2.Zero, Vector2.One);
            }
        }
    }
}
