using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class FileTransferPacket
{
    public string FileName
    {
        get;
        private set;
    }
    public long FileSize
    {
        get;
        private set;
    }

    public FileTransferPacket(string _Filename, long _FileSize)
    {
        FileName = _Filename;
        FileSize = _FileSize;
    }

    public static byte[] Serialize(object data)
    {
        try
        {
            MemoryStream ms = new MemoryStream(1024 * 4); // 4K
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, data);

            return ms.ToArray();
        }
        catch
        {
            return null;
        }
    }
    
    sealed class AllowAllAssemblyVersionsDeserializationBinder : System.Runtime.Serialization.SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;

            string currentAssembly = Assembly.GetExecutingAssembly().FullName;

            assemblyName = currentAssembly;

            typeToDeserialize = Type.GetType(String.Format("{0}, {1}",
                typeName, assemblyName));

            return typeToDeserialize;
        }
    }

    public static object Deserialize(byte[] data)
    {
        try
        {
            MemoryStream ms = new MemoryStream(1024 * 4);
            ms.Write(data, 0, data.Length);

            ms.Position = 0;
            BinaryFormatter bf = new BinaryFormatter();
            bf.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
            object obj = bf.Deserialize(ms);
            ms.Close();
            return obj;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return null;
        }
    }
}

public class FileTransfer
{
    public class FileTransferEventArgs : EventArgs
    {
        private string _Message;
        private string _DownloadFileFullName;
        private string _UploadFileFullName;
        private double _Percent;
        private bool _IsDownloading;
        private bool _IsUpload;

        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }
        public string DownloadFileFullName
        {
            get { return _DownloadFileFullName; }
            set { _DownloadFileFullName = value; }
        }

        public string UploadFileFullName
        {
            get { return _UploadFileFullName; }
            set { _UploadFileFullName = value; }
        }

        public double Percent
        {
            get { return _Percent; }
            set { _Percent = value; }
        }

        public bool IsDownloading
        {
            get { return _IsDownloading; }
            set { _IsDownloading = value; }
        }

