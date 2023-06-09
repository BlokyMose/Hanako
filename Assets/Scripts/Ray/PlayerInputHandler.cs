using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.KnifeInput;

namespace Hanako
{
    public class PlayerInputHandler : MonoBehaviour, IKnifeActions, IHubActions
    {
        [SerializeField]
        float cursorSpeed = 1f;

        [SerializeField]
        bool knifeActions = true;

        [SerializeField]
        bool hubActions = true;

        public Action<Vector2> OnCursorInput;
        public Action OnClickInput;
        public Action<bool> OnClickStateInput;
        public Action<Vector2> OnMoveInput;
        Vector2 cursorMoveDeltaRaw;
        bool isClicking;

        private void Awake()
        {
            var inputs = new KnifeInput();
            if (knifeActions)
                inputs.Knife.SetCallbacks(this);
            if (hubActions)
                inputs.Hub.SetCallbacks(this);
            inputs.Enable();
        }

        private void Update()
        {
            var cursorMoveDeltaInGame = cursorMoveDeltaRaw * cursorSpeed * Time.deltaTime;
            OnCursorInput?.Invoke(cursorMoveDeltaInGame);
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnClickInput?.Invoke();
                isClicking = true;
                OnClickStateInput?.Invoke(isClicking);
            }
            else if(context.canceled)
            {
                isClicking = false;
                OnClickStateInput?.Invoke(isClicking);
            }
        }

        public void OnCursor(InputAction.CallbackContext context)
        {
            cursorMoveDeltaRaw = context.ReadValue<Vector2>();
        }

        public void SetMouseCursorToCenter()
        {
            var centerPosition = new Vector2(Screen.width / 2, Screen.height / 2);
            Mouse.current.WarpCursorPosition(centerPosition);
        }

        public void HideMouseCursor()
        {
            Cursor.visible = false; 
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            OnMoveInput?.Invoke(context.ReadValue<Vector2>());
        }
    }
}
