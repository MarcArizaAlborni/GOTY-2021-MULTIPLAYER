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
        clientNet.ExecuteAllPendingSnapshots();
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
    public void AddEvent(SerializableEvents eve)
    {
        clientNet.AddEventToSend(serverIpep, eve);
    }
    private float LinearInterpolation(float currentY, float xLow, float xHigh, float yLow, float yHigh)
    {
        return xLow + ((xHigh - xLow) / (yHigh - yLow)) * (currentY - yLow);
    }

}

public enum networkMessages:byte
{
    //Loby
    requestLobbyInfo,       //Request Lobby
    lobbyEvent,             //LobbyEvent
    clientDisconnect,
    characterDie,
    characterSpawn,
    characterTransformEvents,
    playerEvents, 
    zombieEvents,
    enviorentEvents, 
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
        public Queue<byte[]> lastMessagesSended = new Queue<byte[]>(); //max size 24 (line 951)
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
    public struct Snapshot
    {
        public IPEndPoint address;
        public uint seqNum;
        //time received/sended ???
        public List<SerializableEvents> snapshotEvents;
    }
    private List<Snapshot> snapshotsToExecute = new List<Snapshot>();
    private void AddSnapshotsOrdered(ref Snapshot snap)
    {
        if (snapshotsToExecute.Count == 0)
        {
            snapshotsToExecute.Add(snap);
            return;
        }

        for (int i = snapshotsToExecute.Count - 1; i >= 0; --i)
            if (snap.seqNum > snapshotsToExecute[i].seqNum)
            {
                snapshotsToExecute.Insert(i + 1, snap);
                break;
            }
    }
    public delegate void UpdateLobbyTextList(LobbyEvent eve);
    public static event UpdateLobbyTextList lobbyEvent;
    public void ExecuteAllPendingSnapshots()
    {
        foreach(Snapshot snap in snapshotsToExecute)
            foreach(SerializableEvents eve in snap.snapshotEvents)
                ExecuteEvent(eve, snap.address);
        snapshotsToExecute.Clear();
        SendAllEvents();
    }
    private void ExecuteEvent(SerializableEvents eve, IPEndPoint address)
    {

        networkMessages messageType = eve.networkMessagesType;

        switch (messageType)
        {
            case networkMessages.requestLobbyInfo:
                //RequestLobbyInfoEvents req = (RequestLobbyInfoEvents)eve;
                LobbyEvent sendEve = new LobbyEvent();
                sendEve.networkMessagesType = networkMessages.lobbyEvent;
                //sendEve.clientReady = info.clientReady;
                //sendEve.forceGameStart = info.forceGameStart;
                //sendEve.gameStart = info.gameStart;
                List<string> names = new List<string>();
                foreach (Connexion con in currentConnexions)
                    names.Add(con.name);
                sendEve.playerList = names;
                AddEventToSend(address, sendEve);
                break;
            case networkMessages.lobbyEvent:
                if (lobbyEvent != null)
                    lobbyEvent((LobbyEvent)eve);
                break;
            case networkMessages.clientDisconnect:
                //Aki s'ha de fer com un timer de si no rebo missatge del client en x temps esta desconectat i el trec
                RemoveConnexion(address);
                break;
            case networkMessages.characterDie:

                break;
            case networkMessages.characterSpawn:

                break;
            case networkMessages.characterTransformEvents:

                break;
            case networkMessages.playerEvents:

                break;
            case networkMessages.zombieEvents:

                break;
            case networkMessages.enviorentEvents:

                break;
        }
    }
    //private List<SerializableEvents> GetPendingEvents()
    //{
    //    List<SerializableEvents> returnList = EventsToExecute;
    //    EventsToExecute.Clear();
    //    return returnList;
    //}
    public void ProcessMessage(RawMessage msg)
    {
        //FALTA EL DISCONECT
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

            MemoryStream stream = new MemoryStream(msg.data);
            BinaryReader reader = new BinaryReader(stream);
            uint recSeqNum = reader.ReadUInt32();
            uint lastAckseqNum = reader.ReadUInt32();
            ushort sckBitmask = reader.ReadUInt16();
            if(recSeqNum <= currentConnexions[index].lastRecSeqNum)
            {
                //check if packet is too old
                if ((currentConnexions[index].lastRecSeqNum - recSeqNum) > 16)
                    return;
                //chack if we already have the packet (packet duplication)
                uint resta = currentConnexions[index].lastRecSeqNum - recSeqNum;
                if ((currentConnexions[index].ackBitmask & (1 << (char)resta)) == 1 && (currentConnexions[index].sendSeqNum > resta))
                    return;

                //packet older than last received (update bitmask)
                currentConnexions[index].ackBitmask |= (ushort)(1 << (char)resta);
            }
            else
            {
                //update bitmask
                uint resta = recSeqNum - currentConnexions[index].lastRecSeqNum;
                currentConnexions[index].ackBitmask <<= (int)resta;
                currentConnexions[index].ackBitmask += 1;
                currentConnexions[index].lastRecSeqNum = recSeqNum;
            }

            //we maybe will have repeated resending if we process more than 1 message at a time!!!!
            uint indexLastSended = currentConnexions[index].sendSeqNum - lastAckseqNum;
            //resend failed acknowledge messages
            if ((sckBitmask & (1 << 4)) == 0 && lastAckseqNum > 5 && (4 + indexLastSended) < 24/*MaxArraySize*/)
            {
                ResendMessage(currentConnexions[index].address, currentConnexions[index].lastMessagesSended.ToArray()[4 + indexLastSended]);
            }
            if ((sckBitmask & (1 << 9)) == 0 && lastAckseqNum > 10 && (9 + indexLastSended) < 24/*MaxArraySize*/)
            {
                ResendMessage(currentConnexions[index].address, currentConnexions[index].lastMessagesSended.ToArray()[9 + indexLastSended]);
            }
            if ((sckBitmask & (1 << 15)) == 0 && lastAckseqNum > 16 && (15 + indexLastSended) < 24/*MaxArraySize*/)
            {
                ResendMessage(currentConnexions[index].address, currentConnexions[index].lastMessagesSended.ToArray()[15 + indexLastSended]);
            }

            int i = reader.ReadInt32();
            if (i <= 0)
                return;
            Snapshot snapshot = new Snapshot();
            snapshot.address = msg.remote;
            snapshot.seqNum = recSeqNum;
            snapshot.snapshotEvents = new List<SerializableEvents>();
            for (;i>0;--i)
            {
                snapshot.snapshotEvents.Add(DeserializeEvent(ref stream));
            }
            //podriem construir l'snapshot ia a la llista !!!!!
            AddSnapshotsOrdered(ref snapshot);
            //snapshotsToExecute.Add(snapshot);
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
    private void ResendMessage(IPEndPoint address, byte[] eve)
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
            if (con.eventsToSend.Count == 0)
                break;

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
                eve.SerializeEvents(ref stream);
            }

