using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChipSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public int numTag;
        public GameObject prefab;
        public int size;
    }

    public int playerZCoord;
    public List<Pool> pools;
    public List<GameObject> Chips = new List<GameObject>();
    public Dictionary<int, Queue<GameObject>> poolDictionary;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            playerZCoord = 30;
            Camera.main.transform.position = new Vector3((float)0, (float)75.7, (float)38.1);
            Camera.main.transform.rotation = Quaternion.Euler(70, 180, 0);
        }
        else
        {
            playerZCoord = -30;
        }

        poolDictionary = new Dictionary<int, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                if (PhotonNetwork.IsConnected && PhotonNetwork.PlayerList.Length > 1)
                {
                    GameObject obj = PhotonNetwork.Instantiate(pool.prefab.name, new Vector3(0, 0, 0), Quaternion.identity);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                    obj.transform.parent = gameObject.transform;
                }
                else
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                    obj.transform.parent = gameObject.transform;
                }
            }

            poolDictionary.Add(pool.numTag, objectPool);
        }
    }

    public void SpawnChip(int index, int spawnAmount)
    {
        int counter = 0;
        int newPileShift = 0;

        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject newChip = poolDictionary[index].Dequeue();
            newChip.SetActive(true);

            if (PhotonNetwork.IsConnected)
            {
                NetworkPlayer.LocalPlayer.EnableNetworkChip(newChip.GetComponent<PhotonView>().ViewID);
            }

            if (counter >= 10)
            {
                counter = 0;
                newPileShift += 5;
            }

            newChip.transform.position = new Vector3((float)-22.5 + index * 5, (float)counter + 2, (float)playerZCoord + newPileShift);
            newChip.transform.rotation = Quaternion.identity;

            poolDictionary[index].Enqueue(newChip);

            counter++;
        }
    }
}
