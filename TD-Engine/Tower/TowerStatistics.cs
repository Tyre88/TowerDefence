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
    public class TowerStatistics
    {

        #region Writer
        [ContentTypeWriter]
        public class TowerStatisticsWriter : ContentTypeWriter<TowerStatistics>
        {
            protected override void Write(ContentWriter output, TowerStatistics value)
            {
                output.Write(value.Health);
                output.Write(value.Damage);
                output.Write(value.AttackSpeed);
                output.Write(value.Accuracy);
                output.Write(value.CritChance);
                output.Write(value.CritScalar);
                output.Write(value.Radius);
            }

            public override string GetRuntimeReader(Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform targetPlatform)
            {
                return typeof(TowerStatistics.TowerStatisticsReader).AssemblyQualifiedName;
            }
        }

        #endregion

        public int Health
        {
            get;
            set;
        }

        public int Damage
        {
            get;
            set;
        }

        public float AttackSpeed
        {
            get;
            set;
        }

        public float Accuracy
        {
            get;
            set;
        }

        public float CritChance
        {
            get;
            set;
        }

        public float CritScalar
        {
            get;
            set;
        }

        public float Radius
        {
            get;
            set;
        }

        #region Add, Subtract and Multiply

        public static TowerStatistics Add(TowerStatistics a, TowerStatistics b)
        {
            TowerStatistics result = new TowerStatistics();

            result.Health = a.Health + b.Health;
            result.Damage = a.Damage + b.Damage;
            result.AttackSpeed = a.AttackSpeed + b.AttackSpeed;
            result.Accuracy = a.Accuracy + b.Accuracy;
            result.CritChance = a.CritChance + b.CritChance;
            result.CritScalar = a.CritScalar + b.CritScalar;
            result.Radius = a.Radius + b.Radius;

            return result;
        }

        public static TowerStatistics Add(TowerStatistics a, UpgradeStatistics b)
        {
            TowerStatistics result = new TowerStatistics();

            result.Health = a.Health + b.HealthIncrease;
            result.Damage = a.Damage + b.DamageIncrease;
            result.AttackSpeed = a.AttackSpeed - b.AttackSpeedIncrease;
            result.Accuracy = a.Accuracy + b.AccuracyIncrease;
            result.CritChance = a.CritChance + b.CritChanceIncrease;
            result.CritScalar = a.CritScalar + b.CritScalarIncrease;
            result.Radius = a.Radius + b.RadiusIncrease;

            return result;
        }

        public static TowerStatistics Subtract(TowerStatistics a, TowerStatistics b)
        {
            TowerStatistics result = new TowerStatistics();

            result.Health = a.Health - b.Health;
            result.Damage = a.Damage - b.Damage;
            result.AttackSpeed = a.AttackSpeed - b.AttackSpeed;
            result.Accuracy = a.Accuracy - b.Accuracy;
            result.CritChance = a.CritChance - b.CritChance;
            result.CritScalar = a.CritScalar - b.CritScalar;
            result.Radius = a.Radius - b.Radius;

            return result;
        }

        public static TowerStatistics Multiply(TowerStatistics a, float b)
        {
            TowerStatistics result = new TowerStatistics();

            result.Health = (int)(a.Health * b);
            result.Damage = (int)(a.Damage * b);
            result.AttackSpeed = a.AttackSpeed * b;
            result.Accuracy = a.Accuracy * b;
            result.CritChance = a.CritChance * b;
            result.CritScalar = a.CritScalar * b;
            result.Radius = (int)(a.Radius * b);

            return result;
        }

        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(string.Format("[H: {0}", Health));
            sb.Append(string.Format("; D: {0}", Damage));
            sb.Append(string.Format("; FR: {0}", AttackSpeed));
            sb.AppendLine(string.Format("; A: {0}]", Accuracy));

            sb.Append(string.Format("[CC: {0}", CritChance));
            sb.Append(string.Format("; CH: {0}", CritChance));
            sb.Append(string.Format("; R: {0}]", Radius));

            return sb.ToString();
        }

        public string ToShortString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(string.Format("[H: {0}", Health));
            sb.Append(string.Format("; D: {0}", Damage));
            sb.Append(string.Format("; FR: {0}]", AttackSpeed));

            return sb.ToString();
        }

        #region Reader
        public class TowerStatisticsReader : ContentTypeReader<TowerStatistics>
        {
            protected override TowerStatistics Read(ContentReader input, TowerStatistics existingInstance)
            {
                TowerStatistics result = new TowerStatistics();

                result.Health = input.ReadInt32();
                result.Damage = input.ReadInt32();
                result.AttackSpeed = input.ReadSingle();
                result.Accuracy = input.ReadSingle();
                result.CritChance = input.ReadSingle();
                result.CritScalar = input.ReadSingle();
                result.Radius = input.ReadSingle();

                return result;
            }
        }

        #endregion

    }
}
