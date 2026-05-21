using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Vitesse de déplacement
    private Vector3 movement;

    void Update()
    {
        // 1. Récupération des touches (Z, Q, S, D ou flèches)
        float moveX = Input.GetAxisRaw("Horizontal"); // Droite / Gauche
        float moveZ = Input.GetAxisRaw("Vertical");   // Haut / Bas (Profondeur)

        // 2. Création du vecteur de mouvement
        movement = new Vector3(moveX, 0f, moveZ).normalized;
    }

    void FixedUpdate()
    {
        // 3. Application du mouvement
        // On utilise MovePosition pour que la physique soit prise en compte
        transform.Translate(movement * moveSpeed * Time.fixedDeltaTime, Space.World);
    }
}
