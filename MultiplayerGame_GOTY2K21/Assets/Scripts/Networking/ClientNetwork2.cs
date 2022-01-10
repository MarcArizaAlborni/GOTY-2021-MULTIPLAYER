using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Threading;
using System.IO;
using UnityEngine.SceneManagement;

public class ClientNetwork2 : MonoBehaviour
{
    [HideInInspector] public myNet clientNet = new myNet();

    [SerializeField] private GameObject playerPrefab;
    //private bool otherPlayer = false;
    //[SerializeField] private Transform playerTransform;
    //private Transform otherPlayerTransform;

    public ushort sendPerSec = 60;
    private float sendRateSec;
    private float timeSinceSend = 0.0f;

    private IPEndPoint serverIpep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

    private void Awake()
    {
        sendRateSec = (1f / (float)sendPerSec);
        clientNet.InitializeJitterSim();
    }
    private void Update()
    {
        clientNet.UpdatePendent();
        while (clientNet.PollReceive())
        {
            myNet.RawMessage msg = clientNet.ReceiveMessage();
            if(!msg.exception)
                clientNet.ProcessMessage(msg);
        }
        if (clientNet.newConnexion)
        {
            clientNet.PollNewConnexions();
            DontDestroyOnLoad(this);
            SceneManager.LoadScene("LobyScene");
        }
        //Conecting to server
        //if (!connected)
        //{
        //    //To be more precise not use delta time but datetime or some system time?
        //    ackConnectSec += Time.deltaTime;
        //    if (ackConnectSec > maxAckConnectSec)
        //    {
        //        SendHiMessage();
        //        ackConnectSec = 0.0f;
        //    }
        //
        //}
        //while (clientSocket.Poll(0, SelectMode.SelectRead))
        //{
        //    byte[] data = new byte[1700];
        //    IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 0);
        //    EndPoint remote = (EndPoint)ipep;
        //    int reciv = 0;
        //    bool exception = false;
        //    try
        //    {
        //        reciv = clientSocket.ReceiveFrom(data, ref remote);
        //    }
        //    catch (SocketException e)
        //    {
        //        switch (e.SocketErrorCode)
        //        {
        //            //TODO: why does this happen
        //            case SocketError.ConnectionReset:
        //                Debug.Log("The server is not up");
        //                exception = true;
        //                break;
        //
        //        }
        //    }
        //    if (!exception && serverIpep.Equals(remote) && reciv > 0)
        //    {
        //        //Mal haber de setear la variable tot el rato ??
        //        connected = true;
        //        MemoryStream stream = new MemoryStream(data);
        //        BinaryReader reader = new BinaryReader(stream);
        //        ServerSnapshot snap = new ServerSnapshot();
        //        snap.lastInputSeqNum = reader.ReadUInt32();
        //        snap.time = Time.realtimeSinceStartupAsDouble;
        //        snap.position.x = reader.ReadSingle();
        //        snap.position.y = reader.ReadSingle();
        //        snap.position.z = reader.ReadSingle();
        //        snap.rotation.x = reader.ReadSingle();
        //        snap.rotation.y = reader.ReadSingle();
        //        snap.rotation.z = reader.ReadSingle();
        //        Quaternion rotQuat = Quaternion.Euler(snap.rotation.x, snap.rotation.y, snap.rotation.z);
        //
        //        if (!otherPlayer)
        //        {
        //            var instantiated = Instantiate(playerPrefab, snap.position, rotQuat);
        //            otherPlayerTransform = instantiated.GetComponent<Transform>();
        //            otherPlayer = true;
        //        }
        //        else
        //        {
        //            //la seva pose no nomes es canvia kuan rebo el paket!!!!
        //            //fer el tije al k e rebut el paket
        //            //si no es mou laltre player k no senvii paket
        //            if (snap.lastInputSeqNum >= lastSeqNum)
        //            {
        //                otherPlayerTransform.localPosition = snap.position;
        //                otherPlayerTransform.rotation = rotQuat;
        //                lastSeqNum = snap.lastInputSeqNum;
        //            }
        //        }
        //    }
        //}
    }

