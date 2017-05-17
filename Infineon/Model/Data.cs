using System;
using System.Data;
using System.Windows;
using System.Xml.Serialization;

namespace Infineon.Model
{
    [XmlInclude( typeof( InfData ) )]
    public class IData
    {
        public virtual string Type { get; set; }
    }

    public class DataDS
    {
        public IData Data { get; set; }
    }

    public interface IUIControls
    {
        event Action<string> ShowHelp;

        FrameworkElement FrameworkElement { get; }
        void UpdateLanguage();
    }

    public interface IControllerDesc
    {
        string Id { get; }
        string Name { get; }

        IUIControls CreateControls( IData data );
        IData CreateData();
        void PostLoad( IData data );
    }

    public class ControllerDesc : IControllerDesc
    {
        private string id;
        private string name;
        private Func<IData, IUIControls> createControls;
        private Func<IData> createData;
        private Action<IData> postLoad;

        public ControllerDesc( string id, string name, Func<IData, IUIControls> createControls,
            Func<IData> createData, Action<IData> postLoad )
        {
            this.id = id;
            this.name = name;
            this.createControls = createControls;
            this.createData = createData;
            this.postLoad = postLoad;
        }

        public string Id => id;
        public string Name => name;

        public IUIControls CreateControls( IData data )
        {
            return createControls( data );
        }

        public IData CreateData()
        {
            return createData();
        }

        public void PostLoad( IData data )
        {
            postLoad( data );
        }
    }

    public class InfDesc
    {
        public class Inf4Desc : InfDesc
        {
            public override double SpeedMultiplier => 1.0 / 0.8;

            public Inf4Desc()
            {
                BatteryCurrentLimit = 255;
            }
        }

        // Опытным путём выяснено что реальный ток контроллера превышает теоретический в 1.3 - 1.4 раза,
        // поэтому введён поправочный к-т 1.35 на BatteryCurrentMultiplier
        public static InfDesc Inf4_F6 = new Inf4Desc()
        {
            BatteryCurrentMultiplier = 1.0 / 5.10 * 1.35,
            PhaseCurrentMultiplier = 1.0 / 2.85,
            Type = "F6",
            FirmwareType = 1,
            LVCMultiplier = 1.0 / 3.285,
            LVCMaxMultiplier = 1.0 / 3.285,
            VoltageRangeLimitMax = 255,
            VoltageRangeLimitMin = 0,
        };

