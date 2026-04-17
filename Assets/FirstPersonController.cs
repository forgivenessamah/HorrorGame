using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Paramètres de Déplacement")]
    public float walkSpeed = 5f;
    public float gravity = -9.81f;

    [Header("Paramètres de la Caméra")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    void Start()
    {
        // Récupère automatiquement le composant CharacterController
        controller = GetComponent<CharacterController>();

        // Bloque le curseur de la souris au centre de l'écran et le cache (Standard FPS)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    private void HandleMouseLook()
    {
        // Récupère les mouvements de la souris
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Calcule la rotation verticale (regarder en haut/bas) et la bloque pour ne pas se briser le cou
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        // Applique la rotation verticale à la caméra
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Applique la rotation horizontale au corps entier du joueur
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        // Récupère les touches du clavier (Z,Q,S,D ou Flèches)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calcule la direction en fonction d'où regarde le joueur
        Vector3 move = transform.right * x + transform.forward * z;

        // Applique le déplacement horizontal
        controller.Move(move * walkSpeed * Time.deltaTime);

        // Gestion propre de la gravité
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Plaque doucement le joueur au sol s'il le touche
        }

        velocity.y += gravity * Time.deltaTime;

        // Applique le déplacement vertical (chute)
        controller.Move(velocity * Time.deltaTime);
    }
}