    private void LateUpdate()
    {
        timeSinceSend += Time.deltaTime;
        if (timeSinceSend >= sendRateSec)
        {
            //SEND INPUT
            //MemoryStream stream = new MemoryStream();
            //BinaryWriter writer = new BinaryWriter(stream);
            //writer.Write(sendSequenceNumber);
            //writer.Write(playerTransform.position.x);
            //writer.Write(playerTransform.position.y);
            //writer.Write(playerTransform.position.z);
            //Vector3 EulerAngles = playerTransform.rotation.eulerAngles;
            //writer.Write(EulerAngles.x);
            //writer.Write(EulerAngles.y);
            //writer.Write(EulerAngles.z);
            //IncrementSendSequenceNumber();
            //sendMessage(stream.GetBuffer(), serverIpep);
            //timeSinceSend = 0.0f;
        }
    }

    public void SendNameToServer(string name)
    {
        clientNet.InitializeConexion(serverIpep, name);
    }
    private float LinearInterpolation(float currentY, float xLow, float xHigh, float yLow, float yHigh)
    {
        return xLow + ((xHigh - xLow) / (yHigh - yLow)) * (currentY - yLow);
    }

    struct dataSnapShot
    {





    }



    /*public void SerializeMessage(networkMessages message, object obj=null,object obj2=null, object obj3=null)
    {

        MemoryStream stream = new MemoryStream();

        BinaryWriter writer = new BinaryWriter(stream);

        switch (message)
        {

            case networkMessages.clientReady:

                writer.Write((byte)message);

                break;

            case networkMessages.forceGameStart:
                writer.Write((byte)message);
                break;

            case networkMessages.gameStart:
                writer.Write((byte)message);
                break;

            case networkMessages.playerPositions:
                writer.Write((byte)message);

                writer.Write(((Vector3)obj).x);
                writer.Write(((Vector3)obj).y);
                writer.Write(((Vector3)obj).z);

                writer.Write(((Vector3)obj2).x);
                writer.Write(((Vector3)obj2).y);
                writer.Write(((Vector3)obj2).z);

                writer.Write((int)obj3);

                break;

            case networkMessages.playerShoot:
                writer.Write((byte)message);

                writer.Write(((Vector3)obj).x);
                writer.Write(((Vector3)obj).y);
                writer.Write(((Vector3)obj).z);

                writer.Write(((Vector3)obj2).x);
                writer.Write(((Vector3)obj2).y);
                writer.Write(((Vector3)obj2).z);

                //writer.Write((int)obj3); //Questionable

                break;

            case networkMessages.playerDown:
                writer.Write((byte)message);
                writer.Write((int)obj);
                break;

            case networkMessages.playerRevived:
                writer.Write((byte)message);
                writer.Write((int)obj);
                break;

            case networkMessages.playerSwapGun:
                writer.Write((byte)message);
                writer.Write((int)obj);
                break;

            case networkMessages.zombiePositions:
                writer.Write((byte)message);

                writer.Write(((Vector3)obj).x);
                writer.Write(((Vector3)obj).y);
                writer.Write(((Vector3)obj).z);

                writer.Write(((Vector3)obj2).x);
                writer.Write(((Vector3)obj2).y);
                writer.Write(((Vector3)obj2).z);

                writer.Write((int)obj3);

                break;

            case networkMessages.zombieAttack:
                writer.Write((byte)message);
                writer.Write((int)obj2);
                break;

            case networkMessages.zombieDie:
                writer.Write((byte)message);
                writer.Write((int)obj);
                break;

            case networkMessages.zombieSpawned:
                writer.Write((byte)message);

                writer.Write(((Vector3)obj).x);
                writer.Write(((Vector3)obj).y);
                writer.Write(((Vector3)obj).z);

                writer.Write((int)obj2);

                break;

            case networkMessages.doorOpen:
                writer.Write((byte)message);
                writer.Write((int)obj);
                break;

            case networkMessages.messageEnd:
                writer.Write((byte)message);
                break;

        }

    }


    public void DeserializeMessage(byte[] data)
    {

        MemoryStream stream = new MemoryStream(data);

        BinaryReader reader = new BinaryReader(stream);

        networkMessages messagesNet = (networkMessages)reader.ReadByte();


        while (messagesNet != networkMessages.messageEnd)
        {

            switch (messagesNet)
            {

                case networkMessages.clientReady:

                    break;

                case networkMessages.forceGameStart:
         
                    break;

                case networkMessages.gameStart:
                 
                    break;

                case networkMessages.playerPositions:
                    //Pos
                    reader.ReadSingle();
                    reader.ReadSingle();
                    reader.ReadSingle();
                    //Rot
                    reader.ReadSingle();
                    reader.ReadSingle();
                    reader.ReadSingle();

                    reader.ReadInt32();

                    break;

                case networkMessages.playerShoot:
                    //Pos
                    reader.ReadSingle();
                    reader.ReadSingle();
                    reader.ReadSingle();
                    //Rot
                    reader.ReadSingle();
                    reader.ReadSingle();
                    reader.ReadSingle();

                    //writer.Write((int)obj3); //Questionable

                    break;

                case networkMessages.playerDown:
                    reader.ReadInt32();
                    break;

                case networkMessages.playerRevived:
                    reader.ReadInt32();
                    break;

                case networkMessages.playerSwapGun:
                    reader.ReadInt32();
                    break;

                case networkMessages.zombiePositions:
                    //Pos
                    reader.ReadSingle();
                    reader.ReadSingle();
                    reader.ReadSingle();

                    //Rot
                    reader.ReadSingle();
                    reader.ReadSingle();
                    reader.ReadSingle();

                    reader.ReadInt32();

                    break;

                case networkMessages.zombieAttack:
                    reader.ReadInt32();
                    break;

                case networkMessages.zombieDie:
                    reader.ReadInt32();
                    break;

                case networkMessages.zombieSpawned:

                    //Pos
                    reader.ReadSingle();
                    reader.ReadSingle();
                    reader.ReadSingle();

                    reader.ReadInt32();

                    break;

                case networkMessages.doorOpen:

                    reader.ReadInt32();
                    break;

                

            }

             messagesNet = (networkMessages)reader.ReadByte();
        }



    }*/


}



