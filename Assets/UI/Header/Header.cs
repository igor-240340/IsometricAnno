using UnityEngine;
using UnityEngine.EventSystems;

public class Header : MonoBehaviour, IDragHandler, IPointerDownHandler
{
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