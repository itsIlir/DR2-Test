using Networking;
using UnityEngine;

namespace Services
{
    public class ServiceClerk : MonoBehaviour
    {
        [SerializeField]
        NetworkService _networkServicePrefab;

        void Awake()
        {
            var networkService = Instantiate(_networkServicePrefab);
            ServiceLocator<INetworkService>.Bind(networkService);
        }
    }
}
