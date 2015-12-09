﻿  
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


public class DebugConsole : MonoBehaviour
{
    struct Message
    {
        string text;
        string formatted;
        MessageType type;
        public Color color { get; private set; }

        public static Color defaultColor = Color.white;
        public static Color warningColor = Color.yellow;
        public static Color errorColor = Color.red;
        public static Color systemColor = Color.green;
        public static Color inputColor = Color.green;
        public static Color outputColor = Color.cyan;

        public Message(object messageObject)
            : this(messageObject, MessageType.NORMAL, Message.defaultColor)
        {
        }

        public Message(object messageObject, Color displayColor)
            : this(messageObject, MessageType.NORMAL, displayColor)
        {
        }

        public Message(object messageObject, MessageType messageType)
            : this(messageObject, messageType, Message.defaultColor)
        {
            switch (messageType)
            {
                case MessageType.ERROR:
                    color = errorColor;
                    break;
                case MessageType.SYSTEM:
                    color = systemColor;
                    break;
                case MessageType.WARNING:
                    color = warningColor;
                    break;
                case MessageType.OUTPUT:
                    color = outputColor;
                    break;
                case MessageType.INPUT:
                    color = inputColor;
                    break;
            }
        }

        public Message(object messageObject, MessageType messageType, Color displayColor)
            : this()
        {
            if (messageObject == null)
                this.text = "<null>";
            else
                this.text = messageObject.ToString();

            this.formatted = string.Empty;
            this.type = messageType;
            this.color = displayColor;


        }

        public static Message Log(object message)
        {
            Debug.Log(message);
            return new Message(message, MessageType.NORMAL, defaultColor);
        }

        public static Message System(object message)
        {
            Debug.Log(message);
            return new Message(message, MessageType.SYSTEM, systemColor);
        }

        public static Message Warning(object message)
        {
            Debug.LogWarning(message);
            return new Message(message, MessageType.WARNING, warningColor);
        }

        public static Message Error(object message)
        {
            Debug.LogError(message);
            return new Message(message, MessageType.ERROR, errorColor);
        }

        public static Message Output(object message)
        {
            return new Message(message, MessageType.OUTPUT, outputColor);
        }

        public static Message Input(object message)
        {
            return new Message(message, MessageType.INPUT, inputColor);
        }

        public override string ToString()
        {
            switch (type)
            {
                case MessageType.ERROR:
                    return string.Format("[{0}] {1}", type, text);
                case MessageType.WARNING:
                    return string.Format("[{0}] {1}", type, text);
                default:
                    return ToGUIString();
            }
        }

        public string ToGUIString()
        {
            if (!string.IsNullOrEmpty(formatted))
                return formatted;

            switch (type)
            {
                case MessageType.INPUT:
                    formatted = ">>> " + text;
                    break;
                case MessageType.OUTPUT:
                    var lines = text.Trim('\n').Split('\n');
                    var output = new StringBuilder();

                    foreach (var line in lines)
                    {
                        output.AppendLine("= " + line);
                    }

                    formatted = output.ToString();
                    break;
                case MessageType.SYSTEM:
                    formatted = "# " + text;
                    break;
                case MessageType.WARNING:
                    formatted = "* " + text;
                    break;
                case MessageType.ERROR:
                    formatted = "** " + text;
                    break;
                default:
                    formatted = text;
                    break;
            }

            return formatted;
        }
    }

    private static DebugConsole __instance;
    public static DebugConsole Instance { get { return __instance; } }

    void Awake()
    {
        if (__instance == null)
        {
            __instance = this;
        }
    }

    public delegate object DebugCommand(params string[] args);

    public int maxLinesForDisplay = 500;
    public Color defaultColor = Message.defaultColor;
    public Color warningColor = Message.warningColor;
    public Color errorColor = Message.errorColor;
    public Color systemColor = Message.systemColor;
    public Color inputColor = Message.inputColor;
    public Color outputColor = Message.outputColor;
    public static KeyCode toggleKey = KeyCode.Tab;
    public bool IsDebug = true;

    Dict<string, DebugCommand> _cmdTable = new Dict<string, DebugCommand>();
    Dict<string, string> _cmdTableDiscribes = new Dict<string, string>(); //cmd的注释
    string _inputString = string.Empty;
    List<Message> _messages = new List<Message>();
    public Rect _windowRect;


    Vector2 _logScrollPos = Vector2.zero;
    Vector2 _rawLogScrollPos = Vector2.zero;



    bool isOpenLogView;
    public bool isOpenBriefView;


    StringBuilder _displayString = new StringBuilder();

    FPSCounter fps;

    bool dirty;

    #region GUI position values
    Rect logViewRect = new Rect(0, 0, Screen.width, Screen.height);
    Rect cmdViewRect = new Rect(10, Screen.height - 40, 330, 30);
    Rect enterButtonRect = new Rect(350, Screen.height - 40, 50, 30);
    Rect messageLine = new Rect(10, 0, Screen.width - 20, 20);
    int lineOffset = -4;

