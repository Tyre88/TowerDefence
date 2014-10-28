using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TD_Engine
{
    public class Session
    {
        public delegate void PurchaseTowerEventHandler(object sender, TowerEventArgs e);
        public delegate void SellTowerEventHandles(object sender, TowerEventArgs e);

        public Map Map
        {
            get;
            private set;
        }

        public string MoneyDisplay
        {
            get;
            private set;
        }

        public string TowersDisplay
        {
            get;
            private set;
        }

        public Player ActivePlayer
        {
            get;
            private set;
        }

        public Tower SelectedTower
        {
            get;
            private set;
        }

        public int Health
        {
            get;
            private set;
        }

        public string HealthDisplay
        {
            get;
            private set;
        }

        public UserInterface UI
        {
            get;
            private set;
        }

        public bool IsPaused
        {
            get;
            private set;
        }

        public float MinSpeed
        {
            get;
            private set;
        }

        public float Speed
        {
            get;
            private set;
        }

        public float MaxSpeed
        {
            get;
            private set;
        }

        public static Session _singleton;

        public event PurchaseTowerEventHandler TowerPurchased;
        public event SellTowerEventHandles TowerSold;
        public event EventHandler HealthDecreased;
        public event EventHandler MoneyIncreased;
        public event EventHandler MapFinished;

        public Session(Map map)
        {
            Map = map;
            ActivePlayer = new Player();
            ActivePlayer.Money = (uint)map.Money;
            ActivePlayer.PlacedTowers = new List<Tower>(20);
            Health = 20;

            MoneyDisplay = string.Format("Available money: {0}", ActivePlayer.Money);
            TowersDisplay = string.Format("Placed towers: {0}", ActivePlayer.PlacedTowers.Count);
            HealthDisplay = string.Format("Lives: {0}", Health);

            _singleton = this;
            IsPaused = false;

            MinSpeed = 0.5f;
            Speed = 1.0f;
            MaxSpeed = 2.0f;
        }

        public void SetUI(UserInterface UI)
        {
            this.UI = UI;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsPaused)
            {
                UI.Update(gameTime);

                foreach (Tower item in ActivePlayer.PlacedTowers)
                {
                    item.Update(gameTime, UI.Mouse);
                }

                Map.Update(gameTime);

                if (Map.State == MapState.Finished && MapFinished != null)
                {
                    MapFinished(this, EventArgs.Empty);
                }
            }
        }

        public void SelectTower(Tower tower)
        {
            SelectedTower = tower;
        }

        public void DeselectTower()
        {
            SelectedTower = null;
        }

        internal void PurchaseTower(Tower tower, Point mapLocation)
        {
            if (tower.Cost <= ActivePlayer.Money)
            {
                if (!tower.IsInitialised)
                {
                    tower.Initialize(Map);
                }

                ActivePlayer.Money -= (uint)tower.Cost;
                MoneyDisplay = string.Format("Available money: {0}", ActivePlayer.Money);
                tower.MapLocation = mapLocation;
                ActivePlayer.PlacedTowers.Add(tower);
                TowersDisplay = string.Format("Placed towers: {0}", ActivePlayer.PlacedTowers.Count);
                Map.SetValidPlacement(tower.MapLocation.X, tower.MapLocation.Y, false);
                tower.PlaceTower();

                if (TowerPurchased != null)
                {
                    TowerPurchased(this, new TowerEventArgs(tower));
                }
            }
        }

        internal void UpgradeTower(Tower tower)
        {
            if (tower.UpdateCost <= ActivePlayer.Money && tower.Level + 1 < tower.MaxLevel)
            {
                ActivePlayer.Money -= (uint)tower.UpdateCost;
                MoneyDisplay = string.Format("Available money: {0}", ActivePlayer.Money);
                tower.Upgrade();
            }
        }

        internal void SellTower(Tower tower)
        {
            ActivePlayer.Money += (uint)(tower.TotalCost * tower.SellScaler);
            MoneyDisplay = string.Format("Available money: {0}", ActivePlayer.Money);
            ActivePlayer.PlacedTowers.Remove(tower);
            TowersDisplay = string.Format("Placed towers: {0}", ActivePlayer.PlacedTowers.Count);
            Map.SetValidPlacement(tower.MapLocation.X, tower.MapLocation.Y, true);

            if (TowerSold != null)
            {
                TowerSold(this, new TowerEventArgs(tower));
            }
        }

        internal void DecreaseHealth(int amount)
        {
            Health -= amount;
            HealthDisplay = string.Format("Lives: {0}", Health);

            if (HealthDecreased != null)
            {
                HealthDecreased(this, EventArgs.Empty);
            }
        }

        internal void AddMoney(int amount)
        {
            ActivePlayer.Money += (uint)amount;
            MoneyDisplay = string.Format("Available money: {0}", ActivePlayer.Money);

            if (MoneyIncreased != null)
            {
                MoneyIncreased(this, EventArgs.Empty);
            }
        }

        internal void IncreaseSpeed(float amount)
        {
            if (Speed < MaxSpeed)
            {
                Speed += amount;
                MathHelper.Clamp(Speed, MinSpeed, MaxSpeed);
            }
        }

        internal void DecreaseSpeed(float amount)
        {
            if (Speed > MinSpeed)
            {
                Speed -= amount;
                MathHelper.Clamp(Speed, MinSpeed, MaxSpeed);
            }
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }
    }
}
