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
    public class Wave
    {
        #region Writer
        [ContentTypeWriter]
        public class WaveWriter : ContentTypeWriter<Wave>
        {
            protected override void Write(ContentWriter output, Wave value)
            {
                output.WriteObject(value.MonsterAsset);
                output.Write(value.Title);
                output.Write(value.BossWave);
                output.Write(value.SpawnTimer);
                output.Write(value.MoneyPerKill);
                output.Write(value.BossMoneyScalar);
            }

            public override string GetRuntimeReader(Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform targetPlatform)
            {
                return typeof(Wave.WaveReader).AssemblyQualifiedName;
            }
        }


        #endregion

        [ContentSerializer]
        private List<string> MonsterAsset
        {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public List<Monster> Monsters
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        private List<Monster> AllMonsters
        {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public List<Monster> MonstersToRemove
        {
            get;
            protected set;
        }

        [ContentSerializerIgnore]
        public Map Map
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public Path Path
        {
            get { return Map.ActivePath; }
        }

        [ContentSerializer]
        public string Title
        {
            get;
            private set;
        }

        [ContentSerializer]
        public bool BossWave
        {
            get;
            private set;
        }

        [ContentSerializer]
        public float SpawnTimer
        {
            get;
            set;
        }

        [ContentSerializer]
        public int MoneyPerKill
        {
            get;
            set;
        }

        [ContentSerializer]
        public float BossMoneyScalar
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public bool IsDone
        {
            get;
            protected set;
        }

        public Wave() 
        {
            MonsterAsset = new List<string>();
            Monsters = new List<Monster>();
            AllMonsters = new List<Monster>();
            MonstersToRemove = new List<Monster>();
        }

        public void Initialize(Map map) 
        {
            Map = map;
            Reset();
        }

        public void Update(GameTime gameTime) 
        {
            if (!IsDone)
            {
                if (Monsters.Count == 0)
                {
                    IsDone = true;
                    return;
                }

                foreach (Monster item in MonstersToRemove)
                {
                    Monsters.Remove(item);
                }

                MonstersToRemove.Clear();

                foreach (Monster item in Monsters)
                {
                    item.Update(gameTime);
                }
            }
        }

        public void Remove(Monster monster) 
        {
            MonstersToRemove.Add(monster);

            if (monster.Health > 0)
            {
                Session._singleton.DecreaseHealth(monster.IsActive ? 2 : 1);
            }
        }

        public void Reset() 
        {
            Point p = Map.ToWorldCoordinates(Path.StartTile);

            if (Monsters.Count != AllMonsters.Count)
            {
                Monsters.Clear();

                foreach (Monster item in AllMonsters)
                {
                    Monsters.Add(item.Clone());
                }
            }

            for (int i = 0; i < Monsters.Count; i++)
            {
                Monsters[i].Wave = this;
                Monsters[i].Delay = SpawnTimer * (i + 1);
                Monsters[i].Position = new Vector2(p.X, p.Y);
            }

            IsDone = false;
        }

        public override string ToString()
        {
            return Title;
        }

        public Wave Clone()
        {
            Wave wave = new Wave();

            foreach (Monster item in Monsters)
            {
                wave.Monsters.Add(item.Clone());
                wave.AllMonsters.Add(item.Clone());
            }

            wave.Title = Title.Clone().ToString();
            wave.BossWave = BossWave;
            wave.SpawnTimer = SpawnTimer;
            wave.MoneyPerKill = MoneyPerKill;
            wave.BossMoneyScalar = BossMoneyScalar;

            return wave;
        }

        #region Reader

        public class WaveReader : ContentTypeReader<Wave>
        {
            protected override Wave Read(ContentReader input, Wave existingInstance)
            {
                Wave wave = new Wave();

                wave.MonsterAsset.AddRange(input.ReadObject<List<string>>());

                Monster m = null;

                foreach (string item in wave.MonsterAsset)
                {
                    m = input.ContentManager.Load<Monster>(string.Format("Monsters\\{0}", item));
                    wave.Monsters.Add(m.Clone());
                    wave.AllMonsters.Add(m.Clone());
                }

                wave.Title = input.ReadString();
                wave.BossWave = input.ReadBoolean();
                wave.SpawnTimer = input.ReadSingle();
                wave.MoneyPerKill = input.ReadInt32();
                wave.BossMoneyScalar = input.ReadSingle();

                return wave;
            }
        }


        #endregion
    }
}
