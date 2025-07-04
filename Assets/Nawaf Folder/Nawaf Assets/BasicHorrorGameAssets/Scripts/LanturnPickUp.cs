using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanturnPickUp : MonoBehaviour
{
    private GameObject OB;
    public GameObject handUI;
    public GameObject lanturn;
    public AudioClip takeAudio;
    public AudioSource audioSource;


    private bool inReach;


    void Start()
    {
        OB = this.gameObject;

        handUI.SetActive(false);

        lanturn.SetActive(false);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            handUI.SetActive(true);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            handUI.SetActive(false);
        }
    }

    void Update()
    {
        // Use the new Input System for E key
        if (inReach && UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
        {
            handUI.SetActive(false);
            lanturn.SetActive(true);
            if (audioSource != null && takeAudio != null)
            {
                audioSource.PlayOneShot(takeAudio);
            }
            StartCoroutine(end());
        }
    }

    IEnumerator end()
    {
        yield return new WaitForSeconds(.01f);
        Destroy(OB);
    }
}
