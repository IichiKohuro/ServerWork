using ServerWork;
using System;
using System.Windows.Input;

namespace ServerHost
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Private

        /// <summary>
        /// INI Class
        /// </summary>
        private INI ini;

        #endregion

        #region Public

        /// <summary>
        /// Запуск сервера с Windows
        /// </summary>
        public bool ServerStartWithWindows { get; set; } = false;

        /// <summary>
        /// Запуск сервиса с сервером
        /// </summary>
        public bool ServiceSendBidsStartWithServer { get; set; } = false;

        /// <summary>
        /// Индикатор сохранения
        /// </summary>
        public bool IsSaving { get; set; }

        /// <summary>
        /// Главное окно поверх всех окон
        /// </summary>
        public bool TopMostWindow { get; set; }

        /// <summary>
        /// Рабочая папка сервера
        /// </summary>
        public string RootFolder { get; set; }

        /// <summary>
        /// Сервер БД
        /// </summary>
        public string ServerDB { get; set; }

        /// <summary>
        /// Пользователь БД
        /// </summary>
        public string UserDB { get; set; }

        /// <summary>
        /// Пароль БД
        /// </summary>
        public string PasswordDB { get; set; }

        /// <summary>
        /// Название БД
        /// </summary>
        public string NameDB { get; set; }

        /// <summary>
        /// PSI БД
        /// </summary>
        public bool PsiDB { get; set; }

        #endregion


        #region Constructor

        public SettingsViewModel()
        {
            IsSaving = false;

            ini = new INI(AppDomain.CurrentDomain.BaseDirectory + "Configuration.ini");
            GetSettings();

            // Commands
            SaveCommand = new RelayCommand(() => SaveSettings());
            ResetToDefaultCommand = new RelayCommand(() => ResetToDefault());
            CancelCommand = new RelayCommand(() => CancelSettings());
        }

        #endregion


        #region Commands

        /// <summary>
        /// The command to save settings changes
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// The command to cancel settings changes
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// The command to reset settings to default
        /// </summary>
        public ICommand ResetToDefaultCommand { get; }

        #endregion


        #region Private Methods

        /// <summary>
        /// Сохранить изменения настроек
        /// </summary>
        private void SaveSettings()
        {
            IsSaving = true;

            ini.Write("MainConfiguration", "AutostartServer", ServerStartWithWindows.ToString());
            ini.Write("MainConfiguration", "AutostartSendBidService", ServiceSendBidsStartWithServer.ToString());
            ini.Write("MainConfiguration", "TopMost", TopMostWindow.ToString());
            ini.Write("MainConfiguration", "RootFolder", RootFolder);

            ini.Write("DBConfiguration", "Server", ServerDB);
            ini.Write("DBConfiguration", "Login", UserDB);
            ini.Write("DBConfiguration", "Password", PasswordDB);
            ini.Write("DBConfiguration", "DBName", NameDB);
            ini.Write("DBConfiguration", "PSI", PsiDB.ToString());

            IsSaving = false;
        }

        /// <summary>
        /// Сбросить настройки по умолчанию
        /// </summary>
        private void ResetToDefault()
        {
            ServerStartWithWindows         = true;
            ServiceSendBidsStartWithServer = true;
            TopMostWindow                  = true;
            RootFolder                     = "";

            ServerDB   = @"ARM3-RD\PHOENIX4";
            UserDB     = "sa";
            PasswordDB = null;
            NameDB     = "Journals";
            PsiDB      = true;
        }

        /// <summary>
        /// Получить настройки
        /// </summary>
        private void GetSettings()
        {
            ServerStartWithWindows = bool.Parse(ini.Read("MainConfiguration", "AutostartServer"));
            ServiceSendBidsStartWithServer = bool.Parse(ini.Read("MainConfiguration", "AutostartSendBidService"));
            TopMostWindow = bool.Parse(ini.Read("MainConfiguration", "TopMost"));
            RootFolder = ini.Read("MainConfiguration", "RootFolder");

            ServerDB = ini.Read("DBConfiguration", "Server");
            UserDB = ini.Read("DBConfiguration", "Login");
            PasswordDB = ini.Read("DBConfiguration", "Password");
            NameDB = ini.Read("DBConfiguration", "DBName");
            PsiDB = bool.Parse(ini.Read("DBConfiguration", "PSI"));
        }

        /// <summary>
        /// Отмена сохранения
        /// </summary>
        private void CancelSettings()
        {
            IsSaving = false;
        }

        #endregion
    }
}
