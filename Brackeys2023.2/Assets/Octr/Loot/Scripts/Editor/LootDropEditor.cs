using UnityEngine;
using UnityEditor;

namespace octr.Loot
{
    [CustomEditor(typeof(DropSpawner))]
    public class DropSpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            if (GUILayout.Button("Activate"))
            {
                ((DropSpawner)target).GenerateDrops();
            }
        }
    }
}
