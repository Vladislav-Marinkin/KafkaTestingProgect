namespace Models
{
    public class MessageGenerationRequest
    {
        /// <summary>
        /// Минимальный вес сообщения (в байтах)
        /// </summary>
        public int MinMessageSize { get; set; }

        /// <summary>
        /// Максимальный вес сообщения (в байтах)
        /// </summary>
        public int MaxMessageSize { get; set; }

        /// <summary>
        /// Количество сообщений
        /// </summary>
        public int MessageCount { get; set; }

        /// <summary>
        /// Количество потоков для отправки сообщений
        /// </summary>
        public int ThreadCount { get; set; }

        /// <summary>
        /// Задержка между сообщениями (в миллисекундах)
        /// </summary>
        public int DelayMs { get; set; }
    }
}
