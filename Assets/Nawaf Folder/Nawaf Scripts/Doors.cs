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
        // Use the new Input System for E key, similar to UseChest
        if (inReach && UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
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
