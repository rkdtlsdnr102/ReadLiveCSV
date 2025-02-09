using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace PlotLiveCSV
{
    class CSerialReader
    {
        private SerialPort _m_SerialPort;
        public CSerialReader()
        {
            _m_SerialPort = new SerialPort();
        }
    }
}
