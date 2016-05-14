using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;

namespace ASCII
{
    [Serializable]
    internal class Tools
    {
        public Tools() { }

        private static string defaultCurrentDirectory = null;
        private string currentDirectory = null;

        public static string DefaultCurrentDirectory
        {
            get
            {
                if (defaultCurrentDirectory == null)
                {

                    try
                    {
                        defaultCurrentDirectory = HttpContext.Current.Server.MapPath(".");
                    }
                    catch
                    {

                        try
                        {
                            string asmName = Assembly.GetEntryAssembly().Location;
                            defaultCurrentDirectory = (new FileInfo(asmName)).DirectoryName;
                        }
                        catch
                        {
                            try
                            {
                                string asmName = Assembly.GetExecutingAssembly().Location;
                                defaultCurrentDirectory = (new FileInfo(asmName)).DirectoryName;
                            }
                            catch
                            {
                                try
                                {
                                    defaultCurrentDirectory = Directory.GetCurrentDirectory();
                                }
                                catch
                                {
                                    defaultCurrentDirectory = ".";
                                }
                            }
                        }

                    }

                }

                return defaultCurrentDirectory;
            }
        }

        public string CurrentDirectory
        {
            get
            {
                if (currentDirectory == null) currentDirectory = DefaultCurrentDirectory;
                return currentDirectory;
            }

            set
            {
                if (value != null)
                {
                    value = value.Trim().TrimEnd('\\');
                    if (value != "") currentDirectory = value;
                }
            }
        }

        public string GetPathName(string relPath)
        {
            return GetPathName(CurrentDirectory, relPath);
        }
        public static string GetPathName(string rootPath, string relPath)
        {
            if (relPath == null) return (null);
            string path = relPath.Trim();

            if (path == ".")
                path = rootPath;
            else if (path == "..")
                path = rootPath + "\\..";
            else if (path.StartsWith(".\\"))
                path = rootPath + path.Substring(1);
            else if (path.StartsWith("..\\"))
                path = rootPath + "\\" + path;

            return path;
        }

        public static string LoadConfig(string keyName, string defaultValue)
        {
            return LoadConfig(keyName, defaultValue, true);
        }
        public static string LoadConfig(string keyName, string defaultValue, bool allowEmptyString)
        {
            try
            {
                string val = ConfigurationSettings.AppSettings[keyName];
                
                if ((val == "") && !allowEmptyString) 
                    return (defaultValue);
                
                return (val == null ? defaultValue : val);
            }
            catch
            {
                return defaultValue;
            }
        }
        public static int LoadConfig(string keyName, int defaultValue)
        {
            try
            {
                return Int32.Parse(ConfigurationSettings.AppSettings[keyName]);
            }
            catch
            {
                return defaultValue;
            }
        }
        public static uint LoadConfig(string keyName, uint defaultValue)
        {
            try
            {
                return UInt32.Parse(ConfigurationSettings.AppSettings[keyName]);
            }
            catch
            {
                return defaultValue;
            }
        }
        public static long LoadConfig(string keyName, long defaultValue)
        {
            try
            {
                return Int64.Parse(ConfigurationSettings.AppSettings[keyName]);
            }
            catch
            {
                return defaultValue;
            }
        }
        public static ulong LoadConfig(string keyName, ulong defaultValue)
        {
            try
            {
                return UInt64.Parse(ConfigurationSettings.AppSettings[keyName]);
            }
            catch
            {
                return defaultValue;
            }
        }
        public static float LoadConfig(string keyName, float defaultValue)
        {
            try
            {
                return Single.Parse(ConfigurationSettings.AppSettings[keyName]);
            }
            catch
            {
                return defaultValue;
            }
        }
        public static double LoadConfig(string keyName, double defaultValue)
        {
            try
            {
                return Double.Parse(ConfigurationSettings.AppSettings[keyName]);
            }
            catch
            {
                return defaultValue;
            }
        }
        public static bool LoadConfig(string keyName, bool defaultValue)
        {
            try
            {
                return Boolean.Parse(ConfigurationSettings.AppSettings[keyName]);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
