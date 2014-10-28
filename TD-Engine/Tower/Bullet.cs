using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ParticleSystemLibrary;

namespace TD_Engine
{
    public enum BulletState
    {
        InActive,
        Active,
        Hit
    }

    public enum BulletType
    {
        Normal,
        AoE,
        DoT
    }
    public class Bullet
    {
        #region Writer
        [ContentTypeWriter]
        public class BulletWriter : ContentTypeWriter<Bullet>
        {
            protected override void Write(ContentWriter output, Bullet value)
            {
                output.Write(value.Type.ToString());
                output.Write(value.AoERadius);
                output.Write(value.DoTDamagePercentage);
                output.Write(value.DoTTime);
                output.Write(value.TextureAsset);
                output.Write(value.ParticleAsset);
                output.Write(value.Speed);
                output.Write(value.DieTime);
                output.Write(value.ParticleTimer);
                output.Write(value.ParticleBirthRate);
            }

            public override string GetRuntimeReader(Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform targetPlatform)
            {
                return typeof(Bullet.BulletReader).AssemblyQualifiedName;
            }
        }

        #endregion

        #region Properties
        [ContentSerializer]
        public BulletType Type
        {
            get;
            private set;
        }

        [ContentSerializer]
        public int AoERadius
        {
            get;
            private set;
        }

        [ContentSerializer]
        public float DoTDamagePercentage
        {
            get;
            private set;
        }

        [ContentSerializer]
        public float DoTTime
        {
            get;
            private set;
        }

        [ContentSerializer]
        private string TextureAsset
        {
            get;
            set;
        }

        [ContentSerializer]
        private string ParticleAsset
        {
            get;
            set;
        }

        [ContentSerializer]
        public float Speed
        {
            get;
            private set;
        }

        [ContentSerializer]
        private float DieTime
        {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public BulletState State
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public Tower Owner
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
        public Vector2 Origin
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public Rectangle Rectangle
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public Texture2D ParticleTexture
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
        public Vector2 Velocity
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
        public float Timer
        {
            get;
            private set;
        }

        [ContentSerializer]
        public float ParticleTimer { get; private set; }

        [ContentSerializer]
        public int ParticleBirthRate { get; private set; }
        #endregion

        #region Fields
        ParticleSystem _particleSystem;
        Emitter _emitter;

        #endregion

        #region Methods
        public void Initialize(Map map, ContentManager contentManager)
        {
            Texture = contentManager.Load<Texture2D>(string.Format("Textures\\Towers\\Bullets\\{0}", TextureAsset));

            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            ParticleTexture = contentManager.Load<Texture2D>(string.Format("Textures\\Towers\\Bullets\\{0}", ParticleAsset));

            State = BulletState.InActive;
        }

        public void Update(GameTime gameTime)
        {
            if (State == BulletState.Active)
            {
                Position += Vector2.Multiply(Velocity, (float)gameTime.ElapsedGameTime.TotalSeconds * Session._singleton.Speed);

                CalculateBoundingRectangle(CalculateMatrix());

                foreach (Monster monster in Owner.Map.ActiveWave.Monsters)
                {
                    if (monster.IsActive && Rectangle.Intersects(monster.Rectangle))
                    {
                        State = BulletState.Hit;
                        Hit(monster);
                        return;
                    }
                }
            }
            else if (State == BulletState.Hit)
            {
                if (Timer <= 0)
                {
                    State = BulletState.InActive;
                }
                else
                {
                    Timer -= (float)gameTime.ElapsedGameTime.TotalSeconds * Session._singleton.Speed;
                    _particleSystem.Update(gameTime);
                }
            }
        }

        public void Fire(Tower tower)
        {
            Owner = tower;
            Position = new Vector2(tower.Position.X, tower.Position.Y);
            Velocity = new Vector2(Speed * (float)Math.Cos(tower.Rotation), Speed * (float)Math.Sin(tower.Rotation));
            Rotation = tower.Rotation;
            Rectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            State = BulletState.Active;
        }

        public void Hit(Monster monster)
        {
            Timer = DieTime;
            monster.Hit(this, Owner);

            if (Type == BulletType.AoE)
            {
                PerformAoEAttack(monster);
            }
            else if (Type == BulletType.DoT)
            {
                monster.ApplyDOT((int)(Owner.CurrentStatistics.Damage * DoTDamagePercentage), DoTTime);
            }

            _particleSystem = new ParticleSystem();
            _particleSystem.SystemTimer = ParticleTimer;
            _particleSystem.ParticleLongevity = ParticleTimer;
            _particleSystem.BirthRate = ParticleBirthRate;
            _particleSystem.TextureName = ParticleAsset;
            _particleSystem.BirthRevolutions = 0;
            _particleSystem.DeathRevolutions = 8;

            _particleSystem.Initialize();

            _emitter = new Emitter(ParticleTexture);
            _emitter.Radius = new Vector2(1, 1);
            _emitter.Position = Position;
            _emitter.Velocity = new Vector2(Velocity.X / 5.0f, Velocity.Y / 5.0f);
            _emitter.Velocity.Normalize();

            _particleSystem.Emitter = _emitter;
        }

        private void PerformAoEAttack(Monster monster)
        {
            foreach (Monster m in Owner.Map.ActiveWave.Monsters)
            {
                if (m == monster || !m.IsActive)
                {
                    continue;
                }

                float distance = (new Vector2(Position.X - m.Position.X, Position.Y - m.Position.Y)).Length();

                if (distance <= AoERadius * Session._singleton.Map.TileDimensions.X)
                {
                    m.Hit(this, Owner);
                }
            }
        }

        /// <summary>
        /// Calculates the transform matrix of the object with the origin,
        /// rotation, scale, and position.  This will need to be done every
        /// game loop because chances are the position changed.
        /// </summary>
        private Matrix CalculateMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-Origin, 0)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(1.0f) *
                Matrix.CreateTranslation(new Vector3(Position, 0));
        }

