using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWork
{
    public class ProblemDevicesRNCBWork
    {
        #region Private Properties

        /// <summary>
        /// Список проблемных устройств РНКБ
        /// </summary>
        private List<ProblemDevice> problemDevices = new List<ProblemDevice>();

        /// <summary>
        /// Initialize class for work with ini file
        /// </summary>
        private INI iniClass;

        /// <summary>
        /// Initialize class for work with DB
        /// </summary>
        private WorkDB workDB;

        /// <summary>
        /// The Log Manager
        /// </summary>
        private LogManager logger;

        /// <summary>
        /// Настройки отправки почты
        /// </summary>
        private SendMailInfo sendMailInfo = new SendMailInfo();

        /// <summary>
        /// Path to the settings file ini
        /// </summary>
        private string iniSettingsPath;

        /// <summary>
        /// Root folder for the bids
        /// </summary>
        private string rootFolder;

        #endregion


        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProblemDevicesRNCBWork()
        {
            // Указываем путь к файлу логов
            logger = new LogManager(AppDomain.CurrentDomain.BaseDirectory + "logs.txt");

            // Get the path to the settings file ini AppDomain.CurrentDomain.BaseDirectory
            iniSettingsPath = AppDomain.CurrentDomain.BaseDirectory + "Configuration.ini";

            // Инициализация класса работы с ini файлами
            iniClass = new INI(iniSettingsPath);

            // Get DB connection settings string
            workDB = new WorkDB(GetDBConnectionSettings(), GetStemaxDBConfiguration());        

            rootFolder = GetRootFolder();
        }

        #endregion

        #region Main Methods

        /// <summary>
        /// Отправить проблемные устройства РНКБ
        /// </summary>
        /// <returns>Success -> true, Error -> false</returns>
        public bool SendProblemDevicesToRncb()
        {
            string error = string.Empty;

            // Получаем настройку, отправлять ли объекты в формате JSON
            var sendJson = bool.Parse(iniClass.Read("MainConfiguration", "SendProblemsJSON"));

            // Получаем настройку, отправлять ли объекты в формате Excel
            var sendExcel = bool.Parse(iniClass.Read("MainConfiguration", "SendProblemsExcel"));

            if (!sendJson && !sendExcel)
                return false;

            // Логгируем действие
            logger.WriteLog(this.ToString(), "Начало отправки проблемных устройств РНКБ (По запросу пользователя)", LogType.Information, out error);

            #region GetProblemDevices

            // Получаем список неотработанных заявок по названию сервиса (тех. обслуживание)
            logger.WriteLog(this.ToString(), $"Получаем список проблемных устройств на текущее время", LogType.Information, out error);
            problemDevices = workDB.GetProblemDevicesRNCB();

            #endregion

            #region Export To Excel

            string excelfile = "";

            if (sendExcel)
            {
                // Передаем список заявок в класс по экспорту списка в файл Excel
                ExportToExcel excel = new ExportToExcel(problemDevices, rootFolder);
                logger.WriteLog(this.ToString(), $"Экспортируем список проблемных устройств в файл Excel", LogType.Information, out error);
                excelfile = excel.ExportToProblems();
            }

            #endregion

            #region Export To Json
            
            string jsonfile = "";

            if (sendJson)
            {
                logger.WriteLog(this.ToString(), $"Экспортируем список проблемных устройств в файл Json", LogType.Information, out error);
                jsonfile = ExportToJson.Export(problemDevices, rootFolder);
            }

            #endregion

            #region SendMail

            SetSendMailInfo();

            // Получаем список сервисов
            var emails = iniClass.Read("MainConfiguration", "EmailToSendRNCB").Split(',');  

            var servicemail = new List<ServiceMail>();

            foreach (var email in emails)
            {
                servicemail.Add(new ServiceMail() { Email = email });
            }

            List<string> attachedFiles = new List<string>();

            if (sendExcel)          
                attachedFiles.Add(excelfile);

            if (sendJson)
                attachedFiles.Add(jsonfile);

            // Передаем путь файла заявок и списки электронной почты для отправки проблемных устройств
            logger.WriteLog(this.ToString(), $"Передаем список в модуль отправки проблемных устройств РНКБ по почте", LogType.Information, out error);
            SendMail sendMail = new SendMail(sendMailInfo, servicemail, attachedFiles);

            bool result = sendMail.SendMailService(true);

            if (result)
                logger.WriteLog(this.ToString(), $"Проблемные устройства РНКБ → отправлены УСПЕШНО!!!", LogType.Information, out error);
            else
                logger.WriteLog(this.ToString(), $"Проблемные устройства РНКБ  → неудача отправки проблемных устройств РНКБ по почте",
                    LogType.Warning, out error);

            #endregion

            logger.WriteLog(this.ToString(), "Окончание отправки проблемных устройств РНКБ (По запросу пользователя)", LogType.Information, out error);
            return true;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Получаем путь к файлу конфигурации Configuration.ini
        /// </summary>
        /// <returns>Gуть к файлу Configuration.ini</returns>
        public string GetIniSettingsPath() => iniSettingsPath;

        #region INI

        /// <summary>
        /// Получить из файла ini конфигурацию БД
        /// </summary>
        /// <returns>Строка подключения к БД</returns>
        public string GetDBConnectionSettings()
        {
            try
            {
                string server = iniClass.Read("DBConfiguration", "Server");
                string login = iniClass.Read("DBConfiguration", "Login");
                string password = iniClass.Read("DBConfiguration", "Password");
                string dbName = iniClass.Read("DBConfiguration", "DBName");
                string psi = iniClass.Read("DBConfiguration", "PSI");

                System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();

                if (!String.IsNullOrEmpty(server)) builder.DataSource = server;
                if (!String.IsNullOrEmpty(login)) builder.UserID = login;
                if (!String.IsNullOrEmpty(password)) builder.Password = password;
                if (!String.IsNullOrEmpty(dbName)) builder.InitialCatalog = dbName;
                if (!String.IsNullOrEmpty(psi)) builder.PersistSecurityInfo = bool.Parse(psi);

                return builder.ConnectionString;
            }
            catch (Exception ex)
            {
                logger.WriteLog(this.ToString(), $"Неудача считывания настроек БД (Файл: ″{iniClass.Path}″)", LogType.Warning, out string error);
                logger.WriteLog(this.ToString(), ex.ToString(), LogType.Error, out string error2);
                return string.Empty;
            }

        }

        /// <summary>
        /// Получить из файла ini конфигурацию БД Stemax
        /// </summary>
        /// <returns>Строка подключения к БД Стемакса</returns>
        public string GetStemaxDBConfiguration()
        {
            try
            {
                string server = iniClass.Read("StemaxDBConfiguration", "Server");
                string port = iniClass.Read("StemaxDBConfiguration", "Port");
                string login = iniClass.Read("StemaxDBConfiguration", "Login");
                string password = iniClass.Read("StemaxDBConfiguration", "Password");
                string dbName = iniClass.Read("StemaxDBConfiguration", "DBName");

                NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();

                if (!String.IsNullOrEmpty(server)) builder.Host = server;
                if (!String.IsNullOrEmpty(port)) builder.Port = int.Parse(port);
                if (!String.IsNullOrEmpty(login)) builder.UserName = login;
                if (!String.IsNullOrEmpty(password)) builder.Password = password;
                if (!String.IsNullOrEmpty(dbName)) builder.Database = dbName;

                return builder.ConnectionString;
            }
            catch (Exception ex)
            {
                logger.WriteLog(this.ToString(), $"Неудача считывания настроек БД стемакса (Файл: ″{iniClass.Path}″)", LogType.Warning, out string error);
                logger.WriteLog(this.ToString(), ex.ToString(), LogType.Error, out string error2);
                return string.Empty;
            }
        }

        /// <summary>
        /// Получаем настройки отправки по почте
        /// </summary>
        private void SetSendMailInfo() => sendMailInfo = workDB.GetSendMailInfo();

        /// <summary>
        /// Получить из файла ini путь к рабочей папке
        /// </summary>
        /// <returns>Путь к рабочей папке</returns>
        private string GetRootFolder()
        {
            string path = String.IsNullOrEmpty(iniClass.Read("ServerConfiguration", "RootFolder"))
            ? AppDomain.CurrentDomain.BaseDirectory + "RootFolder" : iniClass.Read("ServerConfiguration", "RootFolder");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (!Directory.Exists($"{path}\\Заявки"))
                Directory.CreateDirectory($"{path}\\Заявки");


            return path;
        }

        #endregion

        #endregion
    }
}
