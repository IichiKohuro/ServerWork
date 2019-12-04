namespace ServerWork
{
    /// <summary>
    /// Настройки отправки заявок по почте
    /// </summary>
    public class SendMailInfo
    {
        /// <summary>
        /// Сервер
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Порт
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Почтовый адрес от
        /// </summary>
        public string MailFrom { get; set; }

        /// <summary>
        /// Пароль к почте
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Заголовок письма
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Сообщение письма
        /// </summary>
        public string Message { get; set; }
    }
}
