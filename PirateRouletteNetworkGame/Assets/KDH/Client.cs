using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    public string ClientName;
    
    private int clientID;
    
    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    
    private BinaryWriter bWriter;
    private BinaryReader bReader;

    private bool myTurn = true;
    float camPoint;

    public CollisionColumnScript ccs;

    public GameObject inputField;
    
    private void Start()
    {
        clientID = -1;
    }

    public void SetClientName(string param)
    {
        ClientName = param;
    }


    public void ConnectedToServer()
    {
        //if already connected, ignore this function
        if (socketReady)
            return;
        // Default host / port 
        string host = "127.0.0.1";
        int port = 6321;

        //create the socket
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            
            bWriter = new BinaryWriter(stream);
            bReader = new BinaryReader(stream);
            socketReady = true;
            
            inputField.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.Log("Socket error : " + e.Message);
        }

    }

    public void SetCamPointValue(float value)
    {
        camPoint = value;
        if (myTurn)
        {
            if (socketReady)
            {
                {
                    if (clientID >= 0)
                    {
                        bWriter.Write((int) MessageID.MOVE);
                        bWriter.Write(clientID);
                        bWriter.Write(camPoint);
                    }
                }

                if (stream.DataAvailable)
                {
                    OnIncomingData();
                }
            }
        }
    }

    public void PutKnife()
    {
        if (myTurn)
        {
            if (socketReady)
            {
                {
                    if (clientID >= 0)
                    {
                        bWriter.Write((int) MessageID.KNIFE);
                        bWriter.Write(clientID);
                        bWriter.Write(value);
                    }
                }

                if (stream.DataAvailable)
                {
                    OnIncomingData();
                }
            }
        }
    }

    public void TurnPass()
    {
        
    }

    public void Killed()
    {
        
    }
    
    // 매 프레임 업데이트 절차 거침
    private void Update()
    {
        camPoint = Camera.main.transform.parent.eulerAngles.x;
        if (socketReady)
        {
            if(stream.DataAvailable)
                OnIncomingData();
        }
    }


    private void OnIncomingData()
    {
        int messageID = bReader.ReadInt32();

        Debug.Log((bReader));
        int id;
        byte[] buffer;

        switch(messageID)
        {
            case (int)MessageID.INIT:
                Debug.Log("init");
                id = bReader.ReadInt32();
                clientID = id;
                buffer = Message.getBytes(MessageID.NEW, id, camPoint);
                Send(buffer);
                break;
            
            case (int)MessageID.NEW:
                Debug.Log("new");
                id = bReader.ReadInt32();
                camPoint = bReader.ReadSingle();
                Camera.main.transform.parent.eulerAngles = Vector3.up * camPoint;
                break;
            
            case (int)MessageID.MOVE:
                Debug.Log("move");
                id = bReader.ReadInt32();
                camPoint = bReader.ReadSingle();
                Camera.main.transform.parent.eulerAngles = Vector3.up * camPoint;
                break;    
            case (int)MessageID.KNIFE:
                Debug.Log("KNIFE");
                id = bReader.ReadInt32();
                int putNum = bReader.ReadInt32();
                break;
            case (int)MessageID.KILLED:
                Debug.Log("KILLED");
                id = bReader.ReadInt32();
                bool killed = bReader.ReadBoolean();
                if(killed)
                    ccs.Finish();
                break;
            case (int)MessageID.TURNPASS:
                Debug.Log("TurnPass");
                id = bReader.ReadInt32();
                bool passed = bReader.ReadBoolean();
                if (passed)
                    StartCoroutine(ccs.Pass());
                break;
            default:
                Debug.Log(" default in switch. error!!"+messageID);
                break;
        }
    }

    private void Send(byte[] data)
    {
        if (!socketReady)
            return;
        bWriter.Write(data, 0, 20);
        Debug.Log("sent data: " + data);
    }
    public void OnSendButton()
    {
        //string message = GameObject.Find("SendInput").GetComponent<InputField>().text;
        //Send(message);
    }
    private void CloseSocket()
    {
        if (!socketReady)
            return;
        bWriter.Close();
        bReader.Close();
        socket.Close();
        socketReady = false;
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }
    private void OnDisable()
    {
        CloseSocket();
    }
}
