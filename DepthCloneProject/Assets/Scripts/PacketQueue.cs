using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketQueue
{
    struct PacketInfo{
        public int offset;
        public int size;
    };

    // To save the point cloud data
    private MemoryStream m_streamBuffer;

    private List<PacketInfo> m_offsetList;

    private int m_offset = 0;

    private Object lockObj = new Object();

    // Initialize the packet queue
    public PacketQueue()
    {
        m_streamBuffer = new MemoryStream();
        m_offsetList = new List<PacketInfo>();
    }

    public int Enqueue(byte[] data, int size)
    {
        PacketInfo info = new PacketInfo();

        info.offset = m_offset;
        info.size = size;

        lock(lockObj)
        {
            // Add the packet to the queue
            m_offsetList.Add(info);

            // Write the packet to the stream
            m_streamBuffer.Position = m_offset;
            m_streamBuffer.Write(data, 0, size);
            m_streamBuffer.Flush();
            m_offset += size;
        }

        return size;
    }

    public int Dequeue(ref byte[] buffer, int size)
    {
        if(m_offsetList.Count <= 0)
        {
            return -1;
        }

        int recvSize = 0;
        lock(lockObj)
        {
            PacketInfo info = m_offsetList[0];
            int dataSize = Mathf.Min(size, info.size);
            m_streamBuffer.Position = info.offset;
            recvSize = m_streamBuffer.Read(buffer, 0, dataSize);

            if(recvSize > 0)
            {
                m_offsetList.RemoveAt(0);
            }

            if(m_offsetList.Count == 0)
            {
                Clear();
                m_offset = 0;
            }
        }

        return recvSize;
    }

    public void Clear()
    {
        byte[] buffer = m_streamBuffer.GetBuffer();
        System.Array.Clear(buffer, 0, buffer.Length);

        m_streamBuffer.Position = 0;
        m_streamBuffer.SetLength(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
