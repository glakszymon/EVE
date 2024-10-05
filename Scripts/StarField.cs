using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Dodaj to, aby używać komponentu Dropdown
using TMPro;



public class StarField : MonoBehaviour {
    [Range(0, 100)]
    [SerializeField] private float starSizeMin = 0f;
    [Range(0, 100)]
    [SerializeField] private float starSizeMax = 15f;
    private List<StarDataLoader.Star> stars;
    private StarDataLoader starDataLoader;
    private List<GameObject> starObjects;
    private readonly int starFieldScale = 600;
    public float r0 = 10f;

    private GameControl GameControl;

    // Canvas wyświertlający info o gwiazdach 
    public GameObject Canvas;
    public TextMeshProUGUI starNameText;
    public TextMeshProUGUI starMagnitudeText;
    public TextMeshProUGUI starPositionText;
    public TextMeshProUGUI starColorText;

    // ---------------------------------------------


    // public void StartLoging(int index)
    // {


    // }
    /*void Start()
    {
        starDataLoader = new StarDataLoader();
        stars = starDataLoader.LoadData();
        // LoadStars(0); // Or whatever index you want to use
    }*/

    // Metoda do ładowania gwiazd na podstawie wybranego zestawu
    public void LoadStars(int index) {

        GameControl = FindObjectOfType<GameControl>();
        
        // Sprawdź, czy GameControl nie jest null
        if (GameControl != null)
        {
            GameControl.StartStars();
        }
        else
        {
            Debug.LogError("Nie znaleziono GameControl!");
        }

        // Usuń wcześniej stworzone gwiazdy
        RemoveExistingStars();

        StarDataLoader sdl = new();
        stars = sdl.LoadData(); // Załaduj wszystkie gwiazdy

        // Wybierz zestaw gwiazd na podstawie indeksu
       // List<StarDataLoader.Star> selectedStars = GetStarSetByIndex(index);
        starObjects = new();

        foreach (StarDataLoader.Star star in stars) {
            GameObject stargo = GameObject.CreatePrimitive(PrimitiveType.Quad);
            stargo.transform.parent = transform;
            // stargo.name = $"HR {star.catalog_number}";
            stargo.name = star.name;

            stargo.transform.localPosition = star.position * starFieldScale;

            float starScale = Mathf.Lerp(starSizeMin, starSizeMax, Mathf.Clamp01(star.size));
            stargo.transform.localScale = Vector3.one * starScale;

            stargo.transform.LookAt(transform.position);
            stargo.transform.Rotate(0, 180, 0);

            float radius = r0 * Mathf.Pow(2.512f, (1 - star.size));
            Material material = stargo.GetComponent<MeshRenderer>().material;
            material.shader = Shader.Find("Unlit/StarShader");
            material.SetFloat("_Size", Mathf.Lerp(starSizeMin, starSizeMax, radius));
            material.color = Color.white;

            starObjects.Add(stargo);

            //-------------------------------------------



            if (stars == null || stars.Count == 0)
            {
                Debug.LogError("Nie udało się załadować danych gwiazd.");
                return;
            }


            // GameObject pointObject = Instantiate(pointPrefab, point.Location, Quaternion.identity);
            // Color color;
            // if (ColorUtility.TryParseHtmlString(point.Color, out color))
            // {
            //     pointObject.GetComponent<Renderer>().material.color = color;
            // }
            // pointObject.name = point.Name; // Ustawienie nazwy obiektu
            
        }

        StarDataLoader.Star foundStar = sdl.FindStarByName("HR 1249");
        if (foundStar != null) {
            Debug.Log($"Found star: {foundStar.name}, Magnitude: {foundStar.size}, Position: {foundStar.position}, Color: {foundStar.colour}");
        }
        // GameControl.StartStars();

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                StarDataLoader.Star clickedStar = stars.Find(s => s.name == clickedObject.name);

                // if (clickedStar != null)
                // {
                //     Debug.Log($"Clicked star: {clickedStar.name}, Magnitude: {clickedStar.size}, Position: {clickedStar.position}, Color: {clickedStar.colour}");
                // }
                if (clickedStar != null)
                {
                    Canvas.SetActive(true);
                    starNameText.text = $"Name: {clickedStar.name}";
                    starMagnitudeText.text = $"Magnitude: {clickedStar.size}";
                    starPositionText.text = $"Position: {clickedStar.position}";
                    starColorText.text = $"Color: {clickedStar.colour}";
                }

            }
        }
    }

    // Metoda do usuwania istniejących gwiazd
    private void RemoveExistingStars() {
        if (starObjects != null) { // Check if the list is not null
            foreach (GameObject star in starObjects) {
                Destroy(star); // Destroy the star object
            }
            starObjects.Clear(); // Clear the list of objects
        }
    }

    // Przykładowa metoda do pobierania zestawu gwiazd na podstawie indeksu
    private List<StarDataLoader.Star> GetStarSetByIndex(int index) {
        // Implementacja logiki do zwracania zestawu gwiazd na podstawie indeksu
        return new List<StarDataLoader.Star>(); // Zastąp to rzeczywistymi danymi
    }

    private void FixedUpdate() {
        if (Input.GetKey(KeyCode.Mouse1)) {
            Camera.main.transform.RotateAround(Camera.main.transform.position, Camera.main.transform.right, Input.GetAxis("Mouse Y"));
            Camera.main.transform.RotateAround(Camera.main.transform.position, Vector3.up, -Input.GetAxis("Mouse X"));
        }
    }

    private void OnValidate() {
        if (starObjects != null) {
            for (int i = 0; i < starObjects.Count; i++) {
                Material material = starObjects[i].GetComponent<MeshRenderer>().material;
                material.SetFloat("_Size", Mathf.Lerp(starSizeMin, starSizeMax, stars[i].size));
            }
        }
    }

}



// [System.Serializable]
// public class StarPointInfo
// {
//     public string Loc1;
//     public string Loc2; // Kolor jako string w formacie hex
//     public string Apparent_Magnitude;
//     public string Spectral_Type;

//     public StarPointInfo(string Loc1, string Loc2, string Apparent_Magnitude, string Spectral_Type)
//     {
//         this.Loc1 = Loc1;
//         this.Loc2 = Loc2;
//         this.Apparent_Magnitude = Apparent_Magnitude;
//         this.Spectral_Type = Spectral_Type;
//     }
// }
