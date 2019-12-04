using System.ServiceModel;
using System.Threading.Tasks;

namespace ServerWork
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IServerService" в коде и файле конфигурации.
    [ServiceContract]
    public interface IServerService
    {
        [OperationContract]
        void Monitoring();

        [OperationContract]
        void SendServicesBidsOnRequest();
    }
}
