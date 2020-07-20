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
        attacker.transform.GetComponent<MonoBehaviour>().StartCoroutine(FireProjectile(attacker, targets));
    }

    IEnumerator FireProjectile(IBattler attacker, ITargetSet targets)
    {
        yield return new WaitWhile(() => attacker.IsFrozen);
        // play firing animation
        attacker.Play(playerAnimation);
        // wait until time to fire projectile
        yield return new WaitForSeconds(projectileSpawnTime);
        // Fire projectile and send it to its destination
        GameObject projectileGO = Instantiate(projectilePrefab, attacker.transform.position, Quaternion.identity);
        IProjectile projectile = projectileGO.GetComponentInChildren<IProjectile>();
        projectileGO.GetComponentInChildren<SpriteRenderer>().transform.rotation = Camera.main.transform.rotation;
        Vector3 dest = pool.target.position;
        // put dest point at battlers height
        if (pool.floorPoint)
        {
            dest += Vector3.up * attacker.transform.position.y;
        }
        projectile.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(travelMethod.Travel(projectileGO.transform, dest, projectileSpeed));
        // Check battlers that get hit along the way
        projectile.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(HitBattlers(projectile, attacker));
        onCancellableEvent?.Invoke();
        yield return new WaitForSeconds(playerAnimation.GetLength() - projectileSpawnTime);
        onFinishEvent?.Invoke();
    }

    IEnumerator HitBattlers(IProjectile projectile, IBattler attacker)
    {
        ISet<IBattler> hitBattlers = new HashSet<IBattler>();
        void onCollide (Collider collider)
        {
            IBattler battler = collider.gameObject.GetComponent<Battler>();
            if (battler == null || battler == attacker || hitBattlers.Contains(battler)) return;
            attack.origin = projectile.gameObject.transform;
            battler.GetAttacked(attack);
            hitBattlers.Add(battler);
        }
        do
        {
            projectile.CheckCollision(onCollide);
            yield return new WaitForFixedUpdate();
        } while (projectile != null);
    }
}
