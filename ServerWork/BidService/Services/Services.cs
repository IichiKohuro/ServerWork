using System.Collections.Generic;

namespace ServerWork
{
    /// <summary>
    /// Техническое обслуживание
    /// </summary>
    public class Services
    {
        /// <summary>
        /// ID тех. обслуживания
        /// </summary>
        public int IdService { get; set; }

        /// <summary>
        /// Название тех. обслуживания
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Количество дней за которое будет формироваться список неотработанных заявок
        /// </summary>
        public int CountDays { get; set; }

        /// <summary>
        /// Отправлять ли заявки
        /// </summary>
        public bool IsSendEnabled { get; set; }

        /// <summary>
        /// Список электронных адресов сервиса
        /// </summary>
        public List<ServiceMail> ServiceMailList { get; set; }

        /// <summary>
        /// Список времени отправки заявок сервису
        /// </summary>
        public List<ServiceSendTimes> ServiceSendTimeList { get; set; }
    }
}
