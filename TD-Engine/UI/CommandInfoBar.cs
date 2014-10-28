using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD_Engine
{
    public class CommandInfoBar
    {
        public Rectangle Rectangle
        {
            get;
            private set;
        }

        public UIBlock PurchaseTower
        {
            get;
            private set;
        }

        public UIBlock SelectedTower
        {
            get;
            private set;
        }

        public UIBlock MoneyAndTowers
        {
            get;
            private set;
        }

        public UIBlock StatsAndControls
        {
            get;
            private set;
        }

        public Session Session
        {
            get;
            private set;
        }

        int _padding, _waveIndex;

        Texture2D _background;

        SpriteFont _spriteFont;

        Tower _clickedTower;

        public Tower ClickedTower
        {
            get { return _clickedTower; }
        }

        public CommandInfoBar(Session s, Rectangle r, GraphicsDevice gd)
        {
            Session = s;
            Session.TowerPurchased += new TD_Engine.Session.PurchaseTowerEventHandler(Session_TowerPurchased);
            Session.MoneyIncreased += new EventHandler(Session_MoneyIncreased);
            _background = Session.Map.InfoBarBackground;
            Rectangle = r;
            _padding = 10;
            _waveIndex = Session.Map.WaveIndex;

            MoneyAndTowers = new UIBlock(gd, null, s.Map.BorderColor,
                new Rectangle(r.X, r.Y, r.Width - 5, 50), s);
            PurchaseTower = new UIBlock(gd, s.Map.BorderTexture, s.Map.BorderColor,
                new Rectangle(r.X, MoneyAndTowers.Dimentions.Bottom + 10, r.Width - 5, 420), s);
            SelectedTower = new UIBlock(gd, s.Map.BorderTexture, s.Map.BorderColor,
                new Rectangle(r.X, MoneyAndTowers.Dimentions.Bottom + 10, r.Width - 5, 420), s);
            StatsAndControls = new UIBlock(gd, s.Map.BorderTexture, s.Map.BorderColor,
                new Rectangle(r.X, PurchaseTower.Dimentions.Bottom + 10, r.Width - 5, 200), s);

            s.HealthDecreased += new EventHandler(s_HealthDecreased);
        }

        void s_HealthDecreased(object sender, EventArgs e)
        {
            Text t = StatsAndControls.GetText("Health");
            t.Value = Session.HealthDisplay;
        }

        void Session_MoneyIncreased(object sender, EventArgs e)
        {
            Button bt = null;
            Button ut = null;


            try
            {
                ut = SelectedTower.GetButton("UpgradeButton");
            }
            catch
            {
            }

            try
            {
                bt = SelectedTower.GetButton("BuyTower");
            }
            catch
            {
            }

           

            if (bt != null)
            {
                if (_clickedTower != null && _clickedTower.Cost <= Session.ActivePlayer.Money)
                {
                    bt.Texture = Session.Map.SmallNormalButtonTexture;
                    bt.SetColor(Session.Map.ForeColor);

                    if (bt.State == UIButtonState.Inactive)
                    {
                        bt.LeftClickEvent += new EventHandler(buyTower_LeftClick);
                        bt.Activate();
                    }
                }
            }
            else if (ut != null)
            {
                if (_clickedTower != null && _clickedTower.UpdateCost <= Session.ActivePlayer.Money
                    && _clickedTower.Level + 1 < _clickedTower.MaxLevel)
                {
                    ut.Texture = Session.Map.SmallNormalButtonTexture;
                    ut.SetColor(Session.Map.ForeColor);

                    if (ut.State == UIButtonState.Inactive)
                    {
                        ut.LeftClickEvent += new EventHandler(upgradeTower_LeftClick);
                        ut.Activate();
                    }
                }
            }
        }

        void Session_TowerPurchased(object sender, TowerEventArgs ptea)
        {
            ptea._tower.LeftClickEvent += new EventHandler(clickableTower_LeftClickEvent);
            Button b = SelectedTower.GetButton("BuyTower");

            if (_clickedTower.Cost > Session.ActivePlayer.Money)
            {
                b.Texture = Session.Map.SmallErrorButtonTexture;
                b.SetColor(Session.Map.ErrorColor);

                if (b.State == UIButtonState.Active)
                {
                    b.LeftClickEvent -= buyTower_LeftClick;
                    b.Deactivate();
                }
            }
        }

        public void Initialise(SpriteFont spriteFont)
        {
            _spriteFont = spriteFont;
            InitialiseMoneyAndTowers();
            InitialisePurchasedTower();
            InitialiseStatsAndControls();
        }

        void clickableTower_LeftClickEvent(object sender, EventArgs e)
        {
            Tower t = sender as Tower;

            if (t.IsPlaced && t.PlacedTime > 1)
            {
                _clickedTower = t;
                InitialiseSelectedTowers(t);
            }
        }

        private void InitialiseMoneyAndTowers()
        {
            MoneyAndTowers.Add("Money", new Text(Session.MoneyDisplay,
                new Vector2(Rectangle.Left + _padding, Rectangle.Top + _padding)));
            MoneyAndTowers.Add("Towers", new Text(Session.TowersDisplay,
                new Vector2(Rectangle.Left + _padding, Rectangle.Top + _padding + _spriteFont.LineSpacing)));
        }

        private void InitialisePurchasedTower()
        {
            PurchaseTower.Add("Purchase", new Text("Purchase a tower", new Vector2(PurchaseTower.Dimentions.Left + _padding, PurchaseTower.Dimentions.Top + _padding)));

            Vector2 pos = new Vector2(PurchaseTower.Dimentions.Left + _padding, PurchaseTower.Dimentions.Top + _padding + (_spriteFont.LineSpacing * 2));

            foreach (Tower tower in Session.Map.TowerList)
            {
                Button b = new Button(tower.ThumbNail, Vector2.Add(pos, new Vector2(tower.ThumbNail.Width / 2.0f, tower.ThumbNail.Height / 2.0f)), tower);
                b.LeftClickEvent += new EventHandler(selectTower_LeftClick);
                PurchaseTower.Add(tower.Name, b);
                pos.X += tower.ThumbNail.Width + _padding;

                if (pos.X + tower.ThumbNail.Width >= PurchaseTower.Dimentions.Right)
                {
                    pos = new Vector2(PurchaseTower.Dimentions.Left + _padding, pos.Y + tower.ThumbNail.Height + _padding);
                }
            }
        }

        void selectTower_LeftClick(object sender, EventArgs e)
        {
            if (_clickedTower == null)
            {
                Button b = sender as Button;

                if (b != null)
                {
                    Tower t = b.StoredObject as Tower;

                    if (t != null)
                    {
                        _clickedTower = t;
                        InitialiseSelectedTowers(t);
                    }
                }
            }
        }

        private void InitialiseSelectedTowers(Tower t)
        {
            SelectedTower.ClearAll();

            Image icon = new Image(_clickedTower.Texture, new Vector2(SelectedTower.Dimentions.Left,
                SelectedTower.Dimentions.Top + _padding));
            SelectedTower.Add("TowerIcon", icon);

            SelectedTower.Add("TowerName", new Text(_clickedTower.Name + " " +
                (_clickedTower.Level + 1).ToString(), _spriteFont,
                new Vector2(icon.Rectangle.Right + _padding, SelectedTower.Dimentions.Top + _padding)));
            SelectedTower.Add("TowerDescription", new Text(_clickedTower.Description, _spriteFont,
                new Vector2(icon.Rectangle.Right + _padding, SelectedTower.Dimentions.Top + _padding + _spriteFont.LineSpacing)));

            Text stats = new Text(_clickedTower.CurrentStatistics.ToShortString(), _spriteFont,
                new Vector2(SelectedTower.Dimentions.Left + _padding, icon.Rectangle.Bottom));
            SelectedTower.Add("Stats", stats);

            Text specials = new Text(string.Format("Specials: {0}", t.BulletBase.Type == BulletType.Normal ? "None" : t.BulletBase.Type.ToString()),
                _spriteFont, new Vector2(SelectedTower.Dimentions.Left + _padding, stats.Rectangle.Bottom));
            SelectedTower.Add("Specials", specials);

            Text price = new Text(string.Format("Price: {0}", _clickedTower.TotalCost), _spriteFont,
                new Vector2(SelectedTower.Dimentions.Left + _padding, specials.Rectangle.Bottom));
            SelectedTower.Add("Price", price);

            if (t.IsPlaced)
            {
                int pb = AddUpgradeButton(price.Rectangle.Bottom + _padding);
                AddSellButton(pb + _padding);
            }
            else
            {
                AddPurchaseButton(price.Rectangle.Bottom + _padding);
            }

            string s = t.IsPlaced ? "Deselect tower" : "Cancel";
            Vector2 sdim = _spriteFont.MeasureString(s);

            Vector2 cbpos = new Vector2((int)(SelectedTower.Dimentions.Left +
                (Session.Map.SmallNormalButtonTexture.Width / 2.0f) +
                (SelectedTower.Dimentions.Width - Session.Map.SmallNormalButtonTexture.Width) / 2.0f),
                (int)(SelectedTower.Dimentions.Bottom - (Session.Map.SmallNormalButtonTexture.Height / 2.0f) - _padding));

            Vector2 ctpos = new Vector2((int)(cbpos.X - Session.Map.SmallNormalButtonTexture.Width / 2.0f + _padding),
                (int)(SelectedTower.Dimentions.Bottom - (Session.Map.SmallNormalButtonTexture.Height + sdim.Y) / 2.0f - _padding));

            Button cb = new Button(Session.Map.SmallNormalButtonTexture, cbpos, new Text(s, _spriteFont, ctpos),
                Session.Map.ForeColor, null);
            cb.LeftClickEvent += new EventHandler(cancelButton_LeftClick);
            SelectedTower.Add("Cancel", cb);
        }

        private void AddSellButton(int y)
        {
            Button b = null;
            string st = string.Format("Sell tower (receive {0})", (int)(_clickedTower.TotalCost * _clickedTower.SellScaler));
            Vector2 stdim = _spriteFont.MeasureString(st);
            Vector2 bpos = new Vector2((int)(SelectedTower.Dimentions.Left + (Session.Map.SmallNormalButtonTexture.Width / 2.0f) +
                (SelectedTower.Dimentions.Width - Session.Map.SmallNormalButtonTexture.Width) / 2.0f),
                (int)(y + (Session.Map.SmallNormalButtonTexture.Height / 2.0f)));

            Vector2 tpos = new Vector2((int)(bpos.X - Session.Map.SmallNormalButtonTexture.Width / 2.0f + _padding),
            (int)(y + (Session.Map.SmallNormalButtonTexture.Height - stdim.Y) / 2.0f));

            b = new Button(Session.Map.SmallNormalButtonTexture, bpos, new Text(st, _spriteFont, tpos),
                Session.Map.ForeColor, _clickedTower);
            b.LeftClickEvent += new EventHandler(sellTower_LeftClick);
            SelectedTower.Add("SellTower", b);
        }

        void sellTower_LeftClick(object sender, EventArgs e)
        {
            Button b = sender as Button;

            if(b != null)
            {
                Tower t = b.StoredObject as Tower;
                Session.SellTower(t);
                _clickedTower = null;
            }
        }

        private int AddUpgradeButton(int y)
        {
            Button b = null;
            if (_clickedTower.UpdateCost <= Session.ActivePlayer.Money && _clickedTower.Level + 1 < _clickedTower.MaxLevel)
            {
                string bt = string.Format("Upgrade tower (costs {0})", _clickedTower.UpdateCost);
                Vector2 btdim = _spriteFont.MeasureString(bt);
                Vector2 bpos = new Vector2((int)(SelectedTower.Dimentions.Left + (Session.Map.SmallNormalButtonTexture.Width / 2.0f) +
                    (SelectedTower.Dimentions.Width - Session.Map.SmallNormalButtonTexture.Width) / 2.0f),
                    (int)(y + (Session.Map.SmallNormalButtonTexture.Height / 2.0f)));

                Vector2 tpos = new Vector2((int)(bpos.X - Session.Map.SmallNormalButtonTexture.Width / 2.0f + _padding),
                    (int)(y + (Session.Map.SmallNormalButtonTexture.Height - btdim.Y) / 2.0f));

                b = new Button(Session.Map.SmallNormalButtonTexture, bpos, new Text(bt, _spriteFont, tpos),
                    Session.Map.ForeColor, _clickedTower);
                b.LeftClickEvent += new EventHandler(upgradeTower_LeftClick);
                SelectedTower.Add("UpgradeTower", b);
            }
            else
            {
                string bt = string.Format("Upgrade tower (costs {0})", _clickedTower.UpdateCost);
                Vector2 btdim = _spriteFont.MeasureString(bt);

                Vector2 bpos = new Vector2((int)(SelectedTower.Dimentions.Left +
                    (Session.Map.SmallErrorButtonTexture.Width / 2.0f) +
                    (SelectedTower.Dimentions.Width - Session.Map.SmallErrorButtonTexture.Width) / 2.0f),
                    (int)(y + (Session.Map.SmallErrorButtonTexture.Height / 2.0f)));

                Vector2 tpos = new Vector2((int)(bpos.X - Session.Map.SmallErrorButtonTexture.Width / 2.0f + _padding),
                    (int)(y + (Session.Map.SmallErrorButtonTexture.Height - btdim.Y) / 2.0f));

                b = new Button(Session.Map.SmallErrorButtonTexture, bpos, new Text(bt, _spriteFont, tpos),
                    Session.Map.ForeColor, _clickedTower);
                b.Deactivate();
                SelectedTower.Add("UpgradeTower", b);
            }

            return (int)(b.Position.Y - b.Origin.Y) + b.Texture.Height;
        }

        private void AddPurchaseButton(int y)
        {
            if (_clickedTower.Cost <= Session.ActivePlayer.Money && _clickedTower.Level < _clickedTower.MaxLevel)
            {
                string bt = string.Format("Buy tower (costs {0})", _clickedTower.Cost);
                Vector2 btdim = _spriteFont.MeasureString(bt);
                Vector2 bpos = new Vector2((int)(SelectedTower.Dimentions.Left +
                    (Session.Map.SmallNormalButtonTexture.Width / 2.0f) +
                    (SelectedTower.Dimentions.Width - Session.Map.SmallNormalButtonTexture.Width) / 2.0f),
                    (int)(y + (Session.Map.SmallNormalButtonTexture.Height / 2.0f)));

                Vector2 tpos = new Vector2((int)(bpos.X - Session.Map.SmallNormalButtonTexture.Width / 2.0f + _padding),
                    (int)(y + (Session.Map.SmallNormalButtonTexture.Height - btdim.Y) / 2.0f));

                Button b = new Button(Session.Map.SmallNormalButtonTexture, bpos, new Text(bt, _spriteFont, tpos),
                    Session.Map.ForeColor, _clickedTower);
                b.LeftClickEvent += new EventHandler(buyTower_LeftClick);
                SelectedTower.Add("BuyTower", b);
            }
            else
            {
                string bt = string.Format("Buy tower (costs {0})", _clickedTower.Cost);
                Vector2 btdim = _spriteFont.MeasureString(bt);
                Vector2 bpos = new Vector2((int)(SelectedTower.Dimentions.Left +
                    (Session.Map.SmallErrorButtonTexture.Width / 2.0f) +
                    (SelectedTower.Dimentions.Width - Session.Map.SmallErrorButtonTexture.Width) / 2.0f),
                    (int)(y + (Session.Map.SmallErrorButtonTexture.Height / 2.0f)));

                Vector2 tpos = new Vector2((int)(bpos.X - Session.Map.SmallErrorButtonTexture.Width / 2.0f + _padding),
                    (int)(y + (Session.Map.SmallErrorButtonTexture.Height - btdim.Y) / 2.0f));

                Button b = new Button(Session.Map.SmallErrorButtonTexture, bpos, new Text(bt, _spriteFont, tpos),
                    Session.Map.ForeColor, _clickedTower);
                b.Deactivate();
                SelectedTower.Add("BuyTower", b);
            }
        }

        void upgradeTower_LeftClick(object sender, EventArgs e)
        {
            Button b = sender as Button;

            if (b != null)
            {
                Tower t = b.StoredObject as Tower;
                Session.UpgradeTower(t);
                b.ButtonText.Value = string.Format("Upgrade tower (costs {0})", _clickedTower.UpdateCost);
                SelectedTower.GetButton("SellTower").ButtonText.Value = string.Format("Sell tower (receive {0})",
                    (int)(_clickedTower.TotalCost * _clickedTower.SellScaler));
                SelectedTower.GetText("Stats").Value = _clickedTower.CurrentStatistics.ToShortString();
                SelectedTower.GetText("Price").Value = string.Format("Price: {0}", _clickedTower.TotalCost);
                SelectedTower.GetText("TowerName").Value = string.Format("{0} {1}", _clickedTower.Name, (_clickedTower.Level + 1).ToString());

                if (_clickedTower.UpdateCost > Session.ActivePlayer.Money || _clickedTower.Level == _clickedTower.MaxLevel)
                {
                    b.Texture = Session.Map.SmallErrorButtonTexture;
                    b.SetColor(Session.Map.ErrorColor);
                    b.LeftClickEvent -= upgradeTower_LeftClick;
                    b.Deactivate();
                }
            }
        }

        void buyTower_LeftClick(object sender, EventArgs e)
        {
            Button b = sender as Button;

            if (b != null)
            {
                Tower t = b.StoredObject as Tower;

                if (t != null)
                {
                    Session.SelectTower(t);
                }
            }
        }

        void cancelButton_LeftClick(object sender, EventArgs e)
        {
            ResetTowerReferences();
            Session.UI.MapRegion.ResetTowerReferences();
        }

        private void InitialiseStatsAndControls()
        {
            Text t;
            int y = StatsAndControls.Dimentions.Top + _padding;
            StatsAndControls.Add("Wave", new Text(string.Format("Wave {0} of {1}", _waveIndex + 1,
                Session.Map.WaveList.Count), new Vector2(StatsAndControls.Dimentions.Left + _padding, y)));

            Vector2 d = _spriteFont.MeasureString(Session.HealthDisplay);
            StatsAndControls.Add("Health", new Text(Session.HealthDisplay,
                new Vector2(StatsAndControls.Dimentions.Right - d.X - _padding, y)));

            y += (int)(d.Y + _spriteFont.LineSpacing);

            //string bt = "Launch next wave now";
            //Vector2 btdim = _spriteFont.MeasureString(bt);
            //Texture2D tex = Session.Map.State == MapState.WaveDelay ? Session.Map.SmallNormalButtonTexture : Session.Map.SmallErrorButtonTexture;
            //Color c = Session.Map.State == MapState.WaveDelay ? Session.Map.ForeColor : Session.Map.ErrorColor;

            //Vector2 bpos = new Vector2((int)(SelectedTower.Dimentions.Left + (tex.Width / 2.0f) +
            //    (SelectedTower.Dimentions.Width - tex.Width) / 2.0f), (int)(y + (tex.Height / 2.0f)));

            //Vector2 tpos = new Vector2((int)(bpos.X - tex.Width / 2.0f + _padding),
            //    (int)(y + (tex.Height - btdim.Y) / 2.0f));

            //Button b = new Button(tex, bpos, new Text(bt, _spriteFont, tpos), c, _clickedTower);
            //b.LeftClickEvent += new EventHandler(nextWave_LeftClick);
            //StatsAndControls.Add("LaunchNextWave", b);
        }

        void nextWave_LeftClick(object sender, EventArgs e)
        {
            
        }

        void pause_LeftClick()
        {
        }

        void increaseSpeed_LeftClick()
        {
        }

        void decreaseSpeed_LeftClick()
        {
        }

        internal void ResetTowerReferences()
        {
            if (_clickedTower != null)
            {
                _clickedTower = null;
            }

            if (Session.SelectedTower != null)
            {
                Session.DeselectTower();
            }
        }

        public void Update(GameTime gameTime)
        {
            MoneyAndTowers.GetText("Money").Value = Session.MoneyDisplay;
            MoneyAndTowers.GetText("Towers").Value = Session.TowersDisplay;

            //Button lnw = StatsAndControls.GetButton("LaunchNextWave");
            //Texture2D tex = Session.Map.State == MapState.WaveDelay ? Session.Map.SmallNormalButtonTexture : Session.Map.SmallErrorButtonTexture;
            //Color c = Session.Map.State == MapState.WaveDelay ? Session.Map.ForeColor : Session.Map.ErrorColor;
            //lnw.Texture = tex;
            //lnw.SetColor(c);

            if (_clickedTower != null)
            {
                foreach (var b in SelectedTower.Buttons)
                {
                    b.Value.Update(gameTime, Session.UI.Mouse);
                }
            }
            else
            {
                foreach (var b in PurchaseTower.Buttons)
                {
                    b.Value.Update(gameTime, Session.UI.Mouse);
                }
            }

            foreach (var b in StatsAndControls.Buttons)
            {
                b.Value.Update(gameTime, Session.UI.Mouse);
            }

            if (_waveIndex != Session.Map.WaveIndex)
            {
                _waveIndex = Session.Map.WaveIndex;
                StatsAndControls.GetText("Wave").Value = string.Format("Wave {0} of {1}", _waveIndex + 1,
                    Session.Map.WaveList.Count);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.Draw(_background, Rectangle, Color.White);
            MoneyAndTowers.Draw(gameTime, spriteBatch, _spriteFont);

            if (_clickedTower != null)
            {
                SelectedTower.Draw(gameTime, spriteBatch, _spriteFont);
            }
            else
            {
                PurchaseTower.Draw(gameTime, spriteBatch, _spriteFont);
            }

            StatsAndControls.Draw(gameTime, spriteBatch, _spriteFont);
        }
    }
}
