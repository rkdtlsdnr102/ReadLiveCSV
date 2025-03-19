using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PlotCSV
{
    public class CSerialMessage
    {

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Msg
        {

            // motorSpeed value
            public byte motorSpeed;      

            // angle pid, 1000단위
            public uint angleKp;
            public uint angleKi;
            public uint angleKd;

            // angleSpeed pid, 1000단위
            public uint angleSpeedKp;
            public uint angleSpeedKi;
            public uint angleSpeedKd;

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
        static public byte[] Serialize(Msg msg)
        {
            return GetBytes(msg);
        }

    }
}
