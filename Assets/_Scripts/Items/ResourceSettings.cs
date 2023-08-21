using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ResourceSettings", menuName = "ScriptableObjects/Resource", order = 1)]
public class ResourceSettings
{
    public bool isFinite;
    public int amount;
    public bool canRespawn;
    public float respawnTime;
    public bool ejectsMaterialWhenMining;
    public bool ejectsMaterialAutomatically;
    public float ejectMaterialAutomaticallyTime;
    public bool needsToolToBeMined;
    public int toolID;
}
