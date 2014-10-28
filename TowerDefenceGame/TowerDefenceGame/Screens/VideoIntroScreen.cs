using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;
using TD_Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TowerDefenceGame.Screens
{
    public class VideoIntroScreen : GameScreen
    {
        VideoManager _videoManager;

        public override bool AcceptsInput
        {
            get { return true; }
        }

        public override void InitializeScreen()
        {
            _videoManager = VideoManager._singleton;

            InputMap.NewAction("Skip", Keys.Space);

            Removing += new EventHandler(VideoIntroScreen_Removing);
        }

        public override void LoadContent()
        {
            _videoManager.LoadVideo("Intro", Vector2.Zero);
            _videoManager.Play();
        }

        void VideoIntroScreen_Removing(object sender, EventArgs e)
        {
            ScreenSystem.AddScreen(new MainMenuScreen());
        }

        protected override void UpdateScreen(GameTime gameTime)
        {
            if (InputMap.NewActionPress("Skip") || _videoManager.IsDonePlaying)
            {
                ExitScreen();
            }
        }

        protected override void DrawScreen(GameTime gameTime)
        {
            _videoManager.Draw(ScreenSystem.SpriteBatch);
        }
    }
}
