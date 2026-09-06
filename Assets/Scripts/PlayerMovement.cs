using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction moveJoystick;
    [SerializeField] private int speed;


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
      //  moveJoystick = playerInput.actions.FindAction("JoystickMove");
    }


    void Update()
    {
       MovePlayer();
     //  MovePlayerJoystick();
    }

    private void MovePlayer()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();
        transform.position += new Vector3(direction.x, 0, direction.y) * speed * Time.deltaTime;
    }
    //private void MovePlayerJoystick()
    //{
    //    Vector2 direction = moveJoystick.ReadValue<Vector2>();
    //    transform.position += new Vector3(direction.x, 0, direction.y) * speed * Time.deltaTime;
    //}
}
