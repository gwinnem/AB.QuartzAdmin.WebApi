using AB.QuartzAdmin.WebApi.Models.Jobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AB.QuartzAdmin.WebApi.Controllers
{
    /// <summary>
    /// Api endpoint for job actions and details.
    /// </summary>
    [Produces("application/json")]
    [Route("api/job")]
    [ApiController]
    public class JobsController : BaseApiController
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logger">The Logger instance.</param>
        /// <param name="scheduler">The <see cref="IScheduler"/> instance.</param>
        public JobsController(ILogger<BaseApiController> logger,
            IScheduler scheduler)
            : base(logger, scheduler)
        { }

        /// <summary>
        /// Getting a list of <see cref="JobListDetail"/> for all configured <see cref="IJob"/> instances in the <see cref="IScheduler"/>.
        /// </summary>
        /// <returns>The list of configured jobs..</returns>
        /// <response code="200">Returns the list of configured jobs for the scheduler..</response>
        /// <response code="500">Returns the internal server error..</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [HttpGet, Route("list")]
        public async Task<IActionResult> AllJobs()
        {
            try
            {
                var keys = (await Scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()))
                    .OrderBy(x => x.ToString());

                var jobList = new List<JobListDetail>();
                foreach(var key in keys)
                {
                    var triggers = await Scheduler.GetTriggersOfJob(key);
                    var nextFires = triggers.Select(x => x.GetNextFireTimeUtc()?.UtcDateTime).ToArray();
                    var lastFires = triggers.Select(x => x.GetPreviousFireTimeUtc()?.UtcDateTime).ToArray();
                    var detail = await Scheduler.GetJobDetail(key);
                    var item = new JobListDetail()
                    {
                        ConcurrentExecutionDissallowed = !detail.ConcurrentExecutionDisallowed,
                        PersistJobDataAfterExecution = detail.PersistJobDataAfterExecution,
                        RequestRecovery = detail.RequestsRecovery,
                        Durable = detail.Durable,
                        Name = key.Name,
                        Group = key.Group,
                        JobType = detail.JobType.FullName,
                        Description = detail.Description,
                        NextFireTimeUtc = nextFires.Where(x => x != null).OrderBy(x => x).FirstOrDefault(),
                        LastFireTimeUtc = lastFires.Where(x => x != null).OrderBy(x => x).LastOrDefault()
                    };
                    jobList.Add(item);
                }

                return Ok(jobList);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Interrupts the execution of a job.
        /// </summary>
        /// <param name="fireInstanceId">The id of the running job instance.</param>
        /// <returns>True if the job is interrupted, otherwise false.</returns>
        /// <response code="200">True if job was interrupted.</response>
        /// <response code="500">Returns the internal server error..</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [HttpPost, Route("{fireInstanceId}/interrupt")]
        public async Task<IActionResult> InterruptJob(string fireInstanceId)
        {
            try
            {
                var result = await Scheduler.Interrupt(fireInstanceId);
                return Ok(result);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "InterruptJob");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Deleting job ín scheduler.
        /// </summary>
        /// <param name="jobGroup">Job Group Name.</param>
        /// <param name="jobName">Job Name.</param>
        /// <returns>Status of the operation.</returns>
        /// <response code="200">True if the job was deleted.</response>
        /// <response code="500">Returns the internal server error..</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [HttpDelete, Route("{jobGroup}/{jobName}/delete")]
        public async Task<IActionResult> DeleteJob(string jobGroup, string jobName)
        {
            try
            {
                var result = await Scheduler.DeleteJob(new JobKey(jobName, jobGroup));
                return Ok(result);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "TriggerJob Name: {jobName} Group: {jobGroup}", jobName, jobGroup);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Triggering job ín scheduler.
        /// </summary>
        /// <param name="jobGroup">Job Group Name.</param>
        /// <param name="jobName">Job Name.</param>
        /// <returns>Status of the operation.</returns>
        /// <response code="204">Success.</response>
        /// <response code="500">Returns the internal server error..</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        [HttpGet, Route("{jobGroup}/{jobName}/trigger")]
        public async Task<IActionResult> TriggerJob(string jobGroup, string jobName)
        {
            try
            {
                await Scheduler.TriggerJob(new JobKey(jobName, jobGroup));
                return NoContent();
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "TriggerJob Name: {jobName} Group: {jobGroup}", jobName, jobGroup);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Getting job details from scheduler.
        /// </summary>
        /// <param name="jobGroup">Job Group Name.</param>
        /// <param name="jobName">Job Name.</param>
        /// <returns>The <see cref="IJobDetail"/>.</returns>
        /// <response code="200">Success.</response>
        /// <response code="500">Returns the internal server error..</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [HttpGet, Route("{jobGroup}/{jobName}/details")]
        public async Task<IActionResult> GetJobConfiguration(string jobGroup, string jobName)
        {
            try
            {
                if(string.IsNullOrEmpty(jobGroup) || string.IsNullOrEmpty(jobName)) return BadRequest("JobName or JobGroup is invalid");

                var jobDetails = await Scheduler.GetJobDetail(new JobKey(jobName, jobGroup));

                return Ok(jobDetails);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "EditJobConfiguration");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Creating a new <see cref="IJob"/>.
        /// </summary>
        /// <param name="model">The Job details.</param>
        /// <returns></returns>
        /// <response code="204">Success.</response>
        /// <response code="500">Returns the internal server error..</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        [HttpPost, Route("create")]
        public async Task<IActionResult> CreateJobConfiguration(JobDetails model)
        {
            return await AddEditJobDetails(model, false);
        }

        /// <summary>
        /// Updating a <see cref="IJob"/>.
        /// </summary>
        /// <param name="model">The Job details.</param>
        /// <returns></returns>
        /// <response code="204">Success.</response>
        /// <response code="500">Returns the internal server error..</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        [HttpPut, Route("update")]
        public async Task<IActionResult> UpdateJobConfiguration(JobDetails model)
        {
            return await AddEditJobDetails(model, true);
        }



        private async Task<IActionResult> AddEditJobDetails(JobDetails model, bool replace)
        {
            try
            {
                var jobDetail = new JobDetailImpl(model.Name,
                    model.Group,
                    Type.GetType(model.JobType),
                    model.Durable,
                    model.RequestsRecovery)
                {
                    Description = model.Description
                };
                jobDetail.Validate();
                await Scheduler.AddJob(jobDetail, replace).ConfigureAwait(false);
                return NoContent();
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "AddEditJobDetails");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}