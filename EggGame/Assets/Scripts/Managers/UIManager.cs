using UnityEngine;

public class UIManager : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.onClick += HandleClick;
    }

    private void OnDisable()
    {
        EventManager.onClick -= HandleClick;
    }

    private void HandleClick()
    {
        Debug.Log("UI Clicked");
    }
}
