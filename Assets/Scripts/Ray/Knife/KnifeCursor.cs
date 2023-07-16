using Hanako.Knife;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using UnityUtility;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Animator))]
    public class KnifeCursor : PlayerCursor
    {
        [Header("Knife")]

        [SerializeField]
        KnifePieceInfoCanvas infoCanvas;

        [SerializeField]
        Camera gameCamera;

        [SerializeField]
        KnifeColors colors;

        KnifeTile hoveredTile, hoveredValidTile;
        KnifeLevelManager levelManager;
        int controllerID;
        LivingPieceCache myPiece;
        bool isMyTurn = false;
        Action<KnifeTile> onPleaseClick;
        Vector2 previousPos;
        bool canHover = true;
        bool canPan = true;

        public void Init(KnifeLevelManager levelManager, int controllerID)
        {
            this.levelManager = levelManager;
            this.colors = levelManager.Colors;
            this.controllerID = controllerID;

            levelManager.OnGameOver += (isPlayerDead) => { canHover = false; canPan = false; };
        }

        protected override void Update()
        {
            base.Update();
            Pan();

            void Pan()
            {
                if (!canPan) return;

                if (isClicking && hoveredValidTile == null)
                {
                    if (gameCamera != null)
                    {
                        var cameraPos = gameCamera.transform.position;
                        var direction = (Vector2)transform.position - previousPos;
                        direction *= gameCamera.orthographicSize;
                        var newX = cameraPos.x - direction.x;
                        var newY = cameraPos.y - direction.y;

                        if (newX > levelManager.PanAreaMax.x)
                            newX = levelManager.PanAreaMax.x;
                        else if (newX < levelManager.PanAreaMin.x)
                            newX = levelManager.PanAreaMin.x;

                        if (newY > levelManager.PanAreaMax.y)
                            newY = levelManager.PanAreaMax.y;
                        else if (newY < levelManager.PanAreaMin.y)
                            newY = levelManager.PanAreaMin.y;

                        gameCamera.transform.position = new Vector3(newX, newY, cameraPos.z); ;
                    }
                }
                else
                {

                }
                previousPos = transform.position;

            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isClicking) return;

            if (collision.TryGetComponentInFamily<KnifeTile>(out var tile))
            {
                Hover(tile);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponentInFamily<KnifeTile>(out var tile) && hoveredTile == tile)
            {
                Unhover(tile);
            }
        }

        public void Refresh()
        {
            if (hoveredTile != null)
            {
                Hover(hoveredTile);
            }
        }

        void Hover(KnifeTile tile)
        {
            if (!canHover) return;

            if (hoveredTile != null)
                Unhover(hoveredTile);

            hoveredTile = tile;
            var tilePiece = hoveredTile.GetPiece();

            if (isMyTurn)
            {
                var tileCheckResult = myPiece.CheckTile(hoveredTile);
                if (tileCheckResult.IsValid)
                {
                    hoveredValidTile = hoveredTile;

                    // Hovering a tile with a piece
                    if (tileCheckResult.IsInteractable)
                    {
                        hoveredTile.Hovered(colors.TileActionColor);
                    }

                    // Hovering a tile with no piece, but can be moved into
                    else
                    {
                        hoveredTile.Hovered(colors.TileValidMoveColor);
                    }
                }

                // Hovering a tile that cannot be reached
                else
                {
                    hoveredTile.Hovered(colors.TileInvalidMoveColor);
                    hoveredValidTile = null;
                }

                if (tilePiece != null)
                {
                    if (tilePiece is KnifePiece_Living)
                    {
                        var livingPieceCache = levelManager.GetLivingPiece(tilePiece as KnifePiece_Living);
                        var color = tilePiece == levelManager.PlayerPiece ? colors.TileValidMoveColor : colors.TileOtherValidMoveColor;
                        foreach (var validTile in livingPieceCache.ValidTilesByMoveRule)
                        {
                            validTile.Tile.Hovered(color);
                        }

                        levelManager.ShowTurnOrderTexts();
                    }
                }
            }

            // Hovering a tile when not my turn
            else
            {
                hoveredTile.Hovered(colors.TileNotMyTurnColor);
                hoveredValidTile = null;
            }

            if (infoCanvas != null && tilePiece != null)
            {
                var characterIsFlippedX = tilePiece.transform.localEulerAngles.y == 180;
                infoCanvas.SetInformation(tilePiece, characterIsFlippedX);
            }
        }

        void Unhover(KnifeTile tile)
        {
            if (hoveredTile.TryGetPiece(out var tilePiece))
            {
                if (tilePiece is KnifePiece_Living)
                {
                    var livingPieceCache = levelManager.GetLivingPiece(tilePiece as KnifePiece_Living);
                    foreach (var validTile in livingPieceCache.ValidTilesByMoveRule)
                    {
                        validTile.Tile.Unhovered();
                    }
                }
            }

            tile.Unhovered();
            hoveredTile = null;
            hoveredValidTile = null;
            if (infoCanvas != null )
                infoCanvas.SetDefaultInfo();

            levelManager.HideTurnOrderTexts();
        }

        public void PleaseClick(Action<KnifeTile> onClick)
        {
            myPiece = levelManager.GetLivingPiece(controllerID);
            if (myPiece == null)
            {
                Debug.LogWarning("Cannot find piece with controller ID: "+controllerID);
                onClick(null);
            }

            isMyTurn = true;
            onPleaseClick = (clickedTile) =>
            {
                isMyTurn = false;
                onClick(clickedTile);
            };
            Refresh();

        }

        public override void ClickState(bool isClicking)
        {
            base.ClickState(isClicking);
            ClickHoveredTile(isClicking);

            void ClickHoveredTile(bool isClicking)
            {
                if (hoveredTile != null)
                {
                    if (isClicking)
                    {
                        if (isMyTurn && hoveredValidTile != null)
                        {
                            hoveredValidTile.Clicked(colors.TileClickColor);
                            onPleaseClick?.Invoke(hoveredValidTile);
                        }
                    }
                    else
                    {
                        Unhover(hoveredTile);
                    }
                }
            }
        }

    }
}
