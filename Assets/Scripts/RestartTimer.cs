using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartTimer : MonoBehaviour
{
    public float ShowTimerTarget = 5.0f;
    public float CountdownTimerTarget = 3.0f;

    private bool _showTimerFinished = false;
    private bool _countdownTimerFinished = false;
    private Text _textComponent;
    private GameObject _countDown;

    // Start is called before the first frame update
    void Start()
    {
        _countDown = GameObject.Find("CountDown");
        _countDown.SetActive(false);
        _textComponent = _countDown.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_showTimerFinished)
        {
            ShowTimerTarget -= Time.deltaTime;
            if (ShowTimerTarget <= 0.0f)
            {
                _showTimerFinished = true;
                _countDown.SetActive(true);
                int number = (int) Mathf.Ceil(CountdownTimerTarget);
                _textComponent.text = number.ToString();
            }
        }
        else
        {
            if (!_countdownTimerFinished)
            {
                int number = (int) Mathf.Ceil(CountdownTimerTarget);
                _textComponent.text = number.ToString();
                CountdownTimerTarget -= Time.deltaTime;
                if (CountdownTimerTarget <= 0.0f)
                {
                    _countdownTimerFinished = true;
                    SceneManager.LoadScene("Play");
                }
            }
        }
    }
}
