using CStudios.Content.Items.Modules.Zaphiel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace CStudios.Content.Systems.ZaphielModules
{
    public class ZaphielModuleUI : UIState
    {
        private UIPanel _panel;
        private readonly LanceItemSlot[] _slots = new LanceItemSlot[ZaphielModulePlayer.SlotCount];

        private static readonly string[] Labels =
        {
            "Spearhead", "Guidance", "Energy Core", "Energy Relay", "Psybits"
        };

        public override void OnInitialize()
        {
            _panel = new UIPanel();
            _panel.Width.Set(340f, 0f);
            _panel.Height.Set(230f, 0f);
            _panel.HAlign = 0.5f;
            _panel.VAlign = 0.4f;
            _panel.BackgroundColor = new Color(28, 18, 40) * 0.94f;
            Append(_panel);

            var title = new UIText("Lance Matrix", 0.9f, true);
            title.HAlign = 0.5f;
            title.Top.Set(8f, 0f);
            _panel.Append(title);

            for (int i = 0; i < ZaphielModulePlayer.SlotCount; i++)
            {
                int index = i;

                var slot = new LanceItemSlot(54f);
                slot.Left.Set(18f + i * 62f, 0f);
                slot.Top.Set(48f, 0f);
                slot.OnLeftClick += (_, _) => ClickSlot(index);
                _slots[i] = slot;
                _panel.Append(slot);

                var lab = new UIText(Labels[i], 0.65f);
                lab.Left.Set(14f + i * 62f, 0f);
                lab.Top.Set(108f, 0f);
                lab.TextColor = ZaphielModuleItem.SlotColor((ZaphielSlotCategory)i);
                _panel.Append(lab);
            }

            var hint = new UIText("Click: insert from cursor / extract to cursor", 0.75f);
            hint.HAlign = 0.5f;
            hint.Top.Set(150f, 0f);
            _panel.Append(hint);

            var close = new UIText("[Close]", 0.85f);
            close.HAlign = 0.5f;
            close.Top.Set(185f, 0f);
            close.OnLeftClick += (_, _) => ZaphielModuleUISystem.Close();
            _panel.Append(close);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Main.LocalPlayer == null)
                return;

            var mp = Main.LocalPlayer.GetModPlayer<ZaphielModulePlayer>();
            for (int i = 0; i < ZaphielModulePlayer.SlotCount; i++)
                _slots[i].SetItem(mp.Modules[i] ?? new Item());
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

                Item existing = mp.Modules[index]?.Clone() ?? new Item();
                mp.Modules[index] = cursor.Clone();
                cursor.TurnToAir();
                Main.mouseItem = new Item();

                if (existing != null && !existing.IsAir)
                    Main.mouseItem = existing;

                return;
            }

            if (mp.Modules[index] != null && !mp.Modules[index].IsAir)
            {
                if (Main.mouseItem == null || Main.mouseItem.IsAir)
                {
                    Main.mouseItem = mp.Modules[index].Clone();
                    mp.Modules[index].TurnToAir();
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
            }
        }
    }
}