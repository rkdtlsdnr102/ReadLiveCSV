using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PlotCSV
{
    public class CSerialWriter
    {

        public enum eMsgType : byte
        {
            invalid = 0,
            recalib,
            motorspeed,
            set_anglepid,
            set_angleSpeedPid,
            max
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct Msg
        {
            eMsgType msgType;

            // motorSpeed value
            byte motorSpeedA;
            byte motorSpeedB;
            byte motorSpeedC;
            byte motorSpeedD;

            // angle pid, 1000단위
            int angleKp;
            int angleKi;
            int angleKd;

            // angleSpeed pid, 1000단위
            int angleSpeedKp;
            int angleSpeedKi;
            int angleSpeedKd;

        }

        //////////////////////////////////////////////////
        //  static function
        //////////////////////////////////////////////////
       
        // Convert struct to byte array
        static byte[] GetBytes<T>(T data) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            byte[] arr = new byte[size];
            GCHandle handle = GCHandle.Alloc(arr, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(data, handle.AddrOfPinnedObject(), false);
            }
            finally
            {
                handle.Free();
            }
            return arr;
        }


        //////////////////////////////////////////////////
        // public function
        //////////////////////////////////////////////////
        public byte[] Serialize( Msg msg)
        {
            return GetBytes(msg);
        }

    }
}
