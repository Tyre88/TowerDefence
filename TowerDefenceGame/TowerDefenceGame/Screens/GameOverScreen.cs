using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using ScreenSystemLibrary;
using TowerDefenceGame.MenuEntries;
using Microsoft.Xna.Framework.Graphics;

namespace TowerDefenceGame.Screens
{
    public class GameOverScreen : MenuScreen
    {
        private MainMenuEntry _play, _quit;

        private string _prevEntry, _nextEntry, _selectedEntry, _cancelMenu;

        private LevelSelectionScreen _levelSelect;

        public override string MenuCancelActionName
        {
            get { return _cancelMenu; }
        }

        public override string NextEntryActionName
        {
            get { return _nextEntry; }
        }

        public override string PreviousEntryActionName
        {
            get { return _prevEntry; }
        }

        public override string SelectedEntryActionName
        {
            get { return _selectedEntry; }
        }

        public override void InitializeScreen()
        {
            InputMap.NewAction(PreviousEntryActionName, Keys.Up);
            InputMap.NewAction(NextEntryActionName, Keys.Down);
            InputMap.NewAction(SelectedEntryActionName, Keys.Enter);
            InputMap.NewAction(MenuCancelActionName, Keys.Escape);

            _play = new MainMenuEntry(this, "Play again?", "GAME OVER - PLAY THE GAME AGAIN?");
            _quit = new MainMenuEntry(this, "Quit", "DONE PLAYING FOR NOW?");

            Removing += new EventHandler(GameOverScreen_Removing);
            Entering += new TransitionEventHandler(GameOverScreen_Entering);
            Exiting += new TransitionEventHandler(GameOverScreen_Exiting);

            _play.Selected += new EventHandler(_play_Selected);
            _quit.Selected += new EventHandler(_quit_Selected);

            MenuEntries.Add(_play);
            MenuEntries.Add(_quit);

            Viewport view = ScreenSystem.Viewport;
            SetDescriptionArea(new Rectangle(100, view.Height - 100, view.Width - 100, 50),
                               new Color(11, 38, 40), new Color(29, 108, 117), new Point(10, 0), 0.5f);
        }

        void _quit_Selected(object sender, EventArgs e)
        {
            ScreenSystem.Game.Exit();
        }

        void _play_Selected(object sender, EventArgs e)
        {   
            ScreenSystem.AddScreen(new LevelSelectionScreen());
            ExitScreen();
        }

        void GameOverScreen_Removing(object sender, EventArgs e)
        {
            MenuEntries.Clear();
        }

        void GameOverScreen_Entering(object sender, TransitionEventArgs e)
        {
            float effect = (float) Math.Pow(e.percent - 1, 2) * 100;

            foreach (MenuEntry entry in MenuEntries)
            {
                entry.Acceleration = new Vector2(effect, 0);
                entry.Position = entry.DefaultPosition + entry.Acceleration;
                entry.Scale = e.percent;
                entry.Opacity = e.percent;
            }

            TitlePosition = InitialTitlePosition - new Vector2(0, effect);
            TitleOpacity = e.percent;
        }

        void GameOverScreen_Exiting(object sender, TransitionEventArgs e)
        {
            float effect = (float)Math.Pow(e.percent - 1, 2) * 100;

            foreach (MenuEntry entry in MenuEntries)
            {
                entry.Acceleration = new Vector2(effect, 0);
                entry.Position = entry.DefaultPosition + entry.Acceleration;
                entry.Scale = e.percent;
                entry.Opacity = e.percent;
            }

            TitlePosition = InitialTitlePosition - new Vector2(0, effect);
            TitleOpacity = e.percent;
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenSystem.Content;
            SpriteFont = content.Load<SpriteFont>(@"Fonts\menu");

            Texture2D entryTexture = content.Load<Texture2D>(@"Textures\Menu\MenuEntries");

            BackgroundTexture = content.Load<Texture2D>(@"Textures\Menu\MainMenuBackground");

            TitleTexture = content.Load<Texture2D>(@"Textures\Menu\LogoWithText");

            InitialTitlePosition =
                TitlePosition = new Vector2((ScreenSystem.Viewport.Width - TitleTexture.Width) / 2, 50);

            for (int i = 0; i < MenuEntries.Count; i++)
            {
                MenuEntries[i].AddTexture(entryTexture, 2, 1,
                                          new Rectangle(0, 0, entryTexture.Width / 2, entryTexture.Height),
                                          new Rectangle(entryTexture.Width / 2, 0, entryTexture.Width / 2,
                                                        entryTexture.Height));

                MenuEntries[i].AddPadding(14, 0);

                if(i == 0)
                {
                    MenuEntries[i].SetPosition(new Vector2(180, 250), true);
                }
                else
                {
                    int offsetY = MenuEntries[i - 1].EntryTexture == null ? SpriteFont.LineSpacing : 8;

                    MenuEntries[i].SetRelativePosition(new Vector2(0, offsetY), MenuEntries[i - 1], true);
                }
            }
        }

        public override void UnloadContent()
        {
            SpriteFont = null;
        }

        public GameOverScreen(LevelSelectionScreen lss)
        {
            _prevEntry = "MenuUp";
            _nextEntry = "MenuDown";
            _selectedEntry = "MenuAccept";
            _cancelMenu = "MenuCancel";

            TransitionOnTime = TimeSpan.FromSeconds(1);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            Selected = Highlighted = new Color(214, 232, 223);
            Normal = new Color(104, 173, 178);

            _levelSelect = lss;
        }
    }
}
