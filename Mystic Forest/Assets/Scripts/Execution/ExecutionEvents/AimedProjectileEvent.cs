using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Executable/Execution Event/Aimed Projectile Event")]
public class AimedProjectileEvent : ExecutionEvent
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
        projectile.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(Travel(projectile, targets));
        onCancellableEvent?.Invoke();
        // Wait until end of animation, and then finish
        attacker.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(finishDelay(playerAnimation.GetLength() - projectileSpawnTime));
    }

    IEnumerator Travel(IProjectile projectile, ITargetSet target)
    {
        ISet<IBattler> hitBattlers = new HashSet<IBattler>();
        void onCollide(Collider2D collider)
        {
            IBattler battler = collider.gameObject.GetComponent<Battler>();
            if (battler == null || hitBattlers.Contains(battler)) return;
            battler.GetAttacked();
            hitBattlers.Add(battler);
        }

        Vector3 projectileStart = projectile.gameObject.transform.position;
        Vector3 targetStart = target.GetTarget().transform.position;
        float distance = Vector3.Distance((Vector2) projectileStart, (Vector2) targetStart);
        float newSpeed = distance / projectileSpeed;
        float secondsPassed = 0;
        do
        {
            secondsPassed += Time.deltaTime / newSpeed;
            projectile.gameObject.transform.position = Vector3.LerpUnclamped(projectileStart, targetStart, secondsPassed);
            projectile.CheckCollision(onCollide);
            yield return null;
        } while (projectile != null);
    }
}