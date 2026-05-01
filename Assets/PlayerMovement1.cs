using UnityEngine;

public class PlayerMovement1 : MonoBehaviour
{
    public float moveSpeed = 5f; // Vitesse de déplacement
    private Vector3 movement;

    void Update()
    {
        // 1. Récupération des touches (Z, Q, S, D ou flèches)
        float moveX = Input.GetAxisRaw("Horizontal"); // Droite / Gauche
        float moveZ = Input.GetAxisRaw("Vertical");   // Haut / Bas (Profondeur)

        // 2. Création du vecteur de mouvement
        // On normalise pour éviter de courir plus vite en diagonale
        movement = new Vector3(moveX, 0f, moveZ).normalized;
    }

    void FixedUpdate()
    {
        // 3. Application du mouvement
        // Space.World permet de se déplacer par rapport aux axes de la scène
        transform.Translate(movement * moveSpeed * Time.fixedDeltaTime, Space.World);
    }
}
