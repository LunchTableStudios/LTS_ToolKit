namespace LTS_ToolKit.Controls
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Input : MonoBehaviour
    {
        public int Id;
        public Dictionary<string, float> Axis = new Dictionary<string, float>();
        public Dictionary<string, bool> Buttons = new Dictionary<string, bool>();
    }
}