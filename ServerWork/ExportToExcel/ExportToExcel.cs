using System.Collections.Generic;
using GemBox.Spreadsheet;
using System.IO;
using System;

namespace ServerWork
{
    public class ExportToExcel
    {
        #region Private properties

        /// <summary>
        /// Список заявок
        /// </summary>
        private List<TehBids> listTehBids;

        /// <summary>
        /// Список проблемных устройств РНКБ
        /// </summary>
        private List<ProblemDevice> listProblemDevices;

        /// <summary>
        /// Сервис (Тех. обслуживание)
        /// </summary>
        private Services service;

        /// <summary>
        /// Инициализация логгера
        /// </summary>
        private LogManager logger;

        /// <summary>
        /// Путь к рабочей папке, где хранятся экспортируемые файлы Excel
        /// </summary>
        private string rootFolder;

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор для проблемных устройств РНКБ
        /// </summary>
        public ExportToExcel(List<ProblemDevice> _list, string _rootFolder)
        {
            SpreadsheetInfo.SetLicense("ERDC-FN4O-YKYN-4DBM");

            listProblemDevices = _list;

            rootFolder = _rootFolder;

            // Указываем путь к файлу логов
            logger = new LogManager(AppDomain.CurrentDomain.BaseDirectory + "logs.txt");
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_list">Список заявок</param>
        public ExportToExcel(List<TehBids> _list)
        {
            SpreadsheetInfo.SetLicense("ERDC-FN4O-YKYN-4DBM");
            //SpreadsheetInfo.FreeLimitReached += (sender, e) => e.FreeLimitReachedAction = FreeLimitReachedAction.ContinueAsTrial;
            //ComponentInfo.SetLicense("FREE-LIMITED-KEY");       

            listTehBids = _list;

            // Указываем путь к файлу логов
            logger = new LogManager(AppDomain.CurrentDomain.BaseDirectory + "logs.txt");
        }

        /// <summary>
        /// Расширенный конструктор
        /// </summary>
        /// <param name="_list">Список заявок</param>
        /// <param name="_service">Сервис (Тех. обслуживание)</param>
        /// <param name="_rootfolder">Рабочая папка, где хранятся экспортируемые файлы Excel</param>
        public ExportToExcel(List<TehBids> _list, Services _service, string _rootfolder)
        {
            SpreadsheetInfo.SetLicense("ERDC-FN4O-YKYN-4DBM");

            listTehBids = _list;
            service = _service;
            rootFolder = _rootfolder;

            // Указываем путь к файлу логов
            logger = new LogManager(AppDomain.CurrentDomain.BaseDirectory + "logs.txt");
        }

        #endregion

        #region Methods

        public string ExportTo()
        {
            if (listTehBids.Count < 1)
            {
                logger.WriteLog(this.ToString(), $"По {service.ServiceName} нет заявок;", LogType.Information, out string error);
                return null;
            }

            #region Columns

            List<string> columns = new List<string>();

            columns.Add("Дата/Время");
            columns.Add("№ яч.");
            columns.Add("Название");
            columns.Add("Адрес");
            columns.Add("Характер заявки");
            columns.Add("Доп. информация");
            columns.Add("От кого заявка");
            

            #endregion

            if (!Directory.Exists($"{rootFolder}\\Заявки\\{service.ServiceName}"))
                Directory.CreateDirectory($"{rootFolder}\\Заявки\\{service.ServiceName}");

            string filename = $"{rootFolder}\\Заявки\\{service.ServiceName}\\Заявки " +
                $"({service.ServiceName}) [{DateTime.Now.ToShortDateString()} {DateTime.Now.Hour}-{DateTime.Now.Minute}].xlsx";

            try
            {
                ExcelFile excelFile = new ExcelFile();
                SettingsSheet(excelFile, columns);

                using (FileStream stream = new FileStream(filename, FileMode.Create))
                {
                    // Save file to stream
                    excelFile.Save(stream, SaveOptions.XlsxDefault);
                }

                logger.WriteLog(this.ToString(), $"Успешный экпорт заявок в Excel файл по пути: {filename};", LogType.Information, out string error);                
            }
            catch (Exception ex)
            {
                logger.WriteLog(this.ToString(), ex.ToString(), LogType.Error, out string error);
            }

            return filename;
        }

        public string ExportToProblems()
        {
            if (listProblemDevices.Count < 1)
            {
                logger.WriteLog(this.ToString(), $"Список проблемных устройств пуст;", LogType.Information, out string error);
                return null;
            }

            #region Columns

            List<string> columns = new List<string>();

            columns.Add("Номер устройства");
            columns.Add("Адрес устройства");
            columns.Add("Наличие 220В");
            columns.Add("Наличие канала связи ETHERNET");
            columns.Add("Линия ('физика') связи ETHERNET");
            columns.Add("Работа канала связи ETHERNET");

            #endregion

            if (!Directory.Exists($"{rootFolder}\\Проблемные устройства РНКБ"))
                Directory.CreateDirectory($"{rootFolder}\\Проблемные устройства РНКБ");

            string filename = $"{rootFolder}\\Проблемные устройства РНКБ\\Проблемные устройства РНКБ " +
                $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.Hour}-{DateTime.Now.Minute}].xlsx";

