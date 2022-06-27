using System.Threading.Tasks;

namespace BusinessLibrary.Interfaces
{
    public interface ISocialNetworkClient
    {
        Task Post(string content);
    }
}
