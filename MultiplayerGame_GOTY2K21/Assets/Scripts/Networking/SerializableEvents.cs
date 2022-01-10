using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;
using System.Net;

public struct LobbyInfo
{
    public bool clientReady;       
    public bool forceGameStart;
    public bool gameStart;         
    public List<string> playerList;
    public IPEndPoint addres;
}
public struct PlayerInfo
{

}
public struct ZombieInfo
{

}

class LobbyEvent : SerializableEvents
{
    public bool clientReady;       //If a client is ready
    public bool forceGameStart;    //Less than 5 players but players>2 and want to play
    public bool gameStart;         //All slots completed and players ready

    //uint playerId;          //Necessary?
    public List<string> playerList;

    public override MemoryStream SerializeEvents(MemoryStream stream)
    {
       
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);
        writer.Write(clientReady);
        writer.Write(forceGameStart);
        writer.Write(gameStart);
        writer.Write(playerList.Count);

        for(int i = 0; i < playerList.Count; ++i)
        {
            writer.Write(playerList[i]);
        }

        return stream;
    }

    public override void DeserializeEvents(MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);

        clientReady = reader.ReadBoolean();
        forceGameStart = reader.ReadBoolean();
        gameStart = reader.ReadBoolean();
        int count = reader.ReadInt32();
        for (int i=0; i < count; ++i)
        {
            playerList.Add(reader.ReadString());
        }
    }

    public override void ExecuteEvent(/*ref object obj*/)
    {
        return;
    }
}

class RequestLobbyInfoEvents : SerializableEvents
{
    public override MemoryStream SerializeEvents(MemoryStream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);

        return stream;
    }

    public override void DeserializeEvents(MemoryStream stream)
    {
        return;
    }

    public override void ExecuteEvent(/*ref object obj*/)
    {
        //LobbyInfo info = (LobbyInfo)obj;
        //LobbyEvent eve = new LobbyEvent();
        //eve.networkMessagesType = networkMessages.lobbyEvent;
        //eve.clientReady = info.clientReady;
        //eve.forceGameStart = info.forceGameStart;
        //eve.gameStart = info.gameStart;
        //eve.playerList = info.playerList;
        return;
    }
}
class DisconnectEvents : SerializableEvents
{
    public override MemoryStream SerializeEvents(MemoryStream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);

        return stream;
    }

    public override void DeserializeEvents(MemoryStream stream)
    {
        return;
    }

    public override void ExecuteEvent(/*ref object obj*/)
    {
        throw new NotImplementedException();
    }
}

class DieEvent : SerializableEvents
{
    public override MemoryStream SerializeEvents(MemoryStream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);

        return stream;
    }

    public override void DeserializeEvents(MemoryStream stream)
    {
        return;
    }

    public override void ExecuteEvent()
    {
        throw new NotImplementedException();
    }
}

class SpawnEvent : SerializableEvents
{
    public override MemoryStream SerializeEvents(MemoryStream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);

        return stream;
    }

    public override void DeserializeEvents(MemoryStream stream)
    {
        return;
    }

    public override void ExecuteEvent()
    {
        throw new NotImplementedException();
    }
}

class CharacterEvents : SerializableEvents
{
    uint characterId;

    Transform transform;

    public override MemoryStream SerializeEvents(MemoryStream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);
        writer.Write(characterId);

        writer.Write(transform.position.x);
        writer.Write(transform.position.y);
        writer.Write(transform.position.z);

        writer.Write(transform.eulerAngles.x);
        writer.Write(transform.eulerAngles.y);
        writer.Write(transform.eulerAngles.z);

        return stream;
    }

    public override void DeserializeEvents(MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);

        //not necessary first byte;
        characterId = reader.ReadUInt32();

        //position
        transform.position.Set(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        //rotation
        transform.eulerAngles.Set(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    public override void ExecuteEvent(/*ref object obj*/)
    {
        throw new NotImplementedException();
    }
}


class PlayerEvents : CharacterEvents
{ 
    bool playerShoot;       //Playershoot
    bool currentGun;        //0 pistol 1 rifle;
    bool playerRevived;

    Vector2 inputAnim;

    public override MemoryStream SerializeEvents(MemoryStream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);

        writer.Write(playerShoot);
        writer.Write(currentGun);
        writer.Write(playerRevived);

        writer.Write(inputAnim.x);
        writer.Write(inputAnim.y);  //This will be z axis because y is always 0

        return stream;
    }

    public override void DeserializeEvents(MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);

        playerShoot = reader.ReadBoolean();
        currentGun = reader.ReadBoolean();
        playerRevived = reader.ReadBoolean();

        inputAnim.x = reader.ReadSingle();
        inputAnim.y = reader.ReadSingle();
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

    public override void DeserializeEvents(MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);

        zombieAttack = reader.ReadBoolean();
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

    public override void ExecuteEvent(/*ref object obj*/)
    {
        throw new NotImplementedException();
    }
}

public abstract class SerializableEvents 
{
    public networkMessages networkMessagesType;
    //public DateTime sended;

    public abstract MemoryStream SerializeEvents(MemoryStream stream);

    public abstract void DeserializeEvents(MemoryStream stream);

    public abstract void ExecuteEvent(/*ref object obj*/);
}

