using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerWork
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "ServerService" 
    // в коде и файле конфигурации.
    public class ServerService : IServerService
    {
        #region Private properties

        /// <summary>
        /// Инициализация класса по работе с заявками
        /// </summary>
        private TehBidsWork tbWork;

        /// <summary>
        /// Инициализация класса по работе с проблемными
        /// </summary>
        private ProblemDevicesRNCBWork pdRncbWork;

        /// <summary>
        /// Список сервис. обслуживания
        /// </summary>
        private List<Services> listServices;

        /// <summary>
        /// Инициализация класса, для работы с БД
        /// </summary>
        private WorkDB workDB;

        /// <summary>
        /// Создание логгера
        /// </summary>
        private LogManager logger;

        /// <summary>
        /// Строка подключения к БД
        /// </summary>
        private string connectionString;

        /// <summary>
        /// Происходит отправка заявок
        /// </summary>
        private bool IsSend = false;

        /// <summary>
        /// Происходит отправка проблемных устройств РНКБ
        /// </summary>
        private bool IsSendRNCB = false; 

        /// <summary>
        /// Путь к файлу, который содержит IP адреса клиентов сервиса
        /// </summary>
        private string PathToFileIPClients = AppDomain.CurrentDomain.BaseDirectory + "IPClients.txt";

        #endregion

        #region Constructor

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public ServerService()
        {
            // Указываем путь к файлу логов
            logger = new LogManager(AppDomain.CurrentDomain.BaseDirectory + "logs.txt");

            // Инициализация класса работы с заявками
            tbWork = new TehBidsWork();

            // Инициализация класса работы с проблемными устройствами РНКБ
            pdRncbWork = new ProblemDevicesRNCBWork();

            // Получение настроек подключения к БД
            connectionString = tbWork.GetDBConnectionSettings();

            // Инициализация класса работы с БД
            workDB = new WorkDB(connectionString);

            // Получение списка сервисов (тех. обслуживание)
            listServices = workDB.GetServicesAsync(true);

            // Получение списка времени отправки заявок для каждого сервиса
            SetServiceSendTimes();

            // Запуск таймера для проверки времени отправки
            DoWorkPollingTask();
        }

        /// <summary>
        /// Конструктор для отправки заявок по требованию
        /// </summary>
        public ServerService(bool state)
        {
            // Указываем путь к файлу логов
            logger = new LogManager(AppDomain.CurrentDomain.BaseDirectory + "logs.txt");

            // Инициализация класса работы с заявками
            tbWork = new TehBidsWork();

            // Инициализация класса работы с проблемными устройствами РНКБ
            pdRncbWork = new ProblemDevicesRNCBWork();

            // Получение настроек подключения к БД
            connectionString = tbWork.GetDBConnectionSettings();

            // Инициализация класса работы с БД
            workDB = new WorkDB(connectionString);

            // Получение списка сервисов (тех. обслуживание)
            listServices = workDB.GetServicesAsync(true);

            // Получение списка времени отправки заявок для каждого сервиса
            SetServiceSendTimes();
        }

        #endregion

        #region Operation contract Methods

        /// <summary>
        /// Мониторинг времени для отправки заявок по тайиеру
        /// </summary>
        /// <returns>async Task</returns>
        public void Monitoring()
        {
            string nowtime = DateTime.Now.ToShortTimeString();

            INI ini = new INI(AppDomain.CurrentDomain.BaseDirectory + "Configuration.ini");

            var timesRncb = ini.Read("MainConfiguration", "TimeToSendRNCB").Split(',');

            foreach (var time in timesRncb)
            {
                // Если время (сейчас, тип 15:00) равно времени отправки проблемных устройств РНКБ
                if (nowtime == time)
                {
                    IsSend = true;

                    // Находим и отправляем проблемные устройства РНКБ
                    pdRncbWork.SendProblemDevicesToRncb();

                    IsSend = false;
                }
            }       

            // Проходим по списку сервис. обслуживания
            foreach (var service in listServices)
            {
                // Проходим по списку времен отправки заявок
                foreach (var time in service.ServiceSendTimeList)
                {
                    // Если время (сейчас, тип 15:00) равно времени отправки заявок сервис. обслуживанию
                    if (nowtime == time.Time.ToShortTimeString())
                    {
                        IsSend = true;

                        // Отправляем заявки сервис. обслуживанию
                        tbWork.SendMailToServices(service);

                        IsSend = false;
                    }
                }
            }
        }

        /// <summary>
        /// Отправка заявок сервис. обслуживанию по требованию
        /// </summary>
        public void SendServicesBidsOnRequest()
        {
            // Отправляем заявки сервис. обслуживанию
            tbWork.SendMailToServices();
        }

        /// <summary>
        /// Отправка проблемных устройств РНКБ по требованию
        /// </summary>
        public void SendServicesRNCBOnRequest()
        {
            // Находим и отправляем проблемные устройства РНКБ
            pdRncbWork.SendProblemDevicesToRncb();
        }

        public bool GetStatusMonitoring() => IsSend;

        public bool GetStatusMonitoringRNCB() => IsSendRNCB;

        public string GetIPAddressClient()
        {
            //OperationContext oOperationContext = OperationContext.Current;
            //MessageProperties oMessageProperties = oOperationContext.IncomingMessageProperties;
            //RemoteEndpointMessageProperty oRemoteEndpointMessageProperty = (RemoteEndpointMessageProperty)oMessageProperties[RemoteEndpointMessageProperty.Name];

            //string szAddress = oRemoteEndpointMessageProperty.Address;
            //int nPort = oRemoteEndpointMessageProperty.Port;

            //return $"{szAddress}:{nPort}";
            return string.Empty;
        }

        #endregion

        #region Private Methods    

        /// <summary>
        /// Запуск таймера на 60 секунд
        /// </summary>
        private void DoWorkPollingTask()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    // do the work in the loop
                    Monitoring();

                    // don't run again for at least 200 milliseconds
                    await Task.Delay(60000);
                }
            });
        }        

        /// <summary>
        /// Получение списка времен отправки заявок
        /// </summary>
        private void SetServiceSendTimes()
        {
            foreach (var service in listServices)
            {
                service.ServiceSendTimeList = workDB.GetServiceSendTimes(service.IdService);
            }
        }

        #endregion
    }
}
