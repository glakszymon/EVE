using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TcpClientUnity : MonoBehaviour
{
    private string serverIp = "192.168.43.189";  // Server IP address
    private int port = 12345;               // Server port
    private string csvResponse;             // Store the CSV response

    // Example double values to send
    

    // Start is called before the first frame update
    void Start()
    {
        // Send request asynchronously
        //Task.Run(() => SendData(x, y, z));
    }

    public async void SendData(float x, float y, float z)
    {
        Debug.Log("DSFADSF");
        await SendRequest(x, y, z);
    }


    public async Task SendRequest(float x, float y , float z)
    {
        Debug.Log("Zaczyna");
        // Create a TCP client
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
        Debug.Log("Koniec");
    }

    // Process the CSV response (optional)
    private void ProcessCsvResponse(string csvData)
    {
        string csvFilePath = "assets/scripts/csvrecieved.csv";
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

    // You can optionally display the CSV response in the UI, such as in Unity's OnGUI or using UI Text elements
    void OnGUI()
    {
        if (!string.IsNullOrEmpty(csvResponse))
        {
            GUI.Label(new Rect(10, 10, 300, 20), "CSV Response: " + csvResponse);
        }
    }
}