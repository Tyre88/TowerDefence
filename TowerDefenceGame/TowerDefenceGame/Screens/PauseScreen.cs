using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;
using TD_Engine;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TowerDefenceGame.MenuEntries;
using Microsoft.Xna.Framework;

namespace TowerDefenceGame.Screens
{
    public class PauseScreen : MenuScreen
    {
        string _prevEnrty, _nextEntry, _selectedEntry, _cancelEntry;

        GameScreen _screenBefore;
        Session _session;

        MainMenuEntry _resume, _options, _help, _quit;

        public PauseScreen(GameScreen before, Session session)
        {
            _prevEnrty = "MenuUp";
            _nextEntry = "MenuDown";
            _selectedEntry = "MenuAccept";
            _cancelEntry = "MenuCancel";

            TransitionOnTime = TimeSpan.FromSeconds(1);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            Selected = Highlighted = new Color(214, 232, 223);
            Normal = new Color(104, 173, 178);

            _screenBefore = before;
            _session = session;
        }

        public override void InitializeScreen()
        {
            InputMap.NewAction(PreviousEntryActionName, Keys.Up);
            InputMap.NewAction(NextEntryActionName, Keys.Down);
            InputMap.NewAction(SelectedEntryActionName, Keys.Enter);
            InputMap.NewAction(SelectedEntryActionName, MousePresses.LeftMouse);
            InputMap.NewAction(MenuCancelActionName, Keys.Escape);

            _resume = new MainMenuEntry(this, "Resume", "Resume the game.");
            _options = new MainMenuEntry(this, "Options", "Change the game settings.");
            _help = new MainMenuEntry(this, "Help", "Information on how to play the game.");
            _quit = new MainMenuEntry(this, "Quit", "Quit playing this awsome game?!");

            Removing += new EventHandler(PauseScreenRemoving);
            Entering += new TransitionEventHandler(MainMenuScreen_Entering);
            Exiting += new TransitionEventHandler(MainMenuScreen_Exiting);

            _resume.Selected += new EventHandler(ResumeSelect);
            _options.Selected += new EventHandler(OptionsSelect);
            _help.Selected += new EventHandler(HelpSelect);
            _quit.Selected += new EventHandler(QuitSelect);

            MenuEntries.Add(_resume);
            MenuEntries.Add(_options);
            MenuEntries.Add(_help);
            MenuEntries.Add(_quit);

            Viewport view = ScreenSystem.Viewport;
            SetDescriptionArea(new Rectangle(100, view.Height - 100,
                view.Width - 100, 50), new Color(11, 38, 40), new Color(29, 108, 117),
                new Point(10, 0), 0.5f);

            //AudioManager._singleton.PlaySong("Menu");
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenSystem.Content;
            SpriteFont = content.Load<SpriteFont>(@"Fonts/menu");

            Texture2D entryTexture = content.Load<Texture2D>(@"Textures/Menu/MenuEntries");

            BackgroundTexture = content.Load<Texture2D>(@"Textures/Menu/MainMenuBackground");

            TitleTexture = content.Load<Texture2D>(@"Textures/Menu/LogoWithText");

            //EnableMouse(content.Load<Texture2D>("@Textures/Menu/mouse"));

            InitialTitlePosition = TitlePosition = new Vector2((ScreenSystem.Viewport.Width - TitleTexture.Width) / 2, 50);

            for (int i = 0; i < MenuEntries.Count; i++)
            {
                MenuEntries[i].AddTexture(entryTexture, 2, 1,
                    new Rectangle(0, 0, entryTexture.Width / 2, entryTexture.Height),
                    new Rectangle(entryTexture.Width / 2, 0, entryTexture.Width / 2, entryTexture.Height));
                MenuEntries[i].AddPadding(14, 0);

                if (i == 0)
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

        public override string MenuCancelActionName
        {
            get { return _cancelEntry; }
        }

        public override string NextEntryActionName
        {
            get { return _nextEntry; }
        }

        public override string PreviousEntryActionName
        {
            get { return _prevEnrty; }
        }

        public override string SelectedEntryActionName
        {
            get { return _selectedEntry; }
        }


        #region Events
        void QuitSelect(object sender, EventArgs e)
        {
            ScreenSystem.Game.Exit();
        }

        void HelpSelect(object sender, EventArgs e)
        {
            FreezeScreen();
            ScreenSystem.AddScreen(new HelpScreen(this));
        }

        void OptionsSelect(object sender, EventArgs e)
        {
            FreezeScreen();
            ScreenSystem.AddScreen(new OptionsScreen());
        }

        void ResumeSelect(object sender, EventArgs e)
        {
            ExitScreen();
        }

        void MainMenuScreen_Exiting(object sender, TransitionEventArgs tea)
        {
            float effect = (float)Math.Pow(tea.percent - 1, 2) * 100;
            foreach (MenuEntry entry in MenuEntries)
            {
                entry.Acceleration = new Vector2(effect, 0);
                entry.Position = entry.DefaultPosition + entry.Acceleration;
                entry.Scale = tea.percent;
                entry.Opacity = tea.percent;
            }

            TitlePosition = InitialTitlePosition + new Vector2(0, effect);
            TitleOpacity = tea.percent;
        }

        void MainMenuScreen_Entering(object sender, TransitionEventArgs tea)
        {
            float effect = (float)Math.Pow(tea.percent - 1, 2) * -100;
            foreach (MenuEntry entry in MenuEntries)
            {
                entry.Acceleration = new Vector2(effect, 0);
                entry.Position = entry.DefaultPosition + entry.Acceleration;
                entry.Opacity = tea.percent;
            }

            TitlePosition = InitialTitlePosition + new Vector2(0, effect);
            TitleOpacity = tea.percent;
        }

        void PauseScreenRemoving(object sender, EventArgs e)
        {
            MenuEntries.Clear();
            _screenBefore.ActivateScreen();
            _session.Resume();
        }
        #endregion
    }
}
