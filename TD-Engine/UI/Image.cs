using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD_Engine
{
    public class Image
    {

        public Texture2D Texture
        {
            get;
            private set;
        }

        public Vector2 Position
        {
            get;
            private set;
        }

        public Rectangle Rectangle
        {
            get;
            private set;
        }

        public Image(Texture2D tex, Vector2 pos)
        {
            Texture = tex;
            Position = pos;
            Rectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rectangle, Color.White);
        }

    }
}
