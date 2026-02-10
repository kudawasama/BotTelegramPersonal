using Microsoft.AspNetCore.Mvc;
using BotTelegram.Services;
using BotTelegram.Models;

namespace BotTelegram.API
{
    [ApiController]
    [Route("api/reminders")]
    public class RemindersController : ControllerBase
    {
        private readonly ReminderService _service = new();

        [HttpGet]
        public IActionResult GetAll()
        {
            var reminders = _service.GetAll();
            return Ok(reminders);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var reminders = _service.GetAll();
            var reminder = reminders.FirstOrDefault(r => r.Id == id);
            
            if (reminder == null)
                return NotFound($"Recordatorio {id} no encontrado");

            return Ok(reminder);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var reminders = _service.GetAll();
            var reminder = reminders.FirstOrDefault(r => r.Id == id);
            
            if (reminder == null)
                return NotFound($"Recordatorio {id} no encontrado");

            reminders.Remove(reminder);
            _service.UpdateAll(reminders);

            return Ok($"Recordatorio {id} eliminado");
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] Reminder updated)
        {
            var reminders = _service.GetAll();
            var reminder = reminders.FirstOrDefault(r => r.Id == id);
            
            if (reminder == null)
                return NotFound($"Recordatorio {id} no encontrado");

            reminder.Text = updated.Text;
            reminder.DueAt = updated.DueAt;
            reminder.Recurrence = updated.Recurrence;

            _service.UpdateAll(reminders);

            return Ok(reminder);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Reminder reminder)
        {
            var reminders = _service.GetAll();
            reminder.Id = Guid.NewGuid().ToString("N").Substring(0, 8);
            reminder.CreatedAt = DateTimeOffset.Now;
            reminders.Add(reminder);
            _service.UpdateAll(reminders);

            return Ok(reminder);
        }
    }
}
