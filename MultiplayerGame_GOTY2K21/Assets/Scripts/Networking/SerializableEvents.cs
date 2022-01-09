using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;

class MainLoopEvents : SerializableEvents
{
    bool clientReady;       //If a client is ready
    bool forceGameStart;    //Less than 5 players but players>2 and want to play
    bool gameStart;         //All slots completed and players ready

    List<string> playerList;

    public override MemoryStream SerializeEvents(MemoryStream stream)
    {
       
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);
        writer.Write(clientReady);
        writer.Write(forceGameStart);
        writer.Write(gameStart);

        return stream;
    }

    public override void DeserializeEvents(MemoryStream stream)
    {
        throw new NotImplementedException();
    }
}

class DisconnectEvents : SerializableEvents
{
    public override MemoryStream SerializeEvents(MemoryStream stream)
    {
        throw new NotImplementedException();
    }

    public override void DeserializeEvents(MemoryStream stream)
    {
        throw new NotImplementedException();
    }
}


class CharacterEvents : SerializableEvents
{
    uint id;

    Vector3 position;
    Vector3 rotation;

    bool hasDied;           //Zombie or Player dead 
    bool hasSpawned;

    public override MemoryStream SerializeEvents(MemoryStream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);

        return stream;
    }

    public override void DeserializeEvents(MemoryStream stream)
    {
        throw new NotImplementedException();
    }
}


class PlayerEvents : CharacterEvents
{
    bool playerShoot;       //Playershoot
    bool currentGun;        //0 pistol 1 rifle;

    public override MemoryStream SerializeEvents(MemoryStream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);

        return stream;
    }

}

class ZombieEvents : CharacterEvents
{
    bool zombieAttack;       //Zombie Attack 
    public override MemoryStream SerializeEvents(MemoryStream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);

        return stream;
    }
}


class EnviorentmentEvents : SerializableEvents
{
    public override MemoryStream SerializeEvents(MemoryStream stream)
    { 
        throw new NotImplementedException();
    }

    public override void DeserializeEvents(MemoryStream stream)
    {
    }
}

public abstract class SerializableEvents 
{
    public networkMessages networkMessagesType;

    public abstract MemoryStream SerializeEvents(MemoryStream stream);

    public abstract void DeserializeEvents(MemoryStream stream);
   
}

