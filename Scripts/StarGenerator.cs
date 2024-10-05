using UnityEngine;
using System.Collections.Generic;

public class StarGenerator : MonoBehaviour
{
    public GameObject starPrefab; // Prefab świecącego obiektu
    public List<Vector3> starPositions; // Lista pozycji dla świecących obiektów
    //private Material starMaterial;

    void Start()
    {
        // Inicjalizacja listy pozycji
        starPositions = new List<Vector3>();

        // Dodaj przykładowe pozycje (możesz to zmienić)
        for (int i = 0; i < 100000; i++) // Zmień liczbę na dowolną wartość
        {
            float x = Random.Range(-1000f, 1000f);
            float y = Random.Range(-1000f, 1000f);
            float z = Random.Range(-1000f, 1000f);
            starPositions.Add(new Vector3(x, y, z));
        }

/*        starMaterial = new Material(Shader.Find("Standard"));
        starMaterial.color = Color.yellow; // Ustaw kolor na żółty*/

        GenerateStars();
        Debug.Log("done");
    }

    void GenerateStars()
    {
        foreach (Vector3 position in starPositions)
        {
            GameObject star = Instantiate(starPrefab, position, Quaternion.identity);
            //star.GetComponent<Renderer>().material = starMaterial;
        }
    }
}

