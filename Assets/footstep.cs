using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public AudioSource walkingSound;
    public AudioSource runningSound;
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float speed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;

        if (speed > runSpeed)
        {
            if (!runningSound.isPlaying) runningSound.Play();
            if (walkingSound.isPlaying) walkingSound.Stop();
        }
        else if (speed > 0.1f)
        {
            if (!walkingSound.isPlaying) walkingSound.Play();
            if (runningSound.isPlaying) runningSound.Stop();
        }
        else
        {
            walkingSound.Stop();
            runningSound.Stop();
        }
    }
}