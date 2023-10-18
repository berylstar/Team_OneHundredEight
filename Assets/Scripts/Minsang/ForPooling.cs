using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ForPooling : MonoBehaviour
{
    [PunRPC]
    public void RPCSetActive(bool flag)     // 오브젝트 풀링시 RPC로 활성화
    {
        gameObject.SetActive(flag);
    }
}
