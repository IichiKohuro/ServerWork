using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ServerWork
{
    [Flags]
    /// <summary>
    /// Состояния объектов ПЦН мираж (Стемакс)
    /// </summary>
    public enum ObjectStates : uint
    {
        /// <summary>
        /// Объект на тех. обслуживании (31 бит)
        /// </summary>
        Service = 0x80000000,

        /// <summary>
        /// Потеря связи с объектом (30 бит)
        /// </summary>
        LostConnection = 0x40000000,

        /// <summary>
        /// Объект деактивирован (29 бит)
        /// </summary>
        Deactivate = 0x20000000,

        /// <summary>
        /// Имеются не обработанные события (28 бит)
        /// </summary>
        NotProcessed = 0x10000000,

        /// <summary>
        /// Неизвестно (27 бит)
        /// </summary>
        State27 = 0x8000000,

        /// <summary>
        /// Индикатор соединения по каналу ETHERNET (26 бит)
        /// </summary>
        IndicatorEthernetConnection = 0x4000000,

        /// <summary>
        /// Индикатор соединения по каналу GPRS 2 (25 бит)
        /// </summary>
        IndicatorGPRS2Connection = 0x2000000,

        /// <summary>
        /// Индикатор соединения по каналу GPRS 1 (24 бит)
        /// </summary>
        IndicatorGPRS1Connection = 0x1000000,

        /// <summary>
        /// Неизвестно (23 бит)
        /// </summary>
        State23 = 0x800000,

        /// <summary>
        /// Неисправность ETHERNET канала (22 бит)
        /// </summary>
        MalfunctionEthernet = 0x400000,

        /// <summary>
        /// Неисправность канала GPRS2 (21 бит)
        /// </summary>
        MalfunctionGPRS2 = 0x200000,

        /// <summary>
        /// Неисправность канала GPRS1 (20 бит)
        /// </summary>
        MalfunctionGPRS1 = 0x100000,

        /// <summary>
        /// Неизвестно (19 бит)
        /// </summary>
        State19 = 0x80000,

        /// <summary>
        /// Нарушение расписания (18 бит)
        /// </summary>
        ScheduleViolation = 0x40000,

        /// <summary>
        /// Авария TCP-IP (17 бит)
        /// </summary>
        CrashTcpIP = 0x20000,

        /// <summary>
        /// Объект снят/поставлен (16 бит)
        /// </summary>
        ModeObject = 0x10000,

        /// <summary>
        /// Подавление объекта (15 бит)
        /// </summary>
        SuppressionObject = 0x8000,

        /// <summary>
        /// Потеря активности объекта (14 бит)
        /// </summary>
        LostActivityObject = 0x4000,

        /// <summary>
        /// Неизвестно (13 бит)
        /// </summary>
        State13 = 0x2000,

        /// <summary>
        /// Авария Слот (12 бит)
        /// </summary>
        CrashSlot = 0x1000,

        /// <summary>
        /// Авария Тампер (11 бит)
        /// </summary>
        CrashTamper = 0x800,

        /// <summary>
        /// Авария RS-480 (10 бит)
        /// </summary>
        CrashRS480 = 0x400,

        /// <summary>
        /// Авария 220В (9 бит)
        /// </summary>
        Crash220V = 0x200,

        /// <summary>
        /// Авария АКБ (8 бит)
        /// </summary>
        CrashAKB = 0x100,

        /// <summary>
        /// Неизвестно (7 бит)
        /// </summary>
        State7 = 0x80,

        /// <summary>
        /// Неизвестно (6 бит)
        /// </summary>
        State6 = 0x40,

        /// <summary>
        /// Неизвестно (5 бит)
        /// </summary>
        State5 = 0x20,

        /// <summary>
        /// Неизвестно (4 бит)
        /// </summary>
        State4 = 0x10,

        /// <summary>
        /// Неизвестно (3 бит)
        /// </summary>
        State3 = 0x8,

        /// <summary>
        /// Неизвестно (2 бит)
        /// </summary>
        State2 = 0x4,

        /// <summary>
        /// Неизвестно (1 бит)
        /// </summary>
        State1 = 0x2,

        /// <summary>
        /// Неизвестно (0 бит)
        /// </summary>
        State0 = 0x1,
    }

    /// <summary>
    /// Класс для работы с базой данных
    /// </summary>
    public class WorkDB
    {
        #region Private

        /// <summary>
        /// Подключение к БД
        /// </summary>
        private SqlConnection sqlconn;

        /// <summary>
        /// Подключение к БД Миража/Стемакса
        /// </summary>
        private NpgsqlConnection npgsqlconn;

        /// <summary>
        /// Строка подключения к Базе Данных
        /// </summary>
        private string sqlconnString;

        /// <summary>
        /// Строка подключения к Базе Данных Мираж/Стемакс
        /// </summary>
        private string npgsqlconnString;

        /// <summary>
        /// Инициализация логгера
        /// </summary>
        private LogManager logger;

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_sqlconnString">Строка подключения к Базе Данных</param>
        public WorkDB(string _sqlconnString, string _npgsqlconnString = "")
        {
            // Указываем путь к файлу логов
            logger = new LogManager(AppDomain.CurrentDomain.BaseDirectory + "logs.txt");

            sqlconnString = _sqlconnString;
            npgsqlconnString = _npgsqlconnString;

            sqlconn = new SqlConnection();
            sqlconn.ConnectionString = sqlconnString;

            npgsqlconn = new NpgsqlConnection();
            npgsqlconn.ConnectionString = npgsqlconnString;
        }

        #endregion

        #region Методы

        #region BIDS

        /// <summary>
        /// Метод для получения всех неотработанных заявок
        /// </summary>
        /// <returns>Список заявок</returns>
        public List<TehBids> GetAllTehBids(DateTime firstDate, DateTime secondDate, int state = 0)
        {
            List<TehBids> _tehbids = new List<TehBids>();

            try
            {
                string query = "SELECT Prefix, ZP_nomer, ZP_name, ZP_adres, ZP_char, ZP_info, ZP_who, ZP_datetime FROM [Journals].[dbo].[ZP_bid] " +
                              $"WHERE check_state = '{state}' AND ZP_datetime BETWEEN '{firstDate}' AND '{secondDate}' ORDER BY ZP_datetime";

                sqlconn.Open();

                SqlCommand sqlcommand = new SqlCommand(query, sqlconn);
                SqlDataReader reader = sqlcommand.ExecuteReader();

                if (reader.HasRows == true)
                {
                    // Считываем данные
                    while (reader.Read())
                    {
                        TehBids tehbid = new TehBids()
                        {
                            Prefix = reader[0].ToString().Trim(),
                            NumObj = reader[1].ToString().Trim(),
                            NameObj = reader[2].ToString().Trim(),
                            AddressObj = reader[3].ToString().Trim(),
                            CharacteristicObj = reader[4].ToString().Trim(),
                            DopInfo = reader[5].ToString().Trim(),
                            FromWho = reader[6].ToString().Trim(),
                            DatetimeBid = DateTime.Parse(reader[7].ToString())
                        };

                        _tehbids.Add(tehbid);
                    }  
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog(this.ToString(), ex.ToString(), LogType.Error, out string f);
            }
            finally
            {
                // Если подключение открыто, то закрыть
                if (sqlconn.State == ConnectionState.Open)
                    sqlconn.Close();
            }

            return _tehbids;
        }

        /// <summary>
        /// Метод для получения списка неотработанных заявок по типу тех. обслуживания
        /// </summary>
        /// <param name="_service">Тип тех. обслуживания</param>
        /// <returns>Список заявок</returns>
        public List<TehBids> GetTehBidsByService(string _service, DateTime firstDate, DateTime secondDate, int state = 0)
        {
            List<TehBids> _tehbids = new List<TehBids>();

            try
            {
                string query = "SELECT Prefix, ZP_nomer, ZP_name, ZP_adres, ZP_char, ZP_info, ZP_who, ZP_datetime FROM [Journals].[dbo].[ZP_bid] " +
                              $"WHERE check_state = '{state}' AND tehobsl LIKE '%{_service}%'" +
                              $"AND ZP_datetime BETWEEN '{firstDate}' AND '{secondDate}' ORDER BY ZP_datetime";

                sqlconn.Open();

                SqlCommand sqlcommand = new SqlCommand(query, sqlconn);
                SqlDataReader reader = sqlcommand.ExecuteReader();

                if (reader.HasRows == true)
                {
                    // Считываем данные
                    while (reader.Read())
                    {
                        TehBids tehbid = new TehBids()
                        {
                            Prefix = reader[0].ToString().Trim(),
                            NumObj = reader[1].ToString(),
                            NameObj = reader[2].ToString().Trim(),
                            AddressObj = reader[3].ToString().Trim(),
                            CharacteristicObj = reader[4].ToString().Trim(),
                            DopInfo = reader[5].ToString().Trim(),
                            FromWho = reader[6].ToString().Trim(),
                            DatetimeBid = DateTime.Parse(reader[7].ToString())
                        };

                        _tehbids.Add(tehbid);
                    }
                }

                logger.WriteLog(this.ToString(), $"Получили список неотработанных заявок для ‴{_service}‴ ", LogType.Information, out string error);
            }
            catch (Exception ex)
            {
                logger.WriteLog(this.ToString(), $"Неудача получения списка неотработанных заявок для ‴{_service}‴ ", LogType.Warning, out string error2);
                logger.WriteLog(this.ToString(), ex.ToString(), LogType.Error, out string f);
            }
            finally
            {
                // Если подключение открыто, то закрыть
                if (sqlconn.State == ConnectionState.Open)
                    sqlconn.Close();
            }

            return _tehbids;
        }

        #endregion

        #region Services

        /// <summary>
        /// Метод для получения тех. обслуживания
        /// </summary>
        /// <returns>Список тех. обслуживания</returns>
        public List<Services> GetServicesAsync()
        {
            List<Services> _services = new List<Services>();

            try
            {
                string query = "SELECT * FROM [Journals].[dbo].[Services] ORDER BY serviceName";

                sqlconn.Open();

                SqlCommand sqlcommand = new SqlCommand(query, sqlconn);
                SqlDataReader reader = sqlcommand.ExecuteReader();

                if (reader.HasRows == true)
                {
                    // Считываем данные
                    while (reader.Read())
                    {
                        Services service = new Services()
                        {
                            IdService = int.Parse(reader[0].ToString()),
                            ServiceName = reader[1].ToString().Trim(),
                            CountDays = int.Parse(reader[2].ToString()),
                            IsSendEnabled = bool.Parse(reader[3].ToString())
                        };

                        _services.Add(service);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog(this.ToString(), $"Неудача получения списка сервис. обслуживания", LogType.Warning, out string error2);
                logger.WriteLog(this.ToString(), ex.ToString(), LogType.Error, out string f);
            }
            finally
            {
                // Если подключение открыто, то закрыть
                if (sqlconn.State == ConnectionState.Open)
                    sqlconn.Close();
            }

            return _services;
        }

        /// <summary>
        /// Метод для получения тех. обслуживания
        /// </summary>
        /// <param name="IsSendEnabled">Отправлять заявки или нет</param>
        /// <returns>Список сервис. обслуживаний</returns>
        public List<Services> GetServicesAsync(bool IsSendEnabled)
        {
            List<Services> _services = new List<Services>();

            try
            {
                string query = "SELECT * FROM [Journals].[dbo].[Services] " +
                              $"WHERE isSendEnabled = '{IsSendEnabled}'" +
                               "ORDER BY serviceName";

                sqlconn.Open();

                SqlCommand sqlcommand = new SqlCommand(query, sqlconn);
                SqlDataReader reader = sqlcommand.ExecuteReader();

                if (reader.HasRows == true)
                {
                    // Считываем данные
                    while (reader.Read())
                    {
                        Services service = new Services()
                        {
                            IdService = int.Parse(reader[0].ToString()),
                            ServiceName = reader[1].ToString().Trim(),
                            CountDays = int.Parse(reader[2].ToString()),
                            IsSendEnabled = bool.Parse(reader[3].ToString())
                        };

                        _services.Add(service);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog(this.ToString(), $"Неудача получения списка сервис. обслуживания (По параметру IsSendEnabled='{IsSendEnabled}')", LogType.Warning, out string error2);
                logger.WriteLog(this.ToString(), ex.ToString(), LogType.Error, out string f);
            }
            finally
            {
                // Если подключение открыто, то закрыть
                if (sqlconn.State == ConnectionState.Open)
                    sqlconn.Close();
            }

            return _services;
        }

        /// <summary>
        /// Метод для получения тех. обслуживания по ID сервиса
        /// </summary>
        /// <param name="IdService">ID сервиса</param>
        /// <param name="IsSendEnabled">Отправлять ли заявки тех. обслуживанию</param>
        /// <returns>Service (тех. обслуживание)</returns>
        public Services GetService(int IdService, bool IsSendEnabled = true)
        {
            Services service = new Services();

            try
            {
                string query = "SELECT * FROM [Journals].[dbo].[Services] " +
                              $"WHERE ID = '{IdService}' AND isSendEnabled = '{IsSendEnabled}'";

                sqlconn.Open();

                SqlCommand sqlcommand = new SqlCommand(query, sqlconn);
                SqlDataReader reader = sqlcommand.ExecuteReader();

                if (reader.HasRows == true)
                {
                    // Считываем данные
                    while (reader.Read())
                    {
                        service = new Services()
                        {
                            IdService = int.Parse(reader[0].ToString()),
                            ServiceName = reader[1].ToString().Trim(),
                            CountDays = int.Parse(reader[2].ToString()),
                            IsSendEnabled = bool.Parse(reader[3].ToString())
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog(this.ToString(), $"Неудача получения информации сервис. обслуживания (По параметру IdService='{IdService}', IsSendEnabled='{IsSendEnabled}'", 
                    LogType.Warning, out string error2);
                logger.WriteLog(this.ToString(), ex.ToString(), LogType.Error, out string f);
            }
            finally
            {
                // Если подключение открыто, то закрыть
                if (sqlconn.State == ConnectionState.Open)
                    sqlconn.Close();
            }

            return service;
        }

        /// <summary>
        /// Метод для получения тех. обслуживания
        /// </summary>
        /// <param name="isSendEnabled">Настройка отправлять заявки или нет</param>
        /// <returns>Список тех. обслуживания</returns>
        public List<Services> GetServices(bool isSendEnabled)
        {
            List<Services> _services = new List<Services>();

            try
            {
                string query = "SELECT * FROM [Journals].[dbo].[Services] " +
                              $"WHERE isSendEnabled='{isSendEnabled}' ORDER BY serviceName";

                sqlconn.Open();

                SqlCommand sqlcommand = new SqlCommand(query, sqlconn);
                SqlDataReader reader = sqlcommand.ExecuteReader();

                if (reader.HasRows == true)
                {
                    // Считываем данные
                    while (reader.Read())
                    {
                        Services service = new Services()
                        {
                            IdService = int.Parse(reader[0].ToString()),
                            ServiceName = reader[1].ToString().Trim(),
                            CountDays = int.Parse(reader[2].ToString()),
                            IsSendEnabled = bool.Parse(reader[3].ToString())
                        };

                        _services.Add(service);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog(this.ToString(), $"Неудача получения списка сервис. обслуживания (по параметру IsSendEnabled='{isSendEnabled}')", LogType.Warning, out string error2);
                logger.WriteLog(this.ToString(), ex.ToString(), LogType.Error, out string f);
            }
            finally
            {
                // Если подключение открыто, то закрыть
                if (sqlconn.State == ConnectionState.Open)
                    sqlconn.Close();
            }

            return _services;
        }

        /// <summary>
        /// Метод для получения списка электронных адресов по ID тех. обслуживания
        /// </summary>
        /// <param name="IDService">ID сервиса</param>
        /// <returns>Список электронных адресов</returns>
        public List<ServiceMail> GetServiceMails(int IDService)
        {
            List<ServiceMail> _serviceMail = new List<ServiceMail>();

            try
            {
                string query = "SELECT * FROM [Journals].[dbo].[ServiceEmail] " +
                              $"WHERE id_service='{IDService}' ORDER BY ID";

                sqlconn.Open();

                SqlCommand sqlcommand = new SqlCommand(query, sqlconn);
                SqlDataReader reader = sqlcommand.ExecuteReader();

                if (reader.HasRows == true)
                {
                    // Считываем данные
                    while (reader.Read())
                    {
                        ServiceMail service = new ServiceMail()
                        {
                            ID = int.Parse(reader[0].ToString()),
                            IdService = int.Parse(reader[1].ToString()),
                            Email = reader[2].ToString().Trim()
                        };

                        _serviceMail.Add(service);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog(this.ToString(), $"Неудача получения списка почтовых адресов сервис. обслуживания (по параметру IdService='{IDService}')", LogType.Warning, out string error2);
                logger.WriteLog(this.ToString(), ex.ToString(), LogType.Error, out string f);
            }
            finally
            {
                // Если подключение открыто, то закрыть
                if (sqlconn.State == ConnectionState.Open)
                    sqlconn.Close();
            }

            return _serviceMail;
        }

        /// <summary>
        /// Метод для получения списка времени отправки заявок
        /// </summary>
        /// <param name="IDService">ID сервиса</param>
        /// <returns>Список времени отправки заявок</returns>
        public List<ServiceSendTimes> GetServiceSendTimes(int IDService)
        {
            List<ServiceSendTimes> _serviceSendTimes = new List<ServiceSendTimes>();

            try
            {
                string query = "SELECT * FROM [Journals].[dbo].[ServiceSendTimes] " +
                              $"WHERE id_service='{IDService}' ORDER BY time";

                sqlconn.Open();

                SqlCommand sqlcommand = new SqlCommand(query, sqlconn);
                SqlDataReader reader = sqlcommand.ExecuteReader();

                if (reader.HasRows == true)
                {
                    // Считываем данные
                    while (reader.Read())
                    {
                        ServiceSendTimes sendtime = new ServiceSendTimes()
                        {
                            ID = int.Parse(reader[0].ToString()),
                            IdService = int.Parse(reader[1].ToString()),
                            Time = DateTime.Parse(reader[2].ToString())
                        };

                        _serviceSendTimes.Add(sendtime);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog(this.ToString(), $"Неудача получения списка времен отправки заявок сервис. обслуживания (По параметру IDService='{IDService}')", LogType.Warning, out string error2);
                logger.WriteLog(this.ToString(), ex.ToString(), LogType.Error, out string f);
            }
            finally
            {
                // Если подключение открыто, то закрыть
                if (sqlconn.State == ConnectionState.Open)
                    sqlconn.Close();
            }

            return _serviceSendTimes;
        }

        #endregion

        #region ProblemDevicesRNCB

        public List<ProblemDevice> GetProblemDevicesRNCB()
        {
            List<ProblemDevice> _problemDevices = new List<ProblemDevice>();

            try
            {
                string query = @"SELECT o.object_number, o.state, o.name, o.settings
                                 FROM object as o
                                     LEFT JOIN device as d ON d.device_id = o.device_id
                                 WHERE o.group_id != '5'
                                   AND o.group_id != '8'
                                   AND o.group_id != '44'
                                   AND o.group_id != '10'
                                   AND o.group_id != '16'
                                   AND o.group_id != '18'
                                   AND o.group_id != '33'
                                   AND o.group_id != '38'
                                   AND (o.name LIKE 'Банкомат РНКБ №%' OR o.name LIKE 'Терминал РНКБ №%')
                                 ORDER BY o.object_number";

                npgsqlconn.Open();

                NpgsqlTransaction transaction = npgsqlconn.BeginTransaction();
                NpgsqlCommand command = new NpgsqlCommand(query, npgsqlconn, transaction);
                DataSet dataSet = new DataSet();
                DataTable table = dataSet.Tables.Add("data");
                dataSet.Load(command.ExecuteReader(), LoadOption.OverwriteChanges, table);
                transaction.Commit();

                npgsqlconn.Close();

                if (table != null)
                {
                    // Считываем данные
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow reader = table.Rows[i];
                        ProblemDevice device = new ProblemDevice();

                        #region Подготовка данных

                        string _numobj = reader[0] == null ? "0" : reader[0].ToString();
                        string _numdevice = "";

                        int _numState = int.Parse(reader[1].ToString());
                        string _nameAddress = reader[2].ToString();
                        string _name = _nameAddress;
                        string _address = _nameAddress;
                        string _settings = reader[3].ToString();

                        // Параметры по умолчанию
                        bool? electricPower = false;
                        bool? ethernetMalfunction = null;
                        bool? ethernetConnection = null;
                        bool? ethernetLine = null;

                        // Разбитие на части названия и адреса объекта
                        if (_nameAddress.Contains("  "))
                        {
                            int c = _nameAddress.IndexOf("  ");

                            _name = _nameAddress.Substring(0, c);
                            _address = _nameAddress.Substring(c + 2);
                        }

                        // Нахождение номера устройства
                        if (_nameAddress.Contains("№"))
                        {
                            int indexN = _nameAddress.IndexOf("№");
                            int indexP = _nameAddress.IndexOf(" ", indexN);
                            _numdevice = _nameAddress.Remove(indexP).Substring(indexN + 1);
                        }

                        // Состояние ячейки объекта (Тех. обслуживание 32)
                        if (((ObjectStates)_numState & ObjectStates.Service) == ObjectStates.Service)
                            continue;

                        // Состояние ячейки объекта (Объект деактивирован)
                        if (((ObjectStates)_numState & ObjectStates.Deactivate) == ObjectStates.Deactivate)
                            continue;

                        // Состояние объекта 220В авария
                        electricPower = ((ObjectStates)_numState & ObjectStates.Crash220V)
                                       == ObjectStates.Crash220V;

                        // Состояние Ethernet канала
                        ethernetConnection = ((ObjectStates)_numState & ObjectStates.IndicatorEthernetConnection)
                                       == ObjectStates.IndicatorEthernetConnection;

                        // Неисправность Ethernet канала
                        ethernetMalfunction = ((ObjectStates)_numState & ObjectStates.MalfunctionEthernet)
                                       == ObjectStates.MalfunctionEthernet;

                        #region Проверки

                        if (electricPower == false && _settings.Contains("UseChannel.2=1") && ethernetConnection == true && ethernetMalfunction == false)
                            continue;

                        if (electricPower == false && !_settings.Contains("UseChannel.2=1"))
                            continue;

                        #endregion

                        if (_settings.Contains("UseChannel.2=1"))
                        {
                            // Если есть неисправность канала ETHERNET, проверяем состояние линии
                            if (ethernetMalfunction == true)
                            {
                                // Находим состояние линии
                                var value = GetDataObject($"SELECT event_subtype FROM event " +
                                                          $"WHERE object_number = '{_numobj}' " +
                                                          $"AND event_type = '3' AND (event_subtype = '47' OR event_subtype = '48') " +
                                                          $"ORDER BY event_create_time DESC " +
                                                          $"LIMIT 1");

                                if (value != null)
                                {
                                    if (!string.IsNullOrEmpty(value.ToString()))
                                        ethernetLine = value.ToString().Equals("47") ? false : true;
                                }
                            }
                        }

                        device.NumberDevice = int.Parse(_numdevice);
                        device.AddressDevice = _address;
                        device.V220 = electricPower.HasValue ? (electricPower.Value ? "НЕТ" : "ДА") : "-";
                        device.StatusEth = _settings.Contains("UseChannel.2=1") ? "ДА" : "НЕТ";
                        device.StateEth = ethernetConnection.HasValue ? (ethernetConnection.Value ? "ДА" : "НЕТ") : "-";
                        device.LineEth = ethernetLine.HasValue ? (ethernetLine.Value ? "ДА" : "НЕТ") : "-";

                        #endregion

                        _problemDevices.Add(device);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog(this.ToString(), $"Ошибка получения списка проблемных устройств РНКБ", LogType.Warning, out string error2);
                logger.WriteLog(this.ToString(), ex.ToString(), LogType.Error, out string f);
            }
            finally
            {
                // Если подключение открыто, то закрыть
                if (npgsqlconn.State == ConnectionState.Open)
                    npgsqlconn.Close();
            }

            return _problemDevices;
        }

        /// <summary>
        /// Получение таблицы с данными с параметром Запрос.
        /// </summary>
        /// <param name="query">SQL Запрос</param>
        /// <returns>Таблица с данными</returns>
        public DataTable GetDataTable(string query)
        {
            DataTable table = null;

            // Проверяем подключение на null
            if (npgsqlconn == null)
                return null;

            using (NpgsqlConnection connection = new NpgsqlConnection(npgsqlconnString))
            {
                connection.Open();

                NpgsqlTransaction transaction = connection.BeginTransaction();
                NpgsqlCommand command = new NpgsqlCommand(query, connection, transaction);
                DataSet dataSet = new DataSet();
                table = dataSet.Tables.Add("data");
                dataSet.Load(command.ExecuteReader(), LoadOption.OverwriteChanges, table);
                transaction.Commit();
            }

            return table;
        }

        /// <summary>
        /// Получение одного значения
        /// </summary>
        /// <param name="query">Запрос к Базе Данных</param>
        /// <returns>Возвращает объект типа object</returns>
        public object GetDataObject(string query)
        {
            object result = null;

            // Проверяем подключение на null
            if (npgsqlconn == null)
                return null;

            using (NpgsqlConnection connection = new NpgsqlConnection(npgsqlconnString))
            {
                connection.Open();

                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        result = reader[0].ToString();
                    }
                }

                reader.Close();
            }

            return result;
        }

        /// <summary>
        /// Получение списка значений (Берет первый элемент)
        /// </summary>
        /// <param name="query">Запрос к Базе Данных</param>
        /// <returns>Список найденных значений.</returns>
        public List<object> GetDataList(string query)
        {
            List<object> resultList = new List<object>();

            // Проверяем подключение на null
            if (npgsqlconn == null)
                return null;

            using (NpgsqlConnection connection = new NpgsqlConnection(npgsqlconnString))
            {
                connection.Open();

                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        resultList.Add(reader[0]);
                    }
                }

                reader.Close();
            }

            return resultList;
        }

        #endregion

        #region SETTINGS

        /// <summary>
        /// Получить настройки отправки файла по почте
        /// </summary>
        /// <returns>Насройки почты</returns>
        public SendMailInfo GetSendMailInfo()
        {
            SendMailInfo sendmailinfo = new SendMailInfo();

            try
            {
                string query = "SELECT * FROM [Journals].[dbo].[ServiceSendMailInfo]";

                sqlconn.Open();

                SqlCommand sqlcommand = new SqlCommand(query, sqlconn);
                SqlDataReader reader = sqlcommand.ExecuteReader();

                if (reader.HasRows == true)
                {
                    // Считываем данные
                    while (reader.Read())
                    {
                        sendmailinfo.Server = reader[1].ToString().Trim();
                        sendmailinfo.Port = int.Parse(reader[2].ToString());
                        sendmailinfo.MailFrom = reader[3].ToString().Trim();
                        sendmailinfo.Password = reader[4].ToString().Trim();
                        sendmailinfo.Caption = reader[5].ToString().Trim();
                        sendmailinfo.Message = reader[6].ToString().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog(this.ToString(), $"Неудача получения настроек отправки почты", LogType.Warning, out string error2);
                logger.WriteLog(this.ToString(), ex.ToString(), LogType.Error, out string f);
            }
            finally
            {
                // Если подключение открыто, то закрыть
                if (sqlconn.State == ConnectionState.Open)
                    sqlconn.Close();
            }

            return sendmailinfo;
        }

        #endregion

        #endregion

    }
}
