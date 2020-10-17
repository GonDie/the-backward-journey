using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Kludo.Editor
{
    [InitializeOnLoad]
    public class EditorPlayFromStartScene : EditorWindow
    {
        public SceneAsset Scene;
        [SerializeField] string _scenePath;

#if UNITY_EDITOR

        static EditorPlayFromStartScene()
        {
            EditorWindow w = GetWindow<EditorPlayFromStartScene>();
            w.Close();
        }

        static void OnPlayModeStateChange(PlayModeStateChange state)
        {
            GetWindow<EditorPlayFromStartScene>().Close();
        }

        void OnGUI()
        {
            // Use the Object Picker to select the start SceneAsset
            Scene = (SceneAsset)EditorGUILayout.ObjectField(new GUIContent("Start Scene"), Scene, typeof(SceneAsset), false);
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChange;

            _scenePath = EditorPrefs.GetString("EditorPlayFromStartScene");
            Scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(_scenePath);

            EditorSceneManager.playModeStartScene = Scene;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;

            EditorSceneManager.playModeStartScene = Scene;

            _scenePath = AssetDatabase.GetAssetPath(Scene);

            EditorPrefs.SetString("EditorPlayFromStartScene", _scenePath);
        }

        [MenuItem("Tools/Start From Scene")]
        static void Open()
        {
            GetWindow<EditorPlayFromStartScene>();
        }
#endif
    }
}