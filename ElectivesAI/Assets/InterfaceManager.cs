using TMPro;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    public Tank _tankInterface;
    public TextMeshProUGUI NameDisplay;
    public TextMeshProUGUI HealthDisplay;

    private void Start()
    {
        UpdateTankInterface();
    }

    public void UpdateTankInterface()
    {
        NameDisplay.text = "Tank - " + _tankInterface.Name;
        HealthDisplay.text = "Hit Points - " + _tankInterface.GetHealth();
    }
}
