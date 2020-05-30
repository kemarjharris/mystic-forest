using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Executable/Execution Event/Projectile Event")]
public class ProjectileEvent : ExecutionEvent
{
    public float projectileSpawnTime = 0;
    public float projectileSpeed = 1;
    public PlayableAnimSO playerAnimation = null;
    public GameObject projectilePrefab = null;
    public TravelMethodSO travelMethod = null;
    public Attack attack;

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
        projectile.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(travelMethod.Travel(projectile.gameObject.transform, targets.GetTarget(), projectileSpeed));
        // Check battlers that get hit along the way
        projectile.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(HitBattlers(projectile, attacker));
        onCancellableEvent?.Invoke();
        // Wait until end of animation, and then finish
        attacker.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(finishDelay(playerAnimation.GetLength() - projectileSpawnTime));
    }

    IEnumerator HitBattlers(IProjectile projectile, IBattler attacker)
    {
        ISet<IBattler> hitBattlers = new HashSet<IBattler>();
        void onCollide (Collider collider)
        {
            IBattler battler = collider.gameObject.GetComponent<Battler>();
            if (battler == null || battler == attacker || hitBattlers.Contains(battler)) return;
            battler.GetAttacked(attack);
            hitBattlers.Add(battler);
        }
        do
        {
            projectile.CheckCollision(onCollide);
            yield return null;
        } while (projectile != null);
    }
}
