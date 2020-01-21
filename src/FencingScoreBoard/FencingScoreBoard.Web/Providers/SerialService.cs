using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace FencingScoreBoard.Web.Providers
{
    public class SerialService : IHostedService
    {
        private readonly ISerialPipe _serial;

        public SerialService(
            ISerialPipe serial
            )
        {
            _serial = serial;
        }

        CancellationTokenSource _cancellationTokenSouce = new CancellationTokenSource();
        Task _task;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _task = Task.Run(async () => await _serial.Start(_cancellationTokenSouce.Token));
            return Task.FromResult(0);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSouce.Cancel(false);
            return _task;
        }
    }
}