public enum networkMessages:byte
{
    //Loby
    requestLobbyInfo,
    lobbyEvent,

    clientReady, //Player sets ready state
    forceGameStart, //Player forces game to start players>2
    gameStart,//Game starts, tell all players
    playerList,

    clientDisconnect,

    characterId,
    characterPos,
    characterRot,
    characterDie,
    characterSpawn,

    //Gameplay Player
    playerShoot, //If player has shot
    playerCurrentGun,
    playerRevived, //If player is revived
    playerInputAnimation,

    //Gameplay Zombies
    zombieAttack,


    //Gameplay Environment
    doorOpen,


    //Gameplay Loop
   
}


//Serialitzar i deserialitzar la data dels missatges

//Treure el id (funcionarem amb el socket)
//Fer el accept/acknowledgement de una conexio
//Fer el send i el receive
//Fer el rtt
public class myNet
{
    public void InitializeJitterSim()
    {
        NetSimThread = new Thread(sendMessages);
        NetSimThread.IsBackground = true;
        NetSimThread.Start();
    }
    public struct PacketHeader
    {
        public uint seqNum;
        public uint lastRecSeqNum;
        public ushort ackBitmask;
        public ushort time;
    }
    public struct Packet
    {
        public PacketHeader header;
        byte[] data;
    }
    public enum ConnexionState : byte
    {
        disconnected,
        connected,
        badNameReceived,
        waitingNameAcceptAck,
        waitingInputBadName,
        requestNewConnection
    }
    public class Connexion
    {
        public IPEndPoint address;
        public uint sendSeqNum = 0;
        public uint lastRecSeqNum = 0;
        public ushort ackBitmask = 0;
        public string name = "";
        public ConnexionState state = ConnexionState.disconnected;
        public DateTime lastReceivedMsgTime = new DateTime(1, 1, 1);
        public List<SerializableEvents> eventsToSend = new List<SerializableEvents>();
        public Queue<byte[]> lastMessagesSended = new Queue<byte[]>();
        //public float rtt = 0.0f;
        //public uint numMsgToTestRtt = 0;
        //public uint currMsgToTestRtt = 0;
    }
    ~myNet()
    {
        lock (exitLock)
        {
            exit = true;
        }
        mySocket.Close();
    }
    private Socket mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    public void BindAddress(IPEndPoint address)
    {
        mySocket.Bind(address);
    }
    private Connexion[] currentConnexions = new Connexion[0];
    public int numOfConnexions
    {
        get
        {
            return currentConnexions.Length;
        }
    }
    private Connexion[] pendingConnexions = new Connexion[0];
    private bool _newConnexion = false;
    public bool newConnexion
    {
        get
        {
            return _newConnexion;
        }
    }
    private IPEndPoint[] newConnexions = new IPEndPoint[0];
    public IPEndPoint[] PollNewConnexions()
    {
        IPEndPoint[] connexions = newConnexions;
        newConnexions = new IPEndPoint[0];
        _newConnexion = false;
        return connexions;
    }
    public bool badName
    {
        get
        {
            return _badName;
        }
    }
    private bool _badName = false;
    private List<IPEndPoint> badNames = new List<IPEndPoint>();
    public List<IPEndPoint> PollBadNames()
    {
        List<IPEndPoint> names = badNames;
        badNames.Clear();
        _badName = false;
        return names;
    }
    public void SendNewName(string name, IPEndPoint address)
    {
        int pendIndex = FindPendingConnexionIndex((IPEndPoint)address);
        if (pendIndex != -1)
        {
            sendMessage(Encoding.ASCII.GetBytes("New Connexion " + name), (IPEndPoint)address);
        }
    }
    //public bool acceptConnexions = true;
    public void InitializeConexion(IPEndPoint address, string name)
    {
        if (FindConnexionIndex(address) != -1)
            return;
        Connexion[] newHeaders = new Connexion[pendingConnexions.Length + 1];
        int i = 0;
        foreach (Connexion con in pendingConnexions)
        {
            if(con.address.Equals(address))
            {
                //connexion already pending, just send the new name request
                con.name = name;
                con.state = ConnexionState.requestNewConnection;
                sendMessage(Encoding.ASCII.GetBytes("New Connexion " + name), (IPEndPoint)address);
                con.lastReceivedMsgTime = DateTime.Now;
                return;
            }
            newHeaders[i] = con;
            ++i;
        }
        pendingConnexions = newHeaders;
        pendingConnexions[pendingConnexions.Length - 1] = new Connexion();
        pendingConnexions[pendingConnexions.Length - 1].address = address;
        pendingConnexions[pendingConnexions.Length - 1].name = name;
        pendingConnexions[pendingConnexions.Length - 1].state = ConnexionState.requestNewConnection;
        pendingConnexions[pendingConnexions.Length - 1].lastReceivedMsgTime = DateTime.Now;
        //Send to acknowledge message
        sendMessage(Encoding.ASCII.GetBytes("New Connexion " + name), (IPEndPoint)address);
    }
    private void ConnexionBadName(IPEndPoint address, string name)
    {
        Connexion[] newHeaders = new Connexion[pendingConnexions.Length + 1];
        int i = 0;
        foreach (Connexion con in pendingConnexions)
        {
            if (con.address.Equals(address))
            {
                //connexion already pending, just send the new name request
                con.name = name;
                sendMessage(Encoding.ASCII.GetBytes("Bad Name"), address);
                con.lastReceivedMsgTime = DateTime.Now;
                return;
            }
            newHeaders[i] = con;
            ++i;
        }
        pendingConnexions = newHeaders;
        pendingConnexions[pendingConnexions.Length - 1] = new Connexion();
        pendingConnexions[pendingConnexions.Length - 1].address = address;
        pendingConnexions[pendingConnexions.Length - 1].name = name;
        pendingConnexions[pendingConnexions.Length - 1].state = ConnexionState.badNameReceived;
        sendMessage(Encoding.ASCII.GetBytes("Bad Name"), address);
        pendingConnexions[pendingConnexions.Length - 1].lastReceivedMsgTime = DateTime.Now;
    }
    private void NewConnexion(Connexion connexion)
    {
        RemovePendingConnexion(connexion.address);
        Connexion[] newHeaders = new Connexion[currentConnexions.Length + 1];
        int i = 0;
        foreach (Connexion con in currentConnexions)
        {
            newHeaders[i] = con;
            ++i;
        }
        currentConnexions = newHeaders;
        currentConnexions[currentConnexions.Length - 1] = connexion;
        NewConnexionToList(connexion.address);
    }
    private void NewConnexionToList(IPEndPoint address)
    {
        IPEndPoint[] newConnexionsArray = new IPEndPoint[newConnexions.Length + 1];
        int i = 0;
        foreach (IPEndPoint con in newConnexions)
        {
            newConnexionsArray[i] = con;
            ++i;
        }
        newConnexions = newConnexionsArray;
        newConnexions[newConnexions.Length - 1] = address;
        _newConnexion = true;
    }
    public void RemoveConnexion(IPEndPoint address)
    {
        int i = 0;
        bool found = false;
        for (; i < currentConnexions.Length; ++i)
        {
            if (found)
                currentConnexions[i] = currentConnexions[i - 1];
            if (currentConnexions[i].Equals(address))
            {
                found = true;
            }
        }
        if (found)
        {
            Connexion[] newHeaders = new Connexion[currentConnexions.Length - 1];
            foreach (Connexion con in currentConnexions)
            {
                newHeaders[i] = con;
                ++i;
            }
        }
        else
        {
            RemovePendingConnexion(address);
        }
    }
    private void RemovePendingConnexion(IPEndPoint address)
    {
        int i = 0;
        bool found = false;
        for (; i < pendingConnexions.Length; ++i)
        {
            if (found)
                pendingConnexions[i] = pendingConnexions[i - 1];
            if (pendingConnexions[i].address.Equals(address))
            {
                found = true;
            }
        }
        if (found)
        {
            Connexion[] newHeaders = new Connexion[pendingConnexions.Length - 1];
            for (int j=0; j<pendingConnexions.Length-1;++j)
            {
                newHeaders[j] = pendingConnexions[j];
                ++i;
            }
            pendingConnexions = newHeaders;
        }
    }
    private int FindConnexionIndex(IPEndPoint address)
    {
        for (int i = 0; i < currentConnexions.Length; ++i)
        {
            if (currentConnexions[i].address.Equals(address))
                return i;
        }
        return -1;
    }
    private int FindPendingConnexionIndex(IPEndPoint address)
    {
        for (int i = 0; i < pendingConnexions.Length; ++i)
        {
            if (pendingConnexions[i].address.Equals(address))
                return i;
        }
        return -1;
    }
    private bool NameExists(string name)
    {
        if (name == "")
        {
            return true;
        }
        for (int i = 0; i < currentConnexions.Length; ++i)
        {
            if (currentConnexions[i].name == name)
                return true;
        }
        return false;
    }
    private double resendSeconds = 2;
    public void UpdatePendent()
    {
        foreach(Connexion con in pendingConnexions)
        {
            if (DateTime.Now > con.lastReceivedMsgTime.AddSeconds(resendSeconds)) 
            {
                switch (con.state)
                {
                    case ConnexionState.waitingInputBadName:
                        sendMessage(Encoding.ASCII.GetBytes("Bad Name"), con.address);
                        con.lastReceivedMsgTime = DateTime.Now;
                        break;
                    case ConnexionState.waitingNameAcceptAck:
                        sendMessage(Encoding.ASCII.GetBytes("Ready"), con.address);
                        con.lastReceivedMsgTime = DateTime.Now;
                        break;
                    case ConnexionState.requestNewConnection:
                        sendMessage(Encoding.ASCII.GetBytes("New Connexion " + con.name), con.address);
                        con.lastReceivedMsgTime = DateTime.Now;
                        break;
                } 
            }
        }
    }
    public void RemoveAllConnections()
    {
        currentConnexions = new Connexion[0];
    }

