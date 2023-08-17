using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BubblesShot
{
    public class SimpleCoroutine : Singleton<SimpleCoroutine>
    {
        public bool IsMineGameObject { get; private set; }

        public bool AutoDestruct { get; set; }
        private IEnumerator TheCoroutine(Action resolve, Action<Exception> reject)
        {
            // ... add your async operations here ...

            yield return new WaitForSeconds(5f);

            // ... several yields later ...

            var someErrorOccurred = false;
            if (someErrorOccurred)
            {
                // An error occurred, reject the promise.
                reject(new ApplicationException("My error"));
            }
            else
            {
                // Completed successfully, resolve the promise.
                resolve();
            }
        }
        public void StopCoroutineManually()
        {
            StopAllCoroutines();
            FinishAndDestroy();
        }

        private void FinishAndDestroy(Action onFinished = null)
        {
            onFinished?.Invoke();
            if (!AutoDestruct)
                return;
            if (IsMineGameObject)
                DestroyImmediate(gameObject);
            else
            {
                DestroyImmediate(this);
            }
        }


        public void CallCoroutineForEveryFrame(Func<bool> terminateFunc, Action frameFunction, Action onFinished = null)
        {
            StartCoroutine(CallCoroutineForEveryFrameEnumerator(terminateFunc, frameFunction, () =>
            {
                FinishAndDestroy(onFinished);
            }));
        }

        public static IEnumerator CallCoroutineForEveryFrameEnumerator(Func<bool> terminateFunc, Action frameFunction, Action onFinished = null)
        {
            if (terminateFunc == null)
                throw new Exception("Terminate Action important to Call this Coroutine");

            while (!terminateFunc())
            {
                frameFunction?.Invoke();
                yield return null;
            }

            onFinished?.Invoke();
        }


        public void Delay(float delay, Action onFinished = null)
        {
            StartCoroutine(DelayEnumerator(delay, () =>
            {
                FinishAndDestroy(onFinished);
            }));
        }

        public void WaitUntil(Func<bool> condition, Action action)
        {
            StartCoroutine(WaitUntilEnumerator(condition, () => { FinishAndDestroy(action); }));
        }

        public static IEnumerator WaitUntilEnumerator(Func<bool> condition, Action completed = null)
        {
            while (!condition())
            {
                yield return null;
            }
            completed?.Invoke();
        }

        public static IEnumerator DelayEnumerator(float delay, Action onFinished = null)
        {
            yield return new WaitForSeconds(delay);
            onFinished?.Invoke();
        }

        public void LerpNormalized(Action<float> onCallOnFrame, Action onFinished = null,
                                   float lerpSpeed = 1f, float startNormalized = 0f,
                                   float targetNormalized = 1.1f)
        {
            StartCoroutine(LerpNormalizedEnumerator(onCallOnFrame, () =>
            {
                FinishAndDestroy(onFinished);
            }, lerpSpeed, startNormalized, targetNormalized));
        }

        public static IEnumerator LerpNormalizedEnumerator(Action<float> onCallOnFrame, Action onFinished = null,
                                      float lerpSpeed = 1f, float startNormalized = 0f, float targetNormalized = 1.1f)
        {
            var currentNormalized = startNormalized;
            while (true)
            {
                currentNormalized = Mathf.Lerp(currentNormalized, targetNormalized, lerpSpeed * Time.deltaTime);

                if (currentNormalized >= 1)
                {
                    currentNormalized = 1f;
                    onCallOnFrame?.Invoke(currentNormalized);
                    break;
                }


                onCallOnFrame?.Invoke(currentNormalized);
                yield return null;
            }

            onFinished?.Invoke();

        }


        public void Coroutine(IEnumerator coroutine, Action completed = null)
        {
            StartCoroutine(CoroutineEnumerator(coroutine, () => FinishAndDestroy(completed)));
        }

        public static IEnumerator CoroutineEnumerator(IEnumerator coroutine, Action completed = null)
        {

            yield return coroutine;
            completed?.Invoke();
        }

        public static IEnumerator MergeSequence(IEnumerable<IEnumerator> coroutines, Action completed = null, float delayBetween = 0f)
        {
            var list = coroutines.ToList();
            foreach (var enumerator in list)
            {
                yield return enumerator;
                if (delayBetween > 0)
                    yield return new WaitForSeconds(delayBetween);
            }
            completed?.Invoke();
        }

        public void MoveTowards(Quaternion start, Quaternion end, Action<Quaternion> onFrame, Action onFinished = null,
            float normalizedSpeed = 1)
        {
            MoveTowards(0, 1, (n) =>
            {
                onFrame?.Invoke(Quaternion.Lerp(start, end, n));
            }, onFinished, normalizedSpeed);
        }

        public void MoveTowards(Vector3 start, Vector3 end, Action<Vector3> onFrame, Action onFinished = null,
            float speed = 1)
        {
            MoveTowards(0, 1, (n) =>
            {
                onFrame?.Invoke(Vector3.Lerp(start, end, n));
            }, onFinished, speed / (end - start).magnitude);
        }

        public void MoveTowards(float start = 0f, float end = 1f, Action<float> onCallOnFrame = null, Action onFinished = null,
            float speed = 1f)
        {
            MoveTowards(start, end, onCallOnFrame, () =>
            {
                FinishAndDestroy(onFinished);
            }, (n) => speed);
        }


        public void MoveTowards(float start = 0f, float end = 1f, Action<float> onCallOnFrame = null, Action onFinished = null,
            Func<float, float> speed = null)
        {
            StartCoroutine(MoveTowardsEnumerator(start, end, onCallOnFrame, () =>
            {
                FinishAndDestroy(onFinished);
            }, speed));
        }

        public void MoveTowardsAngle(float start = 0f, float end = 1f, Action<float> onCallOnFrame = null, Action onFinished = null,
            float speed = 1f)
        {
            StartCoroutine(MoveTowardsAngleEnumerator(start, end, onCallOnFrame, () =>
            {
                FinishAndDestroy(onFinished);
            }, speed));
        }

        public static IEnumerator MoveTowardsEnumerator(float start = 0f, float end = 1f,
            Action<float> onCallOnFrame = null, Action onFinished = null,
            float speed = 1)
        {
            yield return MoveTowardsEnumerator(start, end, onCallOnFrame, onFinished, (n) => speed);
        }

        public static IEnumerator MoveTowardsEnumerator(float start = 0f, float end = 1f, Action<float> onCallOnFrame = null, Action onFinished = null,
                                       Func<float, float> speed = null)
        {
            speed = speed ?? (f => 1f);
            if (Math.Abs(start - end) < float.Epsilon)
            {
                onFinished?.Invoke();
                yield break;
            }

            var currentNormalized = start;
            while (true)
            {
                currentNormalized = Mathf.MoveTowards(currentNormalized, end, speed(currentNormalized) * Time.deltaTime);

                if (start < end && currentNormalized >= end || start > end && currentNormalized <= end)
                {

                    currentNormalized = end;
                    onCallOnFrame?.Invoke(currentNormalized);
                    break;
                }

                onCallOnFrame?.Invoke(currentNormalized);
                yield return null;
            }

            onFinished?.Invoke();

        }

        public static IEnumerator MoveTowardsAngleEnumerator(float start = 0f, float end = 1f, Action<float> onCallOnFrame = null, Action onFinished = null,
                                       float speed = 1f)
        {
            if (Math.Abs(start - end) < float.Epsilon)
            {
                onFinished?.Invoke();
                yield break;
            }

            var currentNormalized = start;
            while (true)
            {
                currentNormalized = Mathf.MoveTowardsAngle(currentNormalized, end, speed * Time.deltaTime);

                if (start < end && currentNormalized >= end || start > end && currentNormalized <= end)
                {

                    currentNormalized = end;
                    onCallOnFrame?.Invoke(currentNormalized);
                    break;
                }

                onCallOnFrame?.Invoke(currentNormalized);
                yield return null;
            }

            onFinished?.Invoke();

        }
        public void MoveObject(GameObject obj, Vector3 posStart, Vector3 posEnd, float time, bool scale = true)
        {
            StartCoroutine(MoveObjectIEnumerator(obj, posStart, posEnd, time, scale));
        }
        private IEnumerator MoveObjectIEnumerator(GameObject obj, Vector3 posStart, Vector3 posEnd, float time, bool scale)
        {
            yield return null;
            float startTime = Time.time; // Time.time contains current frame time, so remember starting point
            while (Time.time - startTime <= time)
            {
                // until one second passed
                obj.transform.position = Vector3.Lerp(posStart, posEnd, Time.time - startTime); // lerp from A to B in one second
                if (scale)
                    obj.transform.localScale = new Vector3(time + .2f - (Time.time - startTime), time + .2f - (Time.time - startTime), posEnd.z);
                yield return time; // wait for next frame
            }
        }
        public void FadedSpriteRenderer(SpriteRenderer spriteRenderer, float time)
        {
            StartCoroutine(FadedSpriteRendererIEnumerator(spriteRenderer, time));
        }
        private IEnumerator FadedSpriteRendererIEnumerator(SpriteRenderer spriteRenderer, float time)
        {
            yield return null;
            float startTime = Time.time; // Time.time contains current frame time, so remember starting point
            var color = spriteRenderer.color;
            while (Time.time - startTime <= time)
            {
                color.a = time - (Time.time - startTime);
                spriteRenderer.color = color;
                yield return 1;
            }
        }

        public static SimpleCoroutine Create(GameObject go = null, bool autoDestruct = true)
        {

            SimpleCoroutine simpleCoroutine;

            if (!go)
            {
                go = new GameObject("SimpleCoroutine");
                simpleCoroutine = go.AddComponent<SimpleCoroutine>();
                simpleCoroutine.IsMineGameObject = true;
            }
            else
            {
                simpleCoroutine = go.AddComponent<SimpleCoroutine>();
            }
            simpleCoroutine.AutoDestruct = autoDestruct;

            return simpleCoroutine;
        }
    }
}