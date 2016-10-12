﻿using UnityEngine;
using System.Collections;

public class ScriptManager : TSingleton<ScriptManager> 
{
    static CLRSharp.CLRSharp_Environment env;
    static CLRSharp.ThreadContext context;

    public bool IsScriptInited { set; get; }

    ScriptManager() { IsScriptInited = false; }

    public void Initialize()
    {

        env = new CLRSharp.CLRSharp_Environment(new Logger());
        byte[] dll, pdb;
        if (Application.isEditor)
        {
            dll = FileUtil.ReadFileWithByte(AppPlatform.GetRawResourcesPath() + "Code/" + "HotFixCode.dll.bytes");
            pdb = FileUtil.ReadFileWithByte(AppPlatform.GetRawResourcesPath() + "Code/" + "HotFixCode.pdb.bytes");
        }
        else
        {
            dll = FileUtil.ReadFileWithByte(AppPlatform.GetRuntimePackagePath() + "Code/" + "HotFixCode.dll.bytes");
            pdb = FileUtil.ReadFileWithByte(AppPlatform.GetRuntimePackagePath() + "Code/" + "HotFixCode.pdb.bytes");
        }

        System.IO.MemoryStream msDll = new System.IO.MemoryStream(dll);
        System.IO.MemoryStream msPdb = new System.IO.MemoryStream(pdb);

        env.LoadModule(msDll, msPdb, new Mono.Cecil.Pdb.PdbReaderProvider());
        context = new CLRSharp.ThreadContext(env);
        IsScriptInited = true;
    }

    public object CallScriptMethodStatic(string rTypeName, string rFuncName, params object[] rArgs)
    {
        CLRSharp.IMethod method;
        bool rGot = TryGetMethod(rTypeName, rFuncName, out method, rArgs);
        if (rGot) return method.Invoke(context, null, rArgs);
        else return null;
    }

    public object CallScriptMethod(object rObj, string rTypeName, string rFuncName, params object[] rArgs)
    {
        CLRSharp.IMethod method;
        bool rGot = TryGetMethod(rTypeName, rFuncName, out method, rArgs);
        if (rGot) return method.Invoke(context, rObj, rArgs);
        else return null;
    }

    public bool TryGetType(string rName, out CLRSharp.ICLRType outType)
    {
        string fullTypeName = "HotFixCode." + rName;
        

        outType = env.GetType(fullTypeName);
        
        if (outType != null)
        {
            return true;
        }
        else
        {
            Debug.LogError("Get CLRType failed, check full type name:>" + rName);
            return false;
        }
    }

    public bool TryGetMethod(string rTypeName, string rFuncName, out CLRSharp.IMethod outMethod, params object[] rArgs)
    {
        CLRSharp.ICLRType rType;
        bool rGot = TryGetType(rTypeName, out rType);
        if (!rGot)
        {
            outMethod = null;
            return false;
        }

        var rList = MatchParamList(rArgs);

        outMethod = rType.GetMethod(rFuncName, rList);

        if (outMethod != null)
        {
            return true;
        }
        else
        {
            Debug.LogError("Get CLRMethod failed, check full type name and args:>" + rTypeName);
            return false;
        }
    }

    public bool TryGetMethod(CLRSharp.ICLRType rType, string rFuncName, out CLRSharp.IMethod outMethod, params object[] rArgs)
    {
        var rList = MatchParamList(rArgs);

        outMethod = rType.GetMethod(rFuncName, rList);

        if (outMethod != null) return true;
        else return false;
    }

    public bool CheckExistMethod(CLRSharp.ICLRType rType, string rFuncName, params object[] rArgs)
    {
        var rList = MatchParamList(rArgs);

        var method = rType.GetMethod(rFuncName, rList);

        if (method != null) return true;
        else return false;
    }


    public object CreateScriptObject(string rName, params object[] rArgs)
    {
        CLRSharp.ICLRType rType;

        bool rGot = TryGetType(rName, out rType);

        if (!rGot) return null;

        var rList = MatchParamList(rArgs);
        CLRSharp.IMethod rCtor = rType.GetMethod(".ctor", rList);

        if (rCtor == null)
        {
            Debug.LogError("Create Script Object failed, check full type name and param list:>" + rName);
            return null;
        }

        object rObj = rCtor.Invoke(context, null, rArgs);

        
        return rObj;
    }

    public object CreateScriptObject(CLRSharp.ICLRType rType, params object[] rArgs)
    {

        var rList = MatchParamList(rArgs);
        CLRSharp.IMethod rCtor = rType.GetMethod(".ctor", rList);

        if (rCtor == null)
        {
            Debug.LogError("Create Script Object failed, check full type name and param list");
            return null;
        }

        object rObj = rCtor.Invoke(context, null, null);
        return rObj;
    }

    CLRSharp.MethodParamList MatchParamList(params object[] rArgs)
    {
        CLRSharp.MethodParamList rList;
        if (rArgs.Length > 0)
        {
            CLRSharp.ICLRType[] rClrTypes = new CLRSharp.ICLRType[rArgs.Length];
            for (int i = 0; i < rArgs.Length; i++)
            {
                var arg = rArgs[i];
                var clrType = env.GetType(arg.GetType());
                rClrTypes[i] = clrType;
            }
            rList = CLRSharp.MethodParamList.Make(rClrTypes);
        }
        else
        {
            rList = CLRSharp.MethodParamList.constEmpty();
        }

        return rList;
    }

    public class Logger : CLRSharp.ICLRSharp_Logger//实现L#的LOG接口
    {
        public void Log(string str)
        {
            Debug.Log(str);
        }

        public void Log_Error(string str)
        {
            Debug.LogError(str);
        }

        public void Log_Warning(string str)
        {
            Debug.LogWarning(str);
        }
    }
}
