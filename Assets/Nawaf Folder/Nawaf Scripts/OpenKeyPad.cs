using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenKeyPad : MonoBehaviour
{


    private bool isOpen = false;
    public GameObject keypadOB;
    public GameObject keypadText;
    public GameObject hud;
    public GameObject inv;
    public bool inReach;


    void Start()
    {
        inReach = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            if (!isOpen)
                keypadText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            keypadText.SetActive(false);
            keypadOB.SetActive(false);
            if (hud != null) hud.SetActive(true);
            if (inv != null) inv.SetActive(true);
            isOpen = false;
        }
    }




    void Update()
    {
        // Use E key to toggle keypad
        if (inReach && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!isOpen)
            {
                keypadOB.SetActive(true);
                keypadText.SetActive(false);
                if (hud != null) hud.SetActive(false);
                if (inv != null) inv.SetActive(false);
                isOpen = true;
            }
            else
            {
                keypadOB.SetActive(false);
                if (hud != null) hud.SetActive(true);
                if (inv != null) inv.SetActive(true);
                if (inReach) keypadText.SetActive(true);
                isOpen = false;
            }
        }

        // Optionally allow closing with Escape
        if (isOpen && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            keypadOB.SetActive(false);
            if (hud != null) hud.SetActive(true);
            if (inv != null) inv.SetActive(true);
            if (inReach) keypadText.SetActive(true);
            isOpen = false;
        }
    }
}
