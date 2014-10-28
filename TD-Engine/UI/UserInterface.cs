using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TD_Engine
{
    public class UserInterface
    {
        public MapRegion MapRegion
        {
            get;
            private set;
        }

        public WaveInformation WaveInformation
        {
            get;
            private set;
        }

        public CommandInfoBar CommandInfoBar
        {
            get;
            private set;
        }

        SpriteFont _font;

        public SpriteFont Font
        {
            get { return _font; }
            set
            {
                _font = value;
                CommandInfoBar.Initialise(value);
            }
        }

        Session _session;

        public Mouse Mouse
        {
            get;
            private set;
        }

        public UserInterface(MapRegion mapRegion, WaveInformation waveInformation, CommandInfoBar commandInfoBar, Session session)
        {
            MapRegion = mapRegion;
            WaveInformation = waveInformation;
            CommandInfoBar = commandInfoBar;
            _session = session;
            Mouse = new Mouse(_session.Map.MouseTexture);
            _session.SetUI(this);
        }

        public void Update(GameTime gameTime)
        {
            Mouse.Update();
            MapRegion.Update(gameTime);
            WaveInformation.Update(gameTime);
            CommandInfoBar.Update(gameTime);
        }

        public void DrawUI(GameTime gameTime, SpriteBatch spriteBatch)
        {
            MapRegion.Draw(gameTime, spriteBatch, Font);
            WaveInformation.Draw(gameTime, spriteBatch, Font);
            CommandInfoBar.Draw(gameTime, spriteBatch, Font);
            Mouse.Draw(spriteBatch);
        }
    }
}
