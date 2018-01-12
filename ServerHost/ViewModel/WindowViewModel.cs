using Hardcodet.Wpf.TaskbarNotification;
using MaterialDesignThemes.Wpf;
using ServerHost.UserControls;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ServerHost
{
    public class WindowViewModel : BaseViewModel
    {
        #region Private Member

        /// <summary>
        /// The window this view model controls
        /// </summary>
        private Window mWindow;

        /// <summary>
        /// The create service host
        /// </summary>
        private ServiceHost host;

        private string serverstatus;

        private string servicestate;

        private bool stateService;

        // Dialog host
        private bool _isDialogOpen;
        private object _loadingContent;

        #endregion

        #region Public Properties

        #region Dialog

                public bool IsDialogOpen
                {
                    get { return _isDialogOpen; }
                    set
                    {
                        if (_isDialogOpen == value) return;
                        _isDialogOpen = value;
                        OnPropertyChanged("IsDialogOpen");
                    }
                }

                public object LoadingContent
                {
                    get { return _loadingContent; }
                    set
                    {
                        if (_loadingContent == value) return;
                        _loadingContent = value;
                        OnPropertyChanged("LoadingContent");
                    }
                }

        #endregion

        public string ServerStatus
        {
            get { return serverstatus; }
            set
            {
                serverstatus = value;
                OnPropertyChanged("ServerStatus");
            }
        }

        public string ServiceState
        {
            get { return servicestate; }
            set
            {
                servicestate = value;
                OnPropertyChanged("ServiceState");
            }
        }

        public bool StateService
        {
            get { return stateService; }
            set
            {
                stateService = value;
                OnPropertyChanged("StateService");
            }
        }

        #endregion

        #region Constructor
        public WindowViewModel(Window window)
        {
            mWindow = window;
            host = new ServiceHost(typeof(ServerWork.ServerService));

            StateService = false;
            ServerStatus = "Ожидание запуска ...";
            ServiceState = "ServerNetworkOff";

            //Note: XAML is suggested for all but the simplest scenarios
            TaskbarIcon tbi = new TaskbarIcon();
            tbi.Icon = new System.Drawing.Icon(Properties.Resources.Control_Panel, 16, 16);
            tbi.ToolTipText = "hello world";

            // Create commands
            MailServiceDialogCommand = new AnotherCommandImplementation(MailServiceDialog);
            AcceptDialogCommand = new AnotherCommandImplementation(AcceptDialog);
            CancelDialogCommand = new AnotherCommandImplementation(CancelDialog);


            MinimizeCommand = new RelayCommand(() => mWindow.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand(() => mWindow.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand(() => mWindow.Close());
            //MenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(mWindow, GetMousePosition()));

        }

        private void MailServiceDialog(object obj)
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

                    ServerStatus = "Сервис запущен ...";
                }
                else
                {
                    host.Abort();

                    ServiceState = "ServerNetworkOff";
                    StateService = false;

                    ServerStatus = "Сервис остановлен ...";
                }

                Task.Delay(TimeSpan.FromSeconds(3));

            }).ContinueWith((t, _) => IsDialogOpen = false, null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void CancelDialog(object obj)
        {
            IsDialogOpen = false;
        }

        private void AcceptDialog(object obj)
        {
            Task.Factory.StartNew(() => 
            {
                if (StateService == false)
                {
                    host = new ServiceHost(typeof(ServerWork.ServerService));
                    host.Open();

                    ServiceState = "ServerNetwork";
                    StateService = true;

                    ServerStatus = "Сервис запущен ...";
                }
                else
                {
                    host.Abort();

                    ServiceState = "ServerNetworkOff";
                    StateService = false;

                    ServerStatus = "Сервис остановлен ...";
                }

            }).ContinueWith((t, _) => IsDialogOpen = false, null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion

        #region Command Methods



        #endregion

        #region Commands

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

        /// <summary>
        /// The command to minimize the window
        /// </summary>
        public ICommand MinimizeCommand { get; set; }

        /// <summary>
        /// The command to maximize the window
        /// </summary>
        public ICommand MaximizeCommand { get; set; }

        /// <summary>
        /// The command to close the window
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command to show the system menu of the window
        /// </summary>
        public ICommand MenuCommand { get; set; }

        #endregion

        #region Private Helpers

        

        #endregion

    }
}
