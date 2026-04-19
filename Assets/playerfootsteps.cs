using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("Configuration Audio")]
    public AudioSource footstepSource;
    public AudioClip[] footstepSounds; // Glisse tes différents sons ici

    [Header("Réglages")]
    [Range(0, 1)] public float volumeRandomization = 0.1f;
    [Range(0, 1)] public float pitchRandomization = 0.1f;

    // Cette fonction sera appelée par l'Animation Event
    public void PlayFootstep()
    {
        if (footstepSounds.Length == 0) return;

        // Choix d'un son aléatoire
        int index = Random.Range(0, footstepSounds.Length);
        footstepSource.clip = footstepSounds[index];

        // Légère variation de pitch et volume pour plus de réalisme
        footstepSource.pitch = 1f + Random.Range(-pitchRandomization, pitchRandomization);
        footstepSource.volume = 1f - Random.Range(0, volumeRandomization);

        footstepSource.Play();
    }
}