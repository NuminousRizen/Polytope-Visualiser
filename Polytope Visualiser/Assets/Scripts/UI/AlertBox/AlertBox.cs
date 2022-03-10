using TMPro;
using UnityEngine;

public class AlertBox : MonoBehaviour
{
    public GameObject AlertBoxObject;
    public TMP_Text AlertBoxMessage;

    public void DisplayMessage(string message)
    {
        AlertBoxMessage.text = message;
        Show();
    }

    public void Show()
    {
        AlertBoxObject.SetActive(true);
    }

    public void Hide()
    {
        AlertBoxObject.SetActive(false);
    }
}
