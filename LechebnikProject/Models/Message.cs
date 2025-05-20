using System;

namespace LechebnikProject.Models
{
    /// <summary>
    /// Модель сообщения.
    /// </summary>
    public class Message
    {
        public int MessageId { get; set; }          // Уникальный идентификатор
        public int SenderId { get; set; }           // Идентификатор отправителя
        public int ReceiverId { get; set; }         // Идентификатор получателя
        public string MessageText { get; set; }     // Текст сообщения
        public DateTime SendDate { get; set; }      // Дата и время отправки
        public string SenderLogin { get; set; }
    }
}
