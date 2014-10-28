using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD_Engine
{
    public class MapRegion
    {
        public Rectangle Rectangle
        {
            get;
            private set;
        }


        public Session Session
        {
            get;
            private set;
        }

        public Texture2D ValidPlacement
        {
            get;
            private set;
        }

        public Texture2D InvalidPlacement
        {
            get;
            private set;
        }

        public Texture2D SpawnPlacement
        {
            get;
            private set;
        }

        public Texture2D MonsterHealthBar
        {
            get;
            private set;
        }

        public Tower SelectedActiveTower
        {
            get;
            private set;
        }

        public MapRegion(Session s, Rectangle r, GraphicsDevice gd)
        {
            Session = s;
            Rectangle = r;

            ValidPlacement = new Texture2D(gd, 1, 1);
            Color[] color = new Color[1];
            color[0] = Session.Map.ForeColor;
            ValidPlacement.SetData<Color>(color);

            InvalidPlacement = new Texture2D(gd, 1, 1);
            color[0] = Color.Red;
            InvalidPlacement.SetData<Color>(color);

            SpawnPlacement = new Texture2D(gd, 1, 1);
            color[0] = Color.Blue;
            SpawnPlacement.SetData<Color>(color);

            MonsterHealthBar = new Texture2D(gd, 1, 1);
            color[0] = Color.Red;
            MonsterHealthBar.SetData<Color>(color);

            Session.TowerPurchased += new Session.PurchaseTowerEventHandler(Session_TowerPurchased);
            Session.TowerSold += new Session.SellTowerEventHandles(Session_TowerSold);
        }

        void Session_TowerSold(object sender, TowerEventArgs ptea)
        {
            if (ptea._tower == SelectedActiveTower)
            {
                SelectedActiveTower = null;
            }
        }

        void Session_TowerPurchased(object sender, TowerEventArgs ptea)
        {
            ptea._tower.LeftClickEvent += new EventHandler(T_LeftClickEvent);
        }

        void T_LeftClickEvent(object sender, EventArgs e)
        {
            Tower t = sender as Tower;

            if (t.PlacedTime > 1)
            {
                SelectedActiveTower = t;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (Session.UI.Mouse.NewLeftClick && Rectangle.Intersects(Session.UI.Mouse.Rectangle) && Session.SelectedTower != null)
            {
                Point mousePointerInMap = Session.Map.ToMapCoordinates((int)Session.UI.Mouse.Position.X, (int)Session.UI.Mouse.Position.Y);

                if(Session.Map.IsValidPlacement(mousePointerInMap.X, mousePointerInMap.Y))
                {
                    Session.PurchaseTower(Session.SelectedTower.Clone(), mousePointerInMap);
                    Session.DeselectTower();
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            int tilecode;

            for (int y = 0; y < Session.Map.Dimensions.Y; y++)
            {
                for (int x = 0; x < Session.Map.Dimensions.X; x++)
                {
                    tilecode = Session.Map.GroundTiles[(y * Session.Map.Dimensions.X) + x] - 1;
                    int tcy = tilecode / Session.Map.NumberOfTilesInSheet.X;
                    int tcx = tilecode - (tcy * Session.Map.NumberOfTilesInSheet.X);
                    spriteBatch.Draw(Session.Map.TileSheet, new Vector2(x * Session.Map.TileDimensions.X,
                        y * Session.Map.TileDimensions.Y), new Rectangle(tcx * Session.Map.TileDimensions.X,
                            tcy * Session.Map.TileDimensions.Y, Session.Map.TileDimensions.X, Session.Map.TileDimensions.Y), Color.White);
                }
            }

            for (int y = 0; y < Session.Map.Dimensions.Y; y++)
            {
                for (int x = 0; x < Session.Map.Dimensions.X; x++)
                {
                    tilecode = Session.Map.FrontTiles[(y * Session.Map.Dimensions.X) + x] - 1;
                    if (tilecode > 0)
                    {
                        int tcy = tilecode / Session.Map.NumberOfTilesInSheet.X;
                        int tcx = tilecode - (tcy * Session.Map.NumberOfTilesInSheet.X);
                        spriteBatch.Draw(Session.Map.TileSheet, new Vector2(x * Session.Map.TileDimensions.X,
                            y * Session.Map.TileDimensions.Y), new Rectangle(tcx * Session.Map.TileDimensions.X,
                                tcy * Session.Map.TileDimensions.Y, Session.Map.TileDimensions.X, Session.Map.TileDimensions.Y), Color.White);
                    }

                }
            }

            //for (int i = 0; i < Session.Map.ActivePath.ShortestPath._path.Count; i++)
            //{
            //    Tile t = Session.Map.ActivePath.ShortestPath._path[i];
            //    tilecode = t.TileCode;

            //    if (tilecode > 0)
            //    {
            //        int tcy = tilecode / Session.Map.NumberOfTilesInSheet.X;
            //        int tcx = tilecode - (tcy * Session.Map.NumberOfTilesInSheet.X);
            //        spriteBatch.Draw(Session.Map.TileSheet, new Vector2(t.MapLocation.X * Session.Map.TileDimensions.X,
            //            t.MapLocation.Y * Session.Map.TileDimensions.Y), new Rectangle(tcx * Session.Map.TileDimensions.X,
            //                tcy * Session.Map.TileDimensions.Y, Session.Map.TileDimensions.X, Session.Map.TileDimensions.Y), Color.White);
            //    }
            //}

            foreach (Monster monster in Session.Map.ActiveWave.Monsters)
            {
                if (monster.IsActive)
                {
                    monster.Draw(gameTime, spriteBatch);

                    spriteBatch.Draw(MonsterHealthBar, monster.HealthBarPosition, monster.MaxHealthRectangle, Color.Black);
                    spriteBatch.Draw(MonsterHealthBar, monster.HealthBarPosition, monster.HealthRectangle, monster.HealthBarColor);

                    //spriteBatch.Draw(MonsterHealthBar, new Rectangle(monster.Rectangle.Left, monster.Rectangle.Top - 4,
                    //    (int)(monster.Rectangle.Width * ((float)monster.Health - (float)monster.MaxHealth)), 2), Color.White);
                }
            }

            Session.Map.Spawn.Draw(gameTime, spriteBatch);
            Session.Map.Castle.Draw(gameTime, spriteBatch);

            if (SelectedActiveTower != null)
            {
                SelectedActiveTower.DrawRadius(gameTime, spriteBatch);
            }

            foreach (Tower tower in Session.ActivePlayer.PlacedTowers)
            {
                tower.Draw(gameTime, spriteBatch);
            }

            if (Session.SelectedTower != null)
            {
                for (int y = 0; y < Session.Map.Dimensions.Y; y++)
                {
                    for (int x = 0; x < Session.Map.Dimensions.X; x++)
                    {
                        if (Session.Map.IsValidPlacement(x, y))
                        {
                            spriteBatch.Draw(ValidPlacement, new Rectangle(x * Session.Map.TileDimensions.X,
                                y * Session.Map.TileDimensions.Y, Session.Map.TileDimensions.X,
                                Session.Map.TileDimensions.Y), Color.White * 0.5f);
                        }
                        else
                        {
                            if (Session.Map.SpawnLocation.X == x && Session.Map.SpawnLocation.Y == y)
                            {
                                spriteBatch.Draw(SpawnPlacement, new Rectangle(x * Session.Map.TileDimensions.X,
                                    y * Session.Map.TileDimensions.Y, Session.Map.TileDimensions.X,
                                    Session.Map.TileDimensions.Y), Color.Blue * 0.5f);
                            }
                            else
                            {
                                spriteBatch.Draw(InvalidPlacement, new Rectangle(x * Session.Map.TileDimensions.X,
                                    y * Session.Map.TileDimensions.Y, Session.Map.TileDimensions.X,
                                    Session.Map.TileDimensions.Y), Color.Red * 0.5f);
                            }
                        }
                    }
                }
            }

            Point mousePointerInMap = Session.Map.ToMapCoordinates((int)Session.UI.Mouse.Position.X,
                (int)Session.UI.Mouse.Position.Y);
            Point mousePointerInWord = Session.Map.ToWorldCoordinates((int)mousePointerInMap.X,
                (int)mousePointerInMap.Y);

            if (Session._singleton.UI.CommandInfoBar.ClickedTower != null)
            {
                if (Session.Map.IsValidPlacement(mousePointerInMap.X, mousePointerInMap.Y) && Session.SelectedTower != null)
                {
                    spriteBatch.Draw(Session.SelectedTower.Texture, new Vector2(mousePointerInWord.X,
                        mousePointerInWord.Y), Color.White * 0.5f);
                }
            }
        }

        internal void ResetTowerReferences()
        {
            SelectedActiveTower = null;
        }
    }
}
