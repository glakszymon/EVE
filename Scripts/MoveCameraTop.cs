using UnityEngine;
using UnityEngine.UI;

public class MoveCameraTop : MonoBehaviour {
    // public GameObject pointB;
    public float radius = 2f;
    // public Slider rotationSliderX;
    public Slider rotationSliderZ; // New slider for Z-axis rotation



    void Start()
    {
        // rotationSliderX.value = 0f;
        // rotationSliderX.onValueChanged.AddListener(OnSliderValueChanged);
        
        rotationSliderZ.value = 0f;
        rotationSliderZ.onValueChanged.AddListener(OnSliderValueChanged);
        
    }

    void OnSliderValueChanged(float value)
    {
        // float xRotation = rotationSliderX.value;
        float zRotation = rotationSliderZ.value;

        transform.rotation = Quaternion.Euler(0, 0, zRotation);
        // pointB.transform.rotation = Quaternion.Euler(xRotation, 0, zRotation);

        // float xAngleRad = xRotation * Mathf.Deg2Rad;
        float zAngleRad = zRotation * Mathf.Deg2Rad;

        // Vector3 newPositionB = new Vector3(
        //     radius * Mathf.Cos(zAngleRad),
        //     radius * Mathf.Sin(xAngleRad) * Mathf.Cos(zAngleRad),
        //     radius * Mathf.Sin(zAngleRad)
        // );



        // Debug.Log($"Przesunięcie punktu B: X: {displacement.x:F6}, Y: {displacement.y:F6}, Z: {displacement.z:F6}");
    }
}
