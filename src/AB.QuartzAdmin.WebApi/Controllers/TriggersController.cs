using AB.QuartzAdmin.WebApi.Models.Jobs;
using AB.QuartzAdmin.WebApi.Models.Triggers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AB.QuartzAdmin.WebApi.Controllers
{
    /// <summary>
    /// Api endpoint for trigger actions and details.
    /// </summary>
    [Produces("application/json")]
    [Route("api/trigger")]
    [ApiController]
    public class TriggersController : BaseApiController
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logger">The Logger instance.</param>
        /// <param name="scheduler">The <see cref="IScheduler"/> instance.</param>
        public TriggersController(ILogger<BaseApiController> logger,
            IScheduler scheduler)
            : base(logger, scheduler)
        { }

        /// <summary>
        /// Getting a list of <see cref="JobListDetail"/> for all configured <see cref="IJob"/> instances in the <see cref="IScheduler"/>.
        /// </summary>
        /// <returns>The list of configured triggers.</returns>
        /// <response code="200">Returns the list of configured triggers for the scheduler..</response>
        /// <response code="500">Returns the internal server error..</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [HttpGet, Route("list")]
        public async Task<IActionResult> AllTriggers()
        {
            try
            {
                var keys = (await Scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup())).OrderBy(x => x.ToString());
                var list = new List<TriggerListItem>();

                foreach(var key in keys)
                {
                    var t = await GetTrigger(key, Scheduler);
                    var state = await Scheduler.GetTriggerState(key);

                    list.Add(new TriggerListItem()
                    {
                        Type = t.GetTriggerType(),
                        TriggerName = t.Key.Name,
                        TriggerGroup = t.Key.Group,
                        IsPaused = state == TriggerState.Paused,
                        ScheduleDescription = t.GetScheduleDescription(),
                        StartTime = t.StartTimeUtc.UtcDateTime.ToString(CultureInfo.InvariantCulture),
                        EndTime = t.FinalFireTimeUtc?.UtcDateTime.ToString(CultureInfo.InvariantCulture),
                        LastFireTime = t.GetPreviousFireTimeUtc()?.UtcDateTime.ToString(CultureInfo.InvariantCulture),
                        NextFireTime = t.GetNextFireTimeUtc()?.UtcDateTime.ToString(CultureInfo.InvariantCulture),
                        Description = t.Description,
                    });
                }
                return Ok(list);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        private static async Task<ITrigger> GetTrigger(TriggerKey key, IScheduler scheduler)
        {
            var trigger = await scheduler.GetTrigger(key);

            if(trigger == null)
                throw new InvalidOperationException("Trigger " + key + " not found.");

            return trigger;
        }
    }
}
