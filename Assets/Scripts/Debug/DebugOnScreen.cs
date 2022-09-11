using UnityEngine;
using UnityEngine.Serialization;

namespace Debug
{
    [ExecuteInEditMode]
    public class DebugOnScreen : DebugGUI
    {
        public float score = 0f;
        public float health = 100f;
    
        private float _velocity = 0f;

        private RectOffset _rectOff;

        private void OnGUI()
        {
            // GUI.skin.box.wordWrap = true;
            _rectOff = GUI.skin.box.overflow;

            RightTopBox(0f, 80, 25, $"Health: {health}");
            RightTopText(25f, 80, 25, $"Score:{score} ");
        
            RightBottomBox($"Velocity: {_velocity}" );
        }
    }
}
