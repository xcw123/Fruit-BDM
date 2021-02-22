using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO; 

namespace FruitSortingVtest1._0
{
    class Ini
    {
        public string inipath;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filrPath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def,StringBuilder retVal,int size,string filePath);
        public Ini(string IniPath)
        {
            inipath = IniPath;
        }

        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.inipath);
        }

        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "",temp,500,this.inipath);
            return temp.ToString();
        }
        public bool ExistIniFile()
        {
            return File.Exists(this.inipath);
        }
    }
}
