using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using LiveCharts;
using LiveCharts.Wpf;
using System.IO;

namespace PlotCSV
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int AxisLengthX { get; set; }
        public static int MaxTextLength { get; set; }

        private SerialPort m_SerialPort;
        private string m_SerialData;
        public ChartValues<double> m_GyroValuesX { get; set; }
        public ChartValues<double> m_GyroValuesY { get; set; }
        public ChartValues<double> m_GyroValuesZ { get; set; }

        public ChartValues<double> m_PidValuesX { get; set; }
        public ChartValues<double> m_PidValuesY { get; set; }
        public ChartValues<double> m_PidValuesZ { get; set; }

        public MainWindow()
        {
            m_GyroValuesX = new ChartValues<double>();
            m_GyroValuesY = new ChartValues<double>();
            m_GyroValuesZ = new ChartValues<double>();
            m_PidValuesX = new ChartValues<double>();
            m_PidValuesY = new ChartValues<double>();
            m_PidValuesZ = new ChartValues<double>();
            DataContext = this;

            AxisLengthX = 25;
            MaxTextLength = 500;
            m_SerialData = null;

            InitializeComponent();                           
        }

        

        private void btnPortOpen_Click(object sender, RoutedEventArgs e)
        {
            if (m_SerialPort != null && m_SerialPort.IsOpen)
            {
                MessageBox.Show("Serial port is already open.");
                return;
            }

            DlgPortSetting dlgPortSetting = null;

            dlgPortSetting = new DlgPortSetting();
            dlgPortSetting.ShowDialog();

            try
            {
                m_SerialPort = new SerialPort(dlgPortSetting.ComName, dlgPortSetting.BaudRate, Parity.None, 8, StopBits.One);
                m_SerialPort.DataReceived += SerialDataReceived;
                m_SerialPort.ReadBufferSize = 8192;
                m_SerialPort.WriteBufferSize = 8192;
                m_SerialPort.Handshake = Handshake.RequestToSend;
                m_SerialPort.RtsEnable = true;
                m_SerialPort.DtrEnable = true;

                m_SerialPort.Open();            
            }
            catch (Exception ex)
            {
                string errmsg = string.Format($"에러메시지 : {ex.Message}");
                MessageBox.Show(errmsg);
            }
        }

        private void btnPortClose_Click(object sender, RoutedEventArgs e)
        {
            if( null != m_SerialPort && m_SerialPort.IsOpen)
            {
                try
                {
                    m_SerialPort.Close();
                }
                catch( IOException ex)
                {
                    string errmsg = string.Format($"에러메시지 : {ex.Message}");
                    MessageBox.Show(errmsg);
                }
            }
        }

        public void SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = m_SerialPort.ReadLine();
                Dispatcher.Invoke(() => ProcessSerialRead(data));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Read Error: {ex.Message}");
            }
        }

        private void ProcessSerialRead(string data)
        {      
            CDataParser.eRecordType recType;
            List<double> values;

            // 헤더:x,y,z,x,y,z로 구분된데이터를 파싱
            if (true == CDataParser.ParseLine(data, out recType, out values))
            {
                // pid 또는 gyro차트값에 넣어줌
                switch (recType)
                {
                    case CDataParser.eRecordType.gyro:
                        {
                            ChartValues<double>[] lists = new ChartValues<double>[] { m_GyroValuesX, m_GyroValuesY, m_GyroValuesZ };

                            const int nNumChart = 3;
                            int chartIndex = 0;

                            foreach (double value in values)
                            {
                                if (AxisLengthX == lists[chartIndex].Count)
                                    lists[chartIndex].RemoveAt(0);

                                lists[chartIndex].Add(value);

                                chartIndex = (chartIndex + 1) % nNumChart;
                            }
                        }
                        break;

                    case CDataParser.eRecordType.pid:
                        {
                            ChartValues<double>[] lists = new ChartValues<double>[] { m_PidValuesX, m_PidValuesY, m_PidValuesZ };

                            const int nNumChart = 3;
                            int chartIndex = 0;

                            foreach (double value in values)
                            {
                                if (AxisLengthX == lists[chartIndex].Count)
                                    lists[chartIndex].RemoveAt(0);

                                lists[chartIndex].Add(value);

                                chartIndex = (chartIndex + 1) % nNumChart;
                            }
                        }
                        break;

                    default:
                        break;
                }

                // serial에서 읽은 데이터 모두 TextBlock에 저장
                if(CDataParser.eRecordType.pid == recType)
                {
                    txtbox.AppendText(data + Environment.NewLine);

                    if (MaxTextLength < txtbox.Text.Length)
                    {
                        txtbox.Text = txtbox.Text.Substring(txtbox.Text.Length - MaxTextLength);
                    }

                    txtbox.ScrollToEnd();
                }
            }
        }

        private void txtBoxSerialWrite_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (0 < txtBoxSerialWrite.Text.Length)
                m_SerialData = txtBoxSerialWrite.Text;
        }

        private void btnSendSerialData_Click(object sender, RoutedEventArgs e)
        {
            if( false == m_SerialPort.IsOpen)
            {
                MessageBox.Show(string.Format("시리얼 포트가 열려있지 않습니다."));
            }
            else
            {
                if( null != m_SerialData)
                    m_SerialPort.Write(m_SerialData);
            }
        }
    }
}
