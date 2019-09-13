using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Quartz;

namespace AB.QuartzAdmin.WebApi.Controllers
{
    /// <summary>
    /// Api endpoint for a job actions.
    /// </summary>
    [Produces("application/json")]
    [Route("api/scheduler")]
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


    }
}