            byte[] data = stream.ToArray();
            con.lastMessagesSended.Enqueue(data);
            if(con.lastMessagesSended.Count >= 25)
                con.lastMessagesSended.Dequeue();
            if (data.Length > 1500)
                Debug.Log(data.Length);
            sendMessage(data, con.address);
            con.eventsToSend.Clear();
        }
    }

    //---------------------------------------Deserialize---------------------------------------------------------------------------------------
    public SerializableEvents DeserializeEvent(ref MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);

        networkMessages messageType = (networkMessages)reader.ReadByte();

        SerializableEvents serializable = null;

        switch (messageType)
        {

            case networkMessages.requestLobbyInfo:
                serializable = new RequestLobbyInfoEvents();
                break;
            case networkMessages.lobbyEvent:
                serializable = new LobbyEvent();
                break;
            case networkMessages.clientDisconnect:
                serializable = new DisconnectEvents();
                break;
            case networkMessages.characterDie:
                serializable = new DieEvent();
                break;
            case networkMessages.characterSpawn:
                serializable = new SpawnEvent();
                break;
            case networkMessages.characterTransformEvents:
                serializable = new CharacterEvents();
                break;
            case networkMessages.playerEvents:
                serializable = new PlayerEvents();
                break;
            case networkMessages.zombieEvents:
                serializable = new ZombieEvents();
                break;
            case networkMessages.enviorentEvents:
                serializable = new EnviorentmentEvents();
                break;
        }

        serializable.networkMessagesType = messageType;
        serializable.DeserializeEvents(ref stream);

        return serializable;
    }

    //---------------------------------------Net Simulation---------------------------------------------------------------------------------------
    public bool jitter = true;
    public bool packetLoss = true;
    public int minJitt = 0;
    public int maxJitt = 200;
    public int lossThreshold = 5;
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