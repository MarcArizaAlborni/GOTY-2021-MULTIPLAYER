using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;

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


    private int packetSequenceNum = 0;
    private Socket serverSocket;
    private IPEndPoint[] clientAddresses;
    [SerializeField] private int maxClients = 5;
    private int numberofclients = 0;
    public int targetTickRate = 60;
    //necesitarem alguna mena de timer
    public int snapshotSendRate = 20;

    Thread listeningThread;
    bool filterAddresses = false;
    object filterLock = new object();

    private void Start()
    {
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
        serverSocket.Bind(ipep);
        clientAddresses = new IPEndPoint[5];


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
        byte[] data = new byte[1700];
        int reciv = serverSocket.ReceiveFrom(data, ref remote);
        bool filter;
        lock (filterLock) 
        {
            filter = filterAddresses; 
        }
        if (reciv != 0)
        {

        }
    }
    //private bool IsClientAddress(IPEndPoint address)
    //{
    //    for (int i = 0; i< numberofclients)
    //}

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