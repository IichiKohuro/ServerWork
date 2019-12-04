using ServerWork;
using System;
using System.Windows;
using System.Windows.Input;

namespace ServerHost.ViewModel
{
    public class SettingsSendBidsViewModel : BaseViewModel
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
        public ApplicationServicePage CurrentPage { get; set; } = ApplicationServicePage.Mail;

        /// <summary>
        /// Главное окно повер всех окон
        /// </summary>
        public bool TopMostWindow { get; set; } = false;

        #endregion


        #region Constructor

        public SettingsSendBidsViewModel(Window window)
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
            logger.WriteLog(this.ToString(), "Открываем окно с настройками сервиса", LogType.Information, out string error);

            // Create commands
            MinimizeCommand = new RelayCommand(() => mWindow.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand(() => mWindow.WindowState ^= WindowState.Maximized);

            CloseCommand = new RelayCommand(() =>
            {
                // Логгируем действие
                logger.WriteLog(this.ToString(), "Закрываем окно с настройками сервиса", LogType.Information, out error);

                mWindow.Close();
            });

            GeneralPageCommand = new RelayCommand(() => CurrentPage = ApplicationServicePage.General);
            MailPageCommand = new RelayCommand(() => CurrentPage = ApplicationServicePage.Mail);
            ServicesPageCommand = new RelayCommand(() => CurrentPage = ApplicationServicePage.Services);
        }

        #endregion


        #region Commands

        /// <summary>
        /// The command to show settings page
        /// </summary>
        public ICommand GeneralPageCommand { get; set; }

        /// <summary>
        /// The command to show settings page
        /// </summary>
        public ICommand MailPageCommand { get; set; }

        /// <summary>
        /// The command to show logs page
        /// </summary>
        public ICommand ServicesPageCommand { get; set; }

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
        /// Получаем настройки для сервисов
        /// </summary>
        private void GetServicesSettings()
        {
            TopMostWindow = true;
        }

        #endregion

    }
}
