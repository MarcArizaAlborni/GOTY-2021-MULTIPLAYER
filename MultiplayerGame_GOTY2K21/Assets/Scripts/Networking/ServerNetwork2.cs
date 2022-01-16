using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Text;

public class ServerNetwork2 : MonoBehaviour
{
    myNet serverNetwork = new myNet();

    Thread listeningThread;
    object frameProcessingLock = new object();
    public int targetTickRate = 30;
    private bool once = true;

    private void Awake()
    {
        if (once)
        {
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
}