using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO.Ports;

namespace PlotCSV
{
    /// <summary>
    /// DlgPortSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DlgPortSetting : Window
    {
        public int BaudRate { get; set; }
        public string ComName { get; set; }

#if DEVELOPE_MANAGE_REGISTRY

#else
        private readonly string _m_RegComName = "PORT";
        private readonly string _m_RegBaudRate = "BAUDRATE";
#endif
        public ICommand m_cmdLoadLastPortSetting
        {
            get
            {
                return new RelayCommand(LoadLastPortSetting);
            }
        }

        public DlgPortSetting()
        {
            InitializeComponent();                    
            DataContext = this;

        }

        private void TextBoxBaudRate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !RegexChecker.IsNumber(e.Text);
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int.TryParse(TextBoxBaudRate.Text,out int baudrate);
            BaudRate = baudrate;
            ComName = TextBoxPortName.Text;

            // 포트 값 저장
            SavePortSetting(ComName, BaudRate);

            Close();
        }

        private void SavePortSetting( string comName, int baudRate)
        {
#if DEVELOPE_MANAGE_REGISTRY
            RegistryData regData = RegistryData.GetInstance();

            regData.RegPortNum = comName;
            regData.RegBaudRate = baudRate;

            regData.Save();
#else
            RegistryManager registryManager = RegistryManager.Instance;

            registryManager.SetKeyValue(_m_RegComName, comName);
            registryManager.SetKeyValue(_m_RegBaudRate, Convert.ToString(baudRate));
#endif
        }

        private void LoadLastPortSetting(object obj)
        {
#if DEVELOPE_MANAGE_REGISTRY
            RegistryData regData = RegistryData.GetInstance();

            regData.Load();

            TextBoxPortName.Text = regData.RegPortNum;
            TextBoxBaudRate.Text = Convert.ToString(regData.RegBaudRate);
#else
            RegistryManager registryManager = RegistryManager.Instance;

            string comName, baudRate;

            if (true == registryManager.GetKeyValue(_m_RegComName, out comName) &&
                true == registryManager.GetKeyValue(_m_RegBaudRate, out baudRate))
            {
                TextBoxPortName.Text = comName;
                TextBoxBaudRate.Text = baudRate;
            }
#endif

        }
    }
}
