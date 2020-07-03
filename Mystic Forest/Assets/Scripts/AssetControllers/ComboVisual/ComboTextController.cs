using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(ComboCounter))]
public class ComboTextController : MonoBehaviour
{
    public Text number;
    ComboCounter counter;

    private void Start()
    {
        counter = GetComponent<ComboCounter>();
        counter.onCountIncremented += DisplayNumberCount;
        counter.onComboFinished += HideComboDisplay;
        HideComboDisplay();
    }

    public void DisplayNumberCount(int count)
    {
        if (count > 1)
        {
            if (!number.gameObject.activeSelf)
            {
                number.gameObject.SetActive(true);
            }
            number.text = count.ToString();
        }
    }

    public void HideComboDisplay()
    {
        number.text = "0";
        number.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        counter.onCountIncremented -= DisplayNumberCount;
        counter.onComboFinished -= HideComboDisplay;
    }

}
