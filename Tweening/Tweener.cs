namespace AugustEngine.Tweening
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    public class Tweener : MonoBehaviour
    {

        public Coroutine StartVectorTween(Vector3 start, Vector3 end, AnimationCurve curve,
          Action<Vector3> callback)
        {
            return StartCoroutine(VectorTween(start, end, curve, callback));
        }
        IEnumerator VectorTween(Vector3 start, Vector3 end, AnimationCurve curve,
          Action<Vector3> callback)
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
                Debug.Log(curve.Evaluate(_c_t));
                callback?.Invoke(Vector3.LerpUnclamped(start, end, ((_c_t / _max_t) - 1f) + curve.Evaluate(_c_t)));
                yield return new WaitForEndOfFrame();
            }
        }
        public Coroutine StartQuaternionTween(Quaternion start, Quaternion end, AnimationCurve curve,
         Action<Quaternion> callback)
        {
            return StartCoroutine(QuaternionTween(start, end, curve, callback));
        }
        IEnumerator QuaternionTween(Quaternion start, Quaternion end, AnimationCurve curve,
          Action<Quaternion> callback)
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
                Debug.Log(curve.Evaluate(_c_t));
                callback?.Invoke(Quaternion.LerpUnclamped(start, end, ((_c_t / _max_t) - 1f) + curve.Evaluate(_c_t)));
                yield return new WaitForEndOfFrame();
            }
        }
        public Coroutine StartFloatTween(float start, float end, AnimationCurve curve,
        Action<float> callback)
        {
            return StartCoroutine(FloatTween(start, end, curve, callback));
        }
        IEnumerator FloatTween(float start, float end, AnimationCurve curve,
          Action<float> callback)
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
                Debug.Log(curve.Evaluate(_c_t));
                callback?.Invoke(Mathf.LerpUnclamped(start, end, ((_c_t / _max_t) - 1f) + curve.Evaluate(_c_t)));
                yield return new WaitForEndOfFrame();
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

    }
}