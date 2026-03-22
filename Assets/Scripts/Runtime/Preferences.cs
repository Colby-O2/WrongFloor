using Unity.VisualScripting;
using UnityEngine;

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
    }
}
