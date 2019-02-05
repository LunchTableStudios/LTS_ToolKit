namespace LTS_ToolKit.Controls
{
    using System.Collections.Generic;

    using UnityEngine;

    public class AxisInput : MonoBehaviour
    {
        public int Id;
        public string[] Names;
        public Dictionary<string, float> Values;
    }
}