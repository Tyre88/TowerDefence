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
    public class UpgradeStatistics
    {

        #region Writer
        public class UpgradeStatisticsWriter : ContentTypeWriter<UpgradeStatistics>
        {
            protected override void Write(ContentWriter output, UpgradeStatistics value)
            {
                output.Write(value.HealthIncrease);
                output.Write(value.DamageIncrease);
                output.Write(value.AttackSpeedIncrease);
                output.Write(value.AccuracyIncrease);
                output.Write(value.CritChanceIncrease);
                output.Write(value.CritScalarIncrease);
                output.Write(value.RadiusIncrease);
            }

            public override string GetRuntimeReader(Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform targetPlatform)
            {
                return typeof(UpgradeStatistics.UpgradeStatisticsWriter).AssemblyQualifiedName;
            }
        }

        #endregion

        [ContentSerializer(Optional = true)]
        public int HealthIncrease
        {
            get;
            set;
        }

        [ContentSerializer(Optional = true)]
        public int CostIncrease
        {
            get;
            set;
        }

        [ContentSerializer(Optional = true)]
        public int DamageIncrease
        {
            get;
            set;
        }

        [ContentSerializer(Optional = true)]
        public float AttackSpeedIncrease
        {
            get;
            set;
        }

        [ContentSerializer(Optional = true)]
        public float AccuracyIncrease
        {
            get;
            set;
        }

        [ContentSerializer(Optional = true)]
        public float CritChanceIncrease
        {
            get;
            set;
        }

        [ContentSerializer(Optional = true)]
        public float CritScalarIncrease
        {
            get;
            set;
        }

        [ContentSerializer(Optional = true)]
        public float RadiusIncrease
        {
            get;
            set;
        }

        #region Add, Subtract and Multiply

        public static UpgradeStatistics Add(UpgradeStatistics a, UpgradeStatistics b)
        {
            UpgradeStatistics result = new UpgradeStatistics();

            result.HealthIncrease = a.HealthIncrease + b.HealthIncrease;
            result.DamageIncrease = a.DamageIncrease + b.DamageIncrease;
            result.AttackSpeedIncrease = a.AttackSpeedIncrease + b.AttackSpeedIncrease;
            result.AccuracyIncrease = a.AccuracyIncrease + b.AccuracyIncrease;
            result.CritChanceIncrease = a.CritChanceIncrease + b.CritChanceIncrease;
            result.CritScalarIncrease = a.CritScalarIncrease + b.CritScalarIncrease;
            result.RadiusIncrease = a.RadiusIncrease + b.RadiusIncrease;

            return result;
        }

        public static UpgradeStatistics Subtract(UpgradeStatistics a, UpgradeStatistics b)
        {
            UpgradeStatistics result = new UpgradeStatistics();

            result.HealthIncrease = a.HealthIncrease - b.HealthIncrease;
            result.DamageIncrease = a.DamageIncrease - b.DamageIncrease;
            result.AttackSpeedIncrease = a.AttackSpeedIncrease - b.AttackSpeedIncrease;
            result.AccuracyIncrease = a.AccuracyIncrease - b.AccuracyIncrease;
            result.CritChanceIncrease = a.CritChanceIncrease - b.CritChanceIncrease;
            result.CritScalarIncrease = a.CritScalarIncrease - b.CritScalarIncrease;
            result.RadiusIncrease = a.RadiusIncrease - b.RadiusIncrease;

            return result;
        }

        public static UpgradeStatistics Multiply(UpgradeStatistics a, float b)
        {
            UpgradeStatistics result = new UpgradeStatistics();

            result.HealthIncrease = (int)(a.HealthIncrease * b);
            result.DamageIncrease = (int)(a.DamageIncrease * b);
            result.AttackSpeedIncrease = a.AttackSpeedIncrease * b;
            result.AccuracyIncrease = a.AccuracyIncrease * b;
            result.CritChanceIncrease = a.CritChanceIncrease * b;
            result.CritScalarIncrease = a.CritScalarIncrease * b;
            result.RadiusIncrease = (int)(a.RadiusIncrease * b);

            return result;
        }

        #endregion

        #region Reader
        public class UpgradeStatisticsReader : ContentTypeReader<UpgradeStatistics>
        {
            protected override UpgradeStatistics Read(ContentReader input, UpgradeStatistics existingInstance)
            {
                UpgradeStatistics result = new UpgradeStatistics();

                result.HealthIncrease = input.ReadInt32();
                result.DamageIncrease = input.ReadInt32();
                result.AttackSpeedIncrease = input.ReadSingle();
                result.AccuracyIncrease = input.ReadSingle();
                result.CritChanceIncrease = input.ReadSingle();
                result.CritScalarIncrease = input.ReadSingle();
                result.RadiusIncrease = input.ReadSingle();

                return result;
            }
        }

        #endregion
    }
}
