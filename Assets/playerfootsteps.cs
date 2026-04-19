using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource footstepSource;
    public AudioClip[] footstepSounds; // Tableau pour varier les sons

    // Cette fonction sera appelée par l'animation
    public void PlayFootstep()
    {
        // On choisit un son au hasard dans la liste pour éviter la répétition monotone
        int index = Random.Range(0, footstepSounds.Length);
        footstepSource.clip = footstepSounds[index];
        
        // On joue le son
        footstepSource.Play();
    }
}