using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLibrary
{
    public interface IMailProcessingService
    {
        Task ConnectAsync(string imapServer, int port, bool useSsl, CancellationToken cancellationToken);

        Task<List<string>> GetUnreadUidsAsync(string username, string password, CancellationToken cancellationToken);

        Task ProcessMail(string uid, string subjectFilter);

        Task DisconnectAsync(CancellationToken cancellationToken);
    }
}
