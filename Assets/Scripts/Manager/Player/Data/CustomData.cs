using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct CustomData : INetworkSerializable
{
    public int hair;
    public int eyes;
    public int nose;
    public int mouth;
    public int race;
    public int @class;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref eyes);
        serializer.SerializeValue(ref nose);
        serializer.SerializeValue(ref mouth);
        serializer.SerializeValue(ref hair);
        serializer.SerializeValue(ref race);
        serializer.SerializeValue(ref @class);
    }
}
