using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.IO;
using System.Threading;



public class server : MonoBehaviour
{
    public int port = 6321;

    private bool newOneCame = false;

    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;

    private TcpListener Server;
    private bool serverStarted;

    public float rot_Pirate = 0f;
    public int putNumber = 0;
    public bool turnPass = false;
    public bool killed = false;
    
  
    private void Start()
    {
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();
        
        try
        {
            Server = new TcpListener(IPAddress.Any, port);
            Server.Start();

            startListening();
            serverStarted = true;
            Debug.Log("Server has been started on port" + port.ToString());
        }
        catch(Exception e)
        {
            Debug.Log("Soket error: " + e.Message);
        }
    }

    
    private void Update()
    {
        Camera.main.transform.parent.eulerAngles = Vector3.up * rot_Pirate;
        
        if (!serverStarted)
            return;
        if (newOneCame)
        {
            SendDataToAllClient();
            newOneCame = false;
        }
        foreach (ServerClient c in clients)
        {
            // Is the client still connected?
            if (!IsConnected(c.tcp))
            {
                c.tcp.Close();
                disconnectList.Add(c);
                continue;
            }
            // check for message from the client
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    BinaryReader reader = new BinaryReader(s);
                    
                    if(reader != null)
                    {
                        OnIncomingData(c, reader);
                    }
                }
            }
        }
        for(int i=0; i<disconnectList.Count-1; i++)
        {
            //Broadcast(disconnectList[i].clientName + " has disconnected", clients);

            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
        }
    }
    private void startListening()
    {
        Server.BeginAcceptTcpClient(AcceptTcpClient, Server);
    }
    
    private bool IsConnected(TcpClient c)
    {
        try
        {
            if(c !=null && c.Client !=null & c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;

            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }
     
    
    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        newOneCame = true;
        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));
        startListening();
    }

    private void SendDataToAllClient ()
    {
        byte[] buffer;

        Debug.Log("after loop in AcceptTcpClient");
        //send a message to everyone, say someone has connected
        buffer = Message.getBytes(MessageID.INIT, clients.Count-1, rot_Pirate);
        Broadcast(buffer,new List<ServerClient>() { clients[clients.Count-1]});
        Debug.Log("end of loop in AcceptTcpClient");

    }
    
    //클라이언트로 부터 받은 정보 수정
    private void OnIncomingData(ServerClient c, BinaryReader reader)
    {
        int messageID;
        int id;
        byte[] buffer;
        Debug.Log("begin in OnIncomingData");
        messageID = reader.ReadInt32();

        switch (messageID)
        {
            case (int)MessageID.NEW:
                Debug.Log("new");
                id = reader.ReadInt32();
                rot_Pirate = reader.ReadSingle();
                int randNum = UnityEngine.Random.Range(1, 18);
                //Debug.Log("Object count:" + objNum + "client count:" + clients.Count);
                buffer = Message.getBytes(MessageID.NEW, id, randNum);
                Broadcast(buffer, clients);
                break;
            
            case (int)MessageID.MOVE:
                Debug.Log("move");
                id = reader.ReadInt32();
                rot_Pirate = reader.ReadSingle();
                buffer = Message.getBytes(MessageID.MOVE, id, rot_Pirate);
                Broadcast(buffer, clients);
                break;
            case (int)MessageID.KNIFE:
                Debug.Log("KNIFE");
                id = reader.ReadInt32();
                putNumber = reader.ReadInt32();
                buffer = Message.getBytes(MessageID.KNIFE, id, putNumber);
                Broadcast(buffer, clients);
                break;
            case (int)MessageID.KILLED:
                Debug.Log("Killed");
                id = reader.ReadInt32();
               
                buffer = Message.getBytes(MessageID.KILLED, id);
                Broadcast(buffer, clients);
                break;
            case (int)MessageID.TURNPASS:
                Debug.Log("PASS");
                id = reader.ReadInt32();
                int personNum = (id + 1) % clients.Count;
                buffer = Message.getBytes(MessageID.TURNPASS, id, personNum);
                
                Broadcast(buffer, clients);
                break;
            default:
                Debug.Log("switch default error!!"+ messageID);
                break;
        }
        
    }

    // 모든 클라이언트들에게 전파
    private void Broadcast(byte[] data,List<ServerClient> c1)
    {
        foreach(ServerClient c in c1)
        {
            try
            {
                BinaryWriter writer = new BinaryWriter(c.tcp.GetStream());
                writer.Write(data, 0, 20);
            }
            catch (Exception e)
            {
                Debug.Log("Write Error : " + e.Message + "to client" + c.clientName);
            }
        }
    }
}
public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}
