using UnityEngine;

public class PlayerControllerFPS : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 150.0f;

    void Update()
    {
        // 1. ROTATION (Tourner à gauche et à droite avec Q/D ou Flèches)
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);

        // 2. AVANCER / RECULER (Avec Z/S ou Flèches)
        float move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        transform.Translate(0, 0, move);
        
        // 3. FOCUS SOURIS (Optionnel pour ton confort)
        // Clique dans la fenêtre Game pour que le clavier réagisse
    }
}
