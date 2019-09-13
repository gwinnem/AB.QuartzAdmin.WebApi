using AB.QuartzAdmin.WebApi.Models.Scheduler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AB.QuartzAdmin.WebApi.Controllers
{
    /// <summary>
    /// Api endpoint for Scheduler actions.
    /// </summary>
    [Produces("application/json")]
    [Route("api/scheduler")]
    [ApiController]
    public class SchedulerController : BaseApiController
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logger">The Logger instance.</param>
        /// <param name="scheduler">The <see cref="IScheduler"/> instance.</param>
        public SchedulerController(ILogger<SchedulerController> logger,
            IScheduler scheduler)
            : base(logger, scheduler)
        { }

        /// <summary>
        /// Getting meta data for a Scheduler.
        /// </summary>
        /// <returns>The Scheduler meta data.</returns>
        /// <response code="200">Returns the metadata for the scheduler.</response>
        /// <response code="500">Returns the internal server error..</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [HttpGet, Route("metadata")]
        public async Task<IActionResult> GetSchedulerMetaData()
        {
            try
            {
                var metaData = await Scheduler.GetMetaData().ConfigureAwait(false);
                return Ok(new SchedulerDetails(Scheduler, metaData));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Starting the Quartz Scheduler Scheduler.
        /// </summary>
        /// <returns>The Status of the operation.</returns>
        /// <response code="204">Ok.</response>
        /// <response code="500">Returns the internal server error..</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        [HttpPost]
        [Route("start")]
        public async Task<IActionResult> StartScheduler(int? delayMilliseconds = null)
        {
            try
            {
                if (delayMilliseconds == null)
                {
                    await Scheduler.Start().ConfigureAwait(false);
                }
                else
                {
                    await Scheduler.StartDelayed(TimeSpan.FromMilliseconds(delayMilliseconds.Value)).ConfigureAwait(false);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Pausing the Quartz Scheduler Scheduler.
        /// </summary>
        /// <returns>The Status of the operation.</returns>
        /// <response code="204">Ok.</response>
        /// <response code="500">Returns the internal server error..</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        [HttpPost]
        [Route("pause")]
        public async Task<IActionResult> PauseScheduler()
        {
            try
            {
                await Scheduler.Standby().ConfigureAwait(false);
                return NoContent();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Clears all Jobs and Triggers from the Scheduler job store.
        /// </summary>
        /// <returns>The Status of the operation.</returns>
        /// <response code="204">Ok.</response>
        /// <response code="500">Returns the internal server error..</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        [HttpDelete]
        [Route("clear")]
        public async Task<IActionResult> ClearScheduler()
        {
            try
            {
                await Scheduler.Clear().ConfigureAwait(false);
                return NoContent();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Shutting down the Quartz Scheduler Scheduler.
        /// </summary>
        /// <param name="waitForJobsToComplete">If set to true the scheduler will let running jobs complete before shutting down.</param>
        /// <returns>The Status of the operation.</returns>
        /// <response code="204">Ok.</response>
        /// <response code="500">Returns the internal server error..</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        [HttpPost]
        [Route("shutdown")]
        public async Task<IActionResult> ShutDownScheduler(bool waitForJobsToComplete = false)
        {
            try
            {
                await Scheduler.Shutdown(waitForJobsToComplete).ConfigureAwait(false);
                return NoContent();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Getting a list of <see cref="ExecutingJobContext"/> objects representing all currently executing Jobs in the Scheduler.
        /// </summary>
        /// <returns>The Status of the operation.</returns>
        /// <response code="200">The list of <see cref="ExecutingJobContext"/> objects.</response>
        /// <response code="500">Returns the internal server error..</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [HttpGet]
        [Route("jobs-executing")]
        public async Task<IActionResult> GetCurrentExecutingJobs()
        {
            try
            {
                await Scheduler.Clear().ConfigureAwait(false);
                var metaData = await Scheduler.GetCurrentlyExecutingJobs();
                var model = metaData
                    .Select(context => new ExecutingJobContext(context))
                    .ToList();

                return Ok(model);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Pausing all jobs in group.
        /// </summary>
        /// <param name="groupName">Job Group Name.</param>
        /// <returns>The Status of the operation.</returns>
        /// <response code="204">Ok.</response>
        /// <response code="500">Returns the internal server error..</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        [HttpPost, Route("jobs/{groupName}/pause-all")]
        public async Task<IActionResult> PauseAllJobsInGroup(string groupName)
        {
            try
            {
                await Scheduler.PauseJobs(GroupMatcher<JobKey>.GroupEquals(groupName));
                return NoContent();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Resuming all jobs in group.
        /// </summary>
        /// <param name="groupName">Job Group Name.</param>
        /// <returns>The Status of the operation.</returns>
        /// <response code="204">Ok.</response>
        /// <response code="500">Returns the internal server error..</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        [HttpPost, Route("jobs/{groupName}/resume-all")]
        public async Task<IActionResult> ResumeAllJobsInGroup(string groupName)
        {
            try
            {
                await Scheduler.ResumeJobs(GroupMatcher<JobKey>.GroupEquals(groupName));
                return NoContent();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
