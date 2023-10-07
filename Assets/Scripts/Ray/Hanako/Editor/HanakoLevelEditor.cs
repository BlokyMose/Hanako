using Encore.Utility;
using UnityUtility;
using Hanako.Hanako;
using Hanako.Hub;
using Hanako.Knife;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Web;
using ColorUtility = UnityUtility.ColorUtility;
using static Hanako.Hanako.HanakoEnemySequence;
using static UnityEditor.Searcher.SearcherWindow;

namespace Hanako
{
    public class HanakoLevelEditor : EditorWindow
    {
        #region [Editor]

        [MenuItem("Tools/Hanako Level Editor")]
        public static void OpenWindow()
        {
            GetWindow<HanakoLevelEditor>("Hanako Lvl").Show();
        }

        #endregion


        #region [Classes]

        class EditorPos
        {
            public Vector2 origin;
            public Vector2 current;

            public EditorPos (Vector2 origin)
            {
                this.origin = new(origin.x, origin.y);
                current = new(origin.x, origin.y);
            }

            public EditorPos (float originX, float originY)
            {
                origin = new(originX, originY);
                current = new(originX, originY);
            }

            public EditorPos (EditorPos editorPos)
            {
                origin = new(editorPos.current.x, editorPos.current.y);
                current = new(editorPos.current.x, editorPos.current.y);
            }

            public void AddY(float height) => current.y += height;
        }

        class EditorRect
        {
            public Vector2 size;
            public float x => size.x;
            public float y => size.y;

            public EditorRect(float x, float y)
            {
                this.size = new(x, y);
            }
        }

        class EditorColors
        {
            public Color color;
            public Color content;
            public Color background;

            public EditorColors(Color color, Color content, Color background)
            {
                this.color = color;
                this.content = content;
                this.background = background;
            }
        }

        #endregion


        HanakoLevel level;
        HanakoEnemySequence enemySequence;
        HanakoDestinationSequence destinationSequence;
        HanakoDistractionSequence distractionSequence;
        bool isEnemySequenceExpanded = true;
        bool isDestinationSequenceExpanded = true;
        bool isDistractionSequenceExpanded = true;
        bool isAutoPosition = true;
        Vector2 scrollViewPos, enemySequenceWindowSize, destinationSequenceWindowSize, distractionSequenceWindowSize;


        #region [Editor Properties]

        EditorColors guiColors;

        readonly Padding padding = new(5, 5, 10, 10);
        readonly EditorRect defaultFieldSize = new (100, 25);
        readonly EditorRect iconSize = new (25, 25);
        readonly EditorRect pictSize = new (100, 100);
        readonly EditorRect pictSmallSize = new (75, 75);
        readonly float spaceBetweenRow = 15f;
        readonly float spaceBetweenWaveColumn = 15f;

        #endregion

        private void OnEnable()
        {
            guiColors = new EditorColors(GUI.color, GUI.contentColor, GUI.backgroundColor);
        }

