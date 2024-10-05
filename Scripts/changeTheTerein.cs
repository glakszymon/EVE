using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using UnityEngine;
using UnityEngine.EventSystems; // To jest kluczowe dla EventTrigger

public class changeTheTerein : MonoBehaviour
{
    public GameObject teren;
    public GameObject button;
    public GameObject cam;

    private bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void change()
    {
        TextMeshProUGUI textComponent = button.GetComponentInChildren<TextMeshProUGUI>();

        if(isActive)
        {
            teren.transform.rotation = Quaternion.Euler(0, 0, 0);
            RectTransform rectTransform = teren.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(0, 0, 0); // Hide
            // cam.transform.position -= new Vector3(0, 8, 0); // Move camera down
            isActive = false;
            textComponent.text = "ON";
        }
        else
        {
            teren.transform.rotation = Quaternion.Euler(0, 0, 0);
            RectTransform rectTransform = teren.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(75, 75, 75); // Show
            cam.transform.position += new Vector3(0, 8, 0); // Move camera up
            isActive = true;
            textComponent.text = "OFF";
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
