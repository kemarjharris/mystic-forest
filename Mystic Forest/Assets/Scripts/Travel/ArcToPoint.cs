using UnityEngine;
using UnityEditor;
using System.Collections;

[CreateAssetMenu()]
public class ArcToPoint : TravelMethodSO
{
    public float bounceHeight;

    public override IEnumerator Travel(Transform transform, Vector3 destination, float speed, System.Action onFinish = null)
    {
        if (speed > 0)
        {
            float time = Vector3.Distance(transform.position, destination) / speed;
            yield return Arc(transform, destination, time);
        } else
        {
            yield return null;
        }
        onFinish?.Invoke();        
    }

    public IEnumerator Arc(Transform transform, Vector3 destination, float animLength)
    {
        float distance = Vector3.Distance(transform.position, destination);
        Vector3 startPos = transform.position;
        float start = Time.time;
        float t = 0;
        do
        {
            t = (Time.time - start) / animLength;
            Vector3 lerp = Vector3.Lerp(startPos, destination, t);
            lerp = new Vector3(lerp.x, lerp.y + QuadraticFunction(distance, bounceHeight, t), lerp.z);
            transform.position = lerp;
            yield return new WaitForFixedUpdate();
        } while (t <= 1);
    }

    private float QuadraticFunction(float bounceDistance, float desiredHeight, float t)
    {
        float y;
        if (bounceDistance != 0)
        {

            float x = Mathf.Clamp01(t) * bounceDistance;
            float parabolaHeight = Mathf.Pow(bounceDistance / 2, 2);
            y = (-x * (x - bounceDistance)) / (parabolaHeight / desiredHeight);
        }
        else
        {
            t = Mathf.Clamp01(t);
            if (t > 0.5f) t = 1 - t;
            y = desiredHeight * t;
        }
        return y;
    }
}