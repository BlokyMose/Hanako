using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hanako.Hub
{
    public class HubCharacterBrain_Player : MonoBehaviour
    {
        public event Action<Vector2> OnMove;

        private void Awake()
        {
            var inputHandler = FindObjectOfType<PlayerInputHandler>();
            if (inputHandler != null)
            {
                inputHandler.OnMoveInput += Move;

                if (TryGetComponent<HubCharacterController>(out var characterController))
                {
                    characterController.Init(this);
                }
            }
        }

        void Move(Vector2 direction)
        {
            this.OnMove?.Invoke(direction);
        }

    }
}
