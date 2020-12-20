using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Client : MonoBehaviour
{
    public string ClientName;

    public int clientID;
    public Text label_ClientID;

    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;

    private BinaryWriter bWriter;
    private BinaryReader bReader;

    private bool myTurn = true;
    float camPoint;

    public CollisionColumnScript ccs;

    public GameObject inputField;

    public HoleScript[] hs;

    public string host = "127.0.0.1";
    public int port = 6321;

    public int turn;

    public Text labelTurn;

   

    void Awake()
    {
        Screen.SetResolution(640,960, FullScreenMode.Windowed);
    }

    private void Start()
    {
        clientID = -1;
      
    }

    public void SetClientName(string param)
    {
        ClientName = param;
    }

    public void SetHost(string param)
    {
        host = param;
    }

    public void SetPort(string param)
    {
        port = int.Parse(param);
    }

    public void ConnectedToServer()
    {
        //if already connected, ignore this function
        if (socketReady)
            return;
        // Default host / port 
        string host = this.host;
        int port = this.port;

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
                        bWriter.Write((int)MessageID.MOVE);
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

    public void PutKnife(int idx)
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
                        bWriter.Write(idx);
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
        if (clientID != turn)
        {
            return;
        }
        
        if (myTurn)
        {
            if (socketReady)
            {
                {
                    if (clientID >= 0)
                    {
                        bWriter.Write((int) MessageID.TURNPASS);
                        bWriter.Write(clientID);
                    }
                }

                if (stream.DataAvailable)
                {
                    OnIncomingData();
                }
            }
        }
    }

    public void Killed()
    {
        if (myTurn)
        {
            if (socketReady)
            {
                {
                    if (clientID >= 0)
                    {
                        bWriter.Write((int) MessageID.KILLED);
                        bWriter.Write(clientID);
                    }
                }

                if (stream.DataAvailable)
                {
                    OnIncomingData();
                }
            }
        }
    }

    // 매 프레임 업데이트 절차 거침
    private void Update()
    {
        if (socketReady)
        {
            if (stream.DataAvailable)
                OnIncomingData();
        }
    }
 
    private void OnIncomingData()
    {
        int messageID = bReader.ReadInt32();

        Debug.Log((bReader));
        int id;
        byte[] buffer;

        switch (messageID)
        {
            case (int) MessageID.INIT:
                Debug.Log("init");
                id = bReader.ReadInt32();
                clientID = id;
                label_ClientID.text = "내 차례: " + (clientID+1).ToString();
                
                buffer = Message.getBytes(MessageID.NEW, id, camPoint);
                Send(buffer);
                break;

            case (int) MessageID.NEW:
                Debug.Log("new");
                id = bReader.ReadInt32();
                int rand = bReader.ReadInt32();
                ccs.randomNum = rand;
                break;

            case (int) MessageID.MOVE:
                Debug.Log("move");
                id = bReader.ReadInt32();
                camPoint = bReader.ReadSingle();
                Camera.main.transform.parent.eulerAngles = Vector3.up * camPoint;
                break;
            case (int) MessageID.KNIFE:
                Debug.Log("KNIFE");
                id = bReader.ReadInt32();
                int putNum = bReader.ReadInt32();
                hs[putNum].SpawnKnife();
                break;
            case (int) MessageID.KILLED:
                Debug.Log("KILLED");
                id = bReader.ReadInt32();
                ccs.Finish();
                break;
            case (int) MessageID.TURNPASS:
                Debug.Log("TurnPass");
                id = bReader.ReadInt32();
                turn = bReader.ReadInt32();
                labelTurn.text = (turn + 1).ToString();
                
                StartCoroutine(ccs.Pass(turn));
                break;
            default:
                Debug.Log(" default in switch. error!!" + messageID);
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