        public bool IsUpload
        {
            get { return _IsUpload; }
            set { _IsUpload = value; }
        }
    }

    public enum ETransfer
    {
        Download, Upload
    }

    private IPEndPoint point;
    private TcpClient TCP_FileSocket;
    private TcpListener TCP_Listener;
    private Thread m_Thread;
    private Thread m_ThrdMonitor;
    private bool m_IsTreading;
        
    private string[] m_FilePaths;
    private string[] m_FileNames;
        
    private string m_DownloadDirectory;

    public event EventHandler<FileTransferEventArgs>  EvntDownloadUpload;
    private FileTransferEventArgs eventData = new FileTransferEventArgs();
       
    public FileTransfer(int port, ETransfer type, string ip = null)
    {
        m_IsTreading = true;
        switch (type)
        {
            case ETransfer.Download:
                point = new IPEndPoint(IPAddress.Any, port);
                TCP_Listener = new TcpListener(point);
                m_Thread = new Thread(ThrdTcpReceive);
                break;

            case ETransfer.Upload:
                point = new IPEndPoint(IPAddress.Parse(ip), port);
                break;

            default:
                break;
        }

        m_ThrdMonitor = new Thread(ThrdMonitor);
        m_ThrdMonitor.Start();
    }
        
    public void Download(string downloadDirectory)
    {
        //TCP Lister 시작
        TCP_Listener.Start();

        //저장 폴더 지정
        m_DownloadDirectory = downloadDirectory;

        if (!Directory.Exists(m_DownloadDirectory))//폴더 존재 X -> 생성
        {
            Directory.CreateDirectory(m_DownloadDirectory);
        }

        if (m_Thread.ThreadState == ThreadState.Unstarted)
            m_Thread.Start();
    }

    public void Upload(string[] filePaths, char splite)
    {
        string[] fileNames;

        m_FilePaths = filePaths;
        m_FileNames = new string[m_FilePaths.Length];
        for (int i = 0; i < m_FilePaths.Length; i++)
        {
            fileNames = m_FilePaths[i].Split(splite);
            m_FileNames[i] = fileNames.Last();//파일명만 추출 후 저장
            Debug.Log(m_FileNames[i]);
        }

        m_Thread = new Thread(ThrdTcpSend);
        m_Thread.Start();
    }
  
    public void Close()
    {
        if (TCP_FileSocket != null)
        {
            if (TCP_Listener != null)
            {
                TCP_Listener.Stop();
                if (TCP_FileSocket.Connected)
                {
                    TCP_FileSocket.Close();
                }
            }
        }

        m_IsTreading = false;
        if (m_Thread != null)
        {

            if (TCP_Listener != null)
            {
                TCP_Listener.Stop();
            }
            m_Thread.Join();
            m_ThrdMonitor.Join();
        }
        eventData.Message = "파일전송 서비스 종료";
        Debug.Log(eventData.Message);
    }
        
    private void ThrdTcpReceive()
    {
        NetworkStream networkStream;
        BinaryReader binaryReader;
        int PacketLength;
        byte[] buffers;
        FileTransferPacket fileTransferPacket;
        FileStream fileStream;
        int receiveSize;
        long downloadSize = 0;

        while (m_IsTreading)
        {
            try
            {
                TCP_FileSocket = TCP_Listener.AcceptTcpClient();
                networkStream = TCP_FileSocket.GetStream();
                binaryReader = new BinaryReader(networkStream);

                eventData.Message = "다운로드 시작";
                Debug.Log(eventData.Message);
                eventData.IsDownloading = true;
                               
                PacketLength = binaryReader.ReadInt32();
                
                buffers = binaryReader.ReadBytes(PacketLength);
                fileTransferPacket = (FileTransferPacket)FileTransferPacket.Deserialize(buffers);
                
                fileStream = new FileStream(m_DownloadDirectory + fileTransferPacket.FileName, FileMode.Create);
                eventData.DownloadFileFullName = m_DownloadDirectory + fileTransferPacket.FileName;
                
                while (downloadSize < fileTransferPacket.FileSize)
                {
                    receiveSize = TCP_FileSocket.Client.Receive(buffers);
                    fileStream.Write(buffers, 0, receiveSize);
                    downloadSize += receiveSize;
                    eventData.Percent = ((double)downloadSize / (double)fileTransferPacket.FileSize) * 100.0;
                }

                downloadSize = 0;

                fileStream.Close();
                binaryReader.Close();
                networkStream.Close();
                TCP_FileSocket.Close();

                eventData.IsDownloading = false;//다운로드 대기 표시
                eventData.Message = "다운로드 종료";
                Debug.Log(eventData.Message);
            }
            catch (Exception e)
            {
                Debug.Log("TCPReceive Error : " + e.ToString());
            }
        }
    }
        
    private void ThrdTcpSend()
    {       
        FileStream fileStream;
        BinaryReader binaryReader;
        byte[] buffers;
        FileTransferPacket fileTransferPacket;
        NetworkStream networkStream = null;
        BinaryWriter binaryWriter = null;
        long uploadSize = 0;

        eventData.Message = "파일전송 시작";
        Debug.Log(eventData.Message);
        eventData.IsUpload = true;

        for (int i = 0; i < m_FilePaths.Length; i++)
        {
            try
            {               
                TCP_FileSocket = new TcpClient();
                TCP_FileSocket.Connect(point);

                fileStream = new FileStream(m_FilePaths[i], FileMode.Open, FileAccess.Read);
                binaryReader = new BinaryReader(fileStream);

                fileTransferPacket = new FileTransferPacket(m_FileNames[i], fileStream.Length);
                eventData.UploadFileFullName = m_FilePaths[i];
                                
                if (TCP_FileSocket.Connected)
                {
                    networkStream = TCP_FileSocket.GetStream();
                    binaryWriter = new BinaryWriter(networkStream);

                    binaryWriter.Write(FileTransferPacket.Serialize(fileTransferPacket).Length);

                    buffers = FileTransferPacket.Serialize(fileTransferPacket);
                    binaryWriter.Write(buffers);

                    while (buffers.Length > 0)
                    {
                        buffers = binaryReader.ReadBytes(1024);
                        binaryWriter.Write(buffers);
                        uploadSize += buffers.Length;
                        eventData.Percent = ((double)uploadSize / (double)fileTransferPacket.FileSize) * 100.0;
                    }
                    uploadSize = 0;

                    binaryWriter.Close();
                    networkStream.Close();
                }

                binaryReader.Close();
                fileStream.Close();
                TCP_FileSocket.Close();

            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10061) //Connect 실패 시 -> 대기 후 재접속
                {
                    eventData.Message = "연결실패, 2초 대기 후 재전송 시도";
                    Debug.Log(eventData.Message);
                    Thread.Sleep(2000);
                    m_Thread.Join();
                    m_Thread = new Thread(ThrdTcpSend);
                    m_Thread.Start();
                    ThrdTcpSend();
                }

                else
                {
                    Console.WriteLine(e.ToString());
                }
            }

        }

        eventData.IsUpload = false;
        eventData.Message = "파일전송 종료";
        Debug.Log(eventData.Message);

        if (EvntDownloadUpload != null)
        {
            EvntDownloadUpload(this, eventData);
            EvntDownloadUpload = null;
        }
    }
   
    private void ThrdMonitor()
    {
        while (m_IsTreading)
        {
            if (EvntDownloadUpload != null)
            {
                EvntDownloadUpload(this, eventData);
            }
            Thread.Sleep(100);
        }
    }
}