using UnityEngine.InputSystem;

namespace Src.Tools
{
    public interface ITool
    {
        void OnMouseMove(InputAction.CallbackContext context);
        void OnMouseLeftClick(InputAction.CallbackContext context);
        void OnMouseRightClick(InputAction.CallbackContext context);

        void Init();
        void Clean();
    }
}