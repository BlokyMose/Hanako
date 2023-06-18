using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityUtility;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    public abstract class KnifePiece_Living : KnifePiece
    {
        #region [Classes]

        public enum PieceActingState { Idling, PreActing, Acting, PostActing }

        public enum PieceAnimationState { Die = -1, Idle, Run, Attack }

        [System.Serializable]
        public class VFXProperties
        {
            [SerializeField]
            VisualEffect vfx;

            [SerializeField]
            float duration = 1f;

            [SerializeField]
            float delay = 0f;

            public VisualEffect VFX { get => vfx; }
            public float Duration { get => duration; }
            public float Delay { get => delay; }

            public IEnumerator Play()
            {
                yield return new WaitForSeconds(delay);
                vfx.SetBool("isPlaying", true);
                yield return new WaitForSeconds(duration);
                vfx.SetBool("isPlaying", false);
            }
        }

        #endregion

        [SerializeField]
        protected KnifeMoveRule moveRule;
        public KnifeMoveRule MoveRule { get => moveRule; set => this.moveRule = value; }

        [SerializeField]
        protected Animator animator;

        [Header("Animation Duration")]
        [SerializeField]
        float attackDuration = 0.5f;

        [Header("VFXs")]
        [SerializeField]
        VFXProperties vfxDie;

        protected bool isAlive = true;
        protected bool IsAlive { get => isAlive;  }

        private float moveDuration = 1f;
        public float MoveDuration { get => moveDuration;}

        protected Coroutine corMyTurn, corMoving, corSetParent, corSettingPostActing;
        protected KnifeTile destinationTile = null;
        protected PieceActingState actState = PieceActingState.Idling;
        public void SetActState(PieceActingState actState) => this.actState = actState;
        protected int int_motion;
        protected AnimationCurve moveAnimationCurve;

        private void Awake()
        {
            int_motion = Animator.StringToHash(nameof(int_motion));
            if (animator == null)
            {
                animator = gameObject.GetComponentInFamily<Animator>();
                if(animator == null)
                    Debug.LogWarning($"{gameObject.name} has no Animator");
            }
        }

        public virtual void Init(float moveDuration, AnimationCurve moveAnimationCurve)
        {
            this.moveDuration = moveDuration;
            this.moveAnimationCurve = moveAnimationCurve;
        }

        public virtual void PlaseAct(Action onActDone)
        {
            if (!isAlive)
                onActDone();

            corMyTurn = this.RestartCoroutine(WaitingForDestinationTile(), corMyTurn);

            IEnumerator WaitingForDestinationTile()
            {
                actState = PieceActingState.PreActing;
                WhenWaitingForAct();
                while (true)
                {
                    if (actState == PieceActingState.PostActing)
                        break;

                    yield return null;
                }
                actState = PieceActingState.Idling;

                onActDone();
            }
        }

        public virtual void WhenWaitingForAct()
        {
        }

        public virtual void MoveToTile(KnifeTile tile, bool autoSetAct = true)
        {
            corMoving = this.RestartCoroutine(Moving(tile.transform.position), corMoving);
            corSetParent = this.RestartCoroutine(SettingParent(moveDuration), corSetParent);
            corSettingPostActing = this.RestartCoroutine(SettingPostActing(moveDuration), corSettingPostActing);

            IEnumerator SettingPostActing(float delay)
            {
                destinationTile = tile;
                if (autoSetAct)
                    actState = PieceActingState.Acting;

                yield return new WaitForSeconds(delay);

                transform.localPosition = Vector2.zero;
                destinationTile = null;
                if (autoSetAct)
                    actState = PieceActingState.PostActing;
            }


            IEnumerator SettingParent(float delay)
            {
                if (tile.PieceParent.transform.position.y < transform.position.y)
                {
                    tile.SetAsParentOf(gameObject);
                    yield break;
                }

                else if (tile.PieceParent.transform.position.y > transform.position.y)
                {
                    yield return new WaitForSeconds(delay);
                    tile.SetAsParentOf(gameObject);
                }
            }

        }

        IEnumerator Moving(Vector2 destination)
        {
            animator.SetInteger(int_motion, (int)PieceAnimationState.Run);
            yield return new WaitForSeconds(0.1f);
                
            if (destination.x > transform.position.x)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, transform.localEulerAngles.z);
            }
            else if (destination.x < transform.position.x)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 180f, transform.localEulerAngles.z);
            }

            var time = 0f;
            var originPos = transform.position;
            var curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            while (true)
            {
                var tScale = time / moveDuration;
                tScale = curve.Evaluate(tScale);
                transform.position = Vector2.Lerp(originPos, destination, tScale);

                if (time > moveDuration-0.1f)
                    animator.SetInteger(int_motion, (int)PieceAnimationState.Idle);
                    
                if (time >= moveDuration)
                {
                    break;
                }

                time += Time.deltaTime;
                yield return null;
            }
        }

        public virtual void Attack(bool returnToCurrentStateAfterAttack = false)
        {
            var currentState = animator.GetInteger(int_motion);
            animator.SetInteger(int_motion, (int)PieceAnimationState.Attack);

            StartCoroutine(Delay(attackDuration - 0.05f));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                if (returnToCurrentStateAfterAttack)
                    animator.SetInteger(int_motion, currentState);
                else
                    animator.SetInteger(int_motion, (int)PieceAnimationState.Idle);
            }
        }
        
        public virtual void Die(LivingPieceCache otherPiece)
        {
            isAlive = false;
            levelManager.RemoveLivingPiece(this);
            
            if (otherPiece.Piece.transform.position.x > transform.position.x)
                transform.localEulerAngles = new(transform.localEulerAngles.x, 0, transform.localEulerAngles.z);
            else if (otherPiece.Piece.transform.position.x < transform.position.x)
                transform.localEulerAngles = new(transform.localEulerAngles.x, 180, transform.localEulerAngles.z);

            animator.SetInteger(int_motion, (int) PieceAnimationState.Die);

            if (vfxDie.VFX != null)
                StartCoroutine(vfxDie.Play());
        }

        public virtual void Resurrect(TileCache targetTile)
        {
            isAlive = true;
            animator.SetInteger(int_motion, (int)PieceAnimationState.Idle);
            levelManager.ResurrectLivingPiece(this);
        }

        public virtual void Escape(Vector2 doorPos)
        {
            corMoving = this.RestartCoroutine(Moving(doorPos), corMoving);
            if (transform.TryGetComponentInFamily<SpriteRendererEditor>(out var srEditor))
            {
                StartCoroutine(Delay());
                IEnumerator Delay()
                {
                    yield return new WaitForSeconds(moveDuration / 2f);
                    srEditor.BeTransparent(moveDuration / 1.5f);
                }
            }
        }
    }
}
