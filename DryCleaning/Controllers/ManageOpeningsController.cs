using DryCleaning.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DryCleaning.Controllers
{
    //[Authorize] <- para que no todos puedan acceder a la config
    [ApiController]
    [Route("api/[controller]")]
    public class ManageOpeningsController : ControllerBase
    {
        private /*readonly*/ Scheduler? scheduler; //quitamos el readonly si queremos cargar la configuración del tirón con un JSON
        public ManageOpeningsController(Scheduler s) 
        {
            this.scheduler = s;
        }

        /// <summary>
        /// Actualiza la apertura y cierre de cualquier día
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        [HttpPut("DaysSchedule")]
        [Consumes("application/x-www-form-urlencoded")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateNormalOpen([FromForm] OpenedInterval interval)
        {
            scheduler.NormalOpen = interval;
            return await Task.FromResult(NoContent());
        }

        /// <summary>
        /// Actualiza o añade el intervalo horario de apertura en un día de la semana específico. HAY QUE PONER EL NOMBRE DEL DIA EN INGLÉS
        /// </summary>
        /// <param name="weekday"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        [HttpPut("Days")]
        [Consumes("application/x-www-form-urlencoded")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateWeekDayOpen([FromForm] DayOfWeek weekday, [FromForm] OpenedInterval interval)
        {
            if (scheduler.WeekDayOpen == null)
            {
                scheduler.WeekDayOpen = [];
            }

            scheduler.WeekDayOpen[weekday] = interval;

            return await Task.FromResult(NoContent());
        }

        /// <summary>
        /// Actualiza o añade el intervalo horario de apertura en un día del año específico. Formato yyyy-mm-dd
        /// </summary>
        /// <param name="day"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        [HttpPut("Dates")]
        [Consumes("application/x-www-form-urlencoded")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateYearDayOpen([FromForm] DateOnly day, [FromForm] OpenedInterval interval)
        {
            if (scheduler.YearDayOpen == null)
            {
                scheduler.YearDayOpen = [];
            }

            scheduler.YearDayOpen[day] = interval;

            return await Task.FromResult(NoContent());
        }

        /// <summary>
        /// Actualiza los días de la semana en que el establecimiento estará cerrado. HAY QUE PONER EL NOMBRE DE LOS DIAS EN INGLÉS y separados por comas
        /// </summary>
        /// <param name="weekdays"></param>
        /// <returns></returns>
        [HttpPut("DaysClose")]
        [Consumes("application/x-www-form-urlencoded")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateWeekDaysClose([FromForm] List<DayOfWeek> weekdays)
        {
            scheduler.WeekDaysClose = weekdays;
            return await Task.FromResult(NoContent());
        }

        /// <summary>
        /// Actualiza los días del año en los que el establecimiento estará cerrado. Formato yyyy-mm-dd y separados por comas
        /// </summary>
        /// <param name="dates"></param>
        /// <returns></returns>
        [HttpPut("DatesClose")]
        [Consumes("application/x-www-form-urlencoded")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateYearDaysClose([FromForm] List<DateOnly> dates)
        {
            scheduler.YearDaysClose = dates;
            return await Task.FromResult(NoContent());
        }

        /// <summary>
        /// Entrega el JSON con la configuración actual (no solicitado en el ejercicio pero útil)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ShowConfiguration()
        {
            return await Task.FromResult(Ok(JsonSerializer.Serialize(scheduler)));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> LoadConfiguration([FromBody] string json)
        {
            // Carga del singleton con el json
            var candidate = JsonSerializer.Deserialize<Scheduler>(json);
            if (candidate != null && scheduler != null)
            {
                scheduler.NormalOpen = candidate.NormalOpen;
                scheduler.WeekDayOpen = candidate.WeekDayOpen;
                scheduler.YearDayOpen = candidate.YearDayOpen;
                scheduler.WeekDaysClose = candidate.WeekDaysClose;
                scheduler.YearDaysClose = candidate.YearDaysClose;
            }

            return await Task.FromResult(Created("Loaded", scheduler));
        }
    }
}
