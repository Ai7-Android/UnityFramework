﻿using UnityEngine;

/// <summary>
/// 全局常量
/// </summary>
public static class AppConst
{
    /// <summary>
    /// Http服务器
    /// </summary>
    public static string HttpServerHost = "http://000.000.0.000:0000/";

    /// <summary>
    /// 服务器IP
    /// </summary>
    public static string TcpServerIp = "000.000.0.000";

    /// <summary>
    /// 服务器端口
    /// </summary>
    public static int TcpServerPort = 0000;

    /// <summary>
    /// 限帧（-1： 不限制）
    /// </summary>
    public static int FrameRate = 40;
    
    /// <summary>
    /// 垂直同步
    /// </summary>
    public static int vSyncCount = 0;

    /// <summary>
    /// 研发模式-用于内部开发,   资源文件会走流文件目录
    /// </summary>
    public static bool IsPersistentMode = false;

    /// <summary>
    /// 更新模式
    /// </summary>
    public static bool IsUpdateMode = false;

    /// <summary>
    /// 资源目录路径（流文件夹）
    /// </summary>
    public static string AssetDirName = "StreamingAssets";

    public static string BundleExtName = "unity3d";

    /// <summary>
    /// 约定分辨率
    /// </summary>
    public static Vector2 ReferenceResolution = new Vector2(640, 1136);

    public static string UserId = string.Empty;
    public static string AppName = "EasyUnityFramework";
    public static string AppPrefix = AppName + "_";
}

/// <summary>
/// 管理器名
/// </summary>
public class ManagerName
{
    public const string HttpRequest = "HttpRequestManager";
    public const string Model = "ModelManager";
    public const string Script = "ScriptManager";
    public const string Game = "GameController";
    public const string Timer = "TimerManager";
    public const string Music = "MusicManager";
    public const string Panel = "PanelManager";
    public const string SocketClient = "SocketClientManager";
    public const string Asset = "AssetLoadManager";
    public const string Croutine = "CroutineManager";
    public const string Scene = "SceneManager";
    public const string Gesture = "GestureManager";
    public const string ResourcesUpdate = "ResourcesUpdateManager";
}


/// <summary>
/// UI模板名
/// </summary>
public class TemplateName
{
    public const string DialogBox = "DialogBoxTemplate";
}


/// <summary>
/// 场景名
/// </summary>
public class SceneName
{
    public const string Launch = "LaunchScene";
    public const string Test = "TestScene";
}

/// <summary>
/// General通知名
/// </summary>
public class NoticeName
{
    public const string Test = "General_Test";
}

public enum LocalizationType
{
    en_US,
    zh_CN,
}

