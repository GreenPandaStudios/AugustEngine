namespace AugustEngine.Tweening
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    public class Tweener : MonoBehaviour
    {

        public class TweenAnimation
        {

            internal bool repeats;
            internal Tweener tweenerInstance;

            internal Queue<IEnumerator> queuedAnimations = new Queue<IEnumerator>();
            internal Coroutine runningAnimation = null;
            public IEnumerator GetNext()
            {
                if (queuedAnimations.Count == 0) return null;
                var _a = queuedAnimations.Dequeue();
                if (repeats)
                    queuedAnimations.Enqueue(_a);
                return _a;
            }
            public Coroutine RunningAnimation
            {
                get => runningAnimation;
                internal set
                {
                    runningAnimation = value;
                }
            }
            public void Start()
            {
                if (RunningAnimation == null)
                {
                    //stop all running animations on this tweener
                    tweenerInstance.StopAllCoroutines();
                    var _i = GetNext();
                    if (_i == null)
                    {
                        End();
                        return;
                    }

                    RunningAnimation = tweenerInstance.StartCoroutine(_i);
                }
                else
                {
                    Debug.LogWarning("Already running an animation!");
                }
            }
            public void Stop()
            {
                tweenerInstance.StopCoroutine(RunningAnimation);
                RunningAnimation = null;
            }
            public void End()
            {
                Stop();
                queuedAnimations = null;
            }
        }

        /// <summary>
        /// Creates a TweenAnimation. To start the animation
        /// call <see cref="TweenAnimation.Start()"/>
        /// </summary>
        /// <param name="repeats">Does the animation repeat?</param>
        /// <param name="tweens">What are the actions of this animation</param>
        /// <returns></returns>
        public TweenAnimation CreateTween(bool repeats, params TweenAction[] tweens)
        {
            TweenAnimation tweenAnimation = new TweenAnimation();
            tweenAnimation.repeats = repeats;
            tweenAnimation.tweenerInstance = this;

            if (tweens == null)
            {
                return null;
            }

            for (int i = 0; i < tweens.Length; i++)
            {
                tweens[i].onDone += () => tweenAnimation.Start();

                tweenAnimation.queuedAnimations.Enqueue(tweens[i].enumerator);
            }
            return tweenAnimation;
        }

        public class TweenAction
        {
            internal Action onDone = () => { };
            internal IEnumerator enumerator = null;
            public TweenAction(Vector3 start, Vector3 end, AnimationCurve curve,
            Action<Vector3> callback)
            {
                enumerator = VectorTween(start, end, curve, callback, onDone);
            }
            public TweenAction(Quaternion start, Quaternion end, AnimationCurve curve,
              Action<Quaternion> callback)
            {
                enumerator = QuaternionTween(start, end, curve, callback, onDone);
            }
            public TweenAction(float start, float end, AnimationCurve curve,
              Action<float> callback)
            {
                enumerator = FloatTween(start, end, curve, callback, onDone);
            }


            internal IEnumerator VectorTween(Vector3 start, Vector3 end, AnimationCurve curve,
          Action<Vector3> callback, Action onDone = null
        )
            {
                var _t = Time.time;
                var _max_t = (curve[curve.length - 1]).time;
                while (true)
                {
                    var _c_t = Time.time - _t;
                    if (_c_t > _max_t)
                    {
                        callback?.Invoke(end);
                        break;
                    }
                    callback?.Invoke(Vector3.LerpUnclamped(start, end, ((_c_t / _max_t) - 1f) + curve.Evaluate(_c_t)));
                    yield return new WaitForEndOfFrame();
                }
                onDone?.Invoke();
            }

            internal IEnumerator QuaternionTween(Quaternion start, Quaternion end, AnimationCurve curve,
              Action<Quaternion> callback, Action onDone = null)
            {
                var _t = Time.time;
                var _max_t = (curve[curve.length - 1]).time;
                while (true)
                {
                    var _c_t = Time.time - _t;
                    if (_c_t > _max_t)
                    {
                        callback?.Invoke(end);
                        break;
                    }

                    callback?.Invoke(Quaternion.LerpUnclamped(start, end, ((_c_t / _max_t) - 1f) + curve.Evaluate(_c_t)));
                    yield return new WaitForEndOfFrame();
                }
                onDone?.Invoke();
            }

            internal IEnumerator FloatTween(float start, float end, AnimationCurve curve,
              Action<float> callback, Action onDone = null)
            {
                var _t = Time.time;
                var _max_t = (curve[curve.length - 1]).time;
                while (true)
                {
                    var _c_t = Time.time - _t;
                    if (_c_t > _max_t)
                    {
                        callback?.Invoke(end);
                        break;
                    }

                    callback?.Invoke(Mathf.LerpUnclamped(start, end, ((_c_t / _max_t) - 1f) + curve.Evaluate(_c_t)));
                    yield return new WaitForEndOfFrame();
                }
                onDone?.Invoke();
            }
        }


        private void OnDestroy()
        {
            StopAllCoroutines();
        }

    }
}