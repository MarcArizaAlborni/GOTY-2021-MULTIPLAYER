using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Threading;
using System.IO;

public class ClientNetwork : MonoBehaviour
{
    //---------------------------------------Net Simulation---------------------------------------------------------------------------------------
    [Header("Teacher jitter script")]
    public bool jitter = true;
    public bool packetLoss = true;
    public int minJitt = 0;
    public int maxJitt = 800;
    public int lossThreshold = 90;
    public struct Message
    {
        public Byte[] message;
        public DateTime time;
        public UInt32 id;
        public IPEndPoint ip;

    }
    object myLock = new object();
    object exitLock = new object();
    bool exit = false;
    Thread NetSimThread;
    //--------------------------------------------------------------------------------------------------------------------------------------------
    private uint _sendSequenceNumber = 0;
    //private uint _receiveSequenceNumber = 0;
    public uint sendSequenceNumber
    {
        get
        {
            return _sendSequenceNumber;
        }
    }
    //public void IncrementReceiveSequenceNumber()
    //{
    //    ++_receiveSequenceNumber;
    //}
    public void IncrementSendSequenceNumber()
    {
        ++_sendSequenceNumber;
    }

    [Header("Client Settings")]
    [SerializeField]private GameObject playerPrefab;
    private bool otherPlayer = false;
    [SerializeField] private Transform playerTransform;
    private Transform otherPlayerTransform;

    public ushort sendPerSec = 60;
    private float sendRateSec;
    private float timeSinceSend = 0.0f;

    private bool connected = false;
    private float ackConnectSec = 10.0f;
    private float maxAckConnectSec = 1.5f;

    private uint lastSeqNum = 0;

    private Socket clientSocket;
    private IPEndPoint serverIpep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

    private void Awake()
    {
        sendRateSec = (1f / (float)sendPerSec);
    }
    private void Start()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        NetSimThread = new Thread(sendMessages);
        NetSimThread.IsBackground = true;
        NetSimThread.Start();
    }
    private void Update()
    {
        //Conecting to server
        if(!connected)
        {
            //To be more precise not use delta time but datetime or some system time?
            ackConnectSec += Time.deltaTime;
            if (ackConnectSec > maxAckConnectSec)
            {
                SendHiMessage();
                ackConnectSec = 0.0f;
            }

        }
        while(clientSocket.Poll(0,SelectMode.SelectRead))
        {
            byte[] data = new byte[1700];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 0);
            EndPoint remote = (EndPoint)ipep;
            int reciv = 0;
            bool exception = false;
            try
            {
                reciv = clientSocket.ReceiveFrom(data, ref remote);
            }
            catch(SocketException e)
            {
                switch (e.SocketErrorCode)
                {
                    //TODO: why does this happen
                    case SocketError.ConnectionReset:
                        Debug.Log("The server is not up");
                        exception = true;
                        break;

                }
            }
            if (!exception && serverIpep.Equals(remote) && reciv > 0)
            {
                //Mal haber de setear la variable tot el rato ??
                connected = true;

                MemoryStream stream = new MemoryStream(data);
                BinaryReader reader = new BinaryReader(stream);
                uint lastInputSeqNum = reader.ReadUInt32();
                Vector3 position;
                Vector3 rotation;
                position.x = reader.ReadSingle();
                position.y = reader.ReadSingle();
                position.z = reader.ReadSingle();
                rotation.x = reader.ReadSingle();
                rotation.y = reader.ReadSingle();
                rotation.z = reader.ReadSingle();
                Quaternion rotQuat = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
                if(!otherPlayer)
                {
                    var instantiated = Instantiate(playerPrefab, position, rotQuat);
                    otherPlayerTransform = instantiated.GetComponent<Transform>();
                    otherPlayer = true;
                }
                else
                {
                    if (lastInputSeqNum >= lastSeqNum) 
                    {
                        otherPlayerTransform.localPosition = position;
                        otherPlayerTransform.rotation = rotQuat;
                        lastSeqNum = lastInputSeqNum;
                    }
                }
            }
        }   
    }
    private void LateUpdate()
    {
        timeSinceSend += Time.deltaTime;
        if (timeSinceSend >= sendRateSec)
        {
            //SEND INPUT
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(sendSequenceNumber);
            writer.Write(playerTransform.position.x);
            writer.Write(playerTransform.position.y);
            writer.Write(playerTransform.position.z);
            Vector3 EulerAngles = playerTransform.rotation.eulerAngles;
            writer.Write(EulerAngles.x);
            writer.Write(EulerAngles.y);
            writer.Write(EulerAngles.z);
            IncrementSendSequenceNumber();
            sendMessage(stream.GetBuffer(), serverIpep);
            timeSinceSend = 0.0f;
        }
    }

    //fER UN DESTRUCTOR PER ASSEGURARME K ES CRIDA?
    private void OnDestroy()
    {
        //Fer un lock del exit???
        lock (exitLock)
        {
            exit = true;
        }
        clientSocket.Close();
    }
    private void SendHiMessage()
    {
        sendMessage(Encoding.ASCII.GetBytes("New Player"), serverIpep);
    }


    //---------------------------------------Net Simulation---------------------------------------------------------------------------------------
    public List<Message> messageBuffer = new List<Message>();
    private void sendMessage(Byte[] text, IPEndPoint ip)
    {
        System.Random r = new System.Random();
        if (((r.Next(0, 100) > lossThreshold) && packetLoss) || !packetLoss) // Don't schedule the message with certain probability
        {
            Message m = new Message();
            m.message = text;
            if (jitter)
            {
                m.time = DateTime.Now.AddMilliseconds(r.Next(minJitt, maxJitt)); // delay the message sending according to parameters
            }
            else
            {
                m.time = DateTime.Now;
            }
            m.id = 0;
            m.ip = ip;
            lock (myLock)
            {
                messageBuffer.Add(m);
            }
            //Debug.Log(m.time.ToString());
        }

    }
    //Run this always in a separate Thread, to send the delayed messages
    private void sendMessages()
    {
        Debug.Log("really sending..");
        //Fer un lock del exit???
        bool exiting;
        lock (exitLock)
        {
            exiting = exit;
        }
        while (!exiting)
        {
            DateTime d = DateTime.Now;
            int i = 0;
            //LOCK HERE ???
            if (messageBuffer.Count > 0)
            {
                List<Message> auxBuffer;
                lock (myLock)
                {
                    auxBuffer = new List<Message>(messageBuffer);
                }
                foreach (var m in auxBuffer)
                {
                    if (m.time < d)
                    {
                        clientSocket.SendTo(m.message, m.message.Length, SocketFlags.None, m.ip);
                        lock (myLock)
                        {
                            messageBuffer.RemoveAt(i);
                        }
                        i--;
                        // ???? myLog = Encoding.ASCII.GetString(m.message, 0, m.message.Length);
                        //Debug.Log("message sent!");
                    }
                    i++;
                }
            }
            lock (exitLock)
            {
                exiting = exit;
            }
        }
    }
    //--------------------------------------------------------------------------------------------------------------------------------------------
}
