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

    public GameObject myPrefab;
    public GameObject prefab;

    private int clientID;

    private float speed = 1;

    private List<GameObject> clientObjects;


    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    
    private BinaryWriter bWriter;
    private BinaryReader bReader;
    private void Start()
    {
        clientID = -1;
        clientObjects = new List<GameObject>();
    }


    public void ConnectedToServer()
    {
        //if already connected, ignore this function
        if (socketReady)
            return;
        // Default host / port 
        string host = "127.0.0.1";
        int port = 6321;

        string h;
        int p;
        h = GameObject.Find("HostInputField").GetComponent<InputField>().text;
        if (h != "")
            host = h;
        int.TryParse(GameObject.Find("PortInputField").GetComponent<InputField>().text, out p);
        if (p != 0)
            port = p;

        //create the socket
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            
            bWriter = new BinaryWriter(stream);
            bReader = new BinaryReader(stream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error : " + e.Message);
        }

    }
    private void Update()
    {
        float  move = speed * Time.deltaTime;

        float ver = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");

        Vector3 moveAmount = Vector3.forward * ver * move + Vector3.right * hor * move;
        if (socketReady)
        {
            
            if (moveAmount.magnitude > 0 && clientID >=0)
            {
                bWriter.Write((int)MessageID.MOVE);
                bWriter.Write(clientID);
                bWriter.Write(moveAmount.x);
                bWriter.Write(moveAmount.y);
                bWriter.Write(moveAmount.z);
            }

            if (stream.DataAvailable)
            {
                
                OnIncomingData();
                
            }
        }
    }
    private void OnIncomingData()
    {

        //        Debug.Log(data);
        //        Debug.Log(data.Split('|')[0]);
        //        Debug.Log(data.Split('|')[1]);
        //        Debug.Log(data.Split('|')[2]);

        int messageID = bReader.ReadInt32();
        int id;
        float x=-1, y=-1, z=-1;
        byte[] buffer;

        switch(messageID)
        {
            case (int)MessageID.INIT:
                Debug.Log("init");
                id = bReader.ReadInt32();
                clientID = id;
                x = bReader.ReadSingle();
                y = bReader.ReadSingle();
                z = bReader.ReadSingle();
                break;
            
            case (int)MessageID.NEW:
                Debug.Log("new");
                id = bReader.ReadInt32();
                x = bReader.ReadSingle();
                y = bReader.ReadSingle();
                z = bReader.ReadSingle();
                Debug.Log("id:" + id + " x:" + x + "y:" + y + "z: " + z);

                if(id == clientID)
                {
                    clientObjects.Add(Instantiate(myPrefab, new Vector3(x, y, z), Quaternion.identity));
                }
                else
                {
                    clientObjects.Add(Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity));
                }
                break;
            
            case (int)MessageID.MOVE:
                Debug.Log("move");
                id = bReader.ReadInt32();
                x = bReader.ReadSingle();
                y = bReader.ReadSingle();
                z = bReader.ReadSingle();
                Debug.Log("id:" + id + " x:" + x + "y:" + y + "z: " + z);
                clientObjects[id].GetComponent<Transform>().position = new Vector3(x, y, z);
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
