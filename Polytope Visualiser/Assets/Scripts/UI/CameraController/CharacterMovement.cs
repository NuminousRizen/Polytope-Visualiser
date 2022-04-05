using UnityEngine;

namespace UI.CameraController
{
    /// <summary>
    /// Component responsible for moving a character.
    /// </summary>
    public class CharacterMovement : MonoBehaviour
    {
        public CharacterController controller;

        public Camera camera;

        public float speed = 12f;

        /// <summary>
        /// Event function called by Unity when the application is launched.
        ///
        /// Configures the character controller component to ignore all collisions.
        /// </summary>
        void Start()
        {
            controller.detectCollisions = false;
            Physics.IgnoreLayerCollision(0, 7, true);
        }
    
        /// <summary>
        /// Event function called by Unity every frame.
        ///
        /// Checks if the user has pressed any of the relevant input keys and moves the character controller accordingly.
        /// </summary>
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
                Clear();
            }

            Vector3 move = transform.right * x + transform.forward * z + transform.up * y;

            controller.Move(move * speed * Time.deltaTime);
        }

        /// <summary>
        /// Resets the position and rotation of the character.
        /// </summary>
        public void Clear()
        {
            controller.enabled = false;
            transform.position = new Vector3(0f,0f,-10f);
            controller.enabled = true;
            transform.rotation = Quaternion.identity;
            camera.GetComponent<MouseLook>().Clear();
        }
    }
}
