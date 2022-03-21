using UnityEngine;

namespace UI.CameraController
{
    public class MouseLook : MonoBehaviour
    {
        public float mouseSensitivity = 50f;

        public Transform characterBody;

        private float xRotation = 0f;
    
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

        public void Clear()
        {
            xRotation = 0f;
            transform.localRotation = Quaternion.identity;
        }
    }
}
