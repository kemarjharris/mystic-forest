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
        IProjectile projectile = Instantiate(projectilePrefab, attacker.gameObject.transform.position, Quaternion.identity).GetComponent<IProjectile>();
        projectile.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(Travel(projectile));
        onCancellableEvent?.Invoke();
        // Wait until end of animation, and then finish
        attacker.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(finishDelay(playerAnimation.GetLength() - projectileSpawnTime));
    }

    IEnumerator Travel(IProjectile projectile)
    {
        void onCollide (Collider2D collider)
        {
            IBattler battler = collider.gameObject.GetComponent<Battler>();
            if (battler == null) return;
            battler.GetAttacked();
        }
        do
        {
            float secondsPassed = Time.deltaTime;
            float distanceTravelled = projectileSpeed * secondsPassed;
            projectile.gameObject.transform.position += new Vector3(distanceTravelled, 0);
            projectile.CheckCollision(onCollide);
            yield return null;
        } while (projectile != null);
    }
}
