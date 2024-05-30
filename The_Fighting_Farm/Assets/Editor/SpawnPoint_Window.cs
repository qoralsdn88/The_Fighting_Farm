using UnityEditor;
using UnityEngine;

namespace UserEditor
{
    internal class SpawnPoint_Window : EditorWindow
    {
        private static SpawnPoint_Window window;
        private static Vector2 windowSize = new Vector2(319, 500);

        [MenuItem("Window/SpawnData/Enemy %&F1", priority = 1)]
        private static void OpenWindow()
        {
            //SpawnPoint_Window window = EditorWindow.GetWindow(typeof(SpawnPoint_Window)) as SpawnPoint_Window;
            //window.Show();

            if(window == null)
                window = EditorWindow.CreateInstance<SpawnPoint_Window>();

            window.titleContent = new GUIContent("Create Enemy Spawn Point");
            window.minSize = window.maxSize = windowSize;

            window.ShowUtility();
        }


        private SpawnPoint spawnPoint;
        private SerializedObject serializedObject;

        private void OnEnable()
        {
            spawnPoint = CreateInstance<SpawnPoint>();
            serializedObject = new SerializedObject(spawnPoint);

            Selection.activeObject = null;
        }


        private Vector2 scrollPosition;

        private void OnGUI()
        {
            //MapSize
            {
                SerializedProperty property = serializedObject.FindProperty("MapSize");
                EditorGUILayout.PropertyField(property);
            }

            //Prefab
            {
                SerializedProperty property = serializedObject.FindProperty("EnemyPrefab");
                EditorGUILayout.PropertyField(property);
            }

            //SpawnCount
            {
                SerializedProperty property = serializedObject.FindProperty("SpawnCount");
                EditorGUILayout.PropertyField(property);
            }

            //SpawnPoints
            {
                SerializedProperty property = serializedObject.FindProperty("SpawnPoints");

                if (GUILayout.Button("Create Spawn Points"))
                {
                    serializedObject.ApplyModifiedProperties();

                    property.ClearArray();
                    for (int i = 0; i < spawnPoint.SpawnCount; i++)
                    {
                        property.InsertArrayElementAtIndex(i);
                        SerializedProperty childProperty = property.GetArrayElementAtIndex(i);


                        Vector2 point = new Vector2();
                        point.x = Random.Range(spawnPoint.MapSize.x, spawnPoint.MapSize.y);
                        point.y = Random.Range(spawnPoint.MapSize.x, spawnPoint.MapSize.y);

                        childProperty.vector2Value = point;
                    }//for(i)
                }

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                {
                    EditorGUILayout.PropertyField(property);
                }
                EditorGUILayout.EndScrollView();
            }


            //Save Button
            if(GUILayout.Button("Save SO File"))
            {
                string path = $"{Application.dataPath}/UnitTests/01_Spawner/";
                path = EditorUtility.SaveFilePanel("Save SO File", path, "SpawnPoint", "asset");

                if(path.Length > 0)
                {
                    DirectoryHelpers.ToRelativePath(ref path);

                    serializedObject.ApplyModifiedProperties();

                    
                    SpawnPoint obj = serializedObject.targetObject as SpawnPoint;

                    bool bCheck = true;
                    bCheck &= (obj.EnemyPrefab != null);
                    bCheck &= (obj.SpawnCount > 0);
                    bCheck &= (obj.SpawnPoints != null);

                    if(bCheck)
                    {
                        Enemy enemy = obj.EnemyPrefab.GetComponent<Enemy>();

                        bCheck &= (enemy != null);
                        bCheck &= (obj.SpawnPoints.Length > 0);


                        AssetDatabase.CreateAsset(obj, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        EditorUtility.FocusProjectWindow();

                        Selection.activeObject = obj;


                        string fileName = FileHelpers.ToFileName(path);
                        EditorUtility.DisplayDialog("Create SO File", $"{fileName} 생성이 완료되었습니다.", "확인");
                    }
                }//if(path.Length)
            }
        }
    }
}