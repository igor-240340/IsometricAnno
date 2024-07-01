using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeaderWithCloseButton : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private void Awake()
    {
        Button closeButton = GetComponentInChildren<Button>();
        closeButton.onClick.AddListener(OnClose);
    }

    private void OnClose()
    {
        Destroy(transform.parent.gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 parentPosition = transform.parent.transform.position;
        transform.parent.transform.position = parentPosition + eventData.delta;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.parent.transform.SetAsLastSibling();
    }
}