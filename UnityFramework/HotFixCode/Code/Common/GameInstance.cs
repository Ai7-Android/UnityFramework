﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;

namespace HotFixCode
{
    class GameInstance
    {
        private GameInstance() { }
        private static GameInstance _instance;
        public static GameInstance Instance
        {
            get
            {
                if (null == _instance)
                    _instance = new GameInstance();
                return _instance;
            }
        }

        TaskManager.Task HeartbeatTask = null;

        void Init()
        {
            Localization.Initialize(LocalizationType.zh_CN);

            Start();
        }

        void Start()
        {
            LoadingLayer.Hide();
            PanelStack.Instance.PushPanel(LogicName.Sample);
        }

        void End()
        {
        }

        void Update()
        {
        }

        void FixedUpdate()
        {
        }

        void LateUpdate()
        {
        }

        public void StartHeartTick()
        {
            if (HeartbeatTask == null)
            {
                HeartbeatTask = Global.TaskManager.StartTask(SendHeartbeat());
            }
            else
            {
                HeartbeatTask.Stop();
                HeartbeatTask = Global.TaskManager.StartTask(SendHeartbeat());
            }
        }

        IEnumerator SendHeartbeat()
        {
            while (true)
            {
                yield return new WaitForSeconds(3f);
                //NetworkProxy.Instance.Heartbeat();
            }
        }

    }
}
