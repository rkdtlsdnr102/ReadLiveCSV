using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotCSV
{
    // singleton class
    public class RegistryData
    {
        // port 설정 관련
        private readonly string _m_RegNamePort = "PORT";
        private readonly string _m_RegNameBaudRate = "BAUDRATE";
        public string RegPortNum { get; set; }
        public int RegBaudRate { get; set; }

        // 각도 pid 설정 관련( 1/1000 ) 단위
        private readonly string _m_RegNameAnglePidP = "ANGLE_P";
        private readonly string _m_RegNameAnglePidI = "ANGLE_I";
        private readonly string _m_RegNameAnglePidD = "ANGLE_D";
        public uint RegAnglePidP { get; set; }
        public uint RegAnglePidI { get; set; }
        public uint RegAnglePidD { get; set; }

        // 각속도 pid 설정 관련
        private readonly string _m_RegNameAngleSpeedPidP = "ANGLE_SPEED_P";
        private readonly string _m_RegNameAngleSpeedPidI = "ANGLE_SPEED_I";
        private readonly string _m_RegNameAngleSpeedPidD = "ANGLE_SPEED_D";
        public uint RegAngleSpeedPidP { get; set; }
        public uint RegAngleSpeedPidI { get; set; }
        public uint RegAngleSpeedPidD { get; set; }

        private static RegistryData _m_Instance = null;

        private RegistryData()
        {

        }

        public static RegistryData GetInstance()
        {
            if (null == _m_Instance)
                _m_Instance = new RegistryData();

            return _m_Instance;
        }

        public void Init()
        {
            RegPortNum = "";
            RegBaudRate = 0;

            RegAnglePidP = 0;
            RegAnglePidP = 0;
            RegAnglePidP = 0;

            RegAngleSpeedPidP = 0;
            RegAngleSpeedPidI = 0;
            RegAngleSpeedPidD = 0;
        }

        public void Load()
        {
            RegistryManager registryManager = RegistryManager.Instance;                     

            //포트
            string port, baudRate;
            if (true == registryManager.GetKeyValue(_m_RegNamePort, out port))
                RegPortNum = Convert.ToString(port);
            
            if (true == registryManager.GetKeyValue(_m_RegNameBaudRate, out baudRate))
                RegBaudRate = Convert.ToInt32(baudRate);

            // 각도 pid
            string anglePidP, anglePidI, anglePidD;

            if (true == registryManager.GetKeyValue(_m_RegNameAnglePidP, out anglePidP))
                RegAnglePidP = Convert.ToUInt32(anglePidP);
            
            if (true == registryManager.GetKeyValue(_m_RegNameAnglePidI, out anglePidI))
                RegAnglePidI = Convert.ToUInt32(anglePidI);
            
            if (true == registryManager.GetKeyValue(_m_RegNameAnglePidD, out anglePidD))
                RegAnglePidD = Convert.ToUInt32(anglePidD);

            // 각속도 pid
            string angleSpeedPidP, angleSpeedPidI, angleSpeedPidD;

            if (true == registryManager.GetKeyValue(_m_RegNameAngleSpeedPidP, out angleSpeedPidP))
                RegAngleSpeedPidP = Convert.ToUInt32(angleSpeedPidP);

            if (true == registryManager.GetKeyValue(_m_RegNameAngleSpeedPidI, out angleSpeedPidI))
                RegAngleSpeedPidI = Convert.ToUInt32(angleSpeedPidI);

            if (true == registryManager.GetKeyValue(_m_RegNameAngleSpeedPidD, out angleSpeedPidD))
                RegAngleSpeedPidD = Convert.ToUInt32(angleSpeedPidD);
        }

        public void Save()
        {
            RegistryManager registryManager = RegistryManager.Instance;

            // 포트
            registryManager.SetKeyValue(_m_RegNamePort, RegPortNum);
            registryManager.SetKeyValue(_m_RegNameBaudRate, Convert.ToString(RegBaudRate));

            //각도 PID
            registryManager.SetKeyValue(_m_RegNameAnglePidP, Convert.ToString(RegAnglePidP));
            registryManager.SetKeyValue(_m_RegNameAnglePidI, Convert.ToString(RegAnglePidI));
            registryManager.SetKeyValue(_m_RegNameAnglePidD, Convert.ToString(RegAnglePidD));

            //각속도 PID
            registryManager.SetKeyValue(_m_RegNameAngleSpeedPidP, Convert.ToString(RegAngleSpeedPidP));
            registryManager.SetKeyValue(_m_RegNameAngleSpeedPidI, Convert.ToString(RegAngleSpeedPidI));
            registryManager.SetKeyValue(_m_RegNameAngleSpeedPidD, Convert.ToString(RegAngleSpeedPidD));
        }


    }
}
