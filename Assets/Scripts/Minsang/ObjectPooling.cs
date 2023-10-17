using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectPooling : MonoBehaviourPun
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    public void PrePoolInstantiate()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = PhotonNetwork.Instantiate(pool.tag, Vector3.zero, Quaternion.identity);
                obj.GetComponent<PhotonView>().RPC("RPCSetActive", RpcTarget.All, false);                   // 풀링할 오브젝트에 넣을 메소드
                objectQueue.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectQueue);
        }
    }

    //[PunRPC]
    //private void RPCSetActive(bool flag)
    //{
    //    gameObject.SetActive(flag);
    //}

    public GameObject PoolInstantiate(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
            return null;

        GameObject obj = poolDictionary[tag].Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        poolDictionary[tag].Enqueue(obj);

        return obj;
        // 이후 오브젝트에서 photonView.RPC("RPCSetActive", RpcTarget.All, true);
    }

    public void PoolDestroy(GameObject obj)
    {
        obj.GetComponent<PhotonView>().RPC("RPCSetActive", RpcTarget.All, false);
    }
}
