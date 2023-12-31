using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Break : MonoBehaviourPun
{
    private float _time;
    // private float _fadeCount;
    private bool _isFirstBreak;
    private bool _isSecondBreak;

    [SerializeField] private GameObject[] _first;
    [SerializeField] private GameObject[] _second;
    // [SerializeField] List<GameObject> FirstBreakList = new List<GameObject>();
    // [SerializeField] List<GameObject> SecondBreakList = new List<GameObject>();

    private void Awake()
    {
        _time = 0;
        _isFirstBreak = false;
        _isSecondBreak = false;

        _first = GameObject.FindGameObjectsWithTag("FirstBreak");
        _second = GameObject.FindGameObjectsWithTag("SecondBreak");
    }
    private void Update()
    {
        _time += Time.deltaTime;

        if (!_isFirstBreak && _time > 15f)
        {
            FirstBreak();
            _isFirstBreak = true;
        }

        if (!_isSecondBreak && _time > 30f)
        {
            SecondBreak();
            _isSecondBreak = true;
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < _first.Length; i++)
        {
            Rewind(_first[i].GetComponent<Rigidbody2D>());
        }
        for (int i = 0; i < _second.Length; i++)
        {
            Rewind(_second[i].GetComponent<Rigidbody2D>());
        }
    }

    private void FirstBreak()
    {
        for (int i = 0; i < _first.Length; i++)
        {
            TimeToFalling(_first[i].GetComponent<Rigidbody2D>());
        }
    }
    private void SecondBreak()
    {
        for (int i = 0; i < _second.Length; i++)
        {
            TimeToFalling(_second[i].GetComponent<Rigidbody2D>());
        }
    }
    private void TimeToFalling(Rigidbody2D rb)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.mass = 10f;
        rb.gravityScale = 0.1f;
        Destroy(rb.gameObject, 5f);
    }

    private void Rewind(Rigidbody2D rb)
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    /*
    IEnumerator FadeOut(GameObject obj)
    {
        _fadeCount = 1.0f;
        while (_fadeCount > 0)
        {
            Color c = obj.GetComponent<SpriteRenderer>().color;
            _fadeCount -= 0.005f;
            yield return new WaitForSeconds(0.01f);
            c.a = _fadeCount;
        }
        obj.SetActive(false);
        // yield return null;
    }
    */
}
