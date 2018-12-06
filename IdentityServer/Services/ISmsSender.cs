using System.Threading.Tasks;

namespace StsServerIdentity.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}