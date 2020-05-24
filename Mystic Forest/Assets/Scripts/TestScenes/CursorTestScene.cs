using UnityEngine;
using System.Collections;

public class CursorTestScene : MonoBehaviour
{
    IHitBox cursorHitBox;
    public Cursor cursor;

    private void Awake()
    {
        cursorHitBox = cursor.GetComponent<IHitBox>();
    }

    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w"))
        {
            cursor.Up();
        }
        if (Input.GetKey("a"))
        {
            cursor.Left();
        }
        if (Input.GetKey("s"))
        {
            cursor.Down();
        }
        if (Input.GetKey("d"))
        {
            cursor.Right();
        }

        if (Input.GetKeyDown("return"))
        {
            IEnumerator ToBlack(SpriteRenderer renderer)
            {
                renderer.color = Color.black;
                float t = 0;
                while (renderer.color != Color.white)
                {
                    renderer.color = Color.Lerp(Color.black, Color.white, t);
                    t += Time.deltaTime;
                    yield return null;
                }
            }
            void OnCollide(Collider2D collider)
            {
                SpriteRenderer renderer = collider.gameObject.GetComponent<SpriteRenderer>();
                StopAllCoroutines();
                StartCoroutine(ToBlack(renderer));
            }
            cursorHitBox.CheckCollision(OnCollide);
        }
    }
}
