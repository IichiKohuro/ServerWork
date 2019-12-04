using ServerHost.UserControls;
using ServerWork;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ServerHost
{
    public class RNCBServiceViewModel : BaseViewModel
    {
        #region Private members

        /// <summary>
        /// Создание хоста сервиса
        /// </summary>
        private ServiceHost host;

        /// <summary>
        /// Создание логгера
        /// </summary>
        private LogManager logger;

        /// <summary>
        /// Клиент сервиса
        /// </summary>
        private ServerService client;

        /// <summary>
        /// INI class 
        /// </summary>
        private INI ini;

        #endregion

        #region Public Properties

        /// <summary>
        /// Открыт ли диалог? (да/нет)
        /// </summary>
        public bool IsDialogOpen { get; set; }

        /// <summary>
        /// Загружаемый контент(view) в диалоговое окно
        /// </summary>
        public object LoadingContent { get; set; }

        /// <summary>
        /// Статус сервиса в текстовом виде
        /// </summary>
        public string ServiceStatus { get; set; }

        /// <summary>
        /// Состояние сервиса в виде иконки
        /// </summary>
        public string ServiceState { get; set; }

        /// <summary>
        /// Состояние сервиса в значении да/нет (true/false)
        /// </summary>
        public bool StateService { get; set; }

        /// <summary>
        /// Время работы сервиса
        /// </summary>
        public string TimeWork { get; set; }

        /// <summary>
        /// Отправляются заявки
        /// </summary>
        public bool IsSend { get; set; } = false;

        /// <summary>
        /// Статус сервиса отправки заявок в подсказке трея 
        /// </summary>
        public string ServiceStatusTrayToolTip { get; set; } = "Сервис отправки проблемных устройств РНКБ: Отключен ...";

        public string StateServiceMenuItem { get; set; } = "Запустить сервис";

        #endregion

        #region Constructor

        public RNCBServiceViewModel()
        {
            host = new ServiceHost(typeof(ServerService));

            StateService = false;
            ServiceStatus = "Ожидание запуска ...";
            ServiceState = "ServerNetworkOff";
            ServiceStatusTrayToolTip = "Сервис отправки проблемных устройств РНКБ: Отключен ...";

            // Указываем путь к файлу логов
            logger = new LogManager(AppDomain.CurrentDomain.BaseDirectory + "logs.txt");

            // Создание команд
            MailServiceDialogCommand = new RelayCommand(() => MailServiceDialog());
            SendRNCBOnDemandCommand = new RelayCommand(() => SendRNCBOnDemand());
            AcceptDialogCommand = new RelayCommand(() => AcceptDialog());
            CancelDialogCommand = new RelayCommand(() => CancelDialog());

            ini = new INI(AppDomain.CurrentDomain.BaseDirectory + "Configuration.ini");

            if (bool.Parse(ini.Read("MainConfiguration", "AutostartRNCBService")))
                MailServiceDialog();

        }

        #endregion

        #region Service Methods

        /// <summary>
        /// Запуск диалогового окна при отправке почты по требованию
        /// </summary>
        private void SendRNCBOnDemand()
        {
            if (host != null)
            {
                // Если хост открыт выполняем код
                if (host.State == CommunicationState.Opened)
                {
                    // Открываем диалоговое окно, чтобы пользователь ждал выполнения операции
                    LoadingContent = new LoadView("Отправка данных, ждите ...");
                    IsDialogOpen = true;

                    Task.Factory.StartNew(() =>
                    {
                        var client = new ServerService(true);

                        // Отправка списка проблемных устройств РНКБ по требованию пользователя
                        client.SendServicesBidsOnRequest();

                    }).ContinueWith((t, _) => IsDialogOpen = false, null,
                        TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        /// <summary>
        /// Запуск диалогового окна при запуске или остановке сервиса
        /// </summary>
        /// <param name="obj"></param>
        private void MailServiceDialog()
        {
            string mess = string.Empty;

            if (StateService == false)
                mess = "Запуск сервиса, ждите ...";
            else
                mess = "Остановка сервиса, ждите ...";

            LoadingContent = new LoadView(mess);
            IsDialogOpen = true;

            Task.Factory.StartNew(() =>
            {
                if (StateService == false)
                {
                    host = new ServiceHost(typeof(ServerWork.ServerService));
                    host.Open();          

                    ServiceState = "ServerNetwork";
                    StateService = true;

                    client = new ServerService();
                    
                    IsSend = client.GetStatusMonitoring();

                    // Логгируем действие
                    logger.WriteLog(this.ToString(), "Запуск сервиса отправки проблемных устройст РНКБ ...", LogType.Information, out string error);

                    ServiceStatus = "Сервис запущен ...";
                    ServiceStatusTrayToolTip = "Сервис отправки проблемных устройст РНКБ: Работает ...";

                    StateServiceMenuItem = "Остановить сервис";                    

                    #region Time Work Service

                    DateTime dt = DateTime.Now;

                    Task.Factory.StartNew(() =>
                    {
                        while (StateService)
                        {
                            TimeSpan dt2 = DateTime.Now.Subtract(dt);
                            TimeWork = $"{dt2.Days.ToString("D")}.{dt2.Hours.ToString("D2")}:{dt2.Minutes.ToString("D2")}:{dt2.Seconds.ToString("D2")}";

                            CheckStatusMonitoring();

                            Task.Delay(1000);
                        }
                    });

                    #endregion            
                }
                else
                {
                    //logger.WriteLog(this.ToString(), $"Отключение клиента: ᴵᴾ {client.GetIPAddressClient()}", LogType.Information, out string error3);
                    host.Abort();

                    IsSend = false;

                    ServiceState = "ServerNetworkOff";
                    StateService = false;

                    // Логгируем действие
                    logger.WriteLog(this.ToString(), "Остановка сервиса отправки проблемных устройст РНКБ ...", LogType.Information, out string error);

                    ServiceStatus = "Сервис остановлен ...";
                    ServiceStatusTrayToolTip = "Сервис отправки проблемных устройст РНКБ: Отключен ...";

                    StateServiceMenuItem = "Запустить сервис";
                }

                //Thread.Sleep(5000);

            }).ContinueWith((t, _) => IsDialogOpen = false, null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Отмена диалогового окна
        /// </summary>
        /// <param name="obj"></param>
        private void CancelDialog()
        {
            IsDialogOpen = false;
        }

        /// <summary>
        /// Подтверждение диалогового окна
        /// </summary>
        /// <param name="obj"></param>
        private void AcceptDialog()
        {
            Task.Factory.StartNew(() =>
            {
                if (host.State == CommunicationState.Closed)
                {
                    host = new ServiceHost(typeof(ServerWork.ServerService));
                    host.Open();

                    ServiceState = "ServerNetwork";
                    StateService = true;

                    ServiceStatus = "Сервис запущен ...";
                    StateServiceMenuItem = "Остановить сервис";
                    ServiceStatusTrayToolTip = "Сервис отправки проблемных устройст РНКБ: Работает ...";
                }
                else
                {
                    host.Abort();

                    ServiceState = "ServerNetworkOff";
                    StateService = false;

                    ServiceStatus = "Сервис остановлен ...";
                    ServiceStatusTrayToolTip = "Сервис отправки проблемных устройст РНКБ: Отключен ...";

                    StateServiceMenuItem = "Запустить сервис";
                }

            }).ContinueWith((t, _) => IsDialogOpen = false, null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void CheckStatusMonitoring()
        {
            IsSend = client.GetStatusMonitoring();
        }

        #endregion

        #region Commands

        /// <summary>
        /// The command to send mail on demand
        /// </summary>
        public ICommand SendRNCBOnDemandCommand { get; }

        /// <summary>
        /// The command to change state mail service
        /// </summary>
        public ICommand MailServiceDialogCommand { get; }

        /// <summary>
        /// The command to start dialog
        /// </summary>
        public ICommand AcceptDialogCommand { get; }

        /// <summary>
        /// The command to close dialog
        /// </summary>
        public ICommand CancelDialogCommand { get; }

        /// <summary>
        /// The command to start service
        /// </summary>
        public ICommand ShowCloseDialogCommand { get; set; }

        #endregion
    }
}
