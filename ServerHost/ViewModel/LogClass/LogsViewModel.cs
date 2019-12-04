using ServerWork;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace ServerHost
{
    public class LogsViewModel : BaseViewModel
    {
        #region Private

        /// <summary>
        /// Инициализация логгера
        /// </summary>
        private LogManager logger;

        #endregion

        #region Protected

        /// <summary>
        /// Последняя строка фильтра
        /// </summary>
        protected string mLastFilterText;

        /// <summary>
        /// Строка фильтра
        /// </summary>
        protected string mFilterText;

        /// <summary>
        /// Список логов
        /// </summary>
        protected ObservableCollection<LogListItemViewModel> mLogList;

        #endregion

        #region Public

        /// <summary>
        /// Список логов
        /// </summary>
        public ObservableCollection<LogListItemViewModel> LogList
        {
            get => mLogList;

            set
            {
                // Make sure list has changed
                if (mLogList == value)
                    return;

                // Update value
                mLogList = value;

                // Update filtered list to match
                filteredLogList = new ObservableCollection<LogListItemViewModel>(mLogList);
            }
        }

        /// <summary>
        /// Список отфильтрованных логов
        /// </summary>
        public ObservableCollection<LogListItemViewModel> filteredLogList { get; set; }

        /// <summary>
        /// Строка фильтра
        /// </summary>
        public string FilterText
        {
            get => mFilterText;
            set
            {
                if (mFilterText == value)
                    return;

                // Update value
                mFilterText = value;

                if (!string.IsNullOrEmpty(FilterText))
                    Filter();
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Конструктор
        /// </summary>
        public LogsViewModel()
        {
            // Указываем путь к файлу логов
            logger = new LogManager(AppDomain.CurrentDomain.BaseDirectory + "logs.txt");

            mLogList = GetLogs();
            filteredLogList = mLogList;

            FilterCommand = new RelayCommand(Filter);
            ClearFilterCommand = new RelayCommand(() => FilterText = string.Empty);
            RefreshLogsCommand = new RelayCommand(() => mLogList = GetLogs());
        }

        #endregion

        #region Commands

        /// <summary>
        /// The command to refresh Logs
        /// </summary>
        public ICommand RefreshLogsCommand { get; set; }

        /// <summary>
        /// The command to add new filter
        /// </summary>
        public ICommand FilterCommand { get; set; }

        /// <summary>
        /// The command clear filter
        /// </summary>
        public ICommand ClearFilterCommand { get; set; }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Filters the view logs
        /// </summary>
        public void Filter()
        {
            // Make sure we don't re-filter the same text
            if ((string.IsNullOrEmpty(mLastFilterText) && string.IsNullOrEmpty(FilterText)) || string.Equals(mLastFilterText, FilterText))
                return;

            // If we have no filter text, or no items
            if (string.IsNullOrEmpty(FilterText) || LogList == null || LogList.Count <= 0)
            {
                // Make filtered list the same
                filteredLogList = new ObservableCollection<LogListItemViewModel>(LogList);

                // Set last search text
                mLastFilterText = FilterText;

                return;
            }

            var d = new ObservableCollection<LogListItemViewModel>(
                LogList.Where(item => item.DatetimeLog.ToString().ToLower().Contains(FilterText.ToLower())
                                   || item.Message.ToLower().Contains(FilterText.ToLower())
                                   || item.Source.ToLower().Contains(FilterText.ToLower())
                                   || item.TypeLog.ToString().ToLower().Contains(FilterText.ToLower()))
                                   .OrderByDescending(order => order.DatetimeLog));

            filteredLogList = d;

        }

        /// <summary>
        /// Получаем список логов
        /// </summary>
        /// <returns>Список логов</returns>
        private ObservableCollection<LogListItemViewModel> GetLogs()
        {
            ObservableCollection<LogListItemViewModel> logs = new ObservableCollection<LogListItemViewModel>();

            try
            {
                LogListItemViewModel logclass = new LogListItemViewModel();

                foreach (var log in logger.ReadLogs(out string error))
                {
                    // Check if the log in Null or Empty
                    if (String.IsNullOrEmpty(log))
                    {
                        logclass = new LogListItemViewModel();
                        continue;
                    }

                    #region Type of the log

                    if (log.Contains("<Информация>"))
                    {
                        logclass.TypeLog = LogType.Information;
                        logclass.LogTypeRGB = "#65A7E0";
                    }

                    if (log.Contains("<Предупреждение>"))
                    {
                        logclass.TypeLog = LogType.Warning;
                        logclass.LogTypeRGB = "#FABE5D";
                    }

                    if (log.Contains("<Ошибка>"))
                    {
                        logclass.TypeLog = LogType.Error;
                        logclass.LogTypeRGB = "#F37D7B";
                    }

                    #endregion

                    // Datetime of the log
                    if (log.Contains("[") && log.Contains("]"))
                    {
                        int f = log.IndexOf("[");
                        int l = log.IndexOf("]");

                        string dt = log.Remove(l);
                        dt = dt.Remove(0, 1); // Substring(f + 1, log.Length - l);

                        logclass.DatetimeLog = DateTime.Parse(dt);
                    }

                    // Source of the log
                    if (log.Contains("Source: ") && log.Contains(";"))
                    {
                        int f = log.IndexOf("Source: ");
                        int s = log.IndexOf(";");

                        string source = log.Substring(f + 8, log.Length - (f + 8) - 1);

                        logclass.Source = source;
                    }

                    // Message of the log
                    if (!log.Contains("<Информация>") && !log.Contains("<Предупреждение>") && !log.Contains("<Ошибка>"))
                    {
                        logclass.Message = log;
                        logs.Add(logclass);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return logs; //.OrderByDescending(o => o.DatetimeLog);
        }

        #endregion
    }
}
