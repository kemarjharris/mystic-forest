using UnityEngine;
using System.Collections;
using NSubstitute;

public class ComboTestScene : MonoBehaviour
{

    public ComboCounter counter;

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            StartCoroutine(SimulateHit());
        }
    }

    public IEnumerator SimulateHit()
    {
        IBattler b = Substitute.For<IBattler>();
        counter.OnBattlerHit(b);
        yield return new WaitForSeconds(1);
        counter.OnBattlerRecovered(b);
    }
}