    //public void SendMessage(int id, byte[] data)
    //{
    //    ++currentConnexions[index].seqNum;
    //}
    //public void SendMessageToAll(int id, byte[] data)
    //{
    //    ++currentConnexions[index].seqNum;
    //}
    //public void SendMessageToAllBut(int id, byte[] data)
    //{
    //    ++currentConnexions[index].seqNum;
    //}
    public bool PollReceive()
    {
        if (mySocket.Poll(0, SelectMode.SelectRead))
            return true;
        return false;
    }
    public struct RawMessage
    {
        public bool exception;
        public IPEndPoint remote;
        public byte[] data;
        public int reciv;
    }
    public RawMessage ReceiveMessage()
    {
        byte[] data = new byte[1700];
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)ipep;
        int reciv = 0;
        try
        {
            reciv = mySocket.ReceiveFrom(data, ref remote);
        }
        catch (SocketException e)
        {
            RawMessage msg = new RawMessage();
            msg.exception = true;
            switch (e.SocketErrorCode)
            {
                //TODO: HANDLE EXCEPTIONS !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                case SocketError.ConnectionReset:
                    Debug.Log("The connexion is not up");
                    //K fem kuan es desconecta algu ???
                    return msg;
                //WSACancelBlockingCall() by the Socket.close()
                case SocketError.Interrupted:
                    Debug.Log("Socket was closed");
                    return msg;
            }
        }
        RawMessage message = new RawMessage();
        message.exception = false;
        message.remote = (IPEndPoint)remote;
        message.data = data;
        message.reciv = reciv;
        return message;
    }
    private List<SerializableEvents> EventsToExecute = new List<SerializableEvents>();
    public void ExecutePendingEvents()
    {
        foreach(SerializableEvents eve in EventsToExecute)
        {
            eve.ExecuteEvent();
        }
        EventsToExecute.Clear();
    }
    private List<SerializableEvents> GetPendingEvents()
    {
        List<SerializableEvents> returnList = EventsToExecute;
        EventsToExecute.Clear();
        return returnList;
    }
    public void ProcessMessage(RawMessage msg)
    {
        int index = FindConnexionIndex(msg.remote);
        //packet de algu connectat
        if (index != -1)
        {
            //message accepting conections
            //Warning: puc rebre acki algun missatge trash k arriba tard al fer els acknowledgements
            //AIXO STA MAL: revisar
            string received = Encoding.ASCII.GetString(msg.data, 0, msg.reciv);
            if (received.StartsWith("New Connexion ") || received == "Bad Name" || received == "Ready")
                return;

            if (currentConnexions[index].state == ConnexionState.waitingNameAcceptAck)
            {
                RemovePendingConnexion(msg.remote);
                currentConnexions[index].state = ConnexionState.connected;
            }

            MemoryStream stream = new MemoryStream();
            BinaryReader reader = new BinaryReader(stream);
            uint recSeqNum = reader.ReadUInt32();
            uint lastAckseqNum = reader.ReadUInt32();
            ushort sckBitmask = reader.ReadUInt16();
            if(recSeqNum <= currentConnexions[index].lastRecSeqNum)
            {
                //check if packet is to old
                if ((currentConnexions[index].lastRecSeqNum - recSeqNum) > 16)
                    return;
                //chack if we already have the packet
                uint resta = currentConnexions[index].lastRecSeqNum - recSeqNum;
                if ((currentConnexions[index].ackBitmask & (1 << (char)resta)) == 1 && currentConnexions[index].sendSeqNum > resta)
                    return;

                //old new packet (update bitmask)
                currentConnexions[index].ackBitmask |= (ushort)(1 << (char)resta);
            }
            else
            {
                //update bitmask
                uint resta = currentConnexions[index].lastRecSeqNum - recSeqNum;
                currentConnexions[index].ackBitmask <<= (char)resta;
                currentConnexions[index].lastRecSeqNum = recSeqNum;
            }

            //resend failed acknowledge messages
            if ((currentConnexions[index].ackBitmask & (1 << 5)) == 0 && currentConnexions[index].sendSeqNum > 5)
            {
                ResendEvent(currentConnexions[index].address, currentConnexions[index].lastMessagesSended.ToArray()[4]);
            }
            else if ((currentConnexions[index].ackBitmask & (1 << 10)) == 0 && currentConnexions[index].sendSeqNum > 10)
            {
                ResendEvent(currentConnexions[index].address, currentConnexions[index].lastMessagesSended.ToArray()[9]);
            }
            else if ((currentConnexions[index].ackBitmask & (1 << 16)) == 0 && currentConnexions[index].sendSeqNum > 16)
            {
                ResendEvent(currentConnexions[index].address, currentConnexions[index].lastMessagesSended.ToArray()[15]);
            }

            int i = reader.ReadInt32();
            for (;i>0;--i)
            {
                EventsToExecute.Add(DeserializeEvent(stream));
            }

        }
        //packet de algu no connectat
        else
        {
            string received = Encoding.ASCII.GetString(msg.data, 0, msg.reciv);
            if (received.StartsWith("New Connexion "))
            {
                string name = received.Substring(14);
                if (NameExists(name))
                {
                    ConnexionBadName(msg.remote, name);
                }
                else
                {
                    //Acknowledging a connection
                    Connexion newCon = new Connexion();
                    newCon.address = msg.remote;
                    newCon.name = name;
                    newCon.state = ConnexionState.waitingNameAcceptAck;
                    newCon.lastReceivedMsgTime = DateTime.Now;
                    NewConnexion(newCon);
                    sendMessage(Encoding.ASCII.GetBytes("Ready"), msg.remote);
                }
            }
            else if (received == "Bad Name")
            {
                int pendIndex = FindPendingConnexionIndex(msg.remote);
                if (pendIndex != -1)
                {
                    pendingConnexions[pendIndex].state = ConnexionState.waitingInputBadName;
                    _badName = true;
                }
            }
            else if (received == "Ready")
            {
                int pendIndex = FindPendingConnexionIndex(msg.remote);
                if (pendIndex != -1)
                {
                    NewConnexion(pendingConnexions[pendIndex]);
                    currentConnexions[currentConnexions.Length - 1].state = ConnexionState.connected;
                }
            }
        }
    }
    private void ResendEvent(IPEndPoint address, byte[] eve)
    {
        sendMessage(eve, address);
    }
    public void AddEventToSend(IPEndPoint address, SerializableEvents eve)
    {
        int id = FindConnexionIndex(address);
        if (id == -1)
            return;
        currentConnexions[id].eventsToSend.Add(eve);
    }
    public void SendAllEvents()
    {
        foreach (Connexion con in currentConnexions)
        {
            //Packet packet = new Packet();
            //packet.header = new PacketHeader();
            //packet.header.seqNum = con.sendSeqNum;
            //++con.sendSeqNum;
            //packet.header.lastRecSeqNum = con.lastRecSeqNum;
            //packet.header.ackBitmask = con.ackBitmask;
            //falta el time pel rtt

            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            //writing the header
            ++con.sendSeqNum;
            writer.Write(con.sendSeqNum);
            writer.Write(con.lastRecSeqNum);
            writer.Write(con.ackBitmask);
            //writer.Write(currentConnexions[id].time);

            writer.Write(con.eventsToSend.Count);
            //writing the serialized messages
            foreach (SerializableEvents eve in con.eventsToSend)
            {
                eve.SerializeEvents(stream);
            }

            byte[] data = stream.ToArray();
            con.lastMessagesSended.Enqueue(data);
            if(con.lastMessagesSended.Count >= 16)
                con.lastMessagesSended.Dequeue();
            sendMessage(data, con.address);
            con.eventsToSend.Clear();
        }
    }

    //---------------------------------------Deserialize---------------------------------------------------------------------------------------
    public SerializableEvents DeserializeEvent(MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);

        networkMessages messageType = (networkMessages)reader.ReadByte();

        SerializableEvents serializable = null;

        switch (messageType)
        {
            case networkMessages.clientReady:
                serializable = new LobbyEvent();
                break;
            case networkMessages.forceGameStart:
                serializable = new LobbyEvent();
                break;
            case networkMessages.gameStart:
                serializable = new LobbyEvent();
                break;
            case networkMessages.playerList:
                serializable = new LobbyEvent();
                break;
            case networkMessages.clientDisconnect:
                serializable = new DisconnectEvents();
                break;
            case networkMessages.characterId:
                serializable = new CharacterEvents();
                break;
            case networkMessages.characterPos:
                serializable = new CharacterEvents();
                break;
            case networkMessages.characterRot:
                serializable = new CharacterEvents();
                break;
            case networkMessages.characterDie:
                serializable = new CharacterEvents();
                break;
            case networkMessages.characterSpawn:
                serializable = new CharacterEvents();
                break;
            case networkMessages.playerShoot:
                serializable = new PlayerEvents();
                break;
            case networkMessages.playerRevived:
                serializable = new PlayerEvents();
                break;
            case networkMessages.playerCurrentGun:
                serializable = new PlayerEvents();
                break;
            case networkMessages.playerInputAnimation:
                serializable = new PlayerEvents();
                break;
            case networkMessages.zombieAttack:
                serializable = new ZombieEvents();
                break;
            case networkMessages.doorOpen:
                serializable = new LobbyEvent();
                break;


            case networkMessages.requestLobbyInfo:
                serializable = new RequestLobbyInfoEvents();
                break;
            case networkMessages.lobbyEvent:
                serializable = new LobbyEvent();
                break;

        }

        serializable.networkMessagesType = messageType;
        serializable.DeserializeEvents(stream);

        return serializable;
    }

    //---------------------------------------Net Simulation---------------------------------------------------------------------------------------
    public bool jitter = true;
    public bool packetLoss = true;
    public int minJitt = 0;
    public int maxJitt = 800;
    public int lossThreshold = 10;
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
                        mySocket.SendTo(m.message, m.message.Length, SocketFlags.None, m.ip);
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