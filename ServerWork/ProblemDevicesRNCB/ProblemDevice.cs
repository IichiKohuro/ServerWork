using Newtonsoft.Json;

namespace ServerWork
{
    /// <summary>
    /// Проблемные устройства РНКБ
    /// </summary>
    public class ProblemDevice
    {
        /// <summary>
        /// Номер устройства РНКБ
        /// </summary>
        [JsonProperty("device_id")]
        public int NumberDevice { get; set; }

        /// <summary>
        /// Адрес устройства РНКБ
        /// </summary>
        [JsonProperty("device_name")]
        public string AddressDevice { get; set; }

        /// <summary>
        /// Наличие 220В (ДА/ НЕТ)
        /// </summary>
        [JsonProperty("220 input")]
        public string V220 { get; set; }

        /// <summary>
        /// Наличие канала ETHERNET  (ДА/ НЕТ)
        /// </summary>
        [JsonProperty("ETHERNET")]
        public string StatusEth { get; set; }

        /// <summary>
        /// Работает ли канал ETHERNET (ДА/ НЕТ)
        /// </summary>
        [JsonProperty("Work ETHERNET")]
        public string StateEth { get; set; }

        /// <summary>
        /// Норма ETHERNET линии ("физика") (ДА/ НЕТ)
        /// </summary>
        [JsonProperty("Phisic ETHERNET")]
        public string LineEth { get; set; }
    }
}
