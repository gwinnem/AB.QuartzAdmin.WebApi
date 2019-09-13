using AB.QuartzAdmin.WebApi.Models.Jobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AB.QuartzAdmin.WebApi.Controllers
{
    /// <summary>
    /// Api endpoint for a job actions.
    /// </summary>
    [Produces ( "application/json" )]
    [Route ( "api/jobs" )]
    [ApiController]
    public class JobsController : BaseApiController
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logger">The Logger instance.</param>
        /// <param name="scheduler">The <see cref="IScheduler"/> instance.</param>
        public JobsController ( ILogger<BaseApiController> logger,
            IScheduler scheduler )
            : base ( logger, scheduler )
        {
        }

        /// <summary>
        /// Getting a list of <see cref="JobListDetail"/> for all configured <see cref="IJob"/> instances in the <see cref="IScheduler"/>.
        /// </summary>
        /// <returns>The list of configured jobs..</returns>
        /// <response code="200">Returns the list of configured jobs for the scheduler..</response>
        /// <response code="500">Returns the internal server error..</response>
        [ProducesResponseType ( 200 )]
        [ProducesResponseType ( 500 )]
        [HttpGet, Route ( "list" )]
        public async Task<IActionResult> AllJobs ()
        {
            try
            {
                var keys = ( await Scheduler.GetJobKeys ( GroupMatcher<JobKey>.AnyGroup () ) ).OrderBy ( x => x.ToString () );
                var jobList = new List<JobListDetail> ();
                foreach (var key in keys)
                {
                    var triggers = await Scheduler.GetTriggersOfJob ( key );
                    var nextFires = triggers.Select ( x => x.GetNextFireTimeUtc ()?.UtcDateTime ).ToArray ();
                    var lastFires = triggers.Select ( x => x.GetPreviousFireTimeUtc ()?.UtcDateTime ).ToArray ();
                    var detail = await Scheduler.GetJobDetail ( key );
                    var item = new JobListDetail ()
                    {
                        ConcurrentExecutionDissallowed = !detail.ConcurrentExecutionDisallowed,
                        PersistJobDataAfterExecution = detail.PersistJobDataAfterExecution,
                        RequestRecovery = detail.RequestsRecovery,
                        Durable = detail.Durable,
                        Name = key.Name,
                        Group = key.Group,
                        JobType = detail.JobType.FullName,
                        Description = detail.Description,
                        NextFireTimeUtc = nextFires.Where ( x => x != null ).OrderBy ( x => x ).FirstOrDefault (),
                        LastFireTimeUtc = lastFires.Where ( x => x != null ).OrderBy ( x => x ).LastOrDefault ()
                    };
                    jobList.Add ( item );
                }

                return Ok ( jobList );
            }
            catch (Exception ex)
            {
                Logger.LogError ( ex, ex.Message );
                return StatusCode ( StatusCodes.Status500InternalServerError, ex.Message );
            }
        }
    }
}