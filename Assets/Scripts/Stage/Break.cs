using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Break : MonoBehaviour
{
    private float _time;
    private float _fadeCount;
    private bool _isFirstBreak;
    private bool _isSecondBreak;

    private GameObject[] _first;
    private GameObject[] _second;

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

        if (!_isFirstBreak && _time > 3.0f)
        {
            FirstBreak();
            _isFirstBreak = true;
        }

        if (!_isSecondBreak && _time > 6.0f)
        {
            SecondBreak();
            _isSecondBreak = true;
        }
    }

    private void FirstBreak()
    {
        for (int i = 0; i < _first.Length; i++)
        {
            StartCoroutine(FadeOut(_first[i]));
        }
    }
    private void SecondBreak()
    {
        for (int i = 0; i < _second.Length; i++)
        {
            StartCoroutine(FadeOut(_second[i]));
        }
    }

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
}