using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TD_Engine
{
    public class Tower
    {
        #region Writer
        [ContentTypeWriter]
        public class TowerWriter : ContentTypeWriter<Tower>
        {
            protected override void Write(ContentWriter output, Tower value)
            {
                output.Write(value.Name);
                output.Write(value.Description);
                output.Write(value.ThumbNailAsset);
                output.Write(value.TextureAsset);
                output.Write(value.RadiusTextureAsset);
                output.Write(value.Cost);
                output.Write(value.Level);
                output.Write(value.MaxLevel);
                output.WriteObject(value.InitialStatistics);
                output.WriteObject(value.UpgradeStatistics);
                output.Write(value.BulletAsset);
                output.Write(value.SellScaler);
                output.Write(value.Scale);
            }

            public override string GetRuntimeReader(Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform targetPlatform)
            {
                return typeof(Tower.TowerReader).AssemblyQualifiedName;
            }
        }

        #endregion

        [ContentSerializer]
        public string Name
        {
            get;
            private set;
        }

        [ContentSerializer]
        public string Description
        {
            get;
            private set;
        }

        [ContentSerializer]
        public string ThumbNailAsset
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public Texture2D ThumbNail
        {
            get;
            private set;
        }

        [ContentSerializer]
        public string TextureAsset
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public Texture2D Texture
        {
            get;
            private set;
        }

        [ContentSerializer]
        public string RadiusTextureAsset
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public Texture2D RadiusTexture
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public Vector2 Origin
        {
            get;
            private set;
        }

        [ContentSerializer]
        public int Cost
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public int UpdateCost
        {
            get { return ((Level + 1) * Cost) / 2; }
        }

        [ContentSerializerIgnore]
        public int TotalCost
        {
            get;
            private set;
        }

        int _level;

        [ContentSerializer]
        public int Level
        {
            get { return _level; }
            set
            {
                if (value < MaxLevel)
                {
                    _level = value;
                    LevelUp();
                }
            }
        }

        [ContentSerializer]
        public int MaxLevel
        {
            get;
            private set;
        }

        TowerStatistics _initialStatistics;

        [ContentSerializer]
        public TowerStatistics InitialStatistics
        {
            get { return _initialStatistics; }
            set
            {
                _initialStatistics = value;
                LevelUp();
            }
        }

        [ContentSerializerIgnore]
        public Map Map
        {
            get;
            private set;
        }

        private void LevelUp()
        {
            if (Level > 0 && InitialStatistics != null && UpgradeStatistics != null)
            {
                CurrentStatistics = TowerStatistics.Add(InitialStatistics, UpgradeStatistics.Multiply(_upgraderStatistics, Level));
            }
        }

        [ContentSerializerIgnore]
        public TowerStatistics CurrentStatistics
        {
            get;
            private set;
        }

        UpgradeStatistics _upgraderStatistics;

        [ContentSerializer]
        public UpgradeStatistics UpgradeStatistics
        {
            get { return _upgraderStatistics; }
            set
            {
                _upgraderStatistics = value;
                LevelUp();
            }
        }

        [ContentSerializer]
        private string BulletAsset
        {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public Bullet BulletBase
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public List<Bullet> Bullets
        {
            get;
            private set;
        }

        [ContentSerializer]
        public float SellScaler
        {
            get;
            private set;
        }

        Point _mapLocation;

        [ContentSerializerIgnore]
        public Point MapLocation
        {
            get { return _mapLocation; }
            set
            {
                _mapLocation = value;
                SetLocation();
            }
        }

        [ContentSerializerIgnore]
        public Rectangle Rectangle
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public Vector2 Position
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public float Rotation
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public bool IsInitialised
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public bool IsPlaced
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public ObjectClickedState LeftState
        {
            get;
            private set;
        }


        [ContentSerializerIgnore]
        public Mouse ActiveMouse
        {
            get;
            protected set;
        }

        public event EventHandler LeftClickEvent;

        [ContentSerializerIgnore]
        public Monster MonsterTarget
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public float PlacedTime
        {
            get;
            private set;
        }

        [ContentSerializer(Optional = true)]
        public float Scale { get; set; }

        float _timer;

        public void Initialize(Map map, ContentManager contentManager)
        {
            Map = map;

            if (!string.IsNullOrEmpty(ThumbNailAsset))
            {
                ThumbNail = contentManager.Load<Texture2D>(string.Format("Textures\\Towers\\{0}", ThumbNailAsset));
            }

            if (!string.IsNullOrEmpty(TextureAsset))
            {
                Texture = contentManager.Load<Texture2D>(string.Format("Textures\\Towers\\{0}", TextureAsset));
                Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            }

            if (!string.IsNullOrEmpty(RadiusTextureAsset))
            {
                RadiusTexture = contentManager.Load<Texture2D>(string.Format("Textures\\Towers\\{0}", RadiusTextureAsset));
            }

            BulletBase.Initialize(Map, contentManager);

            _timer = CurrentStatistics.AttackSpeed;

            IsInitialised = true;
        }

        public void Initialize(Map map)
        {
            Map = map;

            IsInitialised = true;

            Bullets = new List<Bullet>();
        }

        private void SetLocation()
        {
            if (Map != null)
            {
                Point p = Map.ToWorldCoordinates(MapLocation.X, MapLocation.Y);
                Rectangle = new Rectangle(p.X, p.Y, Texture.Width, Texture.Height);
                Position = new Vector2(p.X + Origin.X, p.Y + Origin.Y);
            }
        }

        public void PlaceTower()
        {
            IsPlaced = true;
            TotalCost = Cost;
        }

        public void Upgrade()
        {
            Level++;
            LevelUp();
            TotalCost += UpdateCost;
            //this.InitialStatistics.Accuracy += this.UpgradeStatistics.AccuracyIncrease;
            //this.InitialStatistics.Damage += this.UpgradeStatistics.DamageIncrease;
            //this.InitialStatistics.CritChance += this.UpgradeStatistics.CritChanceIncrease;
            //this.InitialStatistics.CritScalar += this.UpgradeStatistics.CritScalarIncrease;
            //this.InitialStatistics.Radius += this.UpgradeStatistics.RadiusIncrease;
            //this.InitialStatistics.Health += this.UpgradeStatistics.HealthIncrease;
            //this.InitialStatistics.AttackSpeed -= this.UpgradeStatistics.AttackSpeedIncrease;
        }

        public void Update(GameTime gameTime, Mouse activeMouse)
        {
            if (IsPlaced)
            {
                PlacedTime += (float)gameTime.ElapsedGameTime.TotalSeconds * Session._singleton.Speed;
                if (_timer > 0)
                {
                    _timer -= (float)gameTime.ElapsedGameTime.TotalSeconds * Session._singleton.Speed;
                }
            }

            UpdateMouse(gameTime, activeMouse);

            foreach (Bullet item in Bullets)
            {
                item.Update(gameTime);
            }

            if (MonsterTarget != null)
            {
                UpdateTarget(gameTime);
            }
            else
            {
                MonsterTarget = FindTarget();
            }
        }

        private void UpdateTarget(GameTime gameTime)
        {
            float distance = Vector2.Distance(MonsterTarget.Position, Position);
            if ((distance <= CurrentStatistics.Radius * Session._singleton.Map.TileDimensions.X && MonsterTarget.IsActive)
                && (!(Map.ToMapCoordinates((int)MonsterTarget.Position.X, (int)MonsterTarget.Position.Y).Equals(Map.CastleLocation))))
            {
                float rate = BulletBase.Speed;
                Vector2 d = Vector2.Add(MonsterTarget.Position, Vector2.Multiply(MonsterTarget.Velocity, distance / rate)) - Position;
                Rotation = (float)Math.Atan2(d.Y, d.X);

                if (_timer <= 0)
                {
                    Bullet b = BulletBase.Clone();
                    b.Fire(this);
                    Bullets.Add(b);
                    _timer = CurrentStatistics.AttackSpeed;
                }
            }
            else
            {
                MonsterTarget.DieEvent -= new EventHandler(m_DieEvent);
                MonsterTarget = null;
            }
        }

        private Monster FindTarget()
        {
            Wave activeWave = Session._singleton.Map.ActiveWave;
            Monster m = null;
            float distance = 0, newdistance = 0;

            if (activeWave.Monsters.Count > 0)
            {
                m = activeWave.Monsters[0];
                distance = Vector2.DistanceSquared(m.Position, Position);
                for (int i = 1; i < activeWave.Monsters.Count; i++)
                {
                    newdistance = Vector2.DistanceSquared(activeWave.Monsters[i].Position, Position);

                    if (newdistance < distance)
                    {
                        distance = newdistance;
                        m = activeWave.Monsters[i];
                    }
                }
            }

            if ((Math.Sqrt(distance) <= CurrentStatistics.Radius * Session._singleton.Map.TileDimensions.X) && (m != null && m.IsActive))
            {
                m.DieEvent += new EventHandler(m_DieEvent);
                return m;
            }
            else
            {
                return null;
            }
        }

        void m_DieEvent(object sender, EventArgs e)
        {
            MonsterTarget = null;
        }

        private void UpdateMouse(GameTime gameTime, Mouse activeMouse)
        {
            bool intersect = activeMouse.Rectangle.Intersects(Rectangle);

            if (intersect || (ActiveMouse != null && (LeftState != ObjectClickedState.Normal)))
            {
                ActiveMouse = activeMouse;

                if (ActiveMouse.LeftClick && (LeftState == ObjectClickedState.Normal || LeftState == ObjectClickedState.Released))
                {
                    LeftState = ObjectClickedState.Clicked;
                    if (LeftClickEvent != null)
                    {
                        LeftClickEvent(this, EventArgs.Empty);
                    }
                }
                else if (ActiveMouse.LeftRelease && LeftState == ObjectClickedState.Clicked)
                {
                    LeftState = ObjectClickedState.Normal;
                }
            }
            else
            {
                if (ActiveMouse != null)
                {
                    ActiveMouse = null;
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Bullet item in Bullets)
            {
                item.Draw(gameTime, spriteBatch);
            }

            spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 1.0f);
        }

        public void DrawRadius(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float scale = (CurrentStatistics.Radius * Session._singleton.Map.TileDimensions.X) / (float)RadiusTexture.Width;
            spriteBatch.Draw(RadiusTexture, Position, null, Color.LightBlue, 0.0f, new Vector2(RadiusTexture.Width / 2, RadiusTexture.Height / 2), scale * 2, SpriteEffects.None, 1.0f);
        }

        #region Reader
        public class TowerReader : ContentTypeReader<Tower>
        {
            protected override Tower Read(ContentReader input, Tower existingInstance)
            {
                Tower t = new Tower();

                t.Name = input.ReadString();
                t.Description = input.ReadString();
                t.ThumbNailAsset = input.ReadString();
                t.TextureAsset = input.ReadString();
                t.RadiusTextureAsset = input.ReadString();
                t.Cost = input.ReadInt32();
                t.Level = input.ReadInt32();
                if(t.Level > 0) t.Name += " " + t.Level.ToString();
                t.MaxLevel = input.ReadInt32();
                t.InitialStatistics = input.ReadObject<TowerStatistics>();
                t.CurrentStatistics = t.InitialStatistics;
                t.UpgradeStatistics = input.ReadObject<UpgradeStatistics>();
                t.BulletAsset = input.ReadString();
                t.BulletBase = input.ContentManager.Load<Bullet>(string.Format("Towers\\Bullets\\{0}", t.BulletAsset));
                t.SellScaler = input.ReadSingle();
                t.Scale = input.ReadSingle();
                t.MapLocation = new Point(-1, -1);

                return t;
            }
        }

        #endregion

        internal Tower Clone()
        {
            Tower clone = new Tower
            {
                Name = Name,
                Description = Description,
                ThumbNailAsset = ThumbNailAsset,
                ThumbNail = ThumbNail,
                Texture = Texture,
                RadiusTextureAsset = RadiusTextureAsset,
                RadiusTexture = RadiusTexture,
                Cost = Cost,
                Level = Level,
                MaxLevel = MaxLevel,
                InitialStatistics = InitialStatistics,
                CurrentStatistics = InitialStatistics,
                UpgradeStatistics = UpgradeStatistics,
                BulletAsset = BulletAsset,
                BulletBase = BulletBase,
                SellScaler = SellScaler,
                MapLocation = MapLocation,
                Origin = Origin,
                TextureAsset = TextureAsset,
                Scale = Scale
            };

            return clone;
        }
    }
}
