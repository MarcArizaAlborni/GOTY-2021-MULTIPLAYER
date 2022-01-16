using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;

public class LobbyEvent : SerializableEvents
{
    public bool gameStart;         //All slots completed and players ready

    //uint playerId;          //Necessary?
    public List<string> playerList;

    public override void SerializeEvents(ref MemoryStream stream)
    {
       
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);
        writer.Write(gameStart);
        writer.Write(playerList.Count);

        for(int i = 0; i < playerList.Count; ++i)
        {
            writer.Write(playerList[i]);
        }

    }

    public override void DeserializeEvents(ref MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);

        gameStart = reader.ReadBoolean();
        int count = reader.ReadInt32();
        playerList = new List<string>();
        for (int i=0; i < count; ++i)
        {
            playerList.Add(reader.ReadString());
        }
    }
}

public class RequestLobbyInfoEvents : SerializableEvents
{
    public bool clientReady;       //If a client is ready
    public bool forceGameStart;    //Less than 5 players but players>2 and want to play
    public override void SerializeEvents(ref MemoryStream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);
        writer.Write(clientReady);
        writer.Write(forceGameStart);
    }

    public override void DeserializeEvents(ref MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        clientReady = reader.ReadBoolean();
        forceGameStart = reader.ReadBoolean();
    }
}
public class DisconnectEvents : SerializableEvents
{
    public override void SerializeEvents(ref MemoryStream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);

    }

    public override void DeserializeEvents(ref MemoryStream stream)
    {
        return;
    }
}

public class DieEvent : SerializableEvents
{
    public override void SerializeEvents(ref MemoryStream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);

    }

    public override void DeserializeEvents(ref MemoryStream stream)
    {
        return;
    }
}

public class SpawnEvent : SerializableEvents
{
    public override void SerializeEvents(ref MemoryStream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);

    }

    public override void DeserializeEvents(ref MemoryStream stream)
    {
        return;
    }
}

public class CharacterEvents : SerializableEvents
{
    uint characterId;

    Transform transform;

    public override void SerializeEvents(ref MemoryStream stream)
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

    }

    public override void DeserializeEvents(ref MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);

        //not necessary first byte;
        characterId = reader.ReadUInt32();

        //position
        transform.position.Set(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        //rotation
        transform.eulerAngles.Set(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }
}


public class PlayerEvents : CharacterEvents
{ 
    bool playerShoot;       //Playershoot
    bool currentGun;        //0 pistol 1 rifle;
    bool playerRevived;

    Vector2 inputAnim;

    public override void SerializeEvents(ref MemoryStream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);

        writer.Write(playerShoot);
        writer.Write(currentGun);
        writer.Write(playerRevived);

        writer.Write(inputAnim.x);
        writer.Write(inputAnim.y);  //This will be z axis because y is always 0

    }

    public override void DeserializeEvents(ref MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);

        playerShoot = reader.ReadBoolean();
        currentGun = reader.ReadBoolean();
        playerRevived = reader.ReadBoolean();

        inputAnim.x = reader.ReadSingle();
        inputAnim.y = reader.ReadSingle();
    }

}

public class ZombieEvents : CharacterEvents
{
    bool zombieAttack;       //Zombie Attack 
    public override void SerializeEvents(ref MemoryStream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write((byte)networkMessagesType);

    }

    public override void DeserializeEvents(ref MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);

        zombieAttack = reader.ReadBoolean();
    }
}


public class EnviorentmentEvents : SerializableEvents
{
    public override void SerializeEvents(ref MemoryStream stream)
    { 
        throw new NotImplementedException();
    }

    public override void DeserializeEvents(ref MemoryStream stream)
    {
    }
}

public abstract class SerializableEvents 
{
    public networkMessages networkMessagesType;
    //public DateTime sended;

    public abstract void SerializeEvents(ref MemoryStream stream);

    public abstract void DeserializeEvents(ref MemoryStream stream);

}

