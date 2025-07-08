using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public Animator door;
    public GameObject openText;

public AudioSource doorSound;
public AudioSource closeDoorSound;


    public bool inReach;
    public bool requiresKeypad = false;
    [HideInInspector] public bool isUnlocked = false;




    void Start()
    {
        inReach = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            openText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            openText.SetActive(false);
        }
    }





    private bool isOpen = false;

    void Update()
    {
        Debug.Log($"isUnlocked: {isUnlocked}, requiresKeypad: {requiresKeypad}");
        // Only allow door interaction if not requiring keypad, or if unlocked by keypad
        if (inReach && UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!requiresKeypad || isUnlocked)
            {
                Debug.Log("E key pressed while in reach");
                openText.SetActive(false);
                if (!isOpen)
                {
                    DoorOpens();
                }
                else
                {
                    DoorCloses();
                }
                isOpen = !isOpen;
            }
            else
            {
                Debug.Log("Door is locked by keypad!");
                // Optionally show a locked UI message here
            }
        }
    }
    void DoorOpens ()
    {
        Debug.Log("It Opens");
        door.SetBool("Open", true);
        door.SetBool("Closed", false);
        doorSound.Play();

    }

    void DoorCloses()
    {
        Debug.Log("It Closes");
        door.SetBool("Open", false);
        door.SetBool("Closed", true);
        if (closeDoorSound != null)
        {
            closeDoorSound.Play();
        }
    }


}
