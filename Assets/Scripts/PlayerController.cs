using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset inputActions;
    public float walkSpeed = 5f;
    public float runSpeed = 6f;
    public float rotationSpeed = 540f;
    public float gravity = -20f;
    public Transform visual;
    public RuntimeAnimatorController animationController;
    public Avatar avatar;
    public Light lantern;
    public float lanternRange = 8f;
    public float lanternIntensity = 3f;
    public Color lanternColor = new Color(1f, 0.4f, 0.08f);
    [Min(0f)] public float lanternDuration = 60f;
    public Rect lanternHudRect = new Rect(20f, 20f, 200f, 24f);
    public string lanternHudLabel = "Linterna";

    CharacterController characterController;
    Camera playerCamera;
    Animator animator;
    InputAction moveAction;
    InputAction sprintAction;
    float verticalSpeed;
    bool isRunning;
    float lanternCharge = 1f;
    float timeScaleBeforeDefeat;
    bool defeated;


    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = Camera.main;
        moveAction = inputActions.FindAction("Player/Move", true).Clone();
        sprintAction = inputActions.FindAction("Player/Sprint", true).Clone();

        animator = visual.GetComponentInChildren<Animator>();
        if (animator == null)
        {
            animator = visual.gameObject.AddComponent<Animator>();
        }
        animator.avatar = avatar;
        animator.runtimeAnimatorController = animationController;
        animator.applyRootMotion = false;
        ConfigureLantern();
    }

    void ConfigureLantern()
    {
        if (lantern == null)
        {
            return;
        }
        lantern.type = LightType.Point;
        lantern.color = lanternColor;
        lantern.range = Mathf.Max(0f, lanternRange);
        lantern.intensity = Mathf.Max(0f, lanternIntensity) * lanternCharge;
    }

    void OnValidate()
    {
        ConfigureLantern();
    }

    void OnEnable()
    {
        moveAction.Enable();
        sprintAction.Enable();
        verticalSpeed = 0f;
    }

    void Update()
    {
        UpdateLantern();
        if (defeated)
        {
            return;
        }
        MovePlayer();
        UpdateAnimation();
    }

    void UpdateLantern()
    {
        if (lantern == null || Time.deltaTime <= 0f)
        {
            return;
        }

        lanternCharge = lanternDuration > 0f
            ? Mathf.Max(0f, lanternCharge - Time.deltaTime / lanternDuration)
            : 0f;
        lantern.intensity = Mathf.Max(0f, lanternIntensity) * lanternCharge;

        if (lanternCharge <= 0f)
        {
            ///////////placeholder
            defeated = true;
            timeScaleBeforeDefeat = Time.timeScale;
            Time.timeScale = 0f;
            gameObject.SetActive(false);
        }
    }

    void OnGUI()
    {
        if (lantern == null)
        {
            return;
        }

        GUI.Box(lanternHudRect, GUIContent.none);
        Rect fill = lanternHudRect;
        fill.width *= lanternCharge;
        Color previousColor = GUI.color;
        GUI.color = lanternColor;
        GUI.DrawTexture(fill, Texture2D.whiteTexture);
        GUI.color = previousColor;
        GUI.Label(lanternHudRect, lanternHudLabel);
    }

    public bool AddFuel(float seconds)
    {
        if (defeated || lantern == null || lanternDuration <= 0f || seconds <= 0f || lanternCharge >= 1f)
        {
            return false;
        }

        lanternCharge = Mathf.Clamp01(lanternCharge + seconds / lanternDuration);
        lantern.intensity = Mathf.Max(0f, lanternIntensity) * lanternCharge;
        return true;
    }

    void MovePlayer()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        input = Vector2.ClampMagnitude(input, 1f);
        isRunning = sprintAction.IsPressed();

        float currentSpeed = walkSpeed;
        if (isRunning)
        {
            currentSpeed = runSpeed;
        }
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        Vector3 direction = forward * input.y + right * input.x;

        if (direction.magnitude > 0.01f)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            visual.rotation = Quaternion.RotateTowards(visual.rotation, rotation, rotationSpeed * Time.deltaTime);
        }

        if (characterController.isGrounded && verticalSpeed < 0f)
        {
            verticalSpeed = -2f;
        }
        verticalSpeed += gravity * Time.deltaTime;

        Vector3 movement = direction * currentSpeed;
        movement.y = verticalSpeed;
        characterController.Move(movement * Time.deltaTime);
    }

    void UpdateAnimation()
    {
        Vector3 movement = characterController.velocity;
        movement.y = 0f;
        float speed = 0f;

        if (movement.magnitude > 0.1f)
        {
            if (isRunning)
            {
                speed = 2f;
            }
            else
            {
                speed = 1f;
            }
        }

        animator.SetFloat("speed", speed);
    }

    void OnDisable()
    {
        moveAction.Disable();
        sprintAction.Disable();

    }

    void OnDestroy()
    {
        if (defeated)
        {
            Time.timeScale = timeScaleBeforeDefeat;
        }
        moveAction.Dispose();
        sprintAction.Dispose();
    }
}

