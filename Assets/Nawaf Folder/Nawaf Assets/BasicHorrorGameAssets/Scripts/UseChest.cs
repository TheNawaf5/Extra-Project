using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UseChest : MonoBehaviour
{
    private GameObject OB;
    public GameObject handUI;
    public GameObject objToActivate;


    private bool inReach;


    void Start()
    {

        OB = this.gameObject;

        handUI.SetActive(false);

        objToActivate.SetActive(false);

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

        if (inReach && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E key pressed while in reach");
            handUI.SetActive(false);
            objToActivate.SetActive(true);
            Animator anim = OB.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetBool("open", true);
                Debug.Log("Animator found and open set to true");
            }
            else
            {
                Debug.LogWarning("Animator component not found on OB");
            }
            BoxCollider boxCol = OB.GetComponent<BoxCollider>();
            if (boxCol != null)
            {
                boxCol.enabled = false;
                Debug.Log("BoxCollider disabled");
            }
            else
            {
                Debug.LogWarning("BoxCollider component not found on OB");
            }
        }
    }

}
