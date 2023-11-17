using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using WebApiNet5.Domain.Options;

namespace WebApiNet5
{
    public class DumpWork : IHostedService
    {
        private DumpOption options;
        public DumpWork(IOptions<DumpOption> options)
        {
            this.options = options.Value;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            


        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
