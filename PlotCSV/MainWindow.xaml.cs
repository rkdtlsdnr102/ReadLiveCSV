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
#if DEVELOPE_MANAGE_SERIAL_DATA
#else
        private string m_SerialData;
#endif
        private bool m_bAutoScroll;
        private CDataParser.eRecordType m_CurrentParsingRecordType;
        public ChartValues<double> m_GyroValuesX { get; set; }
        public ChartValues<double> m_GyroValuesY { get; set; }
        public ChartValues<double> m_GyroValuesZ { get; set; }

        public ChartValues<double> m_PidValuesX { get; set; }
        public ChartValues<double> m_PidValuesY { get; set; }
        public ChartValues<double> m_PidValuesZ { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            m_GyroValuesX = new ChartValues<double>();
            m_GyroValuesY = new ChartValues<double>();
            m_GyroValuesZ = new ChartValues<double>();
            m_PidValuesX = new ChartValues<double>();
            m_PidValuesY = new ChartValues<double>();
            m_PidValuesZ = new ChartValues<double>();
            DataContext = this;

            AxisLengthX = 25;
            MaxTextLength = 50000;
#if DEVELOPE_MANAGE_SERIAL_DATA
            m_SerialPort = null;
#else
            m_SerialData = null;
#endif

            // enum 값을 listview에 추가
            List<CDataParser.eRecordType> enumValues = Enum.GetValues(typeof(CDataParser.eRecordType)).Cast<CDataParser.eRecordType>().ToList();

            SerialValueListView.ItemsSource = enumValues;
            SerialValueListView.SelectedIndex = 0;

            if( null == SerialValueListView.SelectedItem)
            {
                m_CurrentParsingRecordType = CDataParser.eRecordType.invalid;
            }
            else
            {
                m_CurrentParsingRecordType = (CDataParser.eRecordType)SerialValueListView.SelectedItem;
            }

            // auto scroll 값 설정, 기본값 on
            m_bAutoScroll = true;
            SetAutoScrollLabelText(m_bAutoScroll);

            // ui 기본값 셋팅
            {
                // 모터 속도
                txtBoxMotorSpeed.Text = Convert.ToString(0);

                RegistryData regData = RegistryData.GetInstance();

                regData.Load();

                // 각도 pid
                txtBoxAnglePTerm.Text = Convert.ToString(regData.RegAnglePidP);
                txtBoxAngleITerm.Text = Convert.ToString(regData.RegAnglePidI);
                txtBoxAngleDTerm.Text = Convert.ToString(regData.RegAnglePidD);

                // 각속도 pid
                txtBoxAngleSpeedPTerm.Text = Convert.ToString(regData.RegAngleSpeedPidP);
                txtBoxAngleSpeedITerm.Text = Convert.ToString(regData.RegAngleSpeedPidI);
                txtBoxAngleSpeedDTerm.Text = Convert.ToString(regData.RegAngleSpeedPidD);
            }
            
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
                m_SerialPort.ReadBufferSize = 115200;
                m_SerialPort.WriteBufferSize = 115200;
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
                    case CDataParser.eRecordType.gyro_calib:
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
                if(m_CurrentParsingRecordType == recType)
                {
                    txtbox.AppendText(data + Environment.NewLine);

                    if (MaxTextLength < txtbox.Text.Length)
                    {
                        txtbox.Text = txtbox.Text.Substring(txtbox.Text.Length - MaxTextLength);
                    }

                    if( true == m_bAutoScroll)
                        txtbox.ScrollToEnd();
                }
            }
        }

        private void btnSaveSetting_Click(object sender, RoutedEventArgs e)
        {
            RegistryData regData = RegistryData.GetInstance();

            regData.RegAnglePidP = Convert.ToUInt32(txtBoxAnglePTerm.Text);
            regData.RegAnglePidI = Convert.ToUInt32(txtBoxAngleITerm.Text);
            regData.RegAnglePidD = Convert.ToUInt32(txtBoxAngleDTerm.Text);

            regData.RegAngleSpeedPidP = Convert.ToUInt32(txtBoxAngleSpeedPTerm.Text);
            regData.RegAngleSpeedPidI = Convert.ToUInt32(txtBoxAngleSpeedITerm.Text);
            regData.RegAngleSpeedPidD = Convert.ToUInt32(txtBoxAngleSpeedDTerm.Text);

            regData.Save();
        }

        private void btnSendSerialData_Click(object sender, RoutedEventArgs e)
        {
            if( null == m_SerialPort || false == m_SerialPort.IsOpen)
            {
                MessageBox.Show(string.Format("시리얼 포트가 열려있지 않습니다."));
            }
            else
            {
#if DEVELOPE_MANAGE_SERIAL_DATA
                CSerialMessage.Msg msg = new CSerialMessage.Msg();


                try
                {
                    msg.motorSpeed = byte.Parse(txtBoxMotorSpeed.Text);

                    msg.angleKp = uint.Parse(txtBoxAnglePTerm.Text);
                    msg.angleKi = uint.Parse(txtBoxAngleITerm.Text);
                    msg.angleKd = uint.Parse(txtBoxAngleDTerm.Text);

                    msg.angleSpeedKp = uint.Parse(txtBoxAngleSpeedPTerm.Text);
                    msg.angleSpeedKi = uint.Parse(txtBoxAngleSpeedITerm.Text);
                    msg.angleSpeedKd = uint.Parse(txtBoxAngleSpeedDTerm.Text);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(string.Format($"설정 값 세팅에 실패하였습니다 : {ex.Message}"), "설정값 문자열 -> 값 변환 실패");
                }
                

                byte[] serialized = CSerialMessage.Serialize(msg);

                m_SerialPort.Write(serialized, 0, serialized.Length);
#else
                if( null != m_SerialData)
                    m_SerialPort.Write(m_SerialData);
#endif
            }
        }


        private void SerialValueListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = SerialValueListView.SelectedItem;

            if (null != selectedItem)
            {
                m_CurrentParsingRecordType = (CDataParser.eRecordType)selectedItem;
            }
        }

        private void btnAutoScroll_Click(object sender, RoutedEventArgs e)
        {
            m_bAutoScroll = !m_bAutoScroll;

            SetAutoScrollLabelText(m_bAutoScroll);
        }

        private void SetAutoScrollLabelText( bool bAutoScroll )
        {
            if (true == bAutoScroll)
            {
                labelAutoScrollStatus.Content = "On";
            }
            else
            {
                labelAutoScrollStatus.Content = "Off";
            }
        }

        private void txtBoxMotorSpeed_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !RegexChecker.IsNumber(e.Text);
        }

        private void txtBoxAnglePTerm_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !RegexChecker.IsNumber(e.Text);
        }

        private void txtBoxAngleITerm_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !RegexChecker.IsNumber(e.Text);
        }

        private void txtBoxAngleDTerm_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !RegexChecker.IsNumber(e.Text);
        }

        private void txtBoxAngleSpeedPTerm_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !RegexChecker.IsNumber(e.Text);
        }

        private void txtBoxAngleSpeedITerm_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !RegexChecker.IsNumber(e.Text);
        }

        private void txtBoxAngleSpeedDTerm_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !RegexChecker.IsNumber(e.Text);
        }
    }
}
