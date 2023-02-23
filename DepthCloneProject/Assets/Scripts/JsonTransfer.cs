using System.IO;
using System.Net.Http;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;
using System.Net.Sockets;

public class JsonTransfer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ReadJson(string filePath)
    {
        string jsonString;
        using (StreamReader streamReader = new StreamReader(filePath))
        {
            jsonString = streamReader.ReadToEnd();
        }

        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);  // Convert to byte array
    }

    private void TCPConnect()
    {
        TcpClient client = new TcpClient();
        client.Connect("192.168.61.2", 9199);
        NetworkStream stream = client.GetStream();
        
        stream.Write(jsonBytes, 0, jsonBytes.Length);
    }

    private void UDPConnect()
    {
        UdpClient client = new UdpClient();
        client.Connect("192.168.61.2", 9199);
        NetworkStream stream = client.GetStream();

        UdpClient client = new UdpClient();
        client.Send(jsonBytes, jsonBytes.Length, "192.168.61.2", 9199);
    }
}
