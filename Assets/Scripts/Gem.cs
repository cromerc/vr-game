using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private GameObject _portal;

    // Start is called before the first frame update
    void Start()
    {
        _portal = GameObject.FindWithTag("Portal");
        Debug.Log("Found  portal");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            Portal portal = _portal.GetComponent<Portal>();
            portal.Activate();
            Destroy(gameObject);
        }
    }
}
