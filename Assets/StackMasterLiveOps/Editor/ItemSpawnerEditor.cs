using UnityEngine;
using UnityEditor;
using FunCell.GridMerge.GamePlay;

[CustomEditor(typeof(ItemSpawner))]
public class ItemSpawnerEditor : Editor
{
    private MergeItemLevel _selectedLevel = MergeItemLevel.Level1;

    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();

        ItemSpawner spawner = (ItemSpawner)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug Tools", EditorStyles.boldLabel);

        // Dropdown for selecting level
        _selectedLevel = (MergeItemLevel)EditorGUILayout.EnumPopup("Select Level", _selectedLevel);

        // Button to change first item
        if (GUILayout.Button("Change First Item"))
        {
            if (Application.isPlaying)
            {
                spawner.ChangeFirstItem(_selectedLevel);
            }
            else
            {
                Debug.LogWarning("Run the game to use this feature.");
            }
        }
    }
}