        /// <summary>
        /// Calculates the bounding rectangle of the object using the object's transform
        /// matrix to make a correct rectangle.
        /// </summary>
        private void CalculateBoundingRectangle(Matrix transform)
        {
            if (Texture != null)
            {
                Rectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
                Vector2 leftTop = Vector2.Transform(new Vector2(Rectangle.Left, Rectangle.Top), transform);
                Vector2 rightTop = Vector2.Transform(new Vector2(Rectangle.Right, Rectangle.Top), transform);
                Vector2 leftBottom = Vector2.Transform(new Vector2(Rectangle.Left, Rectangle.Bottom), transform);
                Vector2 rightBottom = Vector2.Transform(new Vector2(Rectangle.Right, Rectangle.Bottom), transform);

                Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                  Vector2.Min(leftBottom, rightBottom));
                Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                          Vector2.Max(leftBottom, rightBottom));

                Rectangle = new Rectangle((int)min.X, (int)min.Y,
                    (int)(max.X - min.X), (int)(max.Y - min.Y));
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (State == BulletState.Active)
            {
                spriteBatch.Draw(Texture, Rectangle, null, Color.White, Rotation, Origin, SpriteEffects.None, 1.0f);
            }
            else if (State == BulletState.Hit)
            {
                _particleSystem.DrawOnExistingSpritebatchCycle(gameTime, spriteBatch);
            }
        }
        #endregion

        #region Reader
        public class BulletReader : ContentTypeReader<Bullet>
        {
            protected override Bullet Read(ContentReader input, Bullet existingInstance)
            {
                Bullet b = new Bullet();
                b.Type = (BulletType)Enum.Parse(typeof(BulletType), input.ReadString());
                b.AoERadius = input.ReadInt32();
                b.DoTDamagePercentage = input.ReadSingle();
                b.DoTTime = input.ReadSingle();
                b.TextureAsset = input.ReadString();
                b.ParticleAsset = input.ReadString();
                b.Speed = input.ReadSingle();
                b.DieTime = input.ReadSingle();
                b.ParticleTimer = input.ReadSingle();
                b.ParticleBirthRate = input.ReadInt32();

                if (string.IsNullOrEmpty(b.TextureAsset)) throw new Exception("Your texture asset is blank");
                if (string.IsNullOrEmpty(b.ParticleAsset)) throw new Exception("Your particle asset is blank");
                    

                return b;
            }
        }

        #endregion

        internal Bullet Clone()
        {
            Bullet b = new Bullet();
            b.Type = Type;
            b.AoERadius = AoERadius;
            b.DoTDamagePercentage = DoTDamagePercentage;
            b.DoTTime = DoTTime;
            b.TextureAsset = TextureAsset;
            b.ParticleAsset = ParticleAsset;
            b.Speed = Speed;
            b.DieTime = DieTime;

            b.Texture = Texture;
            b.Origin = Origin;
            b.ParticleTexture = ParticleTexture;
            b.State = BulletState.InActive;

            b.ParticleBirthRate = ParticleBirthRate;
            b.ParticleTimer = ParticleTimer;

            return b;
        }
    }
}
