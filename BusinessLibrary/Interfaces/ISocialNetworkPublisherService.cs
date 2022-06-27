using System.Threading.Tasks;

namespace BusinessLibrary.Interfaces
{
    public interface ISocialNetworkPublisherService
    {
        Task Publish(string content);
    }
}