        void OnGUI()
        {
            var pos = new EditorPos(padding.left, padding.top);
            pos.current.y += MakeTopBar(pos);
            pos.current.y += MakeEnemySequenceUI(pos);
            pos.current.y += MakeDestinationSequenceUI(pos);
            pos.current.y += MakeDistractionSequenceUI(pos);


            #region [Methods: Custom UI]

            float MakeTopBar(EditorPos originPos)
            {
                var pos = new EditorPos(originPos);
                pos.current.x += MakeLabel(pos.current, "Level", width: defaultFieldSize.x / 2);
                pos.current.x += MakeObjectField(pos.current, level, typeof(HanakoLevel), out var selectedObject, width: defaultFieldSize.x * 1.66f);
                var newLevel = selectedObject as HanakoLevel;
                if (newLevel != level)
                {
                    level = newLevel;
                    OnLevelChanged();
                }

                return pos.current.y + defaultFieldSize.y - originPos.current.y;
            }

            float MakeEnemySequenceUI(EditorPos originPos)
            {
                var currentUISize = new Vector2();

                var pos = new EditorPos(originPos);
                pos.current.x += MakeToggleIcon(pos.current, "\u25BC", "\u25ba", ref isEnemySequenceExpanded, "Expand");
                pos.current.x += MakeEnemySequenceField(pos.current);

                if (enemySequence == null || !isEnemySequenceExpanded) 
                    return pos.current.y + defaultFieldSize.y - originPos.current.y;

                pos.current.y += defaultFieldSize.y;
                pos.current.x = pos.origin.x ;

                #region [Begin ScrollView]

                var windowSize = new Vector2(position.width - padding.Horizontal, position.height - padding.Vertical - pos.current.y);
                var scrollViewRect = new Rect(pos.current, windowSize);
                var viewRect = new Rect(Vector2.zero, enemySequenceWindowSize);
                scrollViewPos = GUI.BeginScrollView(scrollViewRect, scrollViewPos, viewRect);
                GUI.contentColor = ColorUtility.darkSlateGray;
                GUI.Box(new(Vector2.zero, windowSize), GUIContent.none);
                GUI.contentColor = guiColors.content;

                #endregion

                pos.current = Vector2.zero;

                for (int i = 0; i < enemySequence.Sequence.Count; i++)
                    pos.current.y += MakeEnemyPanel(pos.current, enemySequence.Sequence[i]);
                pos.current.x += pictSmallSize.x / 2;
                MakeButton(pos.current, "+", AddEnemy, width: pictSmallSize.x/2, height: pictSmallSize.y/2);
                pos.current.y += pictSize.y;
                currentUISize.y = pos.current.y - pos.origin.y;

                GUI.EndScrollView();
                Undo.RecordObject(this, "Hanako Level Editor");
                enemySequenceWindowSize = new(currentUISize.x, currentUISize.y);

                return pos.current.y;

                #region [Methods]

                float MakeEnemySequenceField(Vector2 originPos)
                {
                    var pos = new EditorPos(originPos);
                    pos.current.x += MakeLabel(pos.current, "Enemies", width: defaultFieldSize.x / 1.25f);
                    pos.current.x += MakeObjectField(pos.current, enemySequence, typeof(HanakoEnemySequence), out var selectedObject, width: defaultFieldSize.x * 1.66f);
                    enemySequence = selectedObject as HanakoEnemySequence;

                    return pos.current.x - originPos.x;
                }

                float MakeEnemyPanel(Vector2 originPos, HanakoEnemySequence.Enemy enemy)
                {
                    var pos = new EditorPos(originPos);

                    var leftPanelSize = MakeLeftPanel(pos.current, enemy);
                    pos.current.x += leftPanelSize.x + 5;

                    for (int i = 0; i < enemy.DestinationSequence.Count; i++)
                        pos.current.x += MakeDestinationPanel(pos.current, enemy.DestinationSequence[i]) + 5;
                    pos.current.x += MakeAddDestinationButton(pos.current);

                    if (pos.current.x > currentUISize.x)
                        currentUISize.x = pos.current.x;

                    return leftPanelSize.y;

                    Vector2 MakeLeftPanel(Vector2 originPos, HanakoEnemySequence.Enemy enemy)
                    {
                        var newDelay = enemy.Delay;

                        var pos = new EditorPos(originPos);

                        pos.current.x += MakeEditButtons(pos.current);

                        pos.current.y = originPos.y;
                        pos.current.y += MakePict(pos.current, GetIcon());
                        pos.current.y += MakeEnemyIDField(pos.current, enemy.ID, typeof(HanakoEnemyID), out var newID);
                        pos.current.y += MakeDelayField(pos.current);
                        pos.current.y += MakeVerticalSpace(pos.current, "_____");
                        
                        enemy.SetID(newID as HanakoEnemyID);
                        enemy.SetDelay(newDelay);
                        
                        return new(pictSize.x + iconSize.x, pos.current.y - originPos.y);

                        float MakeEnemyIDField(Vector2 labelPos, Object targetObject, Type objectType, out Object selectedObject, string tooltip = "", float width = -1)
                        {
                            MakeObjectField(labelPos, targetObject, objectType, out selectedObject, tooltip, width);
                            return defaultFieldSize.y;
                        }

                        float MakeDelayField(Vector2 originPos)
                        {
                            var pos = new EditorPos(originPos);
                            pos.current.x += MakeLabel(pos.current, "Delay", width: 40);
                            pos.current.x += MakeFloatField(pos.current, ref newDelay, width: 23);
                            pos.current.x += MakeButton(pos.current, "-", () => newDelay -= .5f, width: 18);
                            pos.current.x += MakeButton(pos.current, "+", () => newDelay += .5f, width: 18);
                            return defaultFieldSize.y;
                        }

                        float MakeEditButtons(Vector2 originPos)
                        {
                            var pos = new EditorPos(originPos);
                            MakeLabel(pos.current, enemySequence.Sequence.IndexOf(enemy).ToString(), "index" ,".", iconSize.x);
                            pos.current.y += iconSize.y;
                            MakeButton(pos.current, "\u25B2", OnMoveUpward, width: pictSmallSize.size.x / 3);
                            pos.current.y += iconSize.y;
                            MakeButton(pos.current, "\u25BC", OnMoveDownward, width: pictSmallSize.size.x / 3);
                            pos.current.y += iconSize.y;
                            MakeButton(pos.current, "X", OnDelete, color: ColorUtility.salmon, pictSmallSize.size.x/3);

                            return iconSize.x;

                            void OnMoveUpward()
                            {
                                var index = enemySequence.Sequence.IndexOf(enemy);
                                if (index<=0) return;

                                enemySequence.Sequence.Move(enemy, index - 1);
                            }

                            void OnMoveDownward()
                            {
                                var index = enemySequence.Sequence.IndexOf(enemy);
                                if (index >= enemySequence.Sequence.Count-1) return;

                                enemySequence.Sequence.Move(enemy, index + 1);
                            }

                            void OnDelete()
                            {
                                enemySequence.Sequence.Remove(enemy);
                            }
                        }

                        Texture2D GetIcon()
                        {
                            if (enemy.ID == null) return null;
                            return enemy.ID.Logo == null ? null : enemy.ID.Logo.texture;
                        }
                    }

                    float MakeDestinationPanel(Vector2 originPos, HanakoEnemySequence.DestinationProperties destination)
                    {
                        var pos = new EditorPos(originPos);
                        var icon = GetDestinationIcon();
                        var color = GetDestinationColor();
                        var newIndex = destination.Index;

                        pos.current.x += MakeButton(pos.current, "-", OnClickMinus, width: pictSmallSize.x / 3);
                        pos.current.x += MakeIntField(pos.current, ref newIndex, pictSmallSize.x/3);
                        pos.current.x += MakeButton(pos.current, "+", OnClickPlus, width: pictSmallSize.x / 3);

                        pos.current.x = originPos.x;
                        pos.current.y += defaultFieldSize.y;

                        pos.current.y += MakePict(pos.current, icon, pictSmallSize.x, pictSmallSize.y, color: color);
                        MakeObjectField(pos.current, destination.ID, typeof(HanakoDestinationID), out var newID, width: pictSmallSize.x);
                        pos.current.y += defaultFieldSize.y;

                        pos.current.x = originPos.x;
                        pos.current.x += MakeButton(pos.current, "\u25C0", OnMoveBackward, width: pictSmallSize.x / 3);
                        pos.current.x += MakeButton(pos.current, "\u25ba", OnMoveForward, width: pictSmallSize.x / 3);
                        pos.current.x += MakeButton(pos.current, "X", OnDelete, color: ColorUtility.salmon, width: pictSmallSize.x / 3);

                        destination.SetID(newID as HanakoDestinationID);
                        destination.SetIndex(newIndex);

                        return pictSmallSize.x;


                        #region [Methods: Event]

                        void OnClickMinus() => newIndex--;
                        void OnClickPlus() => newIndex++;
                        void OnMoveBackward() 
                        {
                            var index = enemy.DestinationSequence.IndexOf(destination);
                            if (index <= 0) return;
                            enemy.DestinationSequence.Move(destination, index-1);
                        }
                        
                        void OnMoveForward() 
                        {
                            var index = enemy.DestinationSequence.IndexOf(destination);
                            if (index >= enemy.DestinationSequence.Count-1) return;
                            enemy.DestinationSequence.Move(destination, index+1);
                        }

                        void OnDelete() => enemy.DestinationSequence.Remove(destination);

                        Texture2D GetDestinationIcon()
                        {
                            if (destination.ID == null) return null;
                            var sprite = destination.ID.GetLogo(destination.Index);
                            return sprite != null ? sprite.texture : null;
                        }

                        Color GetDestinationColor()
                        {
                            return destination.ID != null ? destination.ID.Color : Color.white;
                        }

                        #endregion
                    }

                    float MakeAddDestinationButton(Vector2 originPos)
                    {
                        var pos = new EditorPos(originPos);
                        pos.current.y += defaultFieldSize.y*2;
                        pos.current.x += pictSmallSize.x/4;
                        pos.current.x += MakeButton(pos.current, "+", AddDestination, width:pictSmallSize.x/2, height: pictSmallSize.y/2);

                        return 0;

                        void AddDestination() => enemy.DestinationSequence.Add(new());
                        
                    }
                }

                void AddEnemy()
                {
                    enemySequence.Sequence.Add(new());
                }

                #endregion
            }

            float MakeDestinationSequenceUI(EditorPos originPos)
            {
                var currentUISize = new Vector2();
                var pos = new EditorPos(originPos);
                pos.current.x += MakeToggleIcon(pos.current, "\u25BC", "\u25ba", ref isDestinationSequenceExpanded, "Expand");
                pos.current.x += MakeDestinationSequenceField(pos.current);
                pos.current.x += 5;
                pos.current.x += MakeLabel(pos.current, "Auto-Position", width: 85);
                pos.current.x += MakeToggleIcon(pos.current, "O", "X", ref isAutoPosition, nameof(isAutoPosition), colorTrue: ColorUtility.mediumSpringGreen, colorFalse: ColorUtility.salmon);
                if (isAutoPosition)
                    AutoPositionDestinations();

                if (destinationSequence == null || !isDestinationSequenceExpanded)
                    return pos.current.y + defaultFieldSize.y - originPos.current.y;

                pos.current.y += defaultFieldSize.y;
                pos.current.x = pos.origin.x;

                #region [Begin ScrollView]

                var windowSize = new Vector2(position.width - padding.Horizontal, position.height - padding.Vertical - pos.current.y);
                var scrollViewRect = new Rect(pos.current, windowSize);
                var viewRect = new Rect(Vector2.zero, destinationSequenceWindowSize);
                scrollViewPos = GUI.BeginScrollView(scrollViewRect, scrollViewPos, viewRect);
                GUI.contentColor = ColorUtility.darkSlateGray;
                GUI.Box(new(Vector2.zero, windowSize), GUIContent.none);
                GUI.contentColor = guiColors.content;

                #endregion

                pos.current = Vector2.zero;

                for (int i = 0; i < destinationSequence.Sequence.Count; i++)
                    pos.current.x += MakeDestinationPanel(pos.current, destinationSequence.Sequence[i]);
                pos.current.x += pictSmallSize.x / 2;
                pos.current.y += pictSmallSize.x / 2;
                MakeButton(pos.current, "+", AddDestination, width: pictSmallSize.x / 2, height: pictSmallSize.y / 2);
                var destinationPanelHeight = pictSmallSize.y + defaultFieldSize.y * 6;
                currentUISize.x = pos.current.x - pos.origin.x;
                currentUISize.y = destinationPanelHeight;

                GUI.EndScrollView();
                Undo.RecordObject(this, "Hanako Level Editor: Destination");
                destinationSequenceWindowSize = new(currentUISize.x, currentUISize.y);

                return destinationPanelHeight;

                float MakeDestinationSequenceField(Vector2 originPos)
                {
                    var pos = new EditorPos(originPos);
                    pos.current.x += MakeLabel(pos.current, "Destinations", width: defaultFieldSize.x / 1.25f);
                    pos.current.x += MakeObjectField(pos.current, destinationSequence, typeof(HanakoDestinationSequence), out var selectedObject, width: defaultFieldSize.x * 1.66f);
                    destinationSequence = selectedObject as HanakoDestinationSequence;

                    return pos.current.x - originPos.x;
                }

                float MakeDestinationPanel(Vector2 originPos, HanakoDestinationSequence.Destination destination)
                {
                    var pos = new EditorPos(originPos);
                    var icon = GetDestinationIcon();
                    var color = GetDestinationColor();
                    var newDestinationPosition = destination.Position;

                    pos.current.y += MakePict(pos.current, icon, pictSmallSize.x, pictSmallSize.y, color: color);
                    MakeObjectField(pos.current, destination.Prefab, typeof(GameObject), out var newPrefab, width: pictSmallSize.x);
                    pos.current.y += defaultFieldSize.y;

                    pos.current.x = originPos.x;
                    pos.current.x += MakeButton(pos.current, "\u25C0", OnMoveBackward, width: pictSmallSize.x / 3);
                    pos.current.x += MakeButton(pos.current, "\u25ba", OnMoveForward, width: pictSmallSize.x / 3);
                    pos.current.x += MakeButton(pos.current, "X", OnDelete, color: ColorUtility.salmon, width: pictSmallSize.x / 3);

                    pos.current.y += defaultFieldSize.y;
                    pos.current.x = originPos.x;
                    pos.current.x += MakeLabel(pos.current, "x", width: 15);
                    pos.current.x += MakeFloatField(pos.current, ref newDestinationPosition.x, 35);

                    pos.current.y += defaultFieldSize.y;
                    pos.current.x = originPos.x;
                    pos.current.x += MakeLabel(pos.current, "y", width: 15);
                    pos.current.x += MakeFloatField(pos.current, ref newDestinationPosition.y, 35);

                    destination.SetPrefab(newPrefab as GameObject);
                    destination.SetPosition(newDestinationPosition);

                    return pictSmallSize.x;


                    #region [Methods: Event]

                    void OnMoveBackward()
                    {
                        var index = destinationSequence.Sequence.IndexOf(destination);
                        if (index <= 0) return;
                        destinationSequence.Sequence.Move(destination, index - 1);
                    }

                    void OnMoveForward()
                    {
                        var index = destinationSequence.Sequence.IndexOf(destination);
                        if (index >= destinationSequence.Sequence.Count - 1) return;
                        destinationSequence.Sequence.Move(destination, index + 1);
                    }

                    void OnDelete() => destinationSequence.Sequence.Remove(destination);

                    Texture2D GetDestinationIcon()
                    {
                        if (TryGetID(destination.Prefab,out var id))
                        {
                            var logoIndex = -1;

                            foreach (var d in destinationSequence.Sequence)
                            {
                                if (TryGetID(d.Prefab, out var foundID) && foundID == id)
                                    logoIndex++;
                                if (d == destination) break;
                            }

                            var sprite = id.GetLogo(logoIndex);
                            return sprite != null ? sprite.texture : null;
                        }
                        return null;

                    }

                    Color GetDestinationColor()
                    {
                        if (destination.Prefab != null &&
                            destination.Prefab.TryGetComponent<HanakoDestination>(out var destinationComp) &&
                            destinationComp.ID != null)
                        {
                            return destinationComp.ID != null ? destinationComp.ID.Color : Color.white;
                        }
                        return Color.white;
                    }

                    #endregion
                }
                
                void AddDestination()
                {
                    destinationSequence.Sequence.Add(new());
                }

                void AutoPositionDestinations()
                {
                    if (destinationSequence.Sequence.Count == 0) return;

                    destinationSequence.Sequence[0].SetPosition(Vector2.zero);

                    for (int i = 1; i < destinationSequence.Sequence.Count; i++)
                    {
                        var destination = destinationSequence.Sequence[i];
                        var destinationBehind = destinationSequence.Sequence[i-1];
                        if (TryGetID(destination.Prefab, out var id) &&
                            TryGetID(destinationBehind.Prefab, out var idBehind))
                            destination.SetPosition(new(destinationBehind.Position.x + idBehind.MarginRight + id.MarginLeft, 0));
                    }
                }


                bool TryGetID(GameObject destinationGO, out HanakoDestinationID id)
                {
                    if (destinationGO != null &&
                        destinationGO.TryGetComponent<HanakoDestination>(out var destinationComp) &&
                        destinationComp.ID)
                    {
                        id = destinationComp.ID;
                        return true;
                    }

                    id = null;
                    return false;
                }
            }

            float MakeDistractionSequenceUI(EditorPos originPos)
            {
                var currentUISize = new Vector2();

                var pos = new EditorPos(originPos);
                pos.current.x += MakeToggleIcon(pos.current, "\u25BC", "\u25ba", ref isDistractionSequenceExpanded, "Expand");
                pos.current.x += MakeDistractionSequenceField(pos.current);

                if (distractionSequence == null || !isDistractionSequenceExpanded)
                    return pos.current.y + defaultFieldSize.y - originPos.current.y;

                pos.current.y += defaultFieldSize.y;
                pos.current.x = pos.origin.x;

                #region [Begin ScrollView]

                var windowSize = new Vector2(position.width - padding.Horizontal, position.height - padding.Vertical - pos.current.y);
                var scrollViewRect = new Rect(pos.current, windowSize);
                var viewRect = new Rect(Vector2.zero, distractionSequenceWindowSize);
                scrollViewPos = GUI.BeginScrollView(scrollViewRect, scrollViewPos, viewRect);
                GUI.contentColor = ColorUtility.darkSlateGray;
                GUI.Box(new(Vector2.zero, windowSize), GUIContent.none);
                GUI.contentColor = guiColors.content;

                #endregion

                pos.current = Vector2.zero;

                for (int i = 0; i < distractionSequence.Sequence.Count; i++)
                    pos.current.x += MakeDistractionPanel(pos.current, distractionSequence.Sequence[i]);
                pos.current.x += pictSmallSize.x / 2;
                pos.current.y += pictSmallSize.x / 2;
                MakeButton(pos.current, "+", AddDistraction, width: pictSmallSize.x / 2, height: pictSmallSize.y / 2);
                var distractionPanelHeight = pictSmallSize.y + defaultFieldSize.y * 2;
                currentUISize.x = pos.current.x - pos.origin.x;
                currentUISize.y = distractionPanelHeight;

                GUI.EndScrollView();
                Undo.RecordObject(this, "Hanako Level Editor: Distraction");
                distractionSequenceWindowSize = new(currentUISize.x, currentUISize.y);

                return currentUISize.y;

                float MakeDistractionSequenceField(Vector2 originPos)
                {
                    var pos = new EditorPos(originPos);
                    pos.current.x += MakeLabel(pos.current, "Distractions", width: defaultFieldSize.x / 1.25f);
                    pos.current.x += MakeObjectField(pos.current, distractionSequence, typeof(HanakoDistractionSequence), out var selectedObject, width: defaultFieldSize.x * 1.66f);
                    distractionSequence = selectedObject as HanakoDistractionSequence;

                    return pos.current.x - originPos.x;
                }

                float MakeDistractionPanel(Vector2 originPos, HanakoDistractionSequence.Distraction distraction)
                {
                    var pos = new EditorPos(originPos);
                    var icon = GetDistractionIcon();
                    var newDistractionPosition = distraction.Position;

                    pos.current.y += MakePict(pos.current, icon, pictSmallSize.x, pictSmallSize.y);
                    MakeObjectField(pos.current, distraction.Prefab, typeof(GameObject), out var newPrefab, width: pictSmallSize.x);
                    pos.current.y += defaultFieldSize.y;

                    pos.current.x = originPos.x;
                    pos.current.x += MakeButton(pos.current, "\u25C0", OnMoveBackward, width: pictSmallSize.x / 3);
                    pos.current.x += MakeButton(pos.current, "\u25ba", OnMoveForward, width: pictSmallSize.x / 3);
                    pos.current.x += MakeButton(pos.current, "X", OnDelete, color: ColorUtility.salmon, width: pictSmallSize.x / 3);

                    pos.current.y += defaultFieldSize.y;
                    pos.current.x = originPos.x;
                    pos.current.x += MakeLabel(pos.current, "x", width: 15);
                    pos.current.x += MakeFloatField(pos.current, ref newDistractionPosition.x, 35);

                    pos.current.y += defaultFieldSize.y;
                    pos.current.x = originPos.x;
                    pos.current.x += MakeLabel(pos.current, "y", width: 15);
                    pos.current.x += MakeFloatField(pos.current, ref newDistractionPosition.y, 35);

                    distraction.SetPrefab(newPrefab as GameObject);
                    distraction.SetPosition(newDistractionPosition);

                    return pictSmallSize.x;


                    #region [Methods: Event]

                    void OnMoveBackward()
                    {
                        var index = distractionSequence.Sequence.IndexOf(distraction);
                        if (index <= 0) return;
                        distractionSequence.Sequence.Move(distraction, index - 1);
                    }

                    void OnMoveForward()
                    {
                        var index = distractionSequence.Sequence.IndexOf(distraction);
                        if (index >= distractionSequence.Sequence.Count - 1) return;
                        distractionSequence.Sequence.Move(distraction, index + 1);
                    }

                    void OnDelete() => distractionSequence.Sequence.Remove(distraction);

                    Texture2D GetDistractionIcon()
                    {
                        if (distraction.Prefab == null) return null;
                        return AssetPreview.GetAssetPreview(distraction.Prefab);
                    }

                    #endregion
                }

                void AddDistraction()
                {
                    distractionSequence.Sequence.Add(new());
                }
            }


            #endregion

            #region [Methods: General UI]

            float MakeLabel(Vector2 labelPos, string labelName, string tooltip = "", string separator = ": ", float width = -1)
            {
                width = width == -1 ? defaultFieldSize.x : width;
                var rect = new Rect(labelPos, new Vector2(width, defaultFieldSize.y));
                EditorGUI.LabelField(rect, new GUIContent(labelName + separator, tooltip));
                return rect.width;
            }

            float MakePict(Vector2 pos, Texture2D sprite, float width = -1, float height = -1, Color? color = null)
            {
                width = width > 0 ? width : pictSize.x;
                height = height > 0 ? height : pictSize.y;

                var rect = new Rect(pos, new(width, height));
                if (sprite == null)
                    return rect.size.y;

                GUI.contentColor = color != null ? (Color)color : GUI.contentColor;
                GUI.Box(rect, sprite);
                GUI.contentColor = guiColors.content;

                return rect.size.y;
            }

            float MakeVerticalSpace(Vector2 labelPos, string labelName = "", float height = 25)
            {
                var rect = new Rect(labelPos, new Vector2(0, height));
                EditorGUI.LabelField(rect, new GUIContent(labelName));
                return rect.height;
            }

            float MakeObjectField(Vector2 labelPos, Object targetObject, Type objectType, out Object selectedObject, string tooltip = "", float width = -1)
            {
                width = width == -1 ? defaultFieldSize.x : width;
                var rect = new Rect(labelPos, new Vector2(width, defaultFieldSize.y));
                selectedObject = EditorGUI.ObjectField(rect, targetObject, objectType, false);
                return rect.width;
            }

            float MakeToggleButton(Vector2 buttonPos, Texture2D icon, ref bool boolValue, string boolName)
            {
                var buttonRect = new Rect(buttonPos, iconSize.size);
                if (boolValue)
                {
                    if (GUI.Button(buttonRect, new GUIContent(icon, boolName + ": true")))
                        boolValue = false;
                }
                else
                {
                    GUI.color = Color.white.ChangeAlpha(0.5f);
                    if (GUI.Button(buttonRect, new GUIContent(icon, boolName + ": false")))
                        boolValue = true;
                }

                GUI.color = guiColors.color;

                return buttonRect.width;
            }

            float MakeToggleIcon(Vector2 buttonPos, string iconTrue, string iconFalse, ref bool boolValue, string boolName, Color? colorTrue = null, Color? colorFalse = null)
            {
                var buttonRect = new Rect(buttonPos, iconSize.size);
                if (boolValue)
                {
                    GUI.color = colorTrue != null ? (Color)colorTrue : GUI.color;
                    if (GUI.Button(buttonRect, new GUIContent(iconTrue, boolName + ": true")))
                        boolValue = false;
                }
                else
                {
                    GUI.color = colorFalse != null ? (Color)colorFalse : Color.white.ChangeAlpha(0.5f);
                    if (GUI.Button(buttonRect, new GUIContent(iconFalse, boolName + ": false")))
                        boolValue = true;
                }

                GUI.color = guiColors.color;

                return buttonRect.width;
            }

            float MakeButton(Vector2 buttonPos, string text, Action onClicked, Color? color = null, float width = -1, float height = -1)
            {
                width = width > 0 ? width : defaultFieldSize.x;
                height = height > 0 ? height : defaultFieldSize.y;

                var buttonRect = new Rect(buttonPos, new(width,height));

                if (color != null)
                    GUI.color = (Color)color;

                if (GUI.Button(buttonRect, text))
                    onClicked();

                GUI.color = guiColors.color;
                return buttonRect.width;
            }

            float MakeFloatField(Vector2 textInputPos, ref float floatVar, float width = -1, TextAnchor alignment = TextAnchor.MiddleLeft)
            {
                width = width > 0 ? width : defaultFieldSize.x;
                var style = new GUIStyle(GUI.skin.textField) {alignment = alignment};

                var rect = new Rect(textInputPos, new (width, defaultFieldSize.y));
                floatVar = EditorGUI.FloatField(rect, floatVar, style);
                return rect.width;
            }

            float MakeIntField(Vector2 textInputPos, ref int intVar, float width = -1)
            {
                width = width > 0 ? width : defaultFieldSize.x;
                var style = new GUIStyle(GUI.skin.textField) {alignment = TextAnchor.MiddleLeft};

                var rect = new Rect(textInputPos, new Vector2(width, defaultFieldSize.y));
                intVar = EditorGUI.IntField(rect, intVar, style);
                return rect.width;
            }

            #endregion
        }

        void OnLevelChanged()
        {
            enemySequence = level.EnemySequence;
            destinationSequence = level.DestinationSequence;
            distractionSequence = level.DistractionSequence;
        }


    }
}
