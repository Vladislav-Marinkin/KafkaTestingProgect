using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class KafkaMessage
    {
        /// <summary>
        /// Номер потока, который сгенерировал сообщение.
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Дата и время генерации сообщения.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Порядковый номер сообщения.
        /// </summary>
        public int SequenceNumber { get; set; }

        /// <summary>
        /// Содержимое сообщения.
        /// </summary>
        public string MessageContent { get; set; }

        /// <summary>
        /// Размер сообщения в килобайтах.
        /// </summary>
        public double MessageSizeKb => CalculateMessageSizeInKb();

        /// <summary>
        /// Вычисляет размер сообщения в килобайтах.
        /// </summary>
        /// <returns>Размер в килобайтах</returns>
        private double CalculateMessageSizeInKb()
        {
            var byteCount = System.Text.Encoding.UTF8.GetByteCount(MessageContent);
            return byteCount / 1024.0; // Конвертация в килобайты
        }
    }
}
