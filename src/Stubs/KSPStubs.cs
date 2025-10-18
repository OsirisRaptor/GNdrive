#if KSP_STUBS
using System;
using System.Collections.Generic;

namespace UnityEngine
{
    public class Object { }
    public class MonoBehaviour : Object { }
    public class Transform { }
    public class GameObject { }
}

[AttributeUsage(AttributeTargets.Field)]
public sealed class KSPField : Attribute
{
    public bool isPersistant;
    public bool guiActive;
    public bool guiActiveEditor;
    public string guiName;
    public string guiUnits;
    public float guiFormat;
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class KSPEvent : Attribute
{
    public bool guiActive;
    public bool guiActiveEditor;
    public string guiName;
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class KSPAction : Attribute
{
    public string guiName;
}

public enum KSPActionGroup
{
    Custom01, Custom02, Custom03, Custom04, Custom05, Custom06, Custom07, Custom08, Custom09, Custom10
}

public class BaseField
{
    public object uiControlEditor;
    public object uiControlFlight;
}

public class Part : UnityEngine.MonoBehaviour
{
    public Vessel vessel = new Vessel();
    public double RequestResource(string resourceName, double amount) => 0;
    public double RequestResource(int resourceID, double amount) => 0;
    public float breakingForce;
    public float breakingTorque;
    public float crashTolerance;
}

public class Vessel
{
    public Guid id = Guid.NewGuid();
}

public class PartModule : UnityEngine.MonoBehaviour
{
    public Part part = new Part();
    public Vessel vessel => part.vessel;
    public virtual void OnStart(StartState state) {}
    public virtual void OnAwake() {}
    public virtual void OnLoad(ConfigNode node) {}
    public virtual void OnSave(ConfigNode node) {}
    public virtual void OnDestroy() {}
    public virtual void FixedUpdate() {}
    public virtual string GetInfo() => string.Empty;

    public class StartState { }
}

public class ConfigNode {}
#endif
