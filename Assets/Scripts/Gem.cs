using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private GameObject _portal;
    private AudioSource _sound;
    private MeshRenderer _renderer;
    private Light _light;
    private BoxCollider _collider;

    private bool _pickedUp = false;

    // Start is called before the first frame update
    void Start()
    {
        _portal = GameObject.FindWithTag("Portal");
        _sound = GetComponent<AudioSource>();
        _renderer = GetComponent<MeshRenderer>();
        _light = GetComponent<Light>();
        _collider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_pickedUp && !_sound.isPlaying)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            _light.enabled = false;
            _collider.enabled = false;
            _renderer.enabled = false;
            _sound.Play();
            Portal portal = _portal.GetComponent<Portal>();
            portal.Activate();
            _pickedUp = true;
        }
    }
}
