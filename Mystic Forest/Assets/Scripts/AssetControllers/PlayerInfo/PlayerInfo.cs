using UnityEngine;
using UnityEngine.UI;
using Zenject;
public class PlayerInfo : MonoBehaviour
{
    public Text nameText;
    public ValueBar staminaBar;
    public Color activeColour;
    public Color inActiveColour;
    public Image backPanel;

    [Inject]
    public void Construct(IPlayer player, BoundedValue<float> stamina) //, BoundedFloat stamina*/)
    {
        nameText.text = player.transform.gameObject.name;
        staminaBar.Construct(stamina);
    }

    public void SetActive(bool active)
    {
        backPanel.color = active ? activeColour : inActiveColour;
    }
}