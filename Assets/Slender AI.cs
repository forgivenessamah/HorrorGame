using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SlenderAI : MonoBehaviour
{
    [Header("Teleport Positions")]
    public Transform[] teleportPositions; // glisse tes 3 positions ici

    [Header("Player")]
    public Transform player;
    public Camera playerCamera;

    [Header("Settings")]
    public float baseTeleportRate = 5f;  // secondes entre chaque teleport
    public float fieldOfViewAngle = 60f; // angle de vision du joueur

    [Header("Threat Level")]
    [Range(0f, 1f)]
    public float threatLevel = 0f; // 0 = calme, 1 = maximum

    private bool isTeleporting = true;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(TeleportRoutine());
    }

    void Update()
    {
        // L'ennemi regarde toujours le joueur
        transform.LookAt(new Vector3(
            player.position.x,
            transform.position.y,
            player.position.z
        ));

        // L'ennemi se déplace vers le joueur
        // Vitesse augmente avec le threat level
        agent.speed = Mathf.Lerp(2f, 7f, threatLevel);
        agent.SetDestination(player.position);
    }

    IEnumerator TeleportRoutine()
    {
        while (isTeleporting)
        {
            // Attendre - moins longtemps si threat level élevé
            float waitTime = Mathf.Lerp(baseTeleportRate, 1.5f, threatLevel);
            yield return new WaitForSeconds(waitTime);

            // Téléporter SEULEMENT si le joueur ne regarde pas l'ennemi
            if (!IsPlayerLookingAtEnemy())
            {
                TeleportToRandomPosition();
            }
        }
    }

    bool IsPlayerLookingAtEnemy()
    {
        Vector3 directionToEnemy = transform.position - playerCamera.transform.position;
        float angle = Vector3.Angle(playerCamera.transform.forward, directionToEnemy);
        return angle < fieldOfViewAngle;
    }

    void TeleportToRandomPosition()
    {
        if (teleportPositions.Length == 0) return;

        int randNum = Random.Range(0, teleportPositions.Length);
        transform.position = teleportPositions[randNum].position;
    }

    // Cette fonction est appelée par Person 4 quand une page est collectée
    public void IncreaseThreatLevel(float amount)
    {
        threatLevel = Mathf.Clamp01(threatLevel + amount);
    }
}
