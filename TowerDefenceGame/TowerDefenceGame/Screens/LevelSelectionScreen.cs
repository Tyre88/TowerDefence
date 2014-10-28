using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;
using Microsoft.Xna.Framework;
using TD_Engine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TowerDefenceGame.Screens
{
    public class LevelSelectionScreen : GameScreen
    {
        MapLoader _ml;
        Texture2D _background, pixel;
        Map _selectedMap, _previousMap, _nextMap;
        Point _center;
        Rectangle _border, _left, _selected, _right, _description;
        float _backgroundOpacity;

        SpriteFont _main, _levelInfoSmall, _levelInfoLarge;

        Text _selectText, _numerOfLevelsText;

        int _index;

        public override bool AcceptsInput
        {
            get { return true; }
        }

        protected override void DrawScreen(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenSystem.SpriteBatch;

            spriteBatch.Draw(_background, Vector2.Zero, Color.White);
            spriteBatch.DrawString(_main, _selectText.Value, _selectText.Position, Color.White);
            spriteBatch.DrawString(_main, _numerOfLevelsText.Value, _numerOfLevelsText.Position, Color.Orange);
            spriteBatch.Draw(pixel, _border, Color.White * _backgroundOpacity);

            DrawMapInformation(spriteBatch, _previousMap, _left, Color.White, _levelInfoSmall);
            DrawMapInformation(spriteBatch, _selectedMap, _selected, Color.White, _levelInfoLarge);
            DrawMapInformation(spriteBatch, _nextMap, _right, Color.White, _levelInfoSmall);

            spriteBatch.Draw(pixel, _description, Color.White * _backgroundOpacity);
            spriteBatch.DrawString(_levelInfoLarge, _selectedMap.Description, new Vector2(_description.X + 10, 
                _description.Y + (_description.Height / 2) - (_levelInfoLarge.MeasureString(_selectedMap.Description).Y / 2)), Color.White);
        }

        private void DrawMapInformation(SpriteBatch spriteBatch, Map m, Rectangle r, Color c, SpriteFont sf)
        {
            if (m != null)
            {
                spriteBatch.DrawString(sf, m.Name, new Vector2(r.X, r.Y - sf.MeasureString(m.Name).Y), c);
                spriteBatch.Draw(m.Thumbnail, r, c);

                Vector2 position = new Vector2(r.X , r.Bottom);

                spriteBatch.DrawString(sf, m.TowersInfo, position, c);

                position.Y += sf.MeasureString(m.TowersInfo).Y;

                spriteBatch.DrawString(sf, m.WavesInfo, position, c);

                position.Y += sf.MeasureString(m.WavesInfo).Y;

                spriteBatch.DrawString(sf, m.DificultyInfo, position, c);
            }
        }

        public override void InitializeScreen()
        {
            //CHANGE FOR DYNAMIC RESOLUTION

            _center = new Point(1280 / 2, 720 / 2);
            _border = new Rectangle(_center.X - 608, _center.Y - 300, 1216, 580);
            //Using thumbnail size of 480x270
            _selected = new Rectangle((_border.X + (_border.Width / 2)) - (480 / 2),
                (_border.Y + (_border.Height / 2) - (270 / 2)), 480, 270);

            //Smaller thumbnails are a size of 240x135
            _left = new Rectangle((_border.X + (_border.Width / 2)) - (_selected.Width / 2) - 20 - 240,
                (_border.Y + (_border.Height / 2)) - (135 / 2), 240, 135);
            _right = new Rectangle((_border.X + (_border.Width / 2)) + (_selected.Width / 2) + 20 - 240,
                (_border.Y + (_border.Height / 2)) - (135 / 2), 240, 135);
            _description = new Rectangle(_border.X, _border.Bottom + 10, _border.Width, 50);

            _backgroundOpacity = 0.5f;

            _index = 0;

            InputMap.NewAction("Select Left", Keys.Left);
            InputMap.NewAction("Select Right", Keys.Right);
            InputMap.NewAction("Select Map", Keys.Enter);
            InputMap.NewAction("Back", Keys.Escape);

            Removing += new EventHandler(LevelSelectionScreen_Removing);
        }

        public override void LoadContent()
        {
            _main = ScreenSystem.Content.Load<SpriteFont>(@"Fonts/menu");
            _levelInfoSmall = ScreenSystem.Content.Load<SpriteFont>(@"Fonts/LevelSelectSmall");
            _levelInfoLarge = ScreenSystem.Content.Load<SpriteFont>(@"Fonts/LevelSelectLarge");

            _selectText = new Text("Select a map", _main);
            _selectText.Position = new Vector2(_border.X, _border.Y - _selectText.Dimentions.Y);

            _ml = ScreenSystem.Content.Load<MapLoader>(@"Maps/MapLoader");
            _numerOfLevelsText = new Text(string.Format("[{0} maps]", _ml.Maps.Count), _main);
            _numerOfLevelsText.Position = new Vector2(_border.X + _selectText.Dimentions.X + 10,
                _border.Y - _numerOfLevelsText.Dimentions.Y);

            _previousMap = null;
            _selectedMap = _ml.Maps[_index];
            _nextMap = (_index + 1) < _ml.Maps.Count ? _ml.Maps[_index + 1] : null;

            MapLoader._singleton = _ml;

            _background = ScreenSystem.Content.Load<Texture2D>(@"Textures/Menu/MainMenuBackground");
            pixel = new Texture2D(ScreenSystem.GraphicsDevice, 1, 1);
            Color[] cArray = new Color[] { Color.Black };
            pixel.SetData<Color>(cArray);
        }

        void LevelSelectionScreen_Removing(object sender, EventArgs e)
        {
            //Logic to perform when the menu screen is getting ready to remove from the screen system.
        }

        protected override void UpdateScreen(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (InputMap.NewActionPress("Select Left"))
            {
                if (_previousMap != null)
                {
                    _nextMap = _selectedMap;
                    _selectedMap = _previousMap;

                    _index--;

                    _previousMap = (_index -1) >= 0 ? _ml.Maps[_index - 1] : null;
                }
            }
            else if (InputMap.NewActionPress("Select Right"))
            {
                if (_nextMap != null)
                {
                    _previousMap = _selectedMap;
                    _selectedMap = _nextMap;

                    _index++;

                    _nextMap = (_index + 1) < _ml.Maps.Count ? _ml.Maps[_index + 1] : null;
                }
            }
            else if (InputMap.NewActionPress("Select Map"))
            {
                if (_selectedMap != null)
                {
                    FreezeScreen();
                    _selectedMap.Reset();
                    ExitScreen();
                    ScreenSystem.AddScreen(new PlayScreen(this, _selectedMap));
                }
            }
            else if (InputMap.NewActionPress("Back"))
            {
                ExitScreen();
                ScreenSystem.AddScreen(new MainMenuScreen());
            }
        }
    }
}
