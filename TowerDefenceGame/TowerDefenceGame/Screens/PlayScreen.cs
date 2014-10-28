using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TD_Engine;
using Microsoft.Xna.Framework.Input;

namespace TowerDefenceGame.Screens
{
    public class PlayScreen : GameScreen
    {
        UserInterface _ui;
        Map _map;
        Session _session;
        List<Tower> _towers;
        SpriteFont _font;
        LevelSelectionScreen _levelSelect;

        public override bool AcceptsInput
        {
            get { return true; }
        }

        public PlayScreen(LevelSelectionScreen lss, Map map)
        {
            _map = map;
            _levelSelect = lss;
            _towers = new List<Tower>(10);
            _session = new Session(map);
            _session.HealthDecreased += new EventHandler(_session_HealthDecreased);
            _session.MapFinished += new EventHandler(_session_MapFinished);
        }

        void _session_MapFinished(object sender, EventArgs e)
        {
            if (_session.Health >= 0)
            {
                ScreenSystem.AddScreen(new LevelSelectionScreen());
            }
            else
            {
                ScreenSystem.AddScreen(new GameOverScreen(_levelSelect));
            }

            ExitScreen();
        }

        void _session_HealthDecreased(object sender, EventArgs e)
        {
            if (_session.Health < 0)
            {
                ExitScreen();
                ScreenSystem.AddScreen(new GameOverScreen(_levelSelect));
            }
        }

        protected override void DrawScreen(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenSystem.SpriteBatch;
            _ui.DrawUI(gameTime, spriteBatch);
        }

        public override void InitializeScreen()
        {
            //CHANGE FOR DYNAMIC RESOLUTIONS
            _ui = new UserInterface(new MapRegion(_session, new Rectangle(0, 0, 960, 690), ScreenSystem.GraphicsDevice),
                new WaveInformation(_session, new Rectangle(0, 690, 1280, 30), ScreenSystem.GraphicsDevice, Color.Black),
                new CommandInfoBar(_session, new Rectangle(960, 0, 320, 690), ScreenSystem.GraphicsDevice), _session);

            AudioManager._singleton.PlaySong(_map.SongCueName);
            InputMap.NewAction("Pause", Keys.Escape);
        }

        public override void LoadContent()
        {
            _ui.Font = ScreenSystem.Content.Load<SpriteFont>(@"Fonts/playfont");
        }

        protected override void UpdateScreen(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (_session.IsPaused)
            {
                FreezeScreen();
                ScreenSystem.AddScreen(new PauseScreen(this, _session));
            }

            if (InputMap.NewActionPress("Pause"))
            {
                _session.Pause();
            }

            _session.Update(gameTime);
        }
    }
}
