﻿using UnityEngine;
using System.Reflection;
using System.IO;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class ReceiveEventArgs : EventArgs
{
    public NetBuffer buf { get; set; }

    public ReceiveEventArgs(NetBuffer rBuf)
    {
        this.buf = rBuf;
    }

    public ReceiveEventArgs() { }
}

public class SocketClientManager : MonoBehaviour
{
    #region 接收事件
    public event EventHandler<ReceiveEventArgs> OnReceive_MSG_LOGIN;
    public event EventHandler<ReceiveEventArgs> OnReceive_MSG_LOGOUT;
    #endregion

    #region 事件处理
    void Update()
    {
        if (_respondQueue.Count < 1) return;

        while (_respondQueue.Count > 0)
        {
            KeyValuePair<string, NetBuffer> eve = _respondQueue.Dequeue();

            if (eve.Key == SocketStatusCMD.Connect) OnConnect();
            else if (eve.Key == SocketStatusCMD.Exception) OnException();
            else if (eve.Key == SocketStatusCMD.Disconnect) OnDisconnect();
            else
            {
                var key = (SocketMessageCMD)int.Parse(eve.Key);
                switch (key)
                {
                    case SocketMessageCMD.MSG_LOGIN: OnReceive_MSG_LOGIN(this, new ReceiveEventArgs(eve.Value)); break;
                    case SocketMessageCMD.MSG_LOGOUT: OnReceive_MSG_LOGOUT(this, new ReceiveEventArgs(eve.Value)); break;
                    default:
                        break;
                }
            }
        }
    }
    #endregion

    public UnityAction OnConnectedTCP;

    private static Queue<KeyValuePair<string, NetBuffer>> _respondQueue = new Queue<KeyValuePair<string, NetBuffer>>();

    public static void AddEvent(string _event, NetBuffer data)
    {
        _respondQueue.Enqueue(new KeyValuePair<string, NetBuffer>(_event, data));
    }

    public void SendMessageTCP(SocketMessageCMD rCMD, NetBuffer rBuffer)
    {
        NetBuffer buf = new NetBuffer();
        buf.WriteShort((UInt16)rCMD);
        buf.WriteBytes(rBuffer.ToBytes());
        SocketClient.SendMessageTCP(buf);
    }

    public void SendConnectTCP()
    {
        Util.Add<SocketClient>(this.gameObject);
        SocketClient.SendConnectTCP();
    }

    public void OnConnect()
    {
        DebugConsole.Log("Game Server connected");
        OnConnectedTCP.Invoke();
    }

    public void OnException()
    {
        DebugConsole.LogError("OnException");
    }


    public void OnDisconnect()
    {
        DebugConsole.LogError("OnDisconnect");
    }

    void OnDestroy()
    {
        DebugConsole.Log("SocketClientManager destroy");
    }
}
