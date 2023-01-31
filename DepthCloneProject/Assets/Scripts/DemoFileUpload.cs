using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoFileUpload : MonoBehaviour
{
    public string m_Ip = "192.168.61.2";
    public int m_Port = 9199;
    private FileTransfer m_FileTransfer;
    public string[] m_UploadFilePaths;  // File paths to upload
    private bool m_IsUploading = false;

    void Start()
    {
        m_FileTransfer = new FileTransfer(m_Port, FileTransfer.ETransfer.Upload, m_Ip);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            Upload();
    }

    void OnApplicationQuit()
    {
        if (m_FileTransfer != null)
        {
            m_FileTransfer.Close();
        }
    }

    void Upload()
    {
        m_FileTransfer.Upload(m_UploadFilePaths, '\\');
    }
}