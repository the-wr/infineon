using System;
using System.Linq;
using Infineon.Model;

namespace Infineon
{
    // Inf3 protocol: https://docs.google.com/document/d/10jN2S5Q2AFDBKbGEPjNo5vy1aJBgKZ1aWGTEG4EzeUM/edit#
    // Inf4 protocol: https://docs.google.com/document/d/1_j1sQXE_mUbxM1kt8uC3dCofh9ERctOILo7Q009rxnY/edit#

    public class FirmwareBuilder
    {
        /*
         * EB312/Cellman
         * 02 0F 14 0B 01 03 7F 01 1A 34 5F 14 64 01 01 FF   ........4_.d..я
         * 02 FF 00 00 00 01 01 4F 02 BF 00 08 00 00 00 97   .я.....O.ї.....—
         * 
         * EB312
         * 02 0F 28 16 86 03 7F 01 
         * 1A 34 5F 14 64 01 01 FF   ..(.†...4_.d..я
         * 02 FF 00 00 00 01 01 4F 
         * 02 BF 00 03 00 00 00 3A   .я.....O.ї.....:
         * 
         * 6FET/Cellman
         * 02 0F 28 16 01 03 7F 01 1A 34 5F 14 64 01 01 FF   ..(.....4_.d..я
         * 02 FF 00 00 00 01 01 4F 02 BF 00 06 00 00 00 B8   .я.....O.ї.....ё
         * */


        private static readonly byte[] defaultInf3Data = new byte[]
        {
            0x02, 0x0F, // 0 1     0x02, 0x0f,   . .     . .
            0x28, 0x16, // 2 3     0x1d, 0x0e,   o o     . .
            0x86, 0x03, // 4 5     0x60, 0x03,   o .     . .
            0x7F, 0x01, // 6 7     0x7f, 0x00,   . o     . .
            0x1A, 0x34, // 8 9     0x28, 0x3b,   o o     . .
            0x5F, 0x14, // 10 11   0x4f, 0x0a,   o o     . o
            0x64, 0x01, // 12 13   0x96, 0x01,   o .     o .
            0x01, 0xFF, // 14 15   0x01, 0x08,   . o     . o
            0x02, 0xFF, // 16 17   0xbf, 0xf6,   o o     o o
            0x00, 0x00, // 18 19   0x00, 0x01,   . o     . o
            0x00, 0x01, // 20 21   0x00, 0x04,   . o     . o
            0x01, 0x4F, // 22 23   0x01, 0x4f,   . .     . .
            0x02, 0xBF, // 24 25   0x02, 0xbf,   . .     . .
            0x00, 0x03, // 26 27   0x00, 0x03,   . .     . .
            0x00, 0x00, // 28 29   
            0x00, 0x3A, // 30 31   
            /*                     
            0x02, 0x0f, 
            0x1d, 0x0e,
            0x60, 0x03,
            0x7f, 0x00,
            0x28, 0x3b,
            0x4f, 0x0a,
            0x96, 0x01,
            0x01, 0x08,

            0xbf, 0xf6,
            0x00, 0x01,
            0x00, 0x04,
            0x01, 0x4f,
            0x02, 0xbf,
            0x00, 0x03,
            0,
            0,
            0,
            255*/
        };

        private static readonly byte[] defaultInf4Data = new byte[]
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
            0x26, // 26
            0x1B, 0x00, 0x00, 0x00,
            0xe0, // 31
            0x00, 0x50, 0x50, 0x50, 0x50, 0x80, 0x80, 0x80, 0x80, 0x84, 0x01, 0x1c, 0x2d, 0x55, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x39,
        };

        public static byte[] BuildFirmware( IData data )
        {
            if ( data.Type.StartsWith( "Inf3" ) )
                return BuildInf3Firmware( data as InfData );

            return BuildInf4Firmware( data as InfData );
        }

        public static byte[] BuildInf3Firmware( InfData data )
        {
            var buffer = new byte[defaultInf3Data.Length];
            Array.Copy( defaultInf3Data, buffer, defaultInf3Data.Length - 1 );  // Don't need last byte

            buffer[2] = (byte)data.PhaseCurrent;
            buffer[3] = (byte)data.BatteryCurrent;
            buffer[4] = (byte)data.MinVoltage;
            buffer[5] = (byte)data.MinVoltageTolerance;

            buffer[7] = (byte)( data.ThreePosMode - 1 );
            buffer[8] = (byte)data.Speed1Percentage;
            buffer[9] = (byte)data.Speed2Percentage;
            buffer[10] = (byte)data.Speed3Percentage;

            buffer[13] = data.OnePedalMode ? (byte)0 : (byte)1;

            if ( !data.RegenEnabled ) //"EBSLevel",
                buffer[15] = 0;
            else if ( data.RegenStrength == 1 )
                buffer[15] = 4;
            else if ( data.RegenStrength == 2 )
                buffer[15] = 8;
            else
                buffer[15] = 255;

            buffer[16] = (byte)data.ReverseSpeed;
            buffer[17] = (byte)data.RegenMaxVoltage;

            buffer[19] = data.ThrottleProtection ? (byte)1 : (byte)0;

            buffer[21] = (byte)data.PASPulsesToSkip;
            buffer[22] = 1; //"DefaultSpeed",
            buffer[23] = (byte)data.Speed4Percentage;
            buffer[24] = data.HallsAngle ? (byte)0 : (byte)1;

            buffer[25] = (byte)data.PASMaxSpeed;
            buffer[27] = data.Desc.TypeByte;

            buffer[buffer.Length - 1] = buffer.Aggregate( ( a, b ) => (byte)( a ^ b ) );
            return buffer;
        }

        public static byte[] BuildInf4Firmware( InfData data )
        {
            var buffer = new byte[defaultInf4Data.Length];
            Array.Copy( defaultInf4Data, buffer, defaultInf4Data.Length - 1 );  // Don't need last byte
            // TODO

            buffer[2] = (byte)data.PhaseCurrent;
            buffer[3] = (byte)data.BatteryCurrent;
            buffer[4] = (byte)data.MinVoltage;
            buffer[5] = (byte)data.MinVoltageTolerance;
            //buffer[6] = 96; 
            buffer[7] = (byte)( data.ThreePosMode - 1 );
            buffer[8] = (byte)data.Speed1Percentage;
            buffer[9] = (byte)data.Speed2Percentage;
            buffer[10] = (byte)data.Speed3Percentage;
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
            buffer[23] = (byte)data.Speed4Percentage;
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
            buffer[37] = (byte)data.Speed1CurrentPercentage;
            buffer[38] = (byte)data.Speed2CurrentPercentage;
            buffer[39] = (byte)data.Speed3CurrentPercentage;
            buffer[40] = (byte)data.Speed4CurrentPercentage;
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
