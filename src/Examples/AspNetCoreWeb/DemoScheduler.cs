using Quartz;
using Quartz.Impl.Calendar;
using Quartz.Impl.Matchers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AspNetCoreWeb
{
    /// <summary>
    /// Helper for setting up a demo scheduler with jobs and triggers.
    /// </summary>
    public static class DemoScheduler
    {
        /// <summary>
        /// Creating the Demo Scheduler.
        /// </summary>
        /// <param name="scheduler">The <see cref="IScheduler"/></param>
        /// <param name="start">If true it starts the scheduler. If not it has to be started manually.</param>
        /// <returns>The configured <see cref="IScheduler"/></returns>
        public static async Task<IScheduler> Create(IScheduler scheduler, bool start = true)
        {
            await scheduler.Clear();
            await scheduler.AddCalendar(typeof(AnnualCalendar).Name, new AnnualCalendar(), true, true);
            await scheduler.AddCalendar(typeof(CronCalendar).Name, new CronCalendar("0 0/5 * * * ?"), false, false);
            await scheduler.AddCalendar("AfterBusinessHours", new DailyCalendar("18:00", "23:59"), true, true);
            await scheduler.AddCalendar("BeforeBusinessHours", new DailyCalendar("00:00", "12:00"), true, true);
            await scheduler.AddCalendar(typeof(HolidayCalendar).Name, new HolidayCalendar(), false, false);
            await scheduler.AddCalendar(typeof(MonthlyCalendar).Name, new MonthlyCalendar(), false, false);
            await scheduler.AddCalendar(typeof(WeeklyCalendar).Name, new WeeklyCalendar(), false, false);

            {
                var jobData = new JobDataMap();
                jobData.Put("DateFrom", DateTime.Now);
                jobData.Put("QuartzAssembly", File.ReadAllBytes(typeof(IScheduler).Assembly.Location));

                var job = JobBuilder.Create<DummyJob>()
                    .WithIdentity("Sales", "REPORTS")
                    .WithDescription("Hello Job!")
                    .UsingJobData(jobData)
                    .StoreDurably()
                    .Build();
                var trigger = TriggerBuilder.Create()
                    .WithIdentity("MorningSales")
                    .WithDescription("Generate Sales Report Every Morning")
                    .StartNow()
                    .WithCronSchedule("0 0 8 1/1 * ? *")
                    .Build();
                await scheduler.ScheduleJob(job, trigger);

                trigger = TriggerBuilder.Create()
                    .WithIdentity("MonthlySales")
                    .WithDescription("Generate Sales Report Every Month")
                    .ForJob(job.Key)
                    .StartNow()
                    .WithCronSchedule("0 0 12 1 1/1 ? *")
                    .Build();
                await scheduler.ScheduleJob(trigger);
                await scheduler.PauseTrigger(trigger.Key);

                trigger = TriggerBuilder.Create()
                    .WithIdentity("HourlySales")
                    .WithDescription("Generate Sales Report Every Hour")
                    .ForJob(job.Key)
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever())
                    .Build();
                await scheduler.ScheduleJob(trigger);
            }

            {
                var job = JobBuilder.Create<DummyJob>().WithIdentity("Job1").StoreDurably().Build();
                await scheduler.AddJob(job, false);
                job = JobBuilder.Create<DummyJob>().WithIdentity("Job2").StoreDurably().Build();
                await scheduler.AddJob(job, false);
                job = JobBuilder.Create<DummyJob>().WithIdentity("Job3").StoreDurably().Build();
                await scheduler.AddJob(job, false);
                job = JobBuilder.Create<DummyJob>().WithIdentity("Job4").StoreDurably().Build();
                await scheduler.AddJob(job, false);
                job = JobBuilder.Create<DummyJob>().WithIdentity("Job5").StoreDurably().Build();
                await scheduler.AddJob(job, false);

                var jobData = new JobDataMap();
                jobData.Put("DateFrom", DateTime.Now);
                jobData.Put("Developer", "Geirr Winnem");
                job = JobBuilder.Create<DummyJob>()
                    .UsingJobData(jobData)
                    .WithIdentity("Send SMS", "CRITICAL")
                    .StoreDurably()
                    .RequestRecovery()
                    .Build();
                await scheduler.AddJob(job, false);

                var trigger = TriggerBuilder.Create()
                    .WithIdentity("PushAds  (US)")
                    .WithDescription("Pushing us ads every 5 minutes.")
                    .ForJob(job.Key)
                    .UsingJobData("Location", "US")
                    .StartNow()
                    .WithCronSchedule("0 0/5 * 1/1 * ? *")
                    .Build();
                await scheduler.ScheduleJob(trigger);

                trigger = TriggerBuilder.Create()
                    .WithIdentity("PushAds (EU)")
                    .WithDescription("Pushing us ads every 7 minutes.")
                    .ForJob(job.Key)
                    .UsingJobData("Location", "EU")
                    .StartNow()
                    .WithCronSchedule("0 0/7 * 1/1 * ? *")
                    .Build();
                await scheduler.ScheduleJob(trigger);
                await scheduler.PauseTriggers(GroupMatcher<TriggerKey>.GroupEquals("LONGRUNNING"));

                job = JobBuilder.Create<DummyJob>().WithIdentity("Send Push", "CRITICAL").StoreDurably().RequestRecovery().Build();
                await scheduler.AddJob(job, false);
            }

            {
                var job = JobBuilder.Create<DisallowConcurrentJob>()
                    .WithIdentity("Load CSV", "IMPORT")
                    .WithDescription("Importing sales data from CSV exports.")
                    .StoreDurably()
                    .Build();
                var trigger = TriggerBuilder.Create()
                    .WithIdentity("CSV_small", "FREQUENTLY")
                    .ForJob(job)
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever())
                    .Build();
                await scheduler.ScheduleJob(job, trigger);
                trigger = TriggerBuilder.Create()
                    .WithIdentity("CSV_big", "LONGRUNNING")
                    .ForJob(job)
                    .StartNow()
                    .WithDailyTimeIntervalSchedule(x => x.OnMondayThroughFriday())
                    .Build();
                await scheduler.ScheduleJob(trigger);
            }

            if (start)
                await scheduler.Start();

            return scheduler;
        }

        public class DummyJob : IJob
        {
            private static readonly Random Random = new Random();

            public async Task Execute(IJobExecutionContext context)
            {
                Debug.WriteLine("DummyJob > " + DateTime.Now);

                await Task.Delay(TimeSpan.FromSeconds(Random.Next(1, 20)));

                if (Random.Next(2) == 0)
                    throw new Exception("Fatal error example!");
            }
        }

        [DisallowConcurrentExecution, PersistJobDataAfterExecution]
        public class DisallowConcurrentJob : IJob
        {
            private static readonly Random Random = new Random();

            public async Task Execute(IJobExecutionContext context)
            {
                Debug.WriteLine("DisallowConcurrentJob > " + DateTime.Now);

                context.JobDetail.JobDataMap.Put("LastExecuted", DateTime.Now);

                await Task.Delay(TimeSpan.FromSeconds(Random.Next(1, 5)));

                if (Random.Next(4) == 0)
                    throw new Exception("Fatal error example!");
            }
        }
    }
}