    Rect innerRect = new Rect(0, 0, 0, 0);
    int innerHeight = 0;
    GUIContent guiContent = new GUIContent();
    GUIStyle labelStyle;
    #endregion

    public enum MessageType
    {
        NORMAL,
        WARNING,
        ERROR,
        SYSTEM,
        INPUT,
        OUTPUT
    }

    void OnEnable()
    {
        fps = new FPSCounter();
        StartCoroutine(fps.Update());

        Message.defaultColor = defaultColor;
        Message.warningColor = warningColor;
        Message.errorColor = errorColor;
        Message.systemColor = systemColor;
        Message.inputColor = inputColor;
        Message.outputColor = outputColor;

        _windowRect = new Rect(Screen.width / 2 - 200, 30, 400, 320);


        LogMessage(Message.System("输入 '/?' 显示帮助"));

        this.RegisterCommandCallback("clear", CMD_Clear, "清除调试信息");
        this.RegisterCommandCallback("sys", CMD_SystemInfo, "显示系统信息");
        this.RegisterCommandCallback("/?", CMD_Help, "显示可用命令");
    }

    void OnGUI()
    {
        while (_messages.Count > maxLinesForDisplay)
        {
            _messages.RemoveAt(0);
        }

        if (Event.current.keyCode == toggleKey && Event.current.type == EventType.KeyUp)
        {
            isOpenLogView = !isOpenLogView;
        }

        if (Input.touchCount == 2)
        {
            isOpenLogView = true;
        }

        if (Input.touchCount == 3)
        {
            isOpenLogView = false;
        }

        labelStyle = GUI.skin.label;
        GUI.skin.label.fontSize = 12;

        innerRect.width = messageLine.width;

        if(isOpenBriefView)
            OnBriefView();

        if(isOpenLogView)
            OnLogView();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    #region StaticAccessors

    public static object Log(object message)
    {
        if (!DebugConsole.Instance.IsDebug)
            return null;

        DebugConsole.Instance.LogMessage(Message.Log(message));

        return message;
    }

    public static object Log(object message, MessageType messageType)
    {
        if (!DebugConsole.Instance.IsDebug)
            return null;
        DebugConsole.Instance.LogMessage(new Message(message, messageType));

        return message;
    }

    public static object LogWarning(object message)
    {
        if (!DebugConsole.Instance.IsDebug)
            return null;
        DebugConsole.Instance.LogMessage(Message.Warning(message));

        return message;
    }

    public static object LogError(object message)
    {
        if (!DebugConsole.Instance.IsDebug)
            return null;
        DebugConsole.Instance.LogMessage(Message.Error(message));

        return message;
    }

    public static void Clear()
    {
        DebugConsole.Instance.ClearLog();
    }

    public static void RegisterCommand(string commandString, DebugCommand commandCallback, string CMD_Discribes)
    {
        DebugConsole.Instance.RegisterCommandCallback(commandString, commandCallback, CMD_Discribes);
    }

    public static void UnRegisterCommand(string commandString)
    {
        DebugConsole.Instance.UnRegisterCommandCallback(commandString);
    }

    #endregion

    #region Console commands

    object CMD_Clear(params string[] args)
    {
        this.ClearLog();

        return "clear";
    }

    object CMD_Help(params string[] args)
    {
        var output = new StringBuilder();

        output.AppendLine("可用命令列表: ");
        output.AppendLine("--------------------------");
        foreach (string key in _cmdTable.OriginCollection.Keys)
        {
            output.AppendLine(_cmdTableDiscribes[key] + "  " + key);
        }

        output.Append("--------------------------");

        return output.ToString();
    }

    object CMD_SystemInfo(params string[] args)
    {
        var info = new StringBuilder();

        info.AppendLine("[Base Info]");
        info.AppendLine("Unity Ver: " + Application.unityVersion);
        info.AppendLine("Platform: " + Application.platform);
        info.AppendLine("Language: " + Application.systemLanguage);
        info.AppendLine(string.Format("Level: {0} [{1}]", Application.loadedLevelName, Application.loadedLevel));
        info.AppendLine("Data Path: " + Application.dataPath); 
        info.AppendLine("Persistent Path: " + Application.persistentDataPath);

        info.AppendLine("[Application Info]");
        info.AppendLine("RuntimeData Path: " + AppPlatform.RuntimeAssetsPath);

        info.AppendLine("[System Info]");
        info.AppendLine("SystemMemorySize(MB): " + SystemInfo.systemMemorySize);
        info.AppendLine("DeviceModel: " + SystemInfo.deviceModel);
        info.AppendLine("DeviceType: " + SystemInfo.deviceType);
        info.AppendLine("GraphicsDeviceName: " + SystemInfo.graphicsDeviceName);
        info.AppendLine("GraphicsMemorySize(MB): " + SystemInfo.graphicsMemorySize);
        info.AppendLine("GraphicsShaderLevel: " + SystemInfo.graphicsShaderLevel);
        info.AppendLine("MaxTextureSize: " + SystemInfo.maxTextureSize);
        info.AppendLine("OperatingSystem: " + SystemInfo.operatingSystem);
        info.AppendLine("ProcessorCount: " + SystemInfo.processorCount); 


        info.AppendLine("Profiler.enabled = : " + Profiler.enabled.ToString());
 
        System.GC.Collect();   
        info.AppendLine( string.Format("Total memory: {0:###,###,###,##0} kb" , (System.GC.GetTotalMemory(true))/1024f)); 
 
        return info.ToString();
    }


    #endregion

    #region GUI Window Methods

    void OnCMDView()
    {
        _inputString = GUI.TextField(cmdViewRect, _inputString);

        if (GUI.Button(enterButtonRect, "Enter"))
        {
            EvalInputString(_inputString);
            _inputString = string.Empty;
        }
    }

    void OnLogView()
    {
        GUI.backgroundColor = Color.black;
        GUI.Box(logViewRect, "Console");
        innerRect.height = innerHeight < logViewRect.height ? logViewRect.height : innerHeight;
        _logScrollPos = GUI.BeginScrollView(logViewRect, _logScrollPos, innerRect, false, false);
       
        if (_messages != null || _messages.Count > 0)
        {
            Color oldColor = GUI.contentColor;

            messageLine.y = 0;
            foreach (Message m in _messages)
            {
                GUI.contentColor = m.color;

                guiContent.text = m.ToGUIString();

                messageLine.height = labelStyle.CalcHeight(guiContent, messageLine.width);

                GUI.Label(messageLine, guiContent);

                messageLine.y += (messageLine.height + lineOffset);

                innerHeight = messageLine.y > logViewRect.height ? (int)messageLine.y : (int)logViewRect.height;
            }
            GUI.contentColor = oldColor;
        }
        GUI.EndScrollView();
        OnCMDView();
    }

    void OnBriefView()
    {
        if (isOpenLogView) return;
        var lineRect = new Rect(4, 0, 300, 20);

        var rTemp = GUI.contentColor;
        GUI.contentColor = systemColor;

        GUI.Label(lineRect, "[Runtime Brief]");
        lineRect.y += (lineRect.height + lineOffset);

        GUI.Label(lineRect, string.Format("FPS : {0:00.0}", fps.current));
        lineRect.y += (lineRect.height + lineOffset);

        GUI.Label(lineRect, string.Format("Used Memory : {0}", Util.GetReadableByteSize((double)Profiler.usedHeapSize)));
        lineRect.y += (lineRect.height + lineOffset);

        GUI.Label(lineRect, string.Format("Managed Used Size : {0}", Util.GetReadableByteSize((double)Profiler.GetMonoUsedSize())));
        lineRect.y += (lineRect.height + lineOffset);

        GUI.Label(lineRect, string.Format("Managed Heap Size : {0}", Util.GetReadableByteSize((double)Profiler.GetMonoHeapSize())));
        lineRect.y += (lineRect.height + lineOffset);
        GUI.contentColor = rTemp;
    }

    string BuildDisplayString()
    {
        if (_messages == null)
            return string.Empty;

        if (!dirty)
            return _displayString.ToString();

        dirty = false;

        _displayString.Length = 0;

        for (int i = 0; i < _messages.Count; i++)
        {
            //collapse
            if (i > 0 && _messages[i].ToString() == _messages[i - 1].ToString())
            {
                continue;
            }
            _displayString.AppendLine(_messages[i].ToString());
        }
        return _displayString.ToString();
    }

    #endregion

    #region InternalFunctionality

    void LogMessage(Message msg)
    {
        _messages.Add(msg);

        _logScrollPos.y = 50000.0f;
        _rawLogScrollPos.y = 50000.0f;

        dirty = true;
    }

    void ClearLog()
    {
        _messages.Clear();
    }

    void RegisterCommandCallback(string commandString, DebugCommand commandCallback, string CMD_Discribes)
    {
        try
        {
            _cmdTable[commandString] = new DebugCommand(commandCallback);
            _cmdTableDiscribes.Add(commandString, CMD_Discribes);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    void UnRegisterCommandCallback(string commandString)
    {
        try
        {
            _cmdTable.Remove(commandString);
            _cmdTableDiscribes.Remove(commandString);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

    }

    void EvalInputString(string inputString)
    {
        var input = inputString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (input.Length == 0)
        {
            LogMessage(Message.Input(string.Empty));
            return;
        }

        LogMessage(Message.Input(inputString));

        var cmd = input[0];

        if (_cmdTable.ContainsKey(cmd))
        {
            Log(_cmdTable[cmd](input), MessageType.OUTPUT);
        }
        else
        {
            LogMessage(Message.Output(string.Format("*** Unknown Command: {0} ***", cmd)));
        }
    }

    /// <summary>
    /// 执行一个命令
    /// </summary>
    public void ExecCMDInputString(string inputCMD)
    {
        EvalInputString(inputCMD);
    }

    #endregion
}