using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Linq;

namespace ServerWork
{
    /// <summary>
    /// Класс для отправки файла по почте
    /// </summary>
    public class SendMail
    {
        #region Private

        /// <summary>
        /// Logger
        /// </summary>
        private LogManager logger;

        /// <summary>
        /// Настройки отправки почты
        /// </summary>
        private SendMailInfo sendMailInfo;

        /// <summary>
        /// Список электронных адресов для сервиса
        /// </summary>
        private List<ServiceMail> serviceMailList;

        /// <summary>
        /// Прикрепленный excel файл заявок
        /// </summary>
        private List<string> attachFiles;

        #endregion


        #region Constructor

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_sendmailinfo">Настройки отправки почты</param>
        /// <param name="_serviceMailList">Список электронных адресов сервиса для отправки</param>
        /// <param name="_attachFiles">Прикрепленные файлы к письму</param>
        public SendMail(SendMailInfo _sendmailinfo, List<ServiceMail> _serviceMailList, List<string> _attachFiles)
        {
            // Указываем логгеру путь к файлу с логами
            logger = new LogManager(AppDomain.CurrentDomain.BaseDirectory + "logs.txt");

            sendMailInfo = _sendmailinfo;
            serviceMailList = _serviceMailList;
            attachFiles = _attachFiles;
        }

        #endregion


        #region Main Methods

        /// <summary>
        /// Отправить заявки по почте
        /// </summary>
        /// <param name="problems">Индикатор проблемных устройств</param>
        /// <returns></returns>
        public bool SendMailService(bool problems = false)
        {
            if (problems == true)
            {
                sendMailInfo.Message = "Проблемные устройства в приложении.";
                sendMailInfo.Caption = "Проблемные устройства на " + DateTime.Now.ToString("dd.MM.yyyy HH-mm");
            }

            // Проверяем файл на пустоту или существует ли он
            if (attachFiles == null)
            {
                foreach (var file in attachFiles)
                {
                    if (!File.Exists(file))
                    {
                        logger.WriteLog(this.ToString(), $"Внимание! Файл ″{file}″ не существует. Отмена отправки!!!", LogType.Warning, out string error);
                        return false;
                    }
                }
            }

            int i = 0;

            var mailToList = serviceMailList.Select(s => s.Email).ToList<string>();
            var mailToListString = mailToList.ToString();

            var result = SendToMail(sendMailInfo, mailToList, attachFiles).GetAwaiter().GetResult();

            if (result)
            {
                logger.WriteLog(this.ToString(), $"Отправка на почту ‴{mailToList.ToString()}‴ - УСПЕШНО", LogType.Information, out string error);
                i++;
            }
            else
                logger.WriteLog(this.ToString(), $"Отправка на почту ‴{mailToList.ToString()}‴ - НЕУДАЧА", LogType.Warning, out string error);

            if (i == serviceMailList.Count)
                return true;
            else
                return false;
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Отправка файла по почте
        /// </summary>
        /// <param name="sendmailinfo">Настройки отправки почты</param>
        /// <param name="mailto">Кому отправить письмо</param>
        /// <param name="attachFiles">Список путей файлов, для отправки по почте</param>
        /// <returns></returns>
        private async Task<bool> SendToMail(SendMailInfo sendmailinfo, List<string> mailto, List<string> attachFiles = null)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(sendmailinfo.MailFrom);

                if (mailto != null)
                {
                    foreach (var _mailTo in mailto)
                    {
                        mail.To.Add(new MailAddress(_mailTo));
                    }
                }
                
                mail.Subject = sendmailinfo.Caption;
                mail.Body = sendmailinfo.Message;
                mail.IsBodyHtml = false;

                if (attachFiles != null)
                {
                    foreach (var file in attachFiles)
                    {
                        mail.Attachments.Add(new Attachment(file));
                    }                  
                }

                SmtpClient client = new SmtpClient();
                client.Host = sendmailinfo.Server;
                client.Port = sendmailinfo.Port;
                client.EnableSsl = true;
                //from.Split('@')[0]
                client.Credentials = new NetworkCredential(sendmailinfo.MailFrom, sendmailinfo.Password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                await client.SendMailAsync(mail);

                mail.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                logger.WriteLog(this.ToString(), ex.ToString(), LogType.Error, out string error);
                return false;
            }
        }

        #endregion
    }
}
