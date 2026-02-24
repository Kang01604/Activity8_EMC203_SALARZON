using UnityEngine;
using TMPro; 
using UnityEngine.InputSystem; 
using UnityEngine.SceneManagement; // Needed for Restart

public class PlayerController : MonoBehaviour
{
    public Vector3 position3D = new Vector3(0, 0, 5f);
    public float laneWidth = 4.5f; 
    private int currentLane = 0; 
    public float jumpVelocity = 15f;
    public float gravity = -40f;
    private float verticalVelocity = 0f;
    private bool isJumping = false;
    
    public float hp = 100f;
    public bool isDead = false; // Track death state
    public TextMeshProUGUI hpText;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float flashTimer = 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        // DEATH CONTROLS
        if (isDead)
        {
            if (kb.rKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            if (kb.escapeKey.wasPressedThisFrame)
            {
                Debug.Log("Ending Game...");
                Application.Quit();
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
            }
            return; // Stop player movement if dead
        }

        // NORMAL CONTROLS
        if (kb.aKey.wasPressedThisFrame || kb.leftArrowKey.wasPressedThisFrame) currentLane = Mathf.Max(-1, currentLane - 1);
        if (kb.dKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame) currentLane = Mathf.Min(1, currentLane + 1);
        if ((kb.wKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame) && !isJumping) { verticalVelocity = jumpVelocity; isJumping = true; }

        position3D.x = currentLane * laneWidth;
        if (isJumping)
        {
            verticalVelocity += gravity * Time.deltaTime;
            position3D.y += verticalVelocity * Time.deltaTime;
            if (position3D.y <= 0f) { position3D.y = 0f; isJumping = false; }
        }

        if (hp < 100f && !isDead) hp = Mathf.Min(100f, hp + Time.deltaTime);
        if (hpText != null) hpText.text = "HP: " + Mathf.FloorToInt(hp);

        // Visual Feedback
        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0f && spriteRenderer != null) spriteRenderer.color = originalColor;
        }

        // Perspective Math
        float perspective = CameraComponent.focalLength / (CameraComponent.focalLength + position3D.z);
        transform.localScale = Vector3.one * perspective;
        float projectedX = position3D.x * perspective;
        float projectedY = (position3D.y - CameraComponent.cameraHeight) * perspective;
        transform.position = CameraComponent.vanishingPoint + new Vector2(projectedX, projectedY);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        hp -= amount;
        Debug.Log("PLAYER HIT! Remaining HP: " + Mathf.FloorToInt(hp));

        if (hp <= 0)
        {
            hp = 0;
            Die();
        }
        else
        {
            if (spriteRenderer != null) { spriteRenderer.color = Color.red; flashTimer = 0.2f; }
            CameraComponent.Shake(0.15f, 0.4f);
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("PLAYER DIED! HP = 0");
        Debug.Log("'R' = Restart from the very start | 'ESC' = End Game");
        
        // Turns the player dark or ghosted to show death
        if (spriteRenderer != null) spriteRenderer.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    }
}