using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ObjectPoolEditor : EditorWindow
{
    [MenuItem("Window/Object Pools")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ObjectPoolEditor));
    }

    public void OnEnable()
    {
        {
            titleContent = new GUIContent("Object Pool Overview");
        }
    }

    public void OnGUI()
    {
        foreach (ObjectPool pool in ObjectPool.Pools)
        {
            EditorGUILayout.BeginHorizontal("Box");
            {
                EditorGUILayout.ObjectField(pool.PoolItemName, pool.gameObject, typeof(GameObject), true, GUILayout.Width(300));

                int max = (pool.PoolMode == ObjectPoolMode.FixedSize) ? pool.Size : pool.transform.childCount;
                float value = (pool.ActivePoolObjects * 1.0f) / max;
                Rect rect = EditorGUILayout.GetControlRect(true, 20);
                string stri = pool.ActivePoolObjects + " / " + max;
                EditorGUI.ProgressBar(rect, value, stri);
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    public void OnInspectorUpdate()
    {
        Repaint();
    }
}
