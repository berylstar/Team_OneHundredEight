using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private int _mapindex;

    private GameObject _stage;

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
}
