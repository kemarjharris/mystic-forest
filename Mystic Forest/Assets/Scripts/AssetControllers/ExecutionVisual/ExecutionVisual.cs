using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExecutionVisual : MonoBehaviour
{

    public SpriteRenderer circle;
    public GameObject connection;
    public Text text;
    

    public void SetAsLast()
    {
        connection.SetActive(false);
    }

    IEnumerator FadeOut(){
        for (int i = 100; i >= 0; i-= 5)
        {
            circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, (i * 0.01f));
            text.color = new Color(text.color.r, text.color.g, text.color.b, (i * 0.01f));
            yield return null;
        }
        Destroy (gameObject);
    }

    IEnumerator Grow()
    {
        Vector3 finalSize = transform.localScale * 4;
        Vector3 currSize = transform.localScale;
        float totalTime = 1f;
        float startTime = Time.unscaledTime;
        GameObject x = new GameObject();

        for (float t = 0; t < 1; t = (Time.unscaledTime - startTime)/ totalTime)
        {

            transform.localScale = Vector3.Lerp(currSize, finalSize, t);
            yield return null;
        }
    }

    IEnumerator MoveLeft()
    {

        Vector3 start = transform.position;
        Vector3 finish = transform.position - new Vector3(transform.localScale.x * GetComponent<RectTransform>().rect.width, 0);
        float totalTime = 1f;
        float startTime = Time.unscaledTime;
        for (float t = 0; t < 1; t = (Time.unscaledTime - startTime)/ totalTime)
        {

            transform.position = Vector3.Lerp(start, finish, t);
            yield return null;
        }
    }

    public virtual void MarkFinished()
    {
        Destroy(connection);
        Destroy(gameObject);
        
    }

    public void SetText(string text)
    {
        this.text.text = text.ToUpper();
    }


    public virtual void SetColor(Color color)
    {
        circle.color = color;
    }
    
}