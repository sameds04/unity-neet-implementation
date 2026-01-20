using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraMode
    {
        Static, Moveable, Follow
    }

    public GameObject creature;
    public CharacterController controller;
    public CameraMode mode;
    public Vector2 look;
    public int sens = 2;

    void Start()
    {
        mode = CameraMode.Static;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        ToggleCameraController();
        if (mode == CameraMode.Moveable)
        {
            MoveCamera();
        }
        else if (mode == CameraMode.Follow)
        {
            FollowCreature();
        }
    }

    private void MoveCamera()
    {
        look.x += Input.GetAxis("Mouse X");
        look.y += Input.GetAxis("Mouse Y");
        look.y = Mathf.Clamp(look.y, -45f, 45f);
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector3 input = new Vector3();
        input += transform.forward * vert;
        input += transform.right * horiz;
        input = Vector3.ClampMagnitude(input, 1f);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            controller.Move(input * 32 * Time.deltaTime);
        }
        else
        {
            controller.Move(input * 16 * Time.deltaTime);
        }

        transform.localRotation = Quaternion.Euler(-look.y * sens, look.x * sens, 0);
    }

    private void FollowCreature()
    {
        transform.position = creature.transform.position + new Vector3(0, 7, 7);
        transform.localEulerAngles = new Vector3(30, 180, 0);
    }

    public void ToggleCameraController()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (mode == CameraMode.Static)
            {
                Cursor.lockState = CursorLockMode.Locked;
                mode = CameraMode.Moveable;

            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                mode = CameraMode.Static;
            }
        }
    }

    public void SetCreature(GameObject given_creature) 
    {
        creature = given_creature;
        mode = CameraMode.Follow;
    }

}
