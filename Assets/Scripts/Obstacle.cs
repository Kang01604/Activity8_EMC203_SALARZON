// formerly Item.cs

using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Vector3 position3D;
    public bool hasHitPlayer = false;
    private SpriteRenderer spriteRenderer;

    private void Awake() { spriteRenderer = GetComponent<SpriteRenderer>(); }

    public void ResetObstacle()
    {
        hasHitPlayer = false;
        if (spriteRenderer != null) spriteRenderer.color = new Color(Random.value, Random.value, Random.value); // randomizes color
    }

    private void Update()
    {
        float perspective = CameraComponent.focalLength / (CameraComponent.focalLength + position3D.z);
        transform.localScale = Vector3.one * perspective;
        
        // OVERHEAD MATH: Subtract cameraHeight to look down at the ground
        float projectedX = position3D.x * perspective;
        float projectedY = (position3D.y - CameraComponent.cameraHeight) * perspective;
        
        transform.position = CameraComponent.vanishingPoint + new Vector2(projectedX, projectedY);
    }
}