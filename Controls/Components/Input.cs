namespace LTS_ToolKit.Controls
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Input : MonoBehaviour
    {
        [ System.Serializable ]
        public class AxisAction
        {
            public string Name;
            public float Value;

            public AxisAction( string name )
            {
                Name = name;
                Value = 0;
            }
        }

        [ System.Serializable ]
        public class ButtonAction
        {
            public string Name;
            public bool Value;

            public ButtonAction( string name )
            {
                Name = name;
                Value = false;
            }
        }

        public int Id;
        public List<AxisAction> Axis = new List<AxisAction>();
        public List<ButtonAction> Buttons = new List<ButtonAction>();
    }
}