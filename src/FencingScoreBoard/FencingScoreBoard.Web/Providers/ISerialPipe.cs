using System.Threading;
using System.Threading.Tasks;

namespace FencingScoreBoard.Web.Providers
{
    public interface ISerialPipe
    {
        Task Start(CancellationToken cancellationToken);
    }
}