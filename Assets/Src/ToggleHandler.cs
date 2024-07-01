using UnityEngine;
using UnityEngine.UI;

public class ToggleHandler : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Sprite checkedSprite;

    void Start()
    {
        toggle.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(bool value)
    {
        SwapSprite(value);
        UncheckOtherToggles(value);
    }

    private void SwapSprite(bool newValue)
    {
        Image targetImage = toggle.targetGraphic as Image;
        if (targetImage != null)
            targetImage.overrideSprite = newValue ? checkedSprite : null;
    }

    private void UncheckOtherToggles(bool value)
    {
        if (value)
        {
            GameObject parent = transform.parent.gameObject;
            Toggle[] toggles = parent.GetComponentsInChildren<Toggle>();
            foreach (var t in toggles)
            {
                if (t.isOn && t != GetComponent<Toggle>())
                    t.isOn = false;
            }
        }
    }
}