using UnityEngine;

public class FootstepsSounds : MonoBehaviour
{
    private AudioSource[] audioSources;
    private AudioSource walkSound;
    private AudioSource runSound;

    void Start()
    {
        audioSources = GetComponents<AudioSource>();
        walkSound = audioSources[0];
        runSound = audioSources[1];
    }

    void Update()
    {
        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
                     || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        if (isMoving && isRunning)
        {
            if (!runSound.isPlaying) runSound.Play();
            if (walkSound.isPlaying) walkSound.Stop();
        }
        else if (isMoving)
        {
            if (!walkSound.isPlaying) walkSound.Play();
            if (runSound.isPlaying) runSound.Stop();
        }
        else
        {
            walkSound.Stop();
            runSound.Stop();
        }
    }
}