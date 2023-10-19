using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StageManager : MonoBehaviourPun, IPunObservable
{
    private int _mapindex;
    private string _mapName;

    // private GameObject[] _maps;
    [SerializeField] private GameObject _stage;
    [SerializeField] List<GameObject> StageList = new List<GameObject>();

    private void Start()
    {
        StageInit();
        // StageSelect();
    }
    private void StageInit()
    {
        for (int i = 0; i < 3; i++)
        {
            _stage = GameObject.Find("Stages").transform.GetChild(i).gameObject;
            StageList.Add(_stage);
        }
    }
    public void StageSelect()  // 증강 캔버스 스크립트 안에 OnDisable 에서 호출
    {
        if (photonView.IsMine)
        {
            _mapindex = UnityEngine.Random.Range(0, StageList.Count);   // 맵고름
            // _mapindex = 2;
            _mapName = StageList[_mapindex].name;   // 스폰위치를 위해 맵 이름을 넘겨줌
            photonView.RPC("StageEnable", RpcTarget.All, _mapindex);
        }
    }
    [PunRPC]
    public void StageEnable(int index)
    {
        StageList[index].SetActive(true);   // 활성화
        // StageList.RemoveAt(index);  // 그 맵은 리스트에서 삭제
    }
    public void StageDelete()
    {
        if (photonView.IsMine)
            photonView.RPC("StageDisable", RpcTarget.All, _mapindex);
    }
    [PunRPC]
    public void StageDisable(int index)  // 증강 캔버스 스크립트 안에 OnEnable 에서 호출
    {
        StageList[index].SetActive(false);   // 활성화
        StageList.RemoveAt(index);  // 그 맵은 리스트에서 삭제   // 전판 맵 비활성화
    }
    public List<Vector2> SetSpawn()
    {
        List<Vector2> SpawnList = new List<Vector2>();

        switch (_mapName)
        {
            case "Map 1":
                SpawnList.Add(new Vector2(-7f, 0f));    // 1p
                SpawnList.Add(new Vector2(7f, 0f));     // 2p
                SpawnList.Add(new Vector2(-2.5f, 0f));  // 3p
                SpawnList.Add(new Vector2(2.5f, 0f));   // 4p
                SpawnList.Add(new Vector2(3f, 3f));     // 5p
                break;

            case "Map 2":
                SpawnList.Add(new Vector2(-7.2f, -2f));  
                SpawnList.Add(new Vector2(7f, -2f));    
                SpawnList.Add(new Vector2(-2.5f, 0f)); 
                SpawnList.Add(new Vector2(2.5f, 0f)); 
                SpawnList.Add(new Vector2(0f, -2f));      
                break;

            case "Map 3":
                SpawnList.Add(new Vector2(-7f, 0f));   
                SpawnList.Add(new Vector2(7f, 0f));     
                SpawnList.Add(new Vector2(-2.5f, 0f)); 
                SpawnList.Add(new Vector2(2.5f, 0f));   
                SpawnList.Add(new Vector2(3f, 3f));    
                break;
        }

        return SpawnList;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_mapName);
        }
        else
        {
            _mapName = (string)stream.ReceiveNext();
        }
    }

    /*
_mapindex = Random.Range(0, 3);

switch (_mapindex)
{
   case 0:
       _stage = GameObject.Find("Stages").transform.Find("Map 1").gameObject;
       _stage.SetActive(true);
       break;

   case 1:
       _stage = GameObject.Find("Stages").transform.Find("Map 2").gameObject;
       _stage.SetActive(true);
       break;

   case 2:
       _stage = GameObject.Find("Stages").transform.Find("Map 3").gameObject;
       _stage.SetActive(true);
       break;
}
*/

    /*
    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            _maps[i] = GameObject.Find("Stages").transform.GetChild(i).gameObject;
            _maps[i].SetActive(false);
        }

        _mapindex = Random.Range(0, 3);

        switch (_mapindex)
        {
            case 0:
                // _maps[_mapindex] = GameObject.Find("Stages").transform.Find("Map 1").gameObject;
                Debug.Log(_mapindex);
                _maps[_mapindex].SetActive(true);
                break;

            case 1:
                // _maps[_mapindex] = GameObject.Find("Stages").transform.Find("Map 2").gameObject;
                Debug.Log(_mapindex);
                _maps[_mapindex].SetActive(true);
                break;

            case 2:
                // _maps[_mapindex] = GameObject.Find("Stages").transform.Find("Map 3").gameObject;
                Debug.Log(_mapindex);
                _maps[_mapindex].SetActive(true);
                break;
        }
    }
    */
}
