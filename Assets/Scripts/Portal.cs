using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private bool _activated = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (_activated && collider.tag == "Player")
        {
            SceneManager.LoadScene("Play");
        }
    }

    public void Activate()
    {
        var portalEffects = transform.GetChild(0);
        for (int i = 0; i < portalEffects.childCount; i ++)
        {
            portalEffects.GetChild(i).gameObject.SetActive(true);
        }
        _activated = true;
    }
}
