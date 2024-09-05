using DryCleaning.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DryCleaning.Controllers
{
    [ApiController]
    [Route("schedule-calculator")]
    public class ScheduleCalculatorController : ControllerBase
    {
        private readonly Scheduler scheduler;

        public ScheduleCalculatorController(Scheduler s, ILogger<ScheduleCalculatorController> logger)
        {
            this.scheduler = s;
        }

        /// <summary>
        /// Devuelve la fecha y hora en que estará disponible la ropa
        /// </summary>
        /// <param name="minutes">Tiempo necesario para su limpieza en minutos</param>
        /// <param name="date">Fecha y hora de entrega</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int minutes, [FromQuery] DateTime date)
        {
            return await Task.FromResult(Ok(ProcessingDeliveryDate(minutes, date).ToString("ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture)));
        }

        private DateTime ProcessingDeliveryDate(int minutes, DateTime date)
        {
            var currentDate = date;
            var remainingDuration = TimeSpan.FromMinutes(minutes);

            while (remainingDuration > TimeSpan.Zero)
            {
                var dayOfWeek = currentDate.DayOfWeek;
                var dateOnly = DateOnly.FromDateTime(currentDate);

                if ((scheduler.WeekDaysClose != null && scheduler.WeekDaysClose.Contains(dayOfWeek)) ||
                    (scheduler.YearDaysClose != null && scheduler.YearDaysClose.Contains(dateOnly)))
                {
                    currentDate = currentDate.AddDays(1).Date; // Hasta las 0:00 del día siguiente
                    continue;
                }

                // Respetando el intervalo preferente en caso de coincidir
                var interval = scheduler.YearDayOpen?.GetValueOrDefault(dateOnly);
                if (interval == null)
                {
                    interval = scheduler.WeekDayOpen?.GetValueOrDefault(dayOfWeek);
                }
                if (interval == null)
                {
                    interval = scheduler.NormalOpen;
                }

                if (interval != null)
                {
                    var openingTime = currentDate.Date + interval.Open.ToTimeSpan();
                    var closingTime = currentDate.Date + interval.Close.ToTimeSpan();

                    if (currentDate < openingTime)
                    {
                        currentDate = openingTime;
                    }

                    var availableTimeBeforeClosing = closingTime - currentDate;

                    if (availableTimeBeforeClosing >= remainingDuration)
                    {
                        return currentDate + remainingDuration;
                    }

                    remainingDuration -= availableTimeBeforeClosing;
                }

                currentDate = currentDate.AddDays(1).Date;
            }

            return currentDate;
        }
    }
}
