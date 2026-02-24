using UnityEngine;

public class CameraComponent : MonoBehaviour
{
    public static float focalLength = 15f; 
    public static Vector2 vanishingPoint = new Vector2(0f, 5f); // Horizon is high up
    public static float cameraHeight = 7.5f; // Camera is 10 units in the air

    private static CameraComponent instance;
    private Vector3 originalPosition;
    private float shakeTimer = 0f;
    private float shakeMagnitude = 0f;

    private void Awake()
    {
        instance = this;
        originalPosition = transform.position;
    }

    private void Update()
    {
        if (shakeTimer > 0f)
        {
            transform.position = originalPosition + (Vector3)Random.insideUnitCircle * shakeMagnitude;
            shakeTimer -= Time.deltaTime;
        }
        else { transform.position = originalPosition; }
    }

    // Shake logic for when player takes damage
    public static void Shake(float duration, float magnitude)
    {
        if (instance != null)
        {
            instance.shakeTimer = duration;
            instance.shakeMagnitude = magnitude;
        }
    }
}