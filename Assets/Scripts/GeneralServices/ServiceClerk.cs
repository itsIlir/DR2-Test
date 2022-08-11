using DarkRift.Client.Unity;
using Networking;
using UnityEngine;

namespace Services
{
    public class ServiceClerk : MonoBehaviour
    {
        [SerializeField]
        UnityClient _unityDarkRiftClient;

        void Awake()
        {
            var networkService = new NetworkService(_unityDarkRiftClient);
            ServiceLocator<INetworkService>.Bind(networkService);
        }
    }
}
