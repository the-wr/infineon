using System;
using System.Linq;
using Infineon.Model;

namespace Infineon
{
    // See https://docs.google.com/document/d/1_j1sQXE_mUbxM1kt8uC3dCofh9ERctOILo7Q009rxnY/edit#

    public class FirmwareBuilder
    {
        private static byte[] defaultData = new byte[]
        {
            0x02, 0x0f, 0x44, 0x6d, 0x64, 0x03, 0x5c, 0x00, 0x18, 0x40, 0x53,
            0x05, // 11
            0x96, // 12
            0x01, 0x01, 0x28,
            0x01, 0xf6,
            0x00, //18
            0x00, //19
            0x15, 0x03, 0x00, 0x53, 0x03,
            0x00, // 25
            0x26, 0x00, 0x00, 0x00, 0x00,
            0xe0, // 31
            0x00, 0x50, 0x50, 0x50, 0x50, 0x80, 0x80, 0x80, 0x80, 0x84, 0x01, 0x1c, 0x2d, 0x55, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x39,
        };

        public static byte[] BuildFirmware( Data data )
        {
            var buffer = new byte[defaultData.Length];
            Array.Copy( defaultData, buffer, defaultData.Length - 1 );  // Don't need last byte

            // TODO

            buffer[2] = (byte)data.PhaseCurrent;
            buffer[3] = (byte)data.BatteryCurrent;
            buffer[4] = (byte)data.MinVoltage;
            buffer[5] = (byte)data.MinVoltageTolerance;
            //buffer[6] = 96;
            buffer[7] = (byte)data.ThreePosMode;
            buffer[8] = (byte)data.Speed1Precentage;
            buffer[9] = (byte)data.Speed2Precentage;
            buffer[10] = (byte)data.Speed3Precentage;
            //buffer[11] = 0;
            //buffer[12] = 0;
            buffer[13] = data.OnePedalMode ? (byte)0 : (byte)1;
            //buffer[14] = 0;
            buffer[15] = (byte)data.RegenStrength;
            buffer[16] = (byte)data.ReverseSpeed;
            buffer[17] = (byte)data.RegenMaxVoltage;
            //buffer[18] = 0;
            buffer[19] = data.ThrottleProtection ? (byte)1 : (byte)0;
            //buffer[20] = 0;
            buffer[21] = (byte)data.PASPulsesToSkip;
            buffer[22] = 1;
            buffer[23] = (byte)data.Speed4Precentage;
            buffer[24] = data.Desc.FirmwareType;
            //buffer[25] = 0;
            buffer[26] = (byte)data.PASMaxSpeed;
            //buffer[27] = 0;
            //buffer[28] = 0;
            //buffer[29] = 0;
            //buffer[30] = 0;
            buffer[31] = (byte)( ( data.RegenEnabled ? 1 << 6 : 0 ) +
                                 ( data.HallsAngle ? 1 << 7 : 0 ) );
            //buffer[32] = 0;
            //buffer[33] = 0;
            //buffer[34] = 0;
            //buffer[35] = 0;
            //buffer[36] = 0;
            buffer[37] = (byte)data.Speed1CurrentPrecentage;
            buffer[38] = (byte)data.Speed2CurrentPrecentage;
            buffer[39] = (byte)data.Speed3CurrentPrecentage;
            buffer[40] = (byte)data.Speed4CurrentPrecentage;
            //buffer[41] = 0;
            //buffer[42] = 0;
            //buffer[43] = 0;
            //buffer[44] = 0;
            //buffer[45] = 0;

            buffer[buffer.Length - 1] = buffer.Aggregate( ( a, b ) => (byte)( a ^ b ) );
            return buffer;
        }
    }
}
