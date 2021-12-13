using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;


struct PacketHeader
{
    //PacketHeader
    public uint sequenceNumber;
    public short aknowledgeMask;
    public long time;
    //
    //Data
    public byte[] data;
}

public class ServerNetwork : MonoBehaviour
{
    //---------------------------------------Net Simulation---------------------------------------------------------------------------------------
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

    private struct ClientInput
    {
        public float position;
        public float rotation;
        //all these bools send as bitflags i a single byte
        //public bool grounded;
        //public bool shoot;
        //public bool run; we have two velocities
    }

    private struct Client
    {
        public IPEndPoint address;
        uint packetSequenceNum;
        public ClientInput[] inputBuffer;
    }

    [SerializeField] private int maxClients = 2;
    Client[] clients;
    private int packetSequenceNum = 0;
    private Socket serverSocket;
    private int numOfClients = 0;
    public int targetTickRate = 60;
    //necesitarem alguna mena de timer
    public int snapshotSendRate = 20;

    Thread listeningThread;
    object frameProcessingLock = new object();

    private void Start()
    {
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
        serverSocket.Bind(ipep);
        //clientAddresses = new IPEndPoint[maxClients];


        //TODO: improve ticking counter
        //https://www.youtube.com/watch?v=k6JTaFE7SYI&t=2538s (min. 40:40)
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetTickRate;

        NetSimThread = new Thread(sendMessages);
        NetSimThread.IsBackground = true;
        NetSimThread.Start();

        listeningThread = new Thread(Listening);
        listeningThread.IsBackground = true;
        listeningThread.Start();
    }
    private void Update()
    {
        
    }

    private void OnDestroy()
    {
        //Fer un lock del exit???
        lock (exitLock)
        {
            exit = true;
        }
        serverSocket.Close();
    }

    private void Listening()
    {
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)ipep;
        while (true) 
        {
            byte[] data = new byte[1700];
            int reciv = serverSocket.ReceiveFrom(data, ref remote);
            if (reciv != 0)
            {
                lock (frameProcessingLock)
                {
                                        //Checking if the sender is our client
                    bool newClient = true;
                    for (int i = 0; i < numOfClients; ++i)
                    {
                        if (clients[i].address == remote)
                        {
                            newClient = false;
                            break;
                        }
                    }

                    if (newClient)
                    {
                        //create a new client
                        clients[numOfClients] = new Client();
                        clients[numOfClients].address = (IPEndPoint)remote;
                        ++numOfClients;
                    }
                    else
                    {

                        //process the packet
                        MemoryStream stream = new MemoryStream(data);
                        BinaryReader reader = new BinaryReader(stream);
                        //Reading packet header
                        uint SequenceNumber = reader.ReadUInt32();
                        //Client update sequence number
                        //Reading Packet Data
                        ClientInput input = new ClientInput();
                        input.position = reader.ReadSingle();
                        input.rotation = reader.ReadSingle();
                    }
                }
            }
        }
    }

    //---------------------------------------Net Simulation---------------------------------------------------------------------------------------
    public List<Message> messageBuffer = new List<Message>();
    void sendMessage(Byte[] text, IPEndPoint ip)
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
            Debug.Log(m.time.ToString());
        }

    }
    //Run this always in a separate Thread, to send the delayed messages
    void sendMessages()
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
                        serverSocket.SendTo(m.message, m.message.Length, SocketFlags.None, m.ip);
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