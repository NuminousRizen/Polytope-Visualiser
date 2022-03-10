using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;

    void Start()
    {
        controller.detectCollisions = false;
    }
    
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float y = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            y = 1;
        }
        if (Input.GetKey(KeyCode.E))
        {
            y = -1;
        }

        if (Input.GetKey(KeyCode.F))
        {
            controller.enabled = false;
            transform.position = Vector3.zero;
            controller.enabled = true;
            transform.rotation = Quaternion.identity;
        }

        Vector3 move = transform.right * x + transform.forward * z + transform.up * y;

        controller.Move(move * speed * Time.deltaTime);
    }
}
