using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Threading;

public class ServerNetwork2 : MonoBehaviour
{
    [HideInInspector] public myNet serverNetwork = new myNet();
    [HideInInspector] public OnlineMovement[] playerTransforms;

    private Thread listeningThread;
    private object frameProcessingLock = new object();
    public int targetTickRate = 30;
    private bool once = true;

    private void Awake()
    {
        if (once)
        {
            DontDestroyOnLoad(this);
            Application.targetFrameRate = targetTickRate;
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
            serverNetwork.BindAddress(ipep);
            serverNetwork.InitializeJitterSim();
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetTickRate;

            listeningThread = new Thread(Listening);
            listeningThread.IsBackground = true;
            listeningThread.Start();
            once = false;
        }
    }

    private void Update()
    {
        lock (frameProcessingLock)
        {
            serverNetwork.ExecuteAllPendingSnapshots();
        }
        //SendWorldSnapshot();
    }
    private void Listening()
    {
        while (true)
        {
            myNet.RawMessage msg = serverNetwork.ReceiveMessage();
            lock (frameProcessingLock)
            {
                if (!msg.exception)
                    serverNetwork.ProcessMessage(msg);
                serverNetwork.UpdatePendent();
            }
        }
    }
    private void OnEnable()
    {
        myNet.characterChangesEvent += CharactersUpdate;
    }
    private void OnDisable()
    {
        myNet.characterChangesEvent -= CharactersUpdate;
    }
    public void CharactersUpdate(CharacterEvents eve, int index, uint seqNum)
    {
        playerTransforms[index].SetTransform(eve.pos, eve.rot);
        //playerTransforms[index].SetPositionAndRotation(eve.pos, Quaternion.Euler(eve.rot));
        //playerTransforms[index].position = eve.pos;
        //playerTransforms[index].rotation = Quaternion.Euler(eve.rot);
    }
}