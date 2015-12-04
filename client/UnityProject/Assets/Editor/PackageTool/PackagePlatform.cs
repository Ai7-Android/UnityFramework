﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public enum TextureFormatOSX
{
    Automatic16bit = TextureImporterFormat.Automatic16bit,
    AutomaticTruecolor = TextureImporterFormat.AutomaticTruecolor,
    AutomaticCompressed = TextureImporterFormat.AutomaticCompressed,
    RGBA16 = TextureImporterFormat.RGBA16,
    ARGB16 = TextureImporterFormat.ARGB16,
}

public enum TextureFormatWindows
{
    Automatic16bit = TextureImporterFormat.Automatic16bit,
    AutomaticTruecolor = TextureImporterFormat.AutomaticTruecolor,
    AutomaticCompressed = TextureImporterFormat.AutomaticCompressed,
    RGBA16 = TextureImporterFormat.RGBA16,
    ARGB16 = TextureImporterFormat.ARGB16,
    DXT1 = TextureImporterFormat.DXT1,
    DXT5 = TextureImporterFormat.DXT5,
    DXT1Crunched = TextureImporterFormat.DXT1Crunched,
    DXT5Crunched = TextureImporterFormat.DXT5Crunched
}

public enum TextureFormatAndroid
{
    Automatic16bit = TextureImporterFormat.Automatic16bit,
    AutomaticTruecolor = TextureImporterFormat.AutomaticTruecolor,
    AutomaticCompressed = TextureImporterFormat.AutomaticCompressed,
    RGBA16 = TextureImporterFormat.RGBA16,
    ARGB16 = TextureImporterFormat.ARGB16,
    ETC_RGB4 = TextureImporterFormat.ETC_RGB4,
    EAC_R = TextureImporterFormat.EAC_R,
    EAC_R_SIGNED = TextureImporterFormat.EAC_R_SIGNED,
    EAC_RG = TextureImporterFormat.EAC_RG,
    EAC_RG_SIGNED = TextureImporterFormat.EAC_RG_SIGNED,
    ETC2_RGB4 = TextureImporterFormat.ETC2_RGB4,
    ETC2_RGB4_PUNCHTHROUGH_ALPHA = TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA,
    ETC2_RGBA8 = TextureImporterFormat.ETC2_RGBA8
}

public enum TextureFormatIOS
{
    Automatic16bit = TextureImporterFormat.Automatic16bit,
    AutomaticTruecolor = TextureImporterFormat.AutomaticTruecolor,
    AutomaticCompressed = TextureImporterFormat.AutomaticCompressed,
    RGBA16 = TextureImporterFormat.RGBA16,
    ARGB16 = TextureImporterFormat.ARGB16,
    PVRTC_RGB2 = TextureImporterFormat.PVRTC_RGB2,
    PVRTC_RGBA2 = TextureImporterFormat.PVRTC_RGBA2,
    PVRTC_RGB4 = TextureImporterFormat.PVRTC_RGB4,
    PVRTC_RGBA4 = TextureImporterFormat.PVRTC_RGBA4,
}

public class PackagePlatform : TSingleton<PackagePlatform>
{
    PackagePlatform() { }

    public string RawResourcesDirectory
    {
        get { return "RawResources"; }
    }

    public string assetbundleConfigPath = "Editor/PackageTool/AssetPackageConfig.txt";
    public string sceneDirectoryPath = null;

    public PlatformType platformCurrent = PlatformType.Windows;

    public enum PlatformType
    {
        Windows = BuildTarget.StandaloneWindows,        //Windows
        OSX = BuildTarget.StandaloneOSXIntel,           //OSX
        IOS = BuildTarget.iOS,                          //IOS
        Android = BuildTarget.Android,                  //Android
    };

    public BuildTarget GetBuildTarget()
    {
        switch (platformCurrent)
        {
            case PlatformType.IOS:
                return BuildTarget.iOS;

            case PlatformType.Android:
                return BuildTarget.Android;

            case PlatformType.Windows:
                return BuildTarget.StandaloneWindows64;

            case PlatformType.OSX:
                return BuildTarget.StandaloneOSXIntel64;
        }
        return BuildTarget.StandaloneWindows64;
    }

    public string GetAssetBundlesPath()
    {
        return Path.Combine(Application.dataPath + "/" + AppConst.AssetDirName + "/", platformCurrent.ToString()) + "_Assetbundles";
    }

    public string GetSceneBundlePath()
    {
        return Path.Combine(Application.dataPath + "/" + AppConst.AssetDirName + "/", platformCurrent.ToString()) + "_Scenebundles";
    }
}
