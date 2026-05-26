using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flashlightMovement : MonoBehaviour
{
    [Header("Settings")]
    public Transform targetTransform;

    [Header("Subtle Left/Right Sway (Walking)")]
    public float swayAngle = 14.0f; // Goes further left and right
    public float swaySpeed = 1.5f;  // Slower, more relaxed movement 

    [Header("Mouse Lag")]
    public float mouseLagAmount = 4.0f;
    public float mouseLagSmoothness = 6.0f;

    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private float timer = 0f;
    private float smoothMouseX;
    private float smoothMouseY;
    private float currentWalkWeight = 0f;

    void Start()
    {
        // Auto-detect the flashlight if empty
        if (targetTransform == null)
        {
            Light[] lights = GetComponentsInChildren<Light>(true);
            foreach (Light l in lights)
            {
                if (l.type == LightType.Spot)
                {
                    targetTransform = l.transform;
                    break;
                }
            }
            if (targetTransform == null && lights.Length > 0)
                targetTransform = lights[0].transform;

            if (targetTransform == null)
                targetTransform = this.transform;
        }

        // CRITICAL SAFETY CHECK: Never animate the Player!
        if (targetTransform.GetComponent<CharacterController>() != null || targetTransform.GetComponent<SC_FPSController>() != null)
        {
            Debug.LogError("flashlightMovement ERROR: The script is trying to animate the Player object! This stops you from moving. " +
                           "Please manually drag your Spot Light into the 'Target Transform' slot on the script.");
            enabled = false;
            return;
        }

        // Disable Animator on the Flashlight to stop the old animation
        Animator childAnim = targetTransform.GetComponent<Animator>();
        if (childAnim != null) childAnim.enabled = false;

        // Cache initial position
        initialLocalPosition = targetTransform.localPosition;
        initialLocalRotation = targetTransform.localRotation;
    }

    void Update()
    {
        if (targetTransform == null) return;

        // 1. Mouse Lag (Lerped because mouse input is jagged)
        float mouseX = Input.GetAxis("Mouse X") * mouseLagAmount;
        float mouseY = Input.GetAxis("Mouse Y") * mouseLagAmount;

        mouseX = Mathf.Clamp(mouseX, -10f, 10f);
        mouseY = Mathf.Clamp(mouseY, -10f, 10f);

        smoothMouseX = Mathf.Lerp(smoothMouseX, mouseX, Time.deltaTime * mouseLagSmoothness);
        smoothMouseY = Mathf.Lerp(smoothMouseY, mouseY, Time.deltaTime * mouseLagSmoothness);

        Quaternion mouseRotation = Quaternion.Euler(-smoothMouseY, smoothMouseX, 0);

        // 2. A little left and right movement while walking
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;

        // Smoothly transition between walking (1) and standing still (0)
        float targetWalkWeight = isMoving ? 1f : 0f;
        currentWalkWeight = Mathf.Lerp(currentWalkWeight, targetWalkWeight, Time.deltaTime * 5f);

        // Only increase timer when moving to prevent weird math jumps
        if (isMoving)
        {
            timer += Time.deltaTime * swaySpeed;
        }

        // Apply smooth Sine waves multiplied by the walk weight
        // This is ALREADY perfectly smooth, so we do NOT Slerp it later!
        float currentSwayAngle = Mathf.Sin(timer) * swayAngle * currentWalkWeight;
        float currentBobY = Mathf.Abs(Mathf.Sin(timer * 2f)) * 0.02f * currentWalkWeight; 

        Quaternion walkSwayRotation = Quaternion.Euler(0, currentSwayAngle, 0);
        Vector3 finalPosition = initialLocalPosition + new Vector3(0, currentBobY, 0);
        Quaternion finalRotation = initialLocalRotation * mouseRotation * walkSwayRotation;

        // 3. APPLY DIRECTLY
        // Applying directly removes the "vibrating" jitter at the left/right peaks entirely.
        targetTransform.localPosition = finalPosition;
        targetTransform.localRotation = finalRotation;
    }
}

