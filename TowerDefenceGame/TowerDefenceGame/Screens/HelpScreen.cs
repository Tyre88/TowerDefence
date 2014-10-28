using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace TowerDefenceGame.Screens
{
    public class HelpScreen : GameScreen
    {
        GameScreen _screenBefore;
        SpriteFont _font;

        List<Texture2D> _helpTextures;
        int _helpTexturesCount;

        int _index;

        public override bool AcceptsInput
        {
            get { return true; }
        }

        public HelpScreen(GameScreen before)
        {
            _screenBefore = before;
        }

        public override void InitializeScreen()
        {
            InputMap.NewAction("Finished", Keys.Escape);
            InputMap.NewAction("Next", Keys.Space);

            _helpTexturesCount = 3;
            _index = 0;

            _helpTextures = new List<Texture2D>(_helpTexturesCount);

            EnableFade(Color.Black, 0.85f);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenSystem.Content;

            _font = content.Load<SpriteFont>(@"Fonts/help");

            for (int i = 0; i < _helpTexturesCount; i++)
            {
                _helpTextures.Add(content.Load<Texture2D>(string.Format("Textures/Help/help_{0}", i + 1)));
            }
        }

        protected override void UpdateScreen(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (InputMap.NewActionPress("Finished"))
            {
                ExitScreen();
                _screenBefore.ActivateScreen();
            }
            else if (InputMap.NewActionPress("Next"))
            {
                if (_index >= (_helpTexturesCount - 1))
                {
                    _index = 0;
                }
                else
                {
                    _index++;
                }
            }
        }

        protected override void DrawScreen(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenSystem.SpriteBatch;

            spriteBatch.Draw(_helpTextures[_index], new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.DrawString(_font, "Press space to advance to next image.", Vector2.Zero, Color.Red);
        }
    }
}
