using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public int pilas = 0;
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
    public string lanternHudLabel = "Linterna";

    public float LanternCharge => lanternCharge;
    public FuelPickup NearbyFuel => nearbyFuel;
    public Camera PlayerCamera => playerCamera;
    public bool IsDefeated => defeated;

    CharacterController characterController;
    Camera playerCamera;
    Animator animator;
    InputAction moveAction;
    InputAction sprintAction;
    InputAction interactAction;
    [Min(0f)] public float interactionDistance = 2f;
    public float interactionHeight = 0.4f;
    FuelPickup nearbyFuel;
    float verticalSpeed;
    bool isRunning;
    float lanternCharge = 1f;
    float timeScaleBeforeDefeat;
    bool defeated;

    //Esto lo hice yo (rami)
    private int safeZoneCount;
    private bool inSafeZone => safeZoneCount > 0;
    //

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = Camera.main;
        moveAction = inputActions.FindAction("Player/Move", true).Clone();
        sprintAction = inputActions.FindAction("Player/Sprint", true).Clone();
        interactAction = inputActions.FindAction("Player/Interact", true).Clone();

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
        interactAction.Enable();
        verticalSpeed = 0f;
        SetCursorLocked(true);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            SetCursorLocked(false);
        else if (Application.isFocused && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            SetCursorLocked(true);
        UpdateLantern();
        if (defeated)
        {
            return;
        }
        MovePlayer();
        UpdateAnimation();
        nearbyFuel = FindNearbyFuel();
        if (Time.deltaTime > 0f && nearbyFuel != null && interactAction.WasPressedThisFrame())
        {
            if (nearbyFuel.TryCollect(this)) nearbyFuel = null;
        }
    }

    FuelPickup FindNearbyFuel()
    {
        Vector3 origin = transform.position + Vector3.up * interactionHeight;
        Collider[] nearby = Physics.OverlapSphere(origin, interactionDistance,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
        float nearestDistance = float.PositiveInfinity;
        FuelPickup target = null;
        foreach (Collider candidate in nearby)
        {
            FuelPickup pickup = candidate.GetComponentInParent<FuelPickup>();
            if (pickup == null || !pickup.isActiveAndEnabled) continue;
            Vector3 destination = pickup.transform.position + Vector3.up * interactionHeight;
            float distance = Vector3.Distance(origin, destination);
            if (distance <= interactionDistance && distance < nearestDistance && CanReachFuel(origin, destination, pickup))
            {
                nearestDistance = distance;
                target = pickup;
            }
        }
        return target;
    }

    bool CanReachFuel(Vector3 origin, Vector3 destination, FuelPickup pickup)
    {
        Vector3 direction = destination - origin;
        RaycastHit[] hits = Physics.RaycastAll(origin, direction.normalized, direction.magnitude,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform)) continue;
            if (hit.collider.GetComponentInParent<FuelPickup>() == pickup) continue;
            return false;
        }
        return true;
    }

    void UpdateLantern()
    {
        if (lantern == null || Time.deltaTime <= 0f || inSafeZone) //esto tmb lo cambié 
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

            SceneManager.LoadScene("LoseScene");
        }
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

    void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    void OnApplicationFocus(bool focused)
    {
        if (!focused) SetCursorLocked(false);
    }

    void OnDisable()
    {
        nearbyFuel = null;
        SetCursorLocked(false);
        moveAction.Disable();
        sprintAction.Disable();
        interactAction.Disable();

    }

    void OnDestroy()
    {
        if (defeated)
        {
            Time.timeScale = timeScaleBeforeDefeat;
        }
        moveAction.Dispose();
        sprintAction.Dispose();
        interactAction.Dispose();
    }
    //Esto es nuevo
    public void EnterSafeZone()
    {
        safeZoneCount++;
    }

    public void ExitSafeZone()
    {
        safeZoneCount = Mathf.Max(0, safeZoneCount - 1);
    }
}

