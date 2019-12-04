using System;
using System.Collections.Generic;
using System.IO;

namespace ServerWork
{
    public class LogManager
    {
        #region Private members

        /// <summary>
        /// Путь к файлу логов
        /// </summary>
        private string logPath;

        #endregion

        #region Constructor

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_logpath">Путь к файлу логов</param>
        public LogManager(string _logpath)
        {
            logPath = _logpath;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Запись сообщения в лог файл
        /// </summary>
        /// <param name="source">Источник ошибки (класс, метод, функция)</param>
        /// <param name="message">Сообщение ошибки</param>
        /// <param name="error">Ошибка записи лога</param>
        /// <returns>True если удачно, иначе false</returns>
        public bool WriteLog(string source, string message, LogType type, out string error)
        {
            try
            {
                // If File don't exist, then create it
                if (!File.Exists(logPath))
                {
                    FileStream fs = File.Create(logPath);
                }

                StreamWriter sw = new StreamWriter(logPath, true);

                #region Type String

                string typestring = string.Empty;

                if (type == LogType.Information)
                    typestring = "Информация";
                if (type == LogType.Warning)
                    typestring = "Предупреждение";
                if (type == LogType.Error)
                    typestring = "Ошибка";

                #endregion

                //Write text to the file
                sw.WriteLine($"[{DateTime.Now}] <{typestring}> Source: {source};{Environment.NewLine}{message};{Environment.NewLine}");

                //Close the file
                sw.Close();

                error = null;

                // Return true if Success
                return true;
            }
            catch (Exception ex)
            {
                error = $"Application: {ex.Source}{Environment.NewLine}" +
                        $"Method: {ex.TargetSite}{Environment.NewLine}" +
                        $"HResult: {ex.HResult}{Environment.NewLine}" +
                        $"Message: {ex.Message}{Environment.NewLine}";

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Error] Application: {ex.Source};");
                Console.WriteLine("--------------------------------------");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"        Method: {ex.TargetSite};");
                Console.WriteLine($"        HResult: {ex.HResult};");
                Console.WriteLine($"        Message: {ex.Message};");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("--------------------------------------");
                Console.ResetColor();

                // Return false if Error
                return false;
            }
        }

        /// <summary>
        /// Чтение лог файла
        /// </summary>
        /// <param name="error">Ошибка чтения логов</param>
        /// <returns>Список логов</returns>
        public List<string> ReadLogs(out string error)
        {
            error = null;
            List<string> loglist = new List<string>();

            try
            {
                if (File.Exists(logPath))
                {
                    StreamReader sr = new StreamReader(logPath);

                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        // Add the line to the list
                        loglist.Add(line);
                    }

                    //close the file
                    sr.Close();     
                }

                error = null;
            }
            catch (Exception ex)
            {
                error = $"Application: {ex.Source}{Environment.NewLine}" +
                        $"Method: {ex.TargetSite}{Environment.NewLine}" +
                        $"HResult: {ex.HResult}{Environment.NewLine}" +
                        $"Message: {ex.Message}{Environment.NewLine}";

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Error] Application: {ex.Source};");
                Console.WriteLine("--------------------------------------");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"        Method: {ex.TargetSite};");
                Console.WriteLine($"        HResult: {ex.HResult};");
                Console.WriteLine($"        Message: {ex.Message};");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("--------------------------------------");
                Console.ResetColor();

                // If An error return null
                return null;
            }

            // If success return loglist
            return loglist;
        }

        #endregion
    }
}
