using System.Threading;
using System.Threading.Tasks;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

public class MikrotikDhcpLease : IMikrotikEntity, IMikrotikDeletableEntity
{
    public MikrotikClient Client { get; }

    public MikrotikDhcpLease(MikrotikClient client)
    {
        this.Client = client;
    }
    
    public Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}