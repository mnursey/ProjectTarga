using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while(elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }

    public IEnumerator KnockBack(float duration, AnimationCurve rotationCurve)
    {
        Vector3 originalRot = new Vector3();

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float xRotValue = rotationCurve.Evaluate(elapsed / duration);

            transform.localEulerAngles = new Vector3(-xRotValue, originalRot.y, originalRot.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localEulerAngles = originalRot;
    }
}
