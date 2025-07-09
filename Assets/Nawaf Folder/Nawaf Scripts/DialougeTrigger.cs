using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DialogueTrigger : MonoBehaviour
{
    public AudioClip dialogueClip;
    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && other.CompareTag("Player"))
        {
            audioSource.clip = dialogueClip;
            audioSource.Play();
            hasPlayed = true;
        }
    }
}
