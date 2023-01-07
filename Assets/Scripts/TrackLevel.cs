using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackLevel : MonoBehaviour
{
    public static int CurrentLevel = 0;
    private Text _textComponent;

    // Start is called before the first frame update
    void Start()
    {
        GameObject level = GameObject.Find("Level");
        _textComponent = level.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        _textComponent.text = "Nivel: " + CurrentLevel;
    }
}
