using UnityEngine;

namespace UI.CameraController
{
    /// <summary>
    /// Component responsible getting mouse input and rotating the camera accordingly.
    /// </summary>
    public class MouseLook : MonoBehaviour
    {
        public float mouseSensitivity = 300f;

        public Transform characterBody;

        private float xRotation = 0f;
    
        /// <summary>
        /// Event function called by Unity every frame.
        ///
        /// Gets the mouse inputs from the user and rotates the camera accordingly.
        /// </summary>
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

                xRotation -= mouseY;

                transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            
                characterBody.Rotate(Vector3.up * mouseX);
            }
        }

        /// <summary>
        /// Resets the rotation of the camera.
        /// </summary>
        public void Clear()
        {
            xRotation = 0f;
            transform.localRotation = Quaternion.identity;
        }
    }
}
