using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Quartz;

namespace AB.QuartzAdmin.WebApi.Controllers
{
    /// <summary>
    /// Base class for all api controllers.
    /// </summary>
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Quartz Net Scheduler Instance.
        /// </summary>
        protected IScheduler Scheduler { get; set; }
        /// <summary>
        /// Logger instance.
        /// </summary>
        protected ILogger<BaseApiController> Logger { get; set; }

        /// <summary>
        /// Default constructor used for DI.
        /// </summary>
        /// <param name="logger">The Common logging instance.</param>
        /// <param name="scheduler">The <see cref="IScheduler"/> instance.</param>
        public BaseApiController(ILogger<BaseApiController> logger, IScheduler scheduler)
        {
            Logger = logger;
            Scheduler = scheduler;
        }
    }
}
