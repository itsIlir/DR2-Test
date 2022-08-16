using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DarkRift.Client;
using GameModels;

namespace Networking
{
    public interface INetworkService
    {
        DarkRiftClient Client { get; }
        MessageProcessor<T> GetProcessor<T>() where T : struct, INetworkData;
        Task Connect(IPAddress ip, int port);
        void Disconnect();
        void SendMessage<T>(T networkMessage) where T : struct, INetworkData;
        void SendMessages<T>(IEnumerable<T> networkMessages) where T : struct, INetworkData;
    }
}
