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

        private readonly string _m_RegComName = "PORT";
        private readonly string _m_RegBaudRate = "BAUDRATE";
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
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int.TryParse(TextBoxBaudRate.Text,out int baudrate);
            BaudRate = baudrate;
            ComName = TextBoxPortName.Text;

            Close();
        }

        public void SavePortSetting( string comName, int baudRate)
        {
            RegistryManager registryManager = RegistryManager.Instance;

            registryManager.SetKeyValue(_m_RegComName, comName);
            registryManager.SetKeyValue(_m_RegBaudRate, Convert.ToString(baudRate));
        }

        private void LoadLastPortSetting(object obj)
        {
            RegistryManager registryManager = RegistryManager.Instance;

            string comName, baudRate;

            if (true == registryManager.GetKeyValue(_m_RegComName, out comName) &&
                true == registryManager.GetKeyValue(_m_RegBaudRate, out baudRate))
            {
                TextBoxPortName.Text = comName;
                TextBoxBaudRate.Text = baudRate;
            }

        }
    }
}