            try
            {
                ExcelFile excelFile = new ExcelFile();
                SettingsSheetProblems(excelFile, columns, "Проблемные устройства РНКБ");

                using (FileStream stream = new FileStream(filename, FileMode.Create))
                {
                    // Save file to stream
                    excelFile.Save(stream, SaveOptions.XlsxDefault);
                }

                logger.WriteLog(this.ToString(), $"Успешный экпорт проблемных устройств в файл по пути: {filename};", LogType.Information, out string error);
            }
            catch (Exception ex)
            {
                logger.WriteLog(this.ToString(), ex.ToString(), LogType.Error, out string error);
            }

            return filename;
        }

        public void SettingsSheet(ExcelFile wBook, List<string> columns, string namesheet = "Заявки техникам")
        {
            try
            {
                // Add new worksheet to Excel file.
                ExcelWorksheet w_sheet = wBook.Worksheets.Add(namesheet);

                WriteBidsToExcel(w_sheet, columns.Count);

                //Название колонок------------------------------------------------
                for (int i = 0; i < columns.Count; i++)
                {
                    w_sheet.Cells[0, i].Value = columns[i];
                    w_sheet.Cells[0, i].Style.WrapText = true;
                    w_sheet.Cells[0, i].Style.VerticalAlignment = VerticalAlignmentStyle.Center;
                    w_sheet.Cells[0, i].Style.Borders.SetBorders(MultipleBorders.Outside, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
                    w_sheet.Cells[0, i].Style.Font.Weight = ExcelFont.BoldWeight;

                    if (i == 1) { w_sheet.Columns[i].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center; }
                }

                w_sheet.Rows[0].AutoFit();
            }
            catch (Exception ex)
            { logger.WriteLog(this.ToString(), "[Настройки листа]" + ex.ToString(), LogType.Error, out string error); }
        }

        public void WriteBidsToExcel(ExcelWorksheet w_sheet, int columnsCount)
        {
            try
            {
                if (listTehBids.Count < 1)
                    return;

                int p = 1;

                foreach (var bids in listTehBids)
                {
                    string num = bids.NumObj;
                    if (!String.IsNullOrEmpty(bids.Prefix))
                        num = bids.Prefix + num;

                    w_sheet.Cells[p, 0].Value = bids.DatetimeBid.ToString();
                    w_sheet.Cells[p, 1].Value = num;
                    w_sheet.Cells[p, 2].Value = bids.NameObj;
                    w_sheet.Cells[p, 3].Value = bids.AddressObj;
                    w_sheet.Cells[p, 4].Value = bids.CharacteristicObj;
                    w_sheet.Cells[p, 5].Value = bids.DopInfo;
                    w_sheet.Cells[p, 6].Value = bids.FromWho;

                    // Settings style cell
                    for (int i = 0; i < columnsCount; i++)
                    {
                        w_sheet.Cells[p, i].Style.WrapText = true;
                        w_sheet.Cells[p, i].Style.VerticalAlignment = VerticalAlignmentStyle.Center;
                        w_sheet.Cells[p, i].Style.Borders.SetBorders(MultipleBorders.Outside, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
                    }

                    p++;
                }

                // Ширина колонок
                w_sheet.Columns[0].Width = 19 * 256;
                w_sheet.Columns[1].Width = 10 * 256;
                w_sheet.Columns[2].Width = 30 * 256;
                w_sheet.Columns[3].Width = 30 * 256;
                w_sheet.Columns[4].Width = 30 * 256;
                w_sheet.Columns[5].Width = 30 * 256;
                w_sheet.Columns[6].Width = 20 * 256;
                w_sheet.Columns[7].Width = 20 * 256;
            }
            catch (Exception ex)
            { logger.WriteLog(this.ToString(), "[Заполнение файла данными]" + ex.ToString(), LogType.Error, out string error); }
        }


        public void SettingsSheetProblems(ExcelFile wBook, List<string> columns, string namesheet)
        {
            try
            {
                // Add new worksheet to Excel file.
                ExcelWorksheet w_sheet = wBook.Worksheets.Add(namesheet);

                WriteProblemsToExcel(w_sheet, columns.Count);

                //Название колонок------------------------------------------------
                for (int i = 0; i < columns.Count; i++)
                {
                    w_sheet.Cells[0, i].Value = columns[i];
                    w_sheet.Cells[0, i].Style.WrapText = true;
                    w_sheet.Cells[0, i].Style.VerticalAlignment = VerticalAlignmentStyle.Center;
                    w_sheet.Cells[0, i].Style.Borders.SetBorders(MultipleBorders.Outside, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
                    w_sheet.Cells[0, i].Style.Font.Weight = ExcelFont.BoldWeight;

                    if (i == 1) { w_sheet.Columns[i].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center; }
                }

                w_sheet.Rows[0].AutoFit();
                w_sheet.Panes = new WorksheetPanes(PanesState.Frozen, 0, 1, "A2", PanePosition.BottomLeft);
            }
            catch (Exception ex)
            { logger.WriteLog(this.ToString(), "[Настройки листа]" + ex.ToString(), LogType.Error, out string error); }
        }

        public void WriteProblemsToExcel(ExcelWorksheet w_sheet, int columnsCount)
        {
            try
            {
                if (listProblemDevices.Count < 1)
                    return;

                int p = 1;

                foreach (var device in listProblemDevices)
                {
                    w_sheet.Cells[p, 0].Value = device.NumberDevice;
                    w_sheet.Cells[p, 1].Value = device.AddressDevice;
                    w_sheet.Cells[p, 2].Value = device.V220;
                    w_sheet.Cells[p, 3].Value = device.StatusEth;
                    w_sheet.Cells[p, 4].Value = device.LineEth;
                    w_sheet.Cells[p, 5].Value = device.StateEth;

                    // Settings style cell
                    for (int i = 0; i < columnsCount; i++)
                    {
                        w_sheet.Cells[p, i].Style.WrapText = true;
                        w_sheet.Cells[p, i].Style.VerticalAlignment = VerticalAlignmentStyle.Center;
                        w_sheet.Cells[p, i].Style.Borders.SetBorders(MultipleBorders.Outside, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
                    }

                    p++;
                }

                // Ширина колонок
                w_sheet.Columns[0].Width = 20 * 256;
                w_sheet.Columns[1].Width = 50 * 256;
                w_sheet.Columns[2].Width = 20 * 256;
                w_sheet.Columns[3].Width = 20 * 256;
                w_sheet.Columns[4].Width = 20 * 256;
                w_sheet.Columns[5].Width = 20 * 256;
            }
            catch (Exception ex)
            { logger.WriteLog(this.ToString(), "[Заполнение файла данными]" + ex.ToString(), LogType.Error, out string error); }
        }

        #endregion
    }
}
