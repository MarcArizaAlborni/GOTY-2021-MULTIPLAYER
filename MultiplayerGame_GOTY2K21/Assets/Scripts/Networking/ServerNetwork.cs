using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Text;


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
    private struct ClientInput
    {
        public uint lastInputSeqNum;
        public Vector3 position;
        public Quaternion rotation;
        //all these bools send as bitflags i a single byte
        //public bool grounded;
        //public bool shoot;
        //public bool run; we have two velocities
    }

    private class Client
    {
        //public int rtt;
        public IPEndPoint address;
        private uint _sendSequenceNumber = 0;
        private uint _receiveSequenceNumber = 0;
        public uint sendSequenceNumber{
            get
            {
                return _sendSequenceNumber;
            }
        }
        public void IncrementReceiveSequenceNumber()
        {
            ++_receiveSequenceNumber;
        }
        public void IncrementSendSequenceNumber()
        {
            ++_sendSequenceNumber;
        }
        private ClientInput lastInput = new ClientInput();
        public bool inputToRead = false;
        public void StoreInput(ClientInput input)
        {
            if (input.lastInputSeqNum > lastInput.lastInputSeqNum)
            {
                inputToRead = true;
                lastInput.lastInputSeqNum = input.lastInputSeqNum;
            }
        }
        public ClientInput GetStoredInput()
        {
            inputToRead = false;
            return lastInput;
        }
        //private List<ClientInput> inputBufer = new List<ClientInput>(16);
        //public void StoreInput(ClientInput input)
        //{ 
        //    inputBufer.Insert(0, input);
        //}
    }

    [Header("Server Settings")]
    //private uint serverTick;
    [SerializeField] private int maxClients = 2;
    private int numOfClients = 0;
    Client[] clients;
    private int packetSequenceNum = 0;
    private Socket serverSocket;
    public int targetTickRate = 30;

    Thread listeningThread;
    object frameProcessingLock = new object();

    private void Awake()
    {
        Application.targetFrameRate = targetTickRate;
    }

    private void Start()
    {
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
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
        lock(frameProcessingLock)
        {
            //ProcessClientInputs();

            //Simulate world

            //TROLL
            EchangePlayersPositions();
        }
        //SendWorldSnapshot();
    }

    //fER UN DESTRUCTOR PER ASSEGURARME K ES CRIDA?
    private void OnDestroy()
    {
        //Fer un lock del exit???
        lock (exitLock)
        {
            exit = true;
        }
        serverSocket.Close();
    }

    //TROLL
    private void EchangePlayersPositions()
    {
        for (int i = 0; i < numOfClients; ++i)
        {
            if (clients[i].inputToRead)
            {
                ClientInput input = clients[i].GetStoredInput();
                MemoryStream stream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(clients[i].sendSequenceNumber);
                writer.Write(input.position.x);
                writer.Write(input.position.y);
                writer.Write(input.position.z);
                writer.Write(input.rotation.x);
                writer.Write(input.rotation.y);
                writer.Write(input.rotation.z);
                for (int j = 0; j < numOfClients; ++j)
                {
                    if (j != i)
                    {
                        clients[i].IncrementSendSequenceNumber();
                        sendMessage(stream.GetBuffer(),clients[i].address);
                    }
                }
            }
        }
    }
    private void ProcessClientInputs()
    {
        for (int i = 0; i < numOfClients; ++i)
        {
            if(clients[i].inputToRead)
            {
                clients[i].GetStoredInput();
            }
        }
    }
    private void SendWorldSnapshot()
    {
        
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
                    Client client = new Client();
                    bool newClient = true;
                    for (int i = 0; i < numOfClients; ++i)
                    {
                        if (clients[i].address == remote)
                        {
                            newClient = false;
                            client = clients[i];
                            break;
                        }
                    }

                    if (newClient && Encoding.ASCII.GetString(data,0,reciv) == "New Player")
                    {
                        //create a new client
                        if (numOfClients < maxClients)
                        {
                            clients[numOfClients] = new Client();
                            clients[numOfClients].address = (IPEndPoint)remote;
                            ++numOfClients;
                        }
                        //Send refuse message
                        //else
                        //{
                        //    
                        //}
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
                        input.lastInputSeqNum = SequenceNumber;
                        input.position.x = reader.ReadSingle();
                        input.position.y = reader.ReadSingle();
                        input.position.z = reader.ReadSingle();
                        input.rotation.x = reader.ReadSingle();
                        input.rotation.y = reader.ReadSingle();
                        input.rotation.z = reader.ReadSingle();
                        client.StoreInput(input);
                    }
                }
            }
        }
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
            Debug.Log(m.time.ToString());
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