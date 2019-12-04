using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ServerWork
{
    /// <summary>
    /// Class for work with bids
    /// </summary>
    public class TehBidsWork
    {
        #region Private Properties

        /// <summary>
        /// TehObsl List
        /// </summary>
        private List<Services> services;

        /// <summary>
        /// Список заявок
        /// </summary>
        private List<TehBids> tehbids;

        /// <summary>
        /// Initialize class for work with ini file
        /// </summary>
        private INI iniClass;

        /// <summary>
        /// Initialize class for work with DB
        /// </summary>
        private WorkDB workDB;

        /// <summary>
        /// Настройки отправки почты
        /// </summary>
        private SendMailInfo sendMailInfo;

        /// <summary>
        /// The Log Manager
        /// </summary>
        private LogManager logger;

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
        public TehBidsWork()
        {
            // Get the path to the settings file ini AppDomain.CurrentDomain.BaseDirectory
            iniSettingsPath = AppDomain.CurrentDomain.BaseDirectory + "Configuration.ini";

            // Инициализация класса работы с ini файлами
            iniClass = new INI(iniSettingsPath);

            rootFolder = GetRootFolder();

            // Указываем путь к файлу логов
            logger = new LogManager(AppDomain.CurrentDomain.BaseDirectory + "logs.txt");

            // Get DB connection settings string
            workDB = new WorkDB(GetDBConnectionSettings());

            SetSendMailInfo();
        }

        #endregion


        #region Main Methods

        /// <summary>
        /// Отправить заявки сервис. обслуживанию
        /// </summary>
        /// <returns>Success -> true, Error -> false</returns>
        public bool SendMailToServices()
        {
            string error = string.Empty;

            // Логгируем действие
            logger.WriteLog(this.ToString(), "Начало отправки заявок сервис. обслуживанию (По запросу пользователя)", LogType.Information, out error);

            // Получаем список сервисов
            SetServices();

            // Получаем список электронных адресов для каждого сервиса
            GetServiceMail();

            foreach (var service in services)
            {
                if (service.IsSendEnabled)
                {
                    logger.WriteLog(this.ToString(), $"Начало отправки заявок ‴{service.ServiceName}‴", LogType.Information, out error);

                    #region GetBids

                    // Высчитываем период, за который будут браться заявки
                    DateTime firstDate = DateTime.Now.AddDays(-service.CountDays);
                    DateTime secondDate = DateTime.Now;

                    // Получаем список неотработанных заявок по названию сервиса (тех. обслуживание)
                    logger.WriteLog(this.ToString(), $"Получаем список неотработанных заявок ‴{service.ServiceName}‴", LogType.Information, out error);
                    tehbids = workDB.GetTehBidsByService(service.ServiceName, firstDate, secondDate);

                    #endregion

                    #region ExportToExcel

                    // Передаем список заявок в класс по экспорту списка в файл Excel
                    ExportToExcel excel = new ExportToExcel(tehbids, service, rootFolder);
                    logger.WriteLog(this.ToString(), $"Экспортируем список неотработанных заявок ‴{service.ServiceName}‴", LogType.Information, out error);
                    string excelfile = excel.ExportTo();

                    #endregion

                    #region SendMail

                    // Передаем путь файла заявок и списки электронной почты для отправки заявок сервис. обслуживанию
                    logger.WriteLog(this.ToString(), $"Передаем список в модуль отправки заявок по почте ‴{service.ServiceName}‴", LogType.Information, out error);
                    SendMail sendMail = new SendMail(sendMailInfo, service.ServiceMailList, new List<string>() { excelfile });
                    bool result = sendMail.SendMailService();

                    if (result)
                        logger.WriteLog(this.ToString(), $"Заявки для ‴{service.ServiceName}‴ → отправлены УСПЕШНО!!!", LogType.Information, out error);
                    else
                        logger.WriteLog(this.ToString(), $"Заявки для ‴{service.ServiceName}‴ → неудача отправки заявок по почте",
                            LogType.Warning, out error);

                    #endregion

                    logger.WriteLog(this.ToString(), $"Окончание отправки заявок ‴{service.ServiceName}‴", LogType.Information, out error);
                }
            }

            logger.WriteLog(this.ToString(), "Окончание отправки заявок сервис. обслуживанию (По запросу пользователя)", LogType.Information, out error);
            return true;
        }

        /// <summary>
        /// Отправить заявки сервис. обслуживанию (по выбору)
        /// </summary>
        /// <param name="service">Сервис обслуживание</param>
        /// <returns>True если успешно, False если неудачно.</returns>
        public bool SendMailToServices(Services service)
        {
            string error = string.Empty;

            // Получаем список электронных адресов для сервиса
            GetServiceMail(service);

            if (service.IsSendEnabled)
            {
                logger.WriteLog(this.ToString(), $"Начало отправки заявок ‴{service.ServiceName}‴", LogType.Information, out error);

                #region GetBids

                // Высчитываем период, за который будут браться заявки
                DateTime firstDate = DateTime.Now.AddDays(-service.CountDays);
                DateTime secondDate = DateTime.Now;

                // Получаем список неотработанных заявок по названию сервиса (тех. обслуживание)
                logger.WriteLog(this.ToString(), $"Получаем список неотработанных заявок для ‴{service.ServiceName}‴", LogType.Information, out error);
                tehbids = workDB.GetTehBidsByService(service.ServiceName, firstDate, secondDate);

                #endregion

                #region ExportToExcel

                // Передаем список заявок в класс по экспорту списка в файл Excel
                ExportToExcel excel = new ExportToExcel(tehbids, service, rootFolder);
                logger.WriteLog(this.ToString(), $"Экспортируем список неотработанных заявок для ‴{service.ServiceName}‴", LogType.Information, out error);
                string excelfile = excel.ExportTo();

                #endregion

                #region SendMail

                // Передаем путь файла заявок и списки электронной почты для отправки заявок сервис. обслуживанию
                logger.WriteLog(this.ToString(), $"Передаем список в модуль отправки заявок по почте для ‴{service.ServiceName}‴", LogType.Information, out error);
                SendMail sendMail = new SendMail(sendMailInfo, service.ServiceMailList, new List<string>() { excelfile });
                bool result = sendMail.SendMailService();

                if (result)
                    logger.WriteLog(this.ToString(), $"Заявки для ‴{service.ServiceName}‴ → отправлены УСПЕШНО!!!", LogType.Information, out error);
                else
                    logger.WriteLog(this.ToString(), $"Заявки для ‴{service.ServiceName}‴ → неудача отправки по почте",
                        LogType.Warning, out error);

                #endregion

                logger.WriteLog(this.ToString(), $"Окончание отправки заявок ‴{service.ServiceName}‴", LogType.Information, out error);
            }

            return true;
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Получаем путь к файлу конфигурации Configuration.ini
        /// </summary>
        /// <returns>Gуть к файлу Configuration.ini</returns>
        public string GetIniSettingsPath() => iniSettingsPath;

        /// <summary>
        /// Получаем настройки отправки по почте
        /// </summary>
        public SendMailInfo GetSendMailInfo() => sendMailInfo;

        /// <summary>
        /// Получаем настройки отправки по почте
        /// </summary>
        private void SetSendMailInfo() => sendMailInfo = workDB.GetSendMailInfo();

        /// <summary>
        /// Получаем список сервис. обслуживания
        /// </summary>
        private void SetServices() => services = workDB.GetServicesAsync();

        /// <summary>
        /// Получаем список электронных адресов для сервис. обслуживаний
        /// </summary>
        private void GetServiceMail()
        {
            try
            {
                foreach (var service in services)
                {
                    service.ServiceMailList = workDB.GetServiceMails(service.IdService);
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog(this.ToString(), ex.ToString(), LogType.Error, out string error);
            }
        }

        /// <summary>
        /// Получаем список электронных адресов для сервис. обслуживания
        /// </summary>
        /// <param name="_service">Сервис. обслуживание</param>
        private void GetServiceMail(Services _service) => _service.ServiceMailList = workDB.GetServiceMails(_service.IdService);

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
