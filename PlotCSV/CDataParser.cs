using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotCSV
{
    static class CDataParser
    {
        public enum eRecordType : int
        {
            invalid = 0,
            gyro,
            pid,
            max
        };

        static char[] m_Delimeter = new char[] { ':', ',' };
      
        /// <summary>
        /// 헤더:x,y,z,x,y,z,... 로 구분된 문자열을 파싱한다
        /// </summary>
        /// <param name="line">파싱할 문자열</param>
        /// <param name="recordType">문자열 유형</param>
        /// <param name="values">파싱된 데이터 리스트</param>
        /// <returns>true : 파싱 성공, false : 파싱 실패</returns>
        public static bool ParseLine( string line, out eRecordType recordType, out List<double> values )
        {
            string[] splitValues = line.Split(m_Delimeter);
            recordType = eRecordType.invalid;
            values = new List<double>();

            if (2 > splitValues.Length)
                return false;         

            switch( splitValues[0].ToLower() )
            {
                case "gyro":
                    {
                        recordType = eRecordType.gyro;                      
                    }
                    break;

                case "pid":
                    {
                        recordType = eRecordType.pid;
                    }
                    break;

                default:
                    break;
            }

            if( eRecordType.invalid != recordType )
            {
                foreach (string valueStr in splitValues.Skip(1))
                {
                    double value;
                    double.TryParse(valueStr, out value);

                    values.Add(value);
                }

                return true;
            }
            else
            {
                return false;
            }
            
        }


    }
}
