using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ServerWork
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "ServerService" в коде и файле конфигурации.
    public class ServerService : IServerService
    {

        public string SendBids()
        {
            string mess = string.Empty;

            mess = "Отправка заявок произведена успешно";

            return mess;
        }
    }
}
