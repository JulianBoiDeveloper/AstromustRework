using System;
using UnityEngine;

namespace Astromust.Saving
{
    [Serializable]
    public class DataCustomSaving
    {
        public string name;
        public string description;
        public Component component;
        public string dataSavingName;
        public object data;
    }

}
