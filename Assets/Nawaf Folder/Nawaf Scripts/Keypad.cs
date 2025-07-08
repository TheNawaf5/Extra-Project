using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class Keypad : MonoBehaviour
{

    public Doors doors; // Reference to the Doors script to unlock the door
    public GameObject player;
    public GameObject keypadOB;
    public GameObject hud;
    public GameObject inv;


    public GameObject animateOB;
    public Animator ANI;


    public TextMeshProUGUI textOB;
    public string answer = "12345";

    public AudioSource button;
    public AudioSource correct;
    public AudioSource wrong;

    public bool animate;


    void Start()
    {
        keypadOB.SetActive(false);

    }


    public void Number(int number)
    {
        textOB.text += number.ToString();
        button.Play();
    }

    public void Execute()
    {
        Debug.Log($"Entered: '{textOB.text}', Answer: '{answer}'");
        Debug.Log($"correct: {correct != null}, wrong: {wrong != null}, button: {button != null}, textOB: {textOB != null}, doors: {doors != null}");
        if (textOB.text == answer)
        {
            if (correct != null) correct.Play();
            if (textOB != null) textOB.text = "Right";
            if (doors != null) doors.isUnlocked = true;
        }
        else
        {
            if (wrong != null) wrong.Play();
            if (textOB != null) textOB.text = "Wrong";
        }


    }

    public void Clear()
    {
        {
            textOB.text = "";
            button.Play();
        }
    }

    public void Exit()
    {
        keypadOB.SetActive(false);
        inv.SetActive(true);
        hud.SetActive(true);
        player.GetComponent<PlayerController>().enabled = true;
    }

    public void Update()
    {
        if (textOB.text == "Right" && animate)
        {
            ANI.SetBool("animate", true);
            Debug.Log("its open");
        }


        if(keypadOB.activeInHierarchy)
        {
            hud.SetActive(false);
            inv.SetActive(false);
            player.GetComponent<PlayerController>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

    }


}
