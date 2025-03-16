using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlotCSV
{
    public class RegistryManager
    {
        private static RegistryManager _m_Instance = null;
        private static readonly object _m_Lock = new object();
        private const string _m_RootKeyString = "PlotCsv";
        private RegistryKey _m_RootKey;       

        public bool IsRootKeyValid
        {
            get
            {
                if (null == _m_RootKey)
                    return false;
                else
                    return true;
            }
        }

        public RegistryKey RootKey{
            get
            {
                return _m_RootKey;
            }
        }

        private RegistryManager()
        {
            _m_RootKey = CreateKey(_m_RootKeyString);
        }

        public static RegistryManager Instance
        {
            get
            {
                lock (_m_Lock)
                {
                    if (null == _m_Instance)
                    {
                        _m_Instance = new RegistryManager();
                    }

                    return _m_Instance;
                }
            }
        }

        private RegistryKey CreateKey(string key)
        {
            // 기존 key가 있으면 삭제
            RegistryKey curKey = Registry.CurrentUser.OpenSubKey(key,true);
            if (null != curKey)
                return curKey;

            return Registry.CurrentUser.CreateSubKey(key, true);
        }

        public RegistryKey GetRootKey()
        {
            return _m_RootKey;
        }

        public bool SetKeyValue(string key, string value )
        {
            if(false == IsRootKeyValid)
            {
                MessageBox.Show(string.Format($"rootkey를 찾을 수 없습니다:{_m_RootKey}")) ;
                return false;
            }

            _m_RootKey.SetValue(key, value);

            return true;
        }

        public bool GetKeyValue(string key, out string value )
        {
            value = null;

            if (false == IsRootKeyValid)
                return false;

            value = (string)_m_RootKey.GetValue(key);

            if (null == value)
                return false;

            return true;
        }
    }
}
