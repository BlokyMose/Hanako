using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifePiece_Living;
using UnityUtility;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    public class KnifePiece_Moveable : KnifePiece_NonLiving
    {
        [SerializeField]
        Animator animator;

        [SerializeField]
        float distanceToStartMoveAnimation = 1f;

        protected int int_motion;
        protected AnimationCurve moveAnimationCurve;
        protected Coroutine corMoving, corSetParent;
        protected KnifeTile destinationTile = null;
        protected float moveDuration = 1f;

        private void Awake()
        {
            int_motion = Animator.StringToHash(nameof(int_motion));
            if (animator == null)
            {
                animator = gameObject.GetComponentInFamily<Animator>();
                if (animator == null)
                    Debug.LogWarning($"{gameObject.name} has no Animator");
            }
        }

        public override void Init(KnifeLevelManager levelManager)
        {
            base.Init(levelManager);
            this.moveDuration = levelManager.MoveDuration;
        }

        public override bool CheckInteractabilityAgainst(LivingPieceCache otherPiece, TileCache myTile)
        {
            var moveColRow = ColRow.SubstractBetween(myTile.ColRow, otherPiece.ColRow);
            var targetColRow = ColRow.AddBetween(myTile.ColRow, moveColRow);
            if (levelManager.TryGetTile(targetColRow, out var foundTile))
            {
                if (foundTile.Tile.TryGetPiece(out var occupantPiece))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public override bool CheckValidityAgainst(LivingPieceCache otherPiece, TileCache myTile)
        {
            return CheckInteractabilityAgainst(otherPiece, myTile);
        }

        public override void Interacted(LivingPieceCache otherPiece, TileCache tile)
        {
            otherPiece.LivingPiece.MoveToTile(tile.Tile, autoSetAct: false);

            StartCoroutine(MovingMyselfWhenOtherPieceIsClose());
            IEnumerator MovingMyselfWhenOtherPieceIsClose()
            {
                while (true)
                {
                    if (Vector2.Distance(otherPiece.LivingPiece.transform.position, transform.position) < distanceToStartMoveAnimation)
                        break;
                    yield return null;
                }

                var moveColRow = ColRow.SubstractBetween(tile.ColRow, otherPiece.ColRow);
                var targetColRow = ColRow.AddBetween(tile.ColRow, moveColRow);
                var targetTile = levelManager.GetTile(targetColRow);
                MoveToTile(targetTile.Tile);
                yield return new WaitForSeconds(moveDuration);
                otherPiece.LivingPiece.SetActState(PieceActingState.PostActing);
            }

        }

        public virtual void MoveToTile(KnifeTile tile)
        {
            corMoving = this.RestartCoroutine(Moving(), corMoving);
            corSetParent = this.RestartCoroutine(SettingParent(moveDuration), corSetParent);

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

            IEnumerator Moving()
            {
                destinationTile = tile;
                animator.SetInteger(int_motion, (int)PieceAnimationState.Run);
                yield return new WaitForSeconds(0.1f);

                if (destinationTile.transform.position.x > transform.position.x)
                {
                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, transform.localEulerAngles.z);
                }
                else if (destinationTile.transform.position.x < transform.position.x)
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
                    transform.position = Vector2.Lerp(originPos, tile.PieceParent.position, tScale);

                    if (time > moveDuration - 0.1f)
                        animator.SetInteger(int_motion, (int)PieceAnimationState.Idle);

                    if (time >= moveDuration)
                    {
                        break;
                    }

                    time += Time.deltaTime;
                    yield return null;
                }

                transform.localPosition = Vector2.zero;
                destinationTile = null;
            }
        }
    }
}
