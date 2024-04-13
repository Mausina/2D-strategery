using UnityEngine;
using Mirror;
using TMPro;

public class GameManager : NetworkBehaviour
{
    //[SyncVar] ����������� ������������ ��� �������
    [SyncVar]
    public int globalCoins;

    public TMP_Text globalCoinsText;
    public TMP_Text coinsText;

    [SerializeField]
    private GameObject coinPrefab;

    private void Start()
    {
        if (isServer)
        {
            //SpawnCoin();
        }
    }

    public void StopGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();
        }
    }

    public void SpawnCoin()
    {
        GameObject prefab = Instantiate(coinPrefab, transform.position, Quaternion.identity);
        //������� ��� ������ ���� ���� �����, ��� �� ������ ��� ��� �볺��� 
        NetworkServer.Spawn(prefab);
    }
}
