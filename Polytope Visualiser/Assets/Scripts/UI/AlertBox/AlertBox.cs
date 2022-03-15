using TMPro;
using UnityEngine;

public class AlertBox : MonoBehaviour
{
    public GameObject AlertBoxObject;
    public TMP_Text AlertBoxMessage;

    /// <summary>
    /// Displays the alert box with a given message.
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    public void DisplayMessage(string message)
    {
        AlertBoxMessage.text = message;
        Show();
    }

    /// <summary>
    /// Shows the alert box.
    /// </summary>
    public void Show()
    {
        AlertBoxObject.SetActive(true);
    }

    /// <summary>
    /// Hides the alert box.
    /// </summary>
    public void Hide()
    {
        AlertBoxObject.SetActive(false);
    }
}
