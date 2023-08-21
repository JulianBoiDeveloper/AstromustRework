using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weareable : MonoBehaviour
{
    public enum WeareableType
    {
        tool,
        backpack
    }

    public WeareableType type;
}
