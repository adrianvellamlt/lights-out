using System.Threading;
using System.Threading.Tasks;

namespace LightsOut.Web
{
    public interface IBootstrapper
    {
        Task BootstrapAsync(CancellationToken cancellationToken);
    }
}