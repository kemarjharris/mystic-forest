using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Executable/Execution Event/Projectile Event")]
public class ProjectileEvent : ExecutionEvent
{
    public float projectileSpawnTime = 0;
    public float projectileSpeed = 1;
    public PlayableAnimSO playerAnimation = null;
    public GameObject projectilePrefab = null;

    public override void OnExecute(IBattler attacker, ITargetSet targets)
    {
        IEnumerator wait(float seconds)
        { yield return new WaitForSeconds(seconds); }

        IEnumerator finishDelay(float seconds)
        {
            yield return wait(seconds);
            onFinishEvent?.Invoke();
        }

        // play firing animation
        attacker.Play(playerAnimation);
        // wait until time to fire projectile
        attacker.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(wait(projectileSpawnTime));
        // Fire projectile and send it to its destination
        GameObject projectile = Instantiate(projectilePrefab, attacker.gameObject.transform.position, Quaternion.identity);
        projectile.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(Travel(projectile));
        onCancellableEvent?.Invoke();
        // Wait until end of animation, and then finish
        attacker.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(finishDelay(playerAnimation.GetLength() - projectileSpawnTime));
    }



    IEnumerator Travel(GameObject projectile)
    {
        bool collided = false;
        IHitBox hitBox = projectile.GetComponent<IHitBox>();
        void onCollide (Collider2D collider)
        {
            IBattler battler = collider.gameObject.GetComponent<Battler>();
            if (battler == null) return;
            battler.GetAttacked();
            collided = true;
        }
        do
        {
            float secondsPassed = Time.deltaTime;
            float distanceTravelled = projectileSpeed * secondsPassed;
            projectile.transform.position += new Vector3(distanceTravelled, 0);
            hitBox.CheckCollision(onCollide);
            yield return null;
        } while (!collided && IsOnScreen(projectile));
        Destroy(projectile);
    }

    bool IsOnScreen(GameObject projectile)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(projectile.gameObject.transform.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }

}
