using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ServerWork
{
    public static class ExportToJson
    {
        /// <summary>
        /// Экспорт данных проблемных устройств РНКБ в файл JSON, в указанную папку
        /// </summary>
        /// <param name="_list">Список данных проблемных устройств РНКБ</param>
        /// <param name="exportFolder">Папка куда будет сохранён файл JSON с данными проблемных устройств РНКБ</param>
        /// <returns>Возвращает путь к сгенерируемому файлу JSON</returns>
        public static string Export(List<ProblemDevice> _list, string exportFolder, string nameFile = "zabbix")
        {
            string result = ""; 

            if (_list == null || _list.Count() < 1)
                return result;

            foreach (var item in _list)
            {
                item.V220      = item.V220.Equals("-")      ? "2" : (item.V220.Equals("ДА")      ? "1" : "0");
                item.StatusEth = item.StatusEth.Equals("-") ? "2" : (item.StatusEth.Equals("ДА") ? "1" : "0");
                item.StateEth  = item.StateEth.Equals("-")  ? "2" : (item.StateEth.Equals("ДА")  ? "1" : "0");
                item.LineEth   = item.LineEth.Equals("-")   ? "2" : (item.LineEth.Equals("ДА")   ? "1" : "0");
            }

            JsonObject jsonObject = new JsonObject()
            {
                data = _list
            };

            result = exportFolder + @"\Проблемные устройства РНКБ\" + nameFile + " [" + DateTime.Now.ToString("dd.MM.yyyy HH-mm") + "].json";

            File.WriteAllText(result, JsonConvert.SerializeObject(jsonObject, Formatting.Indented));
            

            return result;
        }
    }

    public class JsonObject
    {
        public List<ProblemDevice> data { get; set; }
    }
}
