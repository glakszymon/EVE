using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 2.0f;
    public float minimumY = -60.0f;
    public float maximumY = 60.0f;

    private float rotationY = 0.0f;

    public float zoomSpeed = 5f; 
    public float minZoom = 3f;    
    public float maxZoom = 100f;   

    public Camera cam;

    void Start()
    {
        // Ukrywa kursor i blokuje go w oknie gry
    //    Cursor.lockState = CursorLockMode.Locked; 
      //  Cursor.visible = false; // Ukrywa kursor
        cam = Camera.main;
    }

    void Update()
    {
    //	Cursor.lockState = CursorLockMode.Locked; 
      //  Cursor.visible = false; // Ukrywa kursor
    
        // Odczytanie ruchu myszy
        if(Input.GetKey(KeyCode.Mouse1))
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
            rotationY += Input.GetAxis("Mouse Y") * sensitivity;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            // Ustawienie rotacji kamery
            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

            // Odblokowanie kursora po naciśnięciu klawisza Escape
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None; // Odblokowuje kursor
                Cursor.visible = true; // Pokazuje kursor
            }
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - scroll * zoomSpeed, minZoom, maxZoom);
    }
}

