using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
public enum MessageID
{
    INIT=10,
    NEW,
    MOVE,
    KNIFE,
    TURNPASS,
    KILLED
}

public class Message : MonoBehaviour
{
    public static byte[] getBytes(MessageID messageID, int clientID, float rot)
    {
        byte[] btBuffer = new byte[4096];
        MemoryStream ms = new MemoryStream(btBuffer, true);
        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((int)messageID);
        bw.Write(clientID);
        bw.Write(rot);
        bw.Close();
        ms.Close();
        return btBuffer;
    }
    
    public static byte[] getBytes(MessageID messageID, int clientID)
    {
        byte[] btBuffer = new byte[4096];
        MemoryStream ms = new MemoryStream(btBuffer, true);
        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((int)messageID);
        bw.Write(clientID);
        bw.Close();
        ms.Close();
        return btBuffer;
    }
    
    
    public static byte[] getBytes(MessageID messageID, int clientID, int num)
    {
        byte[] btBuffer = new byte[4096];
        MemoryStream ms = new MemoryStream(btBuffer, true);
        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((int)messageID);
        bw.Write(clientID);
        bw.Write(num);
        bw.Close();
        ms.Close();
        return btBuffer;
    }
    public int add(int a, int b)
    {
        return a + b;
    }
    
}