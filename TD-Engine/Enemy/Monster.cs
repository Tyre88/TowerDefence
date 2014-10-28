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
    public class Monster : GameplayObject
    {
        #region Monster Reader
        public class MonsterReader : ContentTypeReader<Monster>
        {
            protected override Monster Read(ContentReader input, Monster existingInstance)
            {
                Monster result = new Monster();

                input.ReadRawObject<GameplayObject>(result as GameplayObject);
                result.Health = input.ReadInt32();
                result.MaxHealth = result.Health;
                result.TextureAsset = input.ReadString();
                result.Texture = input.ContentManager.Load<Texture2D>(string.Format("Textures\\Monsters\\{0}", result.TextureAsset));
                result.HitTextureAsset = input.ReadString();
                result.HitTexture = input.ContentManager.Load<Texture2D>(string.Format("Textures\\Monsters\\{0}", result.HitTextureAsset));
                result.HitQueName = input.ReadString();
                result.IsBoss = input.ReadBoolean();
                result.PathIndex = 0;

                return result;
            }
        }

        #endregion
        [ContentSerializerIgnore]
        public Rectangle MaxHealthRectangle
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public Rectangle HealthRectangle
        {
            get
            {
                double percentage = (double)Health / (double)MaxHealth;
                return new Rectangle(MaxHealthRectangle.X, MaxHealthRectangle.Y, (int)(MaxHealthRectangle.Width * percentage), MaxHealthRectangle.Height);
            }
        }

        [ContentSerializerIgnore]
        public Vector2 HealthBarPosition
        {
            get { return new Vector2(base.Position.X - Origin.X, (base.Position.Y - Origin.Y) - 5); }
        }

        [ContentSerializerIgnore]
        public double Percentage
        {
            get { return (double)Health / (double)MaxHealth; }
        }

        [ContentSerializerIgnore]
        public Color HealthBarColor
        {
            get;
            private set;
        }

        [ContentSerializer]
        public int Health
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public int MaxHealth
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

        [ContentSerializer]
        public string HitTextureAsset
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

        [ContentSerializerIgnore]
        public Texture2D HitTexture
        {
            get;
            private set;
        }

        [ContentSerializer]
        public string HitQueName
        {
            get;
            private set;
        }

        [ContentSerializer(Optional = true)]
        public bool IsBoss
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public Wave Wave
        {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public int PathIndex
        {
            get;
            protected set;
        }

        [ContentSerializerIgnore]
        public float DistanceToTravel
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public float DistanceTraveled
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public float Delay
        {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public bool IsActive
        {
            get { return Delay <= 0 && Status == ObjectStatus.Active; }
        }

        float _hitTimer;
        float _statisticsTimer;

        Text _hitDisplay;

        float _deltaHealth;
        float _dotTimer;
        float _dotTime;
        int _dotHealth;
        float _timer;

        public event EventHandler DieEvent;
        public event EventHandler HitEvent;

        public Monster()
        {
            DistanceToTravel = float.MinValue;
            DistanceTraveled = float.MaxValue;
        }

        public override void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 2;

            if (_dotHealth > 0 && _dotTime > 0)
            {
                UpdateDOT(gameTime);
                if (_dotTimer > _dotTime)
                {
                    _deltaHealth = 0.0f;
                    _dotHealth = 0;
                    _dotTime = 0;
                    _dotTimer = 0;
                }
            }

            if (_hitTimer > 0)
            {
                _hitTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds * Session._singleton.Speed;
                _hitDisplay.Update(gameTime);

                if (_hitTimer <= 0)
                {
                    _hitDisplay = null;
                }
            }

            if (Delay > 0)
            {
                Delay -= (float)gameTime.ElapsedGameTime.TotalSeconds * Session._singleton.Speed;
            }
            else
            {
                PathFinding shortestPath = Wave.Path.ShortestPath;

                if (PathIndex == shortestPath._path.Count && DistanceTraveled > DistanceToTravel)
                {
                    Die();
                    Wave.Remove(this);
                }
                else
                {
                    int i = PathIndex + 1;
                    if (DistanceTraveled > DistanceToTravel)
                    {
                        if (i < shortestPath._path.Count)
                        {
                            NewNodeInPath(shortestPath._path[i]);
                        }
                        else
                        {
                            NewNodeInPath(Wave.Path.EndTile);
                        }
                    }
                    else
                    {
                        DistanceTraveled += (Speed * (float)gameTime.ElapsedGameTime.TotalSeconds * Session._singleton.Speed);
                    }
                }
            }

            if (base.Texture == null)
            {
                base.Texture = this.Texture;
            }

            base.Update(gameTime);
        }

        private void UpdateDOT(GameTime gameTime)
        {
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds * Session._singleton.Speed;
            _timer += (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 2) * Session._singleton.Speed;

            _dotTimer += deltaSeconds;
            _deltaHealth += (_dotHealth * deltaSeconds) / _dotTime;

            int damage = (int)_deltaHealth;

            if ((damage > 0 && _timer >= 1000) || (_dotTimer >= _dotTime && damage > 0))
            {
                _timer = 0;
                Health -= damage;
                _deltaHealth = _deltaHealth - damage;

                if (Health <= 0)
                {
                    Wave.Remove(this);
                    Die();
                    Session._singleton.AddMoney(IsBoss ? (int)(Wave.MoneyPerKill * Wave.BossMoneyScalar) : (int)Wave.MoneyPerKill);
                    if (DieEvent != null)
                    {
                        DieEvent(this, EventArgs.Empty);
                    }
                }
                else
                {
                    _hitTimer = 0.2f;
                    _hitDisplay = new Text(damage.ToString(), new Vector2(Rectangle.Right + 3, Rectangle.Top), new Vector2(Velocity.X, -Speed));
                }
            }
        }

        internal void ApplyDOT(int damage, float time)
        {
            if (_dotHealth == 0 && _dotTime == 0)
            {
                _deltaHealth = 0;
                _dotHealth = damage;
                _dotTime = time;
                _dotTimer = 0;
                _timer = 0;
            }
        }

        public void NewNodeInPath(Tile t)
        {
            PathIndex++;
            Point tilePosition = Wave.Map.ToWorldCoordinates(t);
            Vector2 newVelocity = new Vector2(tilePosition.X - Position.X, tilePosition.Y - Position.Y);
            DistanceToTravel = newVelocity.Length();
            DistanceTraveled = 0;
            newVelocity.Normalize();
            Velocity = Vector2.Multiply(newVelocity, Speed);
            Rotation = (float)Math.Atan2(Velocity.Y, Velocity.X);
        }

        public void AddToWave(Wave w)
        {
            Wave = w;
        }

        public void Hit(Bullet bullet, Tower owner)
        {
            AudioManager._singleton.PlaySound(HitQueName);
            Health -= owner.CurrentStatistics.Damage;
            //UpdateHealthBarColor();
            if (Health <= 0)
            {
                Wave.Remove(this);
                Die();
                Session._singleton.AddMoney(IsBoss ? (int)(Wave.MoneyPerKill * Wave.BossMoneyScalar) : (int)Wave.MoneyPerKill);
                if (DieEvent != null)
                {
                    DieEvent(this, EventArgs.Empty);
                }
            }
            else
            {
                _hitTimer = 0.4f;
                _hitDisplay = new Text(owner.CurrentStatistics.Damage.ToString(), new Vector2(Rectangle.Right + 3, Rectangle.Top), new Vector2(Velocity.X, -Speed));
            }
        }

        private void UpdateHealthBarColor()
        {
            if (Percentage > 0.75)
            {
                HealthBarColor = Color.Green;
            }
            else if (Percentage > 0.50)
            {
                HealthBarColor = Color.Yellow;
            }
            else if (Percentage > 0.33)
            {
                HealthBarColor = Color.Orange;
            }
            else
            {
                HealthBarColor = Color.Red;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Delay <= 0)
            {
                Texture2D t = Texture;
                if (_hitTimer > 0)
                {
                    Texture = HitTexture;
                }

                base.Draw(gameTime, spriteBatch);

                if (_hitTimer > 0 && _hitDisplay != null)
                {
                    _hitDisplay.Draw(spriteBatch, Session._singleton.UI.Font, Session._singleton.Map.ForeColor);
                }

                Texture = t;
            }
        }

        #region Monster Writer
        [ContentTypeWriter]
        public class MonsterWriter : ContentTypeWriter<Monster>
        {
            GameplayObjectWriter gameplayObjectWriter = null;

            protected override void Initialize(ContentCompiler compiler)
            {
                gameplayObjectWriter = compiler.GetTypeWriter(typeof(GameplayObject)) as GameplayObjectWriter;

                base.Initialize(compiler);
            }

            protected override void Write(ContentWriter output, Monster value)
            {
                output.WriteRawObject<GameplayObject>(value as GameplayObject, gameplayObjectWriter);
                output.Write(value.Health);
                output.Write(value.TextureAsset);
                output.Write(value.HitTextureAsset);
                output.Write(value.HitQueName);
                output.Write(value.IsBoss);
            }

            public override string GetRuntimeReader(Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform targetPlatform)
            {
                return typeof(Monster.MonsterReader).AssemblyQualifiedName;
            }
        }

        #endregion

        public Monster Clone()
        {
            Monster clone = new Monster
            {
                Name = Name,
                Description = Description,
                Alpha = Alpha,
                Speed = Speed,
                Health = Health,
                MaxHealth = MaxHealth,
                TextureAsset = TextureAsset,
                Texture = Texture,
                HitTextureAsset = HitTextureAsset,
                HitQueName = HitQueName,
                HitTexture = HitTexture,
                IsBoss = IsBoss,
                PathIndex = -1,
                Animate = Animate,
                Rotate = Rotate,
                NFrames = NFrames,
                SFrames = SFrames,
                EFrames = EFrames,
                WFrames = WFrames,
                MaxHealthRectangle = new Rectangle((int)HealthBarPosition.X, (int)HealthBarPosition.Y, 32, 2),
                HealthBarColor = Color.Red
            };

            if (AnimationTime != 0)
            {
                clone.AnimationTime = AnimationTime;
            }
            else
            {
                clone.AnimationTime = 100;
            }

            //clone.UpdateHealthBarColor();

            return clone;
        }
    }
}
