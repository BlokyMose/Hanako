using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityUtility.Editor
{
    public class SetIconWindow : EditorWindow
    {
        const string k_menuPath = "Assets/Create/Set Icon..";
        List<Texture2D> m_Icons = null;
        int m_selectedIcon = 0;

        [MenuItem(k_menuPath, priority =0)]
        public static void ShowMenuItem()
        {
            SetIconWindow window = (SetIconWindow)GetWindow(typeof(SetIconWindow));
            window.titleContent = new GUIContent("Set Icon");
            window.Show();
        }

        [MenuItem(k_menuPath, validate = true)]
        public static bool ShowMenuItemValidation()
        {
            foreach (var asset in Selection.objects)
            {
                if (asset.GetType() != typeof(MonoScript))
                    return false;
            }
            return true;
        }

        void OnGUI()
        {
            if (m_Icons == null)
            {
                m_Icons = new List<Texture2D>();
                string[] assetGuids = AssetDatabase.FindAssets("t:texture2D l:ScriptIcon");

                foreach (var assetGuid in assetGuids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(assetGuid);
                    m_Icons.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(path));
                }
            }

            if (m_Icons == null)
            {
                GUILayout.Label("No icons to display");

                if (GUILayout.Button("Close", GUILayout.Width(100)))
                    Close();
            }
            else
            {
                m_selectedIcon = GUILayout.SelectionGrid(m_selectedIcon, m_Icons.ToArray(), 5);

                if (Event.current != null)
                {
                    if (Event.current.isKey)
                    {
                        switch (Event.current.keyCode)
                        {
                            case KeyCode.KeypadEnter:
                            case KeyCode.Return:
                                ApplyIcon(m_Icons[m_selectedIcon]);
                                Close();
                                break;
                            case KeyCode.Escape:
                                Close();
                                break;
                            default:
                                break;
                        }
                    }
                    else if (Event.current.button == 0 && Event.current.clickCount == 2)
                    {
                        ApplyIcon(m_Icons[m_selectedIcon]);
                        Close();
                    }
                }

                if (GUILayout.Button("Apply", GUILayout.Width(100)))
                {
                    ApplyIcon(m_Icons[m_selectedIcon]);
                    Close();
                }
            }
        }

        void ApplyIcon(Texture2D icon)
        {
            AssetDatabase.StartAssetEditing();
            foreach (var asset in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(asset);
                MonoImporter monoImporter = AssetImporter.GetAtPath(path) as MonoImporter;
                monoImporter.SetIcon(icon);
                AssetDatabase.ImportAsset(path);
            }
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }
    }
}
