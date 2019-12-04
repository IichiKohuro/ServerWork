using System;

namespace ServerWork
{
    /// <summary>
    /// Заявки техникам
    /// </summary>
    public class TehBids
    {
        /// <summary>
        /// Префикс номера ячейки
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Номер ячейки
        /// </summary>
        public string NumObj { get; set; }

        /// <summary>
        /// Название объекта
        /// </summary>
        public string NameObj { get; set; }

        /// <summary>
        /// Адрес объекта
        /// </summary>
        public string AddressObj { get; set; }

        /// <summary>
        /// Характер заявки
        /// </summary>
        public string CharacteristicObj { get; set; }

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public string DopInfo { get; set; }

        /// <summary>
        /// От кого заявка
        /// </summary>
        public string FromWho { get; set; }

        /// <summary>
        /// Дата/Время заявки
        /// </summary>
        public DateTime DatetimeBid { get; set; }

        /// <summary>
        /// Техническое обслуживание
        /// </summary>
        public string TehService { get; set; }
    }
}
