using UnityEngine;

public class GameControl : MonoBehaviour
{
    private bool gamePaused = false;
    public GameObject Menu;
    public GameObject GameUI;
    // public GameObject LoadingScreen;

    private StarField StarField;

    public GameObject PanelStarInfo;

    // public void LoadingScreenON(int index)
    // {
    //     LoadingScreen.SetActive(true);
    //     Menu.SetActive(false);
    //     GameUI.SetActive(false);


    //     StarField = FindObjectOfType<StarField>();
        
    //     // Sprawdź, czy GameControl nie jest null
    //     if (StarField != null)
    //     {
    //         StarField.LoadStars(index); // Wywołanie funkcji z GameControl
    //     }
    //     else
    //     {
    //         Debug.LogError("Nie znaleziono GameControl!");
    //     }
    // }

    public void StartStars()
    {
        Menu.SetActive(false);
        GameUI.SetActive(true);
        // LoadingScreen.SetActive(false);
    }

    public void PauseGame()
    {
        Menu.SetActive(true);
        GameUI.SetActive(false);
    }

    void Start()
    {
        PauseGame();
        // LoadingScreen.SetActive(false);
        PanelStarInfo.SetActive(false);
    }
    

    void Update()
    {
        // Sprawdzenie, czy naciśnięto klawisz Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Menu.activeSelf)
            {
                StartStars();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void turnOffStarInfo()
    {
        PanelStarInfo.SetActive(false);
    }
}

