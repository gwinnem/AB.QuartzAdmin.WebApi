<h1 align="center">AB Quartz Admin Web API</h3>

---
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

AspNEt Core WebApi for the Quartz.Net Scheduler.

The api can easily be plugged into a existing application.

> [Quartz.NET](https://www.quartz-scheduler.net) is a full-featured, open source job scheduling system that can be used from smallest apps to large scale enterprise systems.



## Endpoints
- Scheduler
![Image of actions](https://github.com/gwinnem/AB.QuartzAdmin.WebApi/blob/master/images/SchedulerApiEndpoints.png)
- Jobs
![Image of actions](https://github.com/gwinnem/AB.QuartzAdmin.WebApi/blob/master/images/JobsApiEndpoints.png)
- Triggers - Coming
- Calendars - Coming

## Install

## Minimum requirements
- .NET Standard 2.2

## Usage
```aspx-csharp
...
// Setting up a DemoScheduler
var properties = new NameValueCollection
{
    {"quartz.serializer.type", "json"},
    {"quartz.scheduler.instanceName", "TestScheduler"},
    {"quartz.scheduler.instanceId", "ABQuartzAdmin"},
    {"quartz.threadPool.type", "Quartz.Simpl.SimpleThreadPool, Quartz"},
    {"quartz.threadPool.threadCount", "10"}
};

ISchedulerFactory sf = new StdSchedulerFactory(properties);
var scheduler = sf.GetScheduler().GetAwaiter().GetResult();
services.AddSingleton(scheduler);
scheduler.Clear();
DemoScheduler.Create(scheduler, true).GetAwaiter().GetResult();

// Adding the Api
services.AddQuartzAdmin(scheduler);
...
```


## Notes
The example project has swagger installed so just add /swagger to the url and the swagger document will be loaded.


## License
This project is made available under the MIT license. See [LICENSE](LICENSE) for details.
