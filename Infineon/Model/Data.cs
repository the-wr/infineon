using System.Data;
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
            FirmwareType = 1,
            LVCMultiplier = 1.0 / 3.285,
            LVCMaxMultiplier = 1.0 / 3.285,
            VoltageRangeLimitMax = 255,
            VoltageRangeLimitMin = 0,
    };

        public static InfineonDesc F12 = new InfineonDesc()
        {
            BatteryCurrentMultiplier = 1.0 / 2.73,
            PhaseCurrentMultiplier = 1.0 / 1.20,
            Type = "F12",
            FirmwareType = 3,
            LVCMultiplier = 1.0 / 3.285 * 1.5,
            LVCMaxMultiplier = 0.4592,
            LVCMaxOffset = 2.8232,
            VoltageRangeLimitMax = 212,
            VoltageRangeLimitMin = 70,
            MinVoltageRangeLimitMin = 77
        };

        public static InfineonDesc F18 = new InfineonDesc()
        {
            BatteryCurrentMultiplier = 1.0 / 1.70,
            PhaseCurrentMultiplier = 1.0 / 0.53,
            Type = "F18",
            FirmwareType = 5,
            LVCMultiplier = 1.0 / 3.285 * 1.5,
            LVCMaxMultiplier = 0.4592,
            LVCMaxOffset = 2.8232,
            VoltageRangeLimitMax = 212,
            VoltageRangeLimitMin = 70,
            MinVoltageRangeLimitMin = 77
        };

        public string Type { get; private set; }
        public byte FirmwareType { get; private set; }
        public double BatteryCurrentMultiplier { get; private set; }
        public double PhaseCurrentMultiplier { get; private set; }
        public double LVCMultiplier { get; private set; }
        public double LVCMaxMultiplier { get; private set; }
        public double LVCMaxOffset { get; private set; }
        public int VoltageRangeLimitMax { get; private set; }
        public int VoltageRangeLimitMin { get; private set; }
        public int MinVoltageRangeLimitMin { get; private set; }

        public double SLMultiplier => 1.0 / 0.96;
        public double SpeedMultiplier => 1.0 / 0.8;
        public double TimeMultiplier => 1.0 / 10;
        public double ReverseSpeedMultiplier => 1.0 / 1.28;
        public double PASSpeedMultiplier => 1.0 / 1.28;
        public double SoftStartCurrentMultiplier => 1.0 / 1.28;
        public double LVCBatteryCurrentMultiplier => 1.0 / 13.18;
        public double RecoverSpeedMultiplier => 1.0 / 1.28;
        public double CurrentMultiplierPercent => 1.0 / 1.28;
    }

    public class Data: IFirmware
    {
        public string Type { get; set; }

        public int BatteryCurrent { get; set; }
        public int PhaseCurrent { get; set; }

        public int Speed1Precentage { get; set; }
        public int Speed1CurrentPrecentage { get; set; }
        public int Speed2Precentage { get; set; }
        public int Speed2CurrentPrecentage { get; set; }
        public int Speed3Precentage { get; set; }
        public int Speed3CurrentPrecentage { get; set; }
        public int Speed4Precentage { get; set; }
        public int Speed4CurrentPrecentage { get; set; }

        public int MinVoltage { get; set; }
        public int MinVoltageTolerance { get; set; }

        public bool RegenEnabled { get; set; }
        public int RegenStrength { get; set; }
        public int RegenMaxVoltage { get; set; }

        public int PASMaxSpeed { get; set; }
        public int PASPulsesToSkip { get; set; }

        public int ThreePosMode { get; set; }
        public int ReverseSpeed { get; set; }
        public bool OnePedalMode { get; set; }
        public bool ThrottleProtection { get; set; }
        public bool HallsAngle { get; set; }

        [XmlIgnore]
        public InfineonDesc Desc { get; set; }

        public byte[] GetFirmwareBytes()
        {
            return new byte[0];
        }
    }
}
