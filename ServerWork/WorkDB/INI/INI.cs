using System.Runtime.InteropServices;
using System.Text;

namespace ServerWork
{
    /// <summary>
    /// Class for work with ini files
    /// </summary>
    public class INI
    {
        #region Initialization

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
        int size, string filePath);

        #endregion

        #region Properties

        /// <summary>
        /// Path to file
        /// </summary>
        public string Path { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filepath">Path to file ini</param>
        public INI(string filepath)
        {
            Path = filepath;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Write to file ini
        /// </summary>
        /// <param name="Section">Section parameter</param>
        /// <param name="Key">Key parameter</param>
        /// <param name="Value">Value of the key</param>
        public void Write(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.Path);
        }

        /// <summary>
        /// Read file ini
        /// </summary>
        /// <param name="Section">Section parameter</param>
        /// <param name="Key">Key parameter</param>
        /// <returns></returns>
        public string Read(string Section, string Key)
        {
            var temp = new StringBuilder(255);
            GetPrivateProfileString(Section, Key, "", temp, 255, Path);

            return temp.ToString();
        }

        #endregion
    }
}
