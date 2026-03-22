using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace WrongFloor
{
    [CreateAssetMenu(fileName = "DefaultPreferences", menuName = "Preferences")]
    public class Preferences : ScriptableObject
    {
        public Texture2D Cursor;
        public float DialogueSpeedMul = 1f;
        public float InteractionDistance = 1.0f;
        public float DoorSpeed = 1.8f;
        public float CrouchHeight = 0.4f;
        public int FloorMoveAmountMin = 3;
        public int FloorMoveAmountMax = 6;
        public int StartFloor = 36;
        public float ElevatorMoveSpeed = 2.7f;
        public float ElevatorFallInitialSpeed = 0.2f;
        public float ElevatorFallElevatorOpenTime = 2.0f;
        public float ElevatorFallSpeedUpTime = 3.5f;
        public float ElevatorFallMaxSpeed = 4.0f;
        public float ElevatorFallTime = 14.0f;
        public float ElevatorFallFloatHeight = 1.25f;
        public float ElevatorFallLockTime = 2.0f;
        public float ElevatorFallMaxFloorSpeed = 5.0f;
    }
}
