namespace AugustEngine.Input
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    [System.Obsolete("The Input Manager pattern is deprecated. Use Input Channels instead")]
    public interface IIntputListener
    {
        public void Register(InputManager im);
        public void Unregister(InputManager im);
        public void OnMove(Vector2 moveVec);
        public void OnLook(Vector2 lookVec);
        public void OnButton1(float but1);
        public void OnButton2(float but2);
        public void OnButton3(float but3);

    }

}
