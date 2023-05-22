using Quartz;

namespace PlanQuinquenal
{
    public class JobInfoHIS : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            // Lógica del trabajo a realizar
            Console.WriteLine("El job se ejecutó a las 7 am.");
            return Task.CompletedTask;
        }
    }
}
