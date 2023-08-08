// See https://aka.ms/new-console-template for more information
using Exceptionless;
using ExceptionlessTest.Jobs;
using Microsoft.Extensions.Hosting;
using Quartz;

Host.CreateDefaultBuilder(args)
    .ConfigureLogging(build => build.AddExceptionless())
    .ConfigureServices((hostContext, services) =>
    {
        services.AddExceptionless();
        services.AddQuartz(q =>
        {
            q.ScheduleJob<Job1>(trigger => trigger.StartNow().WithCronSchedule("*/1 * * * * ?"));
            q.ScheduleJob<Job2>(trigger => trigger.StartNow().WithCronSchedule("*/2 * * * * ?"));
            q.ScheduleJob<Job3>(trigger => trigger.StartNow().WithCronSchedule("*/3 * * * * ?"));
            q.ScheduleJob<Job4>(trigger => trigger.StartNow().WithCronSchedule("*/4 * * * * ?"));
            q.ScheduleJob<Job5>(trigger => trigger.StartNow().WithCronSchedule("*/5 * * * * ?"));
            q.ScheduleJob<Job6>(trigger => trigger.StartNow().WithCronSchedule("*/6 * * * * ?"));
            q.ScheduleJob<Job7>(trigger => trigger.StartNow().WithCronSchedule("*/7 * * * * ?"));
            q.ScheduleJob<Job8>(trigger => trigger.StartNow().WithCronSchedule("*/8 * * * * ?"));
            q.ScheduleJob<Job9>(trigger => trigger.StartNow().WithCronSchedule("*/9 * * * * ?"));
            q.ScheduleJob<Job10>(trigger => trigger.StartNow().WithCronSchedule("*/10 * * * * ?"));
            q.ScheduleJob<Job11>(trigger => trigger.StartNow().WithCronSchedule("*/11 * * * * ?"));
            q.ScheduleJob<Job12>(trigger => trigger.StartNow().WithCronSchedule("*/12 * * * * ?"));
            q.ScheduleJob<Job13>(trigger => trigger.StartNow().WithCronSchedule("*/13 * * * * ?"));
        });
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    })
    .UseExceptionless()
    .Build().Run();