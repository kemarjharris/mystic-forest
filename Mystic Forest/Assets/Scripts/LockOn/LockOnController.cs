using UnityEngine;

[RequireComponent(typeof(LockOn))]
public class LockOnController : MonoBehaviour
{
    public System.Action<GameObject> onTargetSelected;
    public System.Action onStartLockOn;

    private static GameObject target;
    LockOn lockOn;
    public System.Predicate<Collider> rule { set => lockOn.rule = value; } 

    IUnityInputService inputService = new UnityInputService();

    public void Awake()
    {
        lockOn = GetComponent<LockOn>();
        lockOn.onLockedOnExit += RemoveLockOn;
    }

    public void Update()
    {
        if (inputService.GetKeyDown("j"))
        {
            target = lockOn.NextToLockOnTo();
            onStartLockOn?.Invoke();
        } else if (inputService.GetKey("j"))
        {
            if (DirectionalInputDown.InputOnAxisDown(true) > 0)
            {
                target = lockOn.NextToLockOnTo();
            }
        }
        else if (inputService.GetKeyUp("j"))
        {
            onTargetSelected?.Invoke(target);
        }
    }

    private void RemoveLockOn()
    {
        target = null;
    }

    public void OnDestroy()
    {
        lockOn.onLockedOnExit -= RemoveLockOn;
    }

    public void OnEnable()
    {
        lockOn.enabled = true;
        gameObject.SetActive(true);
    }

    public void OnDisable()
    {
        lockOn.enabled = false;
        gameObject.SetActive(false);
    }

    public void AttachToBattler(IBattler battler)
    {
        lockOn.enabled = false;
        lockOn.gameObject.SetActive(true);
        lockOn.gameObject.transform.SetParent(battler.gameObject.transform);
        lockOn.gameObject.transform.localPosition = Vector3.zero;
        lockOn.gameObject.transform.localScale = new Vector3(3.2f, 0.16f, 2.25f);
    }
}
