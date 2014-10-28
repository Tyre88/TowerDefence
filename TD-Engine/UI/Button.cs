using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD_Engine
{
    public enum UIButtonState
    {
        Active,
        Inactive
    }

    public class Button : ClickableGameplayObject
    {

        public Text ButtonText
        {
            get;
            private set;
        }

        public Color ButtonForeColor
        {
            get;
            private set;
        }

        public object StoredObject
        {
            get;
            private set;
        }

        public UIButtonState State
        {
            get;
            private set;
        }

        public Button(Text text, Color color, object o)
        {
            ButtonText = text;
            ButtonForeColor = color;
            StoredObject = o;
        }

        public Button(Texture2D tex, Vector2 pos, object o)
        {
            Texture = tex;
            Position = pos;
            StoredObject = o;
        }

        public Button(Texture2D tex, Vector2 pos, Text text, Color color, object o)
        {
            Texture = tex;
            Position = pos;
            StoredObject = o;
            ButtonText = text;
            ButtonForeColor = color;
        }

        public void SetStoredObject(object obj)
        {
            StoredObject = obj;
        }

        public void SetColor(Color color)
        {
            ButtonForeColor = color;
        }

        public void Activate()
        {
            State = UIButtonState.Active;
        }

        public void Deactivate()
        {
            State = UIButtonState.Inactive;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            if (ButtonText != null)
            {
                ButtonText.Draw(spriteBatch, ButtonForeColor);
            }
        }

    }
}
