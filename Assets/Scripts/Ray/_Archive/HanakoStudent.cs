using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class HanakoStudent : MonoBehaviour
    {
        [SerializeField]
        float moveSpeed = 1f;

        Transform moveTarget;

        public void SetMoveTarget(Transform target)
        {
            moveTarget = target;
        }

        private void Update()
        {
            if (moveTarget != null)
            {
                var moveTargetX = new Vector2(moveTarget.position.x, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, moveTargetX, Time.deltaTime * moveSpeed);

                transform.localEulerAngles = new Vector3(0, moveTargetX.x > transform.position.x ? 0 : 180, 0);
            }
        }
    }
}
