namespace LTS_ToolKit.CharacterController
{
    using UnityEngine;
    
    public class CollisionData : MonoBehaviour
    {
        public LayerMask Mask;
        public bool Above, Below, Left, Right;

        public int HorizontalRayCount = 3;
        public int VerticalRayCount = 3;

        public float MaxSlopeAngle = 65;
        [ HideInInspector ] public bool ascendingSlope = false;
        [ HideInInspector ] public bool decendingSlope = false;
        [ HideInInspector ] public float slopeAngle = 0;
        [ HideInInspector ] public float previousSlopeAngle = 0;
    }
}