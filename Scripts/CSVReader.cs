
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // To jest kluczowe dla EventTrigger
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System;
using UnityEngine.Video;
using System.Globalization;



public class CSVReader : MonoBehaviour
{
    public TextAsset csvFile;
    public GameObject buttonPrefab; // Change this to a Button prefab in the Inspector
    public Transform contentPanel;

    public GameObject Menu;

    public GameObject GameManager;
    public VideoPlayer VideoPlayerLoading;
    public GameObject VideoPlayerObject;

    public Texture2D hoverCursor; // Assign this in the Inspector

    private TcpClientUnity tcpClient;

    private List<Exoplanet> exoplanets = new List<Exoplanet>();

    public StarField starField;

    void Start()
    {  
        VideoPlayerObject.SetActive(false);
        VideoPlayerLoading.Pause();      
        ReadCSV();

        // Menu.SetActive(true);
        
        if (exoplanets.Count > 0)
        {
            PopulateUI();
        }
        else
        {
            Debug.LogWarning("No exoplanets were loaded from the CSV file.");
        }
        
        tcpClient = GetComponent<TcpClientUnity>();
        if (tcpClient == null)
        {
            tcpClient = gameObject.AddComponent<TcpClientUnity>();
        }

        starField = FindObjectOfType<StarField>();
        if (starField == null)
        {
            Debug.LogError("StarField component not found in the scene.");
        }


        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

    }

    

    void ReadCSV()
    {
        string[] lines = csvFile.text.Split(new char[] { '\n' });

        // Assuming the first line contains headers
        List<string> headers = ParseCSVLine(lines[0]);
        int pl_nameIndex = headers.IndexOf("pl_name");
        int hostnameIndex = headers.IndexOf("hostname");
        int pl_radeIndex = headers.IndexOf("pl_rade");
        int pl_bmasseIndex = headers.IndexOf("pl_bmasse");

        for (int i = 1; i < lines.Length; i++)
        {
            List<string> parsedColumns = ParseCSVLine(lines[i]);

            if (parsedColumns.Count > pl_nameIndex && parsedColumns.Count > hostnameIndex && 
                parsedColumns.Count > pl_radeIndex && parsedColumns.Count > pl_bmasseIndex)
            {
                Exoplanet planet = new Exoplanet();
                planet.Name = parsedColumns[pl_nameIndex].Trim();
                planet.Hostname = parsedColumns[hostnameIndex].Trim();
                
                float.TryParse(parsedColumns[pl_radeIndex], out float radius);
                planet.Radius = radius;
                
                float.TryParse(parsedColumns[pl_bmasseIndex], out float mass);
                planet.Mass = mass;

                float.TryParse(parsedColumns[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x);
                planet.x = x;

                float.TryParse(parsedColumns[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y);
                planet.y = y;

                float.TryParse(parsedColumns[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z);
                planet.z = z;

                Debug.Log($"Planet: {planet.Name}, x: {planet.x}, y: {planet.y}, z: {planet.z}");

                exoplanets.Add(planet);
            }
            else
            {
                Debug.LogWarning($"Line {i + 1} does not have enough fields: {lines[i]}");
            }
        }
    }

    private List<string> ParseCSVLine(string csvLine)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        StringBuilder field = new StringBuilder();

        for (int i = 0; i < csvLine.Length; i++)
        {
            if (csvLine[i] == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (csvLine[i] == ',' && !inQuotes)
            {
                result.Add(field.ToString());
                field.Clear();
            }
            else
            {
                field.Append(csvLine[i]);
            }
        }

        result.Add(field.ToString());
        return result;
    }

    



    void PopulateUI()
    {
        foreach (var planet in exoplanets)
        {
            GameObject newButton = Instantiate(buttonPrefab, contentPanel);
            Button buttonComponent = newButton.GetComponent<Button>();
            TextMeshProUGUI textComponent = newButton.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonComponent != null && textComponent != null && planet.Name != null)
        {
            textComponent.text = $"{planet.Name}";

            buttonComponent.onClick.AddListener(() => OnButtonClick(planet));

            // Add event triggers for cursor change and button darkening
            EventTrigger trigger = newButton.AddComponent<EventTrigger>();

            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((data) => { 
                Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);
                DarkenButton(buttonComponent);
            });
            trigger.triggers.Add(enterEntry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => { 
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                ResetButtonColor(buttonComponent);
            });
            trigger.triggers.Add(exitEntry);
        }
            else
            {
                if (buttonComponent == null)
                {
                    Debug.LogError("Button component not found on the prefab.");
                }
                else if (textComponent == null)
                {
                    Debug.LogError("TextMeshProUGUI component not found on the button.");
                }
                else
                {
                    Debug.LogError("Planet Name is null");
                }
            }
        }
    }

    private void DarkenButton(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = Color.Lerp(colors.normalColor, Color.black, 10f);
        button.colors = colors;
    }

    private void ResetButtonColor(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white; // Or whatever the original color was
        button.colors = colors;
    }

    private void ProcessCsvResponse(string csvData)
    {
        Debug.Log("Received CSV data: " + csvData);
        string csvFilePath = "csvrecieved.csv";
         if (!string.IsNullOrEmpty(csvData))
        {
            try
            {
                // Save the CSV data to a file
                File.WriteAllText(csvFilePath, csvData);
                Debug.Log($"CSV data saved to {csvFilePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error saving CSV file: {ex.Message}");
            }
        }
    }

    async void OnButtonClick(Exoplanet planet)
    {
        Menu.SetActive(false);

        VideoPlayerLoading.Play();
        VideoPlayerObject.SetActive(true);

        Debug.Log($"Button clicked for: {planet.Name}, Mass: {planet.Mass}");

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        float x = planet.x;
        float y = planet.y;
        float z = planet.z; // You can set this to any relevant value

        Debug.Log($"Sending coordinates: X: {x}, Y: {y}, Z: {z}");

        string serverIp = "192.168.43.189";  // Server IP address
        int port = 12345;               // Server port
        string csvResponse;  
        
        using (TcpClient client = new TcpClient())
        {
            try
            {
                // Connect to the server
                await client.ConnectAsync(serverIp, port);

                // Get the network stream
                using (NetworkStream stream = client.GetStream())
                {
                    // Prepare the data to send (comma-separated double values)
                    string message = x.ToString(System.Globalization.CultureInfo.InvariantCulture) + ',' + y.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + z.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    Debug.Log(message);
                    byte[] dataToSend = Encoding.ASCII.GetBytes(message);

                    // Send the data to the server
                    await stream.WriteAsync(dataToSend, 0, dataToSend.Length);
                    Debug.Log($"Sent: {message}");

                    // Receive the response from the server
                   using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
                    {
                        csvResponse = await reader.ReadToEndAsync();  // Reads the entire response
                        Debug.Log($"Received CSV data: {csvResponse}");

                        // Process the CSV data (optional)
                        ProcessCsvResponse(csvResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in TCP communication: {ex.Message}");
            }
        }

        Debug.Log("Response from server: ");

        starField.LoadStars(0); 

        VideoPlayerObject.SetActive(false);
        VideoPlayerLoading.Pause();
    }
}

[System.Serializable]
public class Exoplanet
{
    public string Name;
    public string Hostname;
    public float Radius;
    public float Mass;

    public float x;
    public float y;
    public float z;

}
