using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TD_Engine
{
    public class UIBlock
    {
        Texture2D _border, _pixel;
        Rectangle _borderTop, _borderBottom, _borderRight;

        public Rectangle Dimentions
        {
            get;
            private set;
        }

        public Session Session
        {
            get;
            private set;
        }

        public Dictionary<string, Button> Buttons
        {
            get;
            private set;
        }

        public Dictionary<string, Text> Texts
        {
            get;
            private set;
        }

        public Dictionary<string, Image> Images
        {
            get;
            private set;
        }

        Color _color;

        public UIBlock(GraphicsDevice gd, Texture2D borderTexture, Color borderColor, Rectangle dimentions, Session session)
        {
            _border = borderTexture;
            _pixel = new Texture2D(gd, 1, 1);
            Color[] c = new Color[1];
            c[0] = borderColor;
            _pixel.SetData<Color>(c);

            Dimentions = dimentions;

            if (_border != null)
            {
                _borderTop = new Rectangle(Dimentions.Right - _border.Width, Dimentions.Top, _border.Width, _border.Height);
                _borderRight = new Rectangle(Dimentions.Right - 1, Dimentions.Top + _border.Height, 1, Dimentions.Height - (_border.Height * 2));
                _borderBottom = new Rectangle(Dimentions.Right - _border.Width, Dimentions.Bottom - _border.Height, _border.Width, _border.Height);
            }

            Session = session;

            Buttons = new Dictionary<string, Button>();

            Texts = new Dictionary<string, Text>();

            Images = new Dictionary<string, Image>();
        }

        public UIBlock(GraphicsDevice gd, Rectangle dims, Color color)
        {
            Dimentions = dims;
            _color = color;

            Buttons = new Dictionary<string, Button>();

            Texts = new Dictionary<string, Text>();

            Images = new Dictionary<string, Image>();
        }

        public void Add(string s, Text t)
        {
            Texts.Add(s, t);
        }

        public Text GetText(string id)
        {
            return Texts[id];
        }

        public void Add(string s, Button b)
        {
            Buttons.Add(s, b);
        }

        public Button GetButton(string id)
        {
            return Buttons[id];
        }

        public void Add(string s, Image i)
        {
            Images.Add(s, i);
        }

        public Image GetImage(string id)
        {
            return Images[id];
        }

        public void ClearAll()
        {
            Texts.Clear();
            Buttons.Clear();
            Images.Clear();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            if (_border != null)
            {
                spriteBatch.Draw(_border, _borderTop, Color.White);
                spriteBatch.Draw(_border, _borderRight, Color.White);
                spriteBatch.Draw(_border, _borderBottom, Color.White);
            }

            foreach (var item in Buttons)
            {
                item.Value.Draw(gameTime, spriteBatch);
            }

            foreach (var item in Images)
            {
                item.Value.Draw(spriteBatch);
            }

            foreach (var item in Texts)
            {
                if(Session != null)
                {
                    item.Value.Draw(spriteBatch, item.Value.Font == null ? spriteFont : item.Value.Font, Session.Map.ForeColor);
                }
                else
                {
                    item.Value.Draw(spriteBatch, item.Value.Font == null ? spriteFont : item.Value.Font, _color);
                }
            }
        }
    }
}
