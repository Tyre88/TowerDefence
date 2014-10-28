using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD_Engine
{
    public class Text
    {

        string _textValue;

        public string Value
        {
            get { return _textValue; }
            set
            {
                _textValue = value;
                if (Font != null)
                {
                    Dimentions = Font.MeasureString(value);
                }
                Rectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)Dimentions.X, (int)Dimentions.Y);
            }
        }

        SpriteFont _font;

        public SpriteFont Font
        {
            get { return _font; }
            set 
            { 
                _font = value;
                Dimentions = Font.MeasureString(_textValue);
                Rectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)Dimentions.X, (int)Dimentions.Y); 
            }
        }

        public Vector2 Dimentions
        {
            get;
            private set;
        }

        Vector2 _position;

        public Vector2 Position
        {
            get { return _position; }
            set 
            {
                _position = value;
                Rectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)Dimentions.X, (int)Dimentions.Y);
            }
        }

        public Vector2 Velocity
        {
            get;
            set;
        }

        public Rectangle Rectangle
        {
            get;
            private set;
        }

        public Text(string value, SpriteFont font, Vector2 pos)
        {
            Value = value;
            Font = font;
            Position = pos;
        }

        public Text(string value, Vector2 pos)
        {
            Value = value;
            Position = pos;
        }

        public Text(string value, SpriteFont font)
        {
            Value = value;
            Font = font;
        }

        public Text(string value, Vector2 pos, Vector2 velocity)
        {
            Value = value;
            Position = pos;
            Velocity = velocity;
        }

        public void Update(GameTime gameTime)
        {
            float deltaseconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Vector2.Multiply(Velocity, deltaseconds);
        }

        public void Draw(SpriteBatch spritebatch, Color color)
        {
            if (Font != null)
            {
                spritebatch.DrawString(Font, Value, Position, color);
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Color color)
        {
            spriteBatch.DrawString(font, Value, Position, color);
        }

    }
}