        public static InfDesc Inf4_F12 = new Inf4Desc()
        {
            BatteryCurrentMultiplier = 1.0 / 2.73 * 1.35,
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

        public static InfDesc Inf4_F18 = new Inf4Desc()
        {
            BatteryCurrentMultiplier = 1.0 / 1.70 * 1.35,
            PhaseCurrentMultiplier = 1.0 / 0.53,
            Type = "F18",
            FirmwareType = 5,
            LVCMultiplier = 1.0 / 3.285 * 1.5,
            LVCMaxMultiplier = 0.4592,
            LVCMaxOffset = 2.8232,
            VoltageRangeLimitMax = 212,
            VoltageRangeLimitMin = 70,
            MinVoltageRangeLimitMin = 77,
        };

        public class Inf3Desc : InfDesc
        {
            public override double SpeedMultiplier => 1.26;
            public override double PASSpeedMultiplier => 1.0 / 1.91;
        }

        public static InfDesc Inf3_F6 = new Inf3Desc()
        {
            Type = "Inf3_F6",
            TypeByte = 1,
            PhaseCurrentMultiplier = 1.0 / 1.25,
            PhaseCurrentOffset = -0.2,
            BatteryCurrentMultiplier = 1.0 / 1.399,
            BatteryCurrentOffset = 0.15,
            BatteryCurrentLimit = 140,
            LVCMultiplier = 1.0 / 3.285 * 1.25,
            VoltageRangeLimitMin = 0,   // TODO
            VoltageRangeLimitMax = 249,
            LVCMaxMultiplier = 1.0 / 3.285 * 1.25,
        };

        public static InfDesc Inf3_F12 = new Inf3Desc()
        {
            Type = "Inf3_F12",
            TypeByte = 3,
            PhaseCurrentMultiplier = 1.0 / 0.624,
            PhaseCurrentOffset = -6,
            BatteryCurrentLimit = 140,
            BatteryCurrentMultiplier = 1.0 / 0.7,
            BatteryCurrentOffset = 0.07,
            LVCMultiplier = 1.0 / 3.285 * 1.25,
            VoltageRangeLimitMin = 0,   // TODO
            VoltageRangeLimitMax = 249,
            LVCMaxMultiplier = 1.0 / 3.285 * 1.25,
        };

        public static InfDesc Inf3_F18 = new Inf3Desc()
        {
            Type = "Inf3_F18",
            // TypeByte = 7, // TODO!!!
            PhaseCurrentMultiplier = 1.0 / 0.416,
            PhaseCurrentOffset = -11.9,
            BatteryCurrentLimit = 140,
            BatteryCurrentMultiplier = 1.0 / 0.467,
            BatteryCurrentOffset = 0.03,
            LVCMultiplier = 1.0 / 3.285 * 1.25,
            VoltageRangeLimitMin = 0,   // TODO
            VoltageRangeLimitMax = 249,
            LVCMaxMultiplier = 1.0 / 3.285 * 1.25,
        };

        public string Type { get; private set; }
        public byte TypeByte { get; private set; }
        public byte FirmwareType { get; private set; }
        public double BatteryCurrentMultiplier { get; private set; }
        public double BatteryCurrentOffset { get; private set; }    // value offset (pre-multiplier)
        public int BatteryCurrentLimit { get; private set; }
        public double PhaseCurrentMultiplier { get; private set; }
        public double PhaseCurrentOffset { get; private set; }      // value offset (pre-multiplier)
        public double LVCMultiplier { get; private set; }
        public double LVCMaxMultiplier { get; private set; }
        public double LVCMaxOffset { get; private set; }
        public int VoltageRangeLimitMax { get; private set; }
        public int VoltageRangeLimitMin { get; private set; }
        public int MinVoltageRangeLimitMin { get; private set; }

        public virtual double SpeedMultiplier { get; private set; }//=> 1.0 / 0.8;
        public double TimeMultiplier => 1.0 / 10;
        public double ReverseSpeedMultiplier => 1.0 / 1.28;
        public virtual double PASSpeedMultiplier => 1.0 / 1.28;
        public double SoftStartCurrentMultiplier => 1.0 / 1.28;
        public double RecoverSpeedMultiplier => 1.0 / 1.28;
        public double CurrentMultiplierPercent => 1.0 / 1.28;
    }

    public class InfData : IData
    {
        public override string Type { get; set; }

        public int BatteryCurrent { get; set; }
        public int PhaseCurrent { get; set; }

        public int Speed1Percentage { get; set; }
        public int Speed1CurrentPercentage { get; set; }
        public int Speed2Percentage { get; set; }
        public int Speed2CurrentPercentage { get; set; }
        public int Speed3Percentage { get; set; }
        public int Speed3CurrentPercentage { get; set; }
        public int Speed4Percentage { get; set; }
        public int Speed4CurrentPercentage { get; set; }

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

        public bool SoftStartEnabled { get; set; }
        public int SoftStartTime { get; set; }
        public int SlowSpeed { get; set; }
        public int RecoverSpeed { get; set; }

        [XmlIgnore]
        public InfDesc Desc { get; set; }

        public InfData()
        {
            BatteryCurrent = 128;
            PhaseCurrent = 128;

            OnePedalMode = false;
            HallsAngle = true;
            MinVoltageTolerance = 2;
            ThreePosMode = 1;
            ThrottleProtection = true;
            PASMaxSpeed = 27;
            PASPulsesToSkip = 5;
            RegenEnabled = true;
            MinVoltage = 84;
            Speed1Percentage = 24;
            Speed1CurrentPercentage = 38;
            Speed2Percentage = 80;
            Speed2CurrentPercentage = 128;
            Speed3Percentage = 104;
            Speed3CurrentPercentage = 141;
            Speed4Percentage = 24;
            Speed4CurrentPercentage = 38;
            RegenMaxVoltage = 168;
            RegenStrength = 50;
            ReverseSpeed = 16;
        }

        public InfData( InfDesc desc ) : this()
        {
            Type = desc.Type;
            Desc = desc;
            BatteryCurrent = Desc.BatteryCurrentLimit / 2;
        }
    }
}
