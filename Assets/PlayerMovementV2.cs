using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;

    void Update()
    {
        // Récupère les entrées clavier (Z,Q,S,D ou Flèches)
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;

        // On multiplie par Time.deltaTime pour que le mouvement soit fluide
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        // Avancer / Reculer
        transform.Translate(0, 0, translation);

        // Tourner sur soi-même (Rotation gauche/droite)
        transform.Rotate(0, rotation, 0);
    }
}