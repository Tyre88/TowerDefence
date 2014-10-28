using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;

namespace TowerDefenceGame.MenuEntries
{
    public class MainMenuEntry : MenuEntry
    {
        public MainMenuEntry(MenuScreen menuScreen, string title, string description)
            : base(menuScreen, title)
        {
            EntryDescription = description;
        }

        public override void AnimateHighlighted(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //Animate when a entry is highlighted like pulsate.
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //Update all entries.
        }
    }
}
