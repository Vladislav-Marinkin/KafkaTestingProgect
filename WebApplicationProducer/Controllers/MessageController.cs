using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace WebApplicationProducer.Controllers
{
    public class MessageController : ControllerBase
    {
        private readonly MessageGenerator _messageGenerator;

        public MessageController(MessageGenerator messageGenerator)
        {
            _messageGenerator = messageGenerator;
        }

        /// <summary>
        /// Запускает генерацию сообщений в Kafka.
        /// </summary>
        /// <param name="request">Условия генерации сообщений</param>
        /// <returns></returns>
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateMessages([FromBody] MessageGenerationRequest request)
        {
            if (request.MinMessageSize <= 0 || request.MaxMessageSize <= 0 || request.MinMessageSize > request.MaxMessageSize)
            {
                return BadRequest("Некорректный диапазон размеров сообщений.");
            }

            if (request.MessageCount <= 0 || request.ThreadCount <= 0 || request.DelayMs < 0)
            {
                return BadRequest("Некорректные параметры генерации.");
            }

            try
            {
                await _messageGenerator.GenerateMessagesAsync(request);
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message, data = e.Data, stackTrace = e.StackTrace });
            }

            return Ok("Сообщения успешно сгенерированы.");
        }
    }
}
