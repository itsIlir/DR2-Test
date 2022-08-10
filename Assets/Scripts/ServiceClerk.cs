using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceClerk : MonoBehaviour
{
    [SerializeField] 
    NetworkService _networkServicePrefab;

    void Awake()
    {
        Debug.Log("AAA ServiceClerk Awake");
        var networkService = Instantiate(_networkServicePrefab);
        ServiceLocator<INetworkService>.Bind(networkService);
    }
}
