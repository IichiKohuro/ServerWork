using Microsoft.Win32;
using ServerHost.Views.Windows;
using ServerHost.Views.Pages;
using ServerWork;
using System;
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
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        private int mOuterMarginSize = 10;

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        private int mWindowRadius = 10;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition mDockPosition = WindowDockPosition.Undocked;

        /// <summary>
        /// The initialization log manager
        /// </summary>
        private LogManager logger;

        /// <summary>
        /// INI class 
        /// </summary>
        private INI ini;

        #endregion


        #region Public properties

        /// <summary>
        /// The smallest width the window can go to
        /// </summary>
        public double WindowMinimumWidth { get; set; } = 700;

        /// <summary>
        /// The smallest height the window can go to
        /// </summary>
        public double WindowMinimumHeight { get; set; } = 600;

        /// <summary>
        /// True if the window should be borderless because it is docked or maximized
        /// </summary>
        public bool Borderless { get { return (mWindow.WindowState == WindowState.Maximized || mDockPosition != WindowDockPosition.Undocked); } }

        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        public int ResizeBorder { get; set; } = 5;

        /// <summary>
        /// The size of the resize border around the window, taking into account the outer margin
        /// </summary>
        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder + OuterMarginSize); } }

        /// <summary>
        /// The padding of the inner content of the main window
        /// </summary>
        public Thickness InnerContentPadding { get { return new Thickness(ResizeBorder); } }

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public int OuterMarginSize
        {
            get
            {
                // If it is maximized or docked, no border
                return Borderless ? 0 : mOuterMarginSize;
            }
            set
            {
                mOuterMarginSize = value;
            }
        }

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public Thickness OuterMarginSizeThickness { get { return new Thickness(OuterMarginSize); } }

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public int WindowRadius
        {
            get
            {
                // If it is maximized or docked, no border
                return Borderless ? 0 : mWindowRadius;
            }
            set
            {
                mWindowRadius = value;
            }
        }

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public CornerRadius WindowCornerRadius { get { return new CornerRadius(WindowRadius); } }

        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        public int TitleHeight { get; set; }

        /// <summary>
        /// The width of the left panel
        /// </summary>
        public int LeftPanelWidth { get; set; } = 300;

        /// <summary>
        /// The current page
        /// </summary>
        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.Dashboard;

        /// <summary>
        /// Главное окно повер всех окон
        /// </summary>
        public bool TopMostWindow { get; set; } = false;

        #endregion


        #region Constructor

        public WindowViewModel(Window window)
        {
            mWindow = window;

            TitleHeight = (int)window.Height;

            // Listen out for the window resizing
            mWindow.StateChanged += (sender, e) =>
            {
                // Fire off events for all properties that are affected by a resize
                WindowResized();
            };

            // Fix window resize issue
            var resizer = new WindowResizer(mWindow);

            // Listen out for dock changes
            resizer.WindowDockChanged += (dock) =>
            {
                // Store last position
                mDockPosition = dock;

                // Fire off resize events
                WindowResized();
            };

            // Указываем путь к файлу логов
            logger = new LogManager(AppDomain.CurrentDomain.BaseDirectory + "logs.txt");

            // Логгируем действие
            logger.WriteLog(this.ToString(), "ЗАПУСК СЕРВЕРА", LogType.Information, out string error);

            // Create commands
            RestoreWindowCommand = new RelayCommand(() => mWindow.Show());
            MinimizeCommand = new RelayCommand(() => mWindow.Hide());
            MaximizeCommand = new RelayCommand(() => mWindow.WindowState ^= WindowState.Maximized);

            CloseCommand = new RelayCommand(() =>
            {

                // Логгируем действие
                logger.WriteLog(this.ToString(), "ОСТАНОВКА СЕРВЕРА", LogType.Information, out error);

                mWindow.Close();
            });

            SettingsServicesCommand = new RelayCommand(() => new SettingsSendBidsServiceWindow().ShowDialog());

            SettingsPageMenuCommand = new RelayCommand(() =>
            {
                mWindow.Show();
                CurrentPage = ApplicationPage.Settings;
            });
            SettingsPageCommand = new RelayCommand(() => CurrentPage = ApplicationPage.Settings);
            LogsPageCommand = new RelayCommand(() => CurrentPage = ApplicationPage.Logs);

            // MenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(mWindow, GetMousePosition()));
  

            ini = new INI(AppDomain.CurrentDomain.BaseDirectory + "Configuration.ini");

            GetServerSettings();  
        }

        #endregion


        #region Commands

        /// <summary>
        /// The command to show settings services page
        /// </summary>
        public ICommand SettingsServicesCommand { get; set; }

        /// <summary>
        /// The command to show settings page
        /// </summary>
        public ICommand SettingsPageMenuCommand { get; set; }

        /// <summary>
        /// The command to show settings page
        /// </summary>
        public ICommand SettingsPageCommand { get; set; }

        /// <summary>
        /// The command to show logs page
        /// </summary>
        public ICommand LogsPageCommand { get; set; }

        /// <summary>
        /// The command to restore main window
        /// </summary>
        public ICommand RestoreWindowCommand { get; set; }

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

        /// <summary>
        /// If the window resizes to a special position (docked or maximized)
        /// this will update all required property change events to set the borders and radius values
        /// </summary>
        private void WindowResized()
        {
            // Fire off events for all properties that are affected by a resize
            OnPropertyChanged(nameof(Borderless));
            OnPropertyChanged(nameof(ResizeBorderThickness));
            OnPropertyChanged(nameof(OuterMarginSize));
            OnPropertyChanged(nameof(OuterMarginSizeThickness));
            OnPropertyChanged(nameof(WindowRadius));
            OnPropertyChanged(nameof(WindowCornerRadius));
        }

        /// <summary>
        /// Запуск программы при загрузке Windows
        /// </summary>
        /// <param name="isChecked">Если true, запускаем, иначе - нет...</param>
        private void RegisterInStartup(bool isChecked)
        {
            // Инициализация записи в регистре
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (isChecked)
                // Записываем запись в регистр
                registryKey.SetValue("Server Host", AppDomain.CurrentDomain.BaseDirectory + "ServerHost.exe");
            else
                // Удаляем запись из регистра
                registryKey.DeleteValue("Server Host", false);
        }

        /// <summary>
        /// Получаем настройки для сервера
        /// </summary>
        private void GetServerSettings()
        {
            TopMostWindow = bool.Parse(ini.Read("MainConfiguration", "AutostartServer"));

            bool _autostart = bool.Parse(ini.Read("MainConfiguration", "TopMost"));

            // Если настройка true, тогда при запуске Windows запускается наша программа
            RegisterInStartup(_autostart);
        }

        #endregion

    }
}
