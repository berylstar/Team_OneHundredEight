using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviourPun
{
    private int _mapindex;

    // private GameObject[] _maps;
    [SerializeField] private GameObject _stage;

    private void Start()
    {
        _mapindex = Random.Range(0, 3);

        switch (_mapindex)
        {
            case 0:
                _stage = GameObject.Find("Stages").transform.Find("Map 1").gameObject;
                //_stage.SetActive(true);
                break;

            case 1:
                _stage = GameObject.Find("Stages").transform.Find("Map 2").gameObject;
                //_stage.SetActive(true);
                break;

            case 2:
                _stage = GameObject.Find("Stages").transform.Find("Map 3").gameObject;
                //_stage.SetActive(true);
                break;
        }
    }

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
