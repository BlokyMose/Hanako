using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace Hanako.Knife
{
    public abstract class KnifePiece_Living : KnifePiece
    {
        public enum PieceMoveState { Idle, PreMove, Moving, PostMove }

        [SerializeField]
        protected KnifeMoveRule moveRule;
        public KnifeMoveRule MoveRule { get => moveRule; }

        [SerializeField]
        protected float moveDuration = 1f;

        protected Coroutine corMyTurn, corMoving;
        protected KnifeTile destinationTile = null;
        protected PieceMoveState moveState = PieceMoveState.Idle;

        public virtual void PleaseMove(Action onMoveDone)
        {
            corMyTurn = this.RestartCoroutine(WaitingForDestinationTile(), corMyTurn);

            IEnumerator WaitingForDestinationTile()
            {
                moveState = PieceMoveState.PreMove;
                WhenWaitingForDestinationTile();
                while (true)
                {
                    if (moveState == PieceMoveState.PostMove)
                        break;

                    yield return null;
                }
                moveState = PieceMoveState.Idle;
                onMoveDone();
            }
        }

        public virtual void WhenWaitingForDestinationTile()
        {
        }

        public virtual void MoveToTile(KnifeTile tile)
        {
            corMoving = this.RestartCoroutine(Update(), corMoving);
            IEnumerator Update()
            {
                destinationTile = tile;
                moveState = PieceMoveState.Moving;
                var startTime = Time.time;
                while (true)
                {
                    float timeSinceStart = Time.time - startTime;
                    float t = Mathf.Clamp01(timeSinceStart / moveDuration); 

                    transform.position = Vector2.Lerp(transform.position, tile.PieceParent.position, t);

                    if (t >= moveDuration)
                    {
                        break;
                    }
                    yield return null;


                }
                tile.SetAsParentOf(gameObject);
                transform.localPosition = Vector2.zero;
                destinationTile = null;
                moveState = PieceMoveState.PostMove;
            }
        }
    }
}
