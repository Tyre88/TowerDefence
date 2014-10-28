using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TD_Engine
{
    public class WaveInformation
    {
        public Rectangle Rectangle
        {
            get;
            private set;
        }

        public Session Session
        {
            get;
            private set;
        }

        Texture2D _background;

        public WaveInformation(Session s, Rectangle r, GraphicsDevice gd, Color c)
        {
            Session = s;
            Rectangle = r;
            _background = new Texture2D(gd, 1, 1);
            Color[] cArray = new Color[] { c };

            _background.SetData<Color>(cArray);
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.Draw(_background, Rectangle, Color.White);


            if (Session.Map.State == MapState.Active)
            {
                string active = Session.Map.ActiveWave.ToString();
                Vector2 dims = font.MeasureString(active);

                spriteBatch.DrawString(font, active, new Vector2(Rectangle.Left + 10, Rectangle.Top),
                    Session.Map.ActiveWave.BossWave ? Session.Map.ErrorColor : Session.Map.ForeColor);

                string wavesLeft = string.Empty;

                bool b = false;

                foreach (Wave w in Session.Map.WaveList)
                {
                    if (w == Session.Map.ActiveWave)
                    {
                        b = true;
                        continue;
                    }
                    else if (b)
                    {
                        wavesLeft += string.Format("; {0}", w.ToString());
                    }
                    else
                    {
                        continue;
                    }
                }

                spriteBatch.DrawString(font, wavesLeft, new Vector2(Rectangle.Left + 10 + dims.X, Rectangle.Top),
                    Color.White * 0.5f);
            }
            else if(Session.Map.State == MapState.WaveDelay)
            {
                string nextWave = string.Format("Wave \"{0}\" starts in {1} seconds", Session.Map.ActiveWave.ToString(),
                    (int)Session.Map.Timer);

                spriteBatch.DrawString(font, nextWave, new Vector2(Rectangle.Left + 10, Rectangle.Top), Session.Map.ForeColor);
            }
        }
    }
}
