using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Infineon.Model
{
    public interface IFirmware
    {
        byte[] GetFirmwareBytes();
    }

    public class InfineonDesc
    {
        public static InfineonDesc F6 = new InfineonDesc()
        {
            BatteryCurrentMultiplier = 1.0 / 5.10,
            PhaseCurrentMultiplier = 1.0 / 2.85,
            Type = "F6",
        };

        public static InfineonDesc F12 = new InfineonDesc()
        {
            BatteryCurrentMultiplier = 1.0 / 2.73,
            PhaseCurrentMultiplier = 1.0 / 1.20,
            Type = "F12",
        };

        public static InfineonDesc F18 = new InfineonDesc()
        {
            BatteryCurrentMultiplier = 1.0 / 1.70,
            PhaseCurrentMultiplier = 1.0 / 0.53,
            Type = "F18",
        };

        public string Type { get; private set; }
        public double BatteryCurrentMultiplier { get; private set; }
        public double PhaseCurrentMultiplier { get; private set; }

        public double LVCMultiplier => 1.0 / 3.285;
        public double SLMultiplier => 1.0 / 0.96;
        public double SpeedMultiplier => 1.0 / 0.8;
        public double TimeMultiplier => 1.0 / 10;
        public double ReverseSpeedMultiplier => 1.0 / 1.28;
        public double PASSpeedMultiplier => 1.0 / 1.28;
        public double SoftStartCurrentMultiplier => 1.0 / 1.28;
        public double LVCBatteryCurrentMultiplier => 1.0 / 13.18;
        public double RecoverSpeedMultiplier => 1.0 / 1.28;
    }

    public class Data: IFirmware
    {
        public int BatteryCurrent { get; set; }
        public int PhaseCurrent { get; set; }

        public int Speed1Precentage { get; set; }
        public int Speed2Precentage { get; set; }
        public int Speed3Precentage { get; set; }
        public int Speed4Precentage { get; set; }

        public int MinVoltage { get; set; }
        public int MinVoltageTolerance { get; set; }

        public int RegenStrength { get; set; }
        public int RegenMaxVoltage { get; set; }

        public string Type { get; set; }

        [XmlIgnore]
        public InfineonDesc Desc { get; set; }

        public byte[] GetFirmwareBytes()
        {
            return new byte[0];
        }
    }
}
