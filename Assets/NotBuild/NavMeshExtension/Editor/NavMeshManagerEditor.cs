/*  This file is part of the "NavMesh Extension" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them directly or indirectly
 *  from Rebound Games. You shall not license, sublicense, sell, resell, transfer, assign,
 *  distribute or otherwise make available to any third party the Service or the Content. 
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace NavMeshExtension
{
    /// <summary>
    /// Adds a new Portal Manager gameobject to the scene.
    /// <summary>
    [CustomEditor(typeof(NavMeshManager))]
    public class NavMeshManagerEditor : Editor
    {
        //manager reference
        private NavMeshManager script;


        void OnEnable()
        {
            script = (NavMeshManager)target;
        }


        /// <summary>
        /// Custom inspector override for buttons.
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUIUtility.LookLikeControls();
            EditorGUILayout.Space();

            if (GUILayout.Button("New NavMesh"))
            {
                CreateNewNavMesh();
                GetSceneView().Focus();
            }

            if (GUILayout.Button("Toggle Renderers"))
            {
                //invert boolean and toggle all renderers
                script.rendererToggle = !script.rendererToggle;
                MeshRenderer[] renderers = script.GetComponentsInChildren<MeshRenderer>(true);

                for (int i = 0; i < renderers.Length; i++)
                    renderers[i].enabled = script.rendererToggle;
            }

            if (GUILayout.Button("Bake NavMesh"))
            {
                BakeNavMesh();
            }

            if (GUILayout.Button("Triangulate"))
            {
                var navMeshes = script.GetComponentsInChildren<NavMeshObject>();
                for (int i = 0; i < navMeshes.Length; i++)
                {
                    var navMesh = navMeshes[i];

                    if (CheckCombine(navMesh))
                    {
                        navMesh.Combine();
                    }

                    navMesh.RegenerateMesh(ConvertAllPoints(navMesh));
					EditorUtility.DisplayProgressBar("NavMeshManagerEditor", i.ToString(),
							i*1.0f / navMeshes.Length);
                }

                for (int i = 0; i < script.transform.childCount; i++)
                {
                    var c = script.transform.GetChild(i);
					MeshCollider co = c.gameObject.GetComponent<MeshCollider>();
					if (co == null)
                    {
                        co = c.gameObject.AddComponent<MeshCollider>();
                        //co.sharedMesh = c.gameObject.GetComponent<MeshFilter>().sharedMesh;
                    }
					co.sharedMesh = c.gameObject.GetComponent<MeshFilter>().sharedMesh;

					EditorUtility.DisplayProgressBar("MeshFilter", i.ToString(),
							i * 1.0f / script.transform.childCount);
                }
				EditorUtility.ClearProgressBar();
            }
			
        }

        private Vector3[] ConvertAllPoints(NavMeshObject script)
        {
            int count = script.list.Count;
            List<Vector3> all = new List<Vector3>();
            for (int i = 0; i < count; i++)
                all.Add(script.transform.TransformPoint(script.list[i]));

            return all.ToArray();
        }

        private bool CheckCombine(NavMeshObject script)
        {
            //get count of all submeshes
            int subPointsCount = script.subPoints.Count;
            if (subPointsCount > 0)
            {
                //remove submesh without references
                if (script.subPoints[subPointsCount - 1].list.Count == 0)
                    script.subPoints.RemoveAt(subPointsCount - 1);
                else if (script.subPoints[subPointsCount - 1].list.Count <= 2)
                {
                    NavMeshManagerEditor.ShowNotification("Can't combine submeshes.\nYou haven't placed enough points.");
                    return false;
                }
            }

            return true;
        }



        /// <summary>
        /// Creates a new gameobject to use it as NavMeshObject.
        /// </summary>
        public void CreateNewNavMesh()
        {
            //create gameobject
            GameObject navGO = new GameObject("New NavMesh");
            navGO.transform.parent = script.transform;
            navGO.isStatic = true;
            navGO.AddComponent<NavMeshObject>();

            //modify renderer to ignore shadows
            MeshRenderer mRenderer = navGO.GetComponent<MeshRenderer>();
            mRenderer.castShadows = false;
            mRenderer.receiveShadows = false;
            if (script.meshMaterial)
                mRenderer.sharedMaterial = script.meshMaterial;
            else
                mRenderer.enabled = false;

            Undo.RegisterCreatedObjectUndo(navGO, "Created NavMesh");
            Selection.activeGameObject = navGO;
        }


        /// <summary>
        /// Bakes the Unity NavMesh on created NavMeshObjects.
        /// </summary>
        public void BakeNavMesh()
        {
            //loop over renderers and enable them for the baking process,
            //as otherwise the NavMeshBuilder will ignore them 
            List<Renderer> disabledObjects = new List<Renderer>();
            {
                var __array1 = Object.FindObjectsOfType(typeof(Renderer));
                var __arrayLength1 = __array1.Length;
                for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var item = (Renderer)__array1[__i1];
                    {
                        if (GameObjectUtility.AreStaticEditorFlagsSet(item.gameObject, StaticEditorFlags.NavigationStatic)
                            && !item.enabled)
                        {
                            disabledObjects.Add(item);
                            item.renderer.enabled = true;
                        }
                    }
                }
            }
            //trigger navmesh builder
            NavMeshBuilder.BuildNavMesh();
            //re-enable disabled renderers
            disabledObjects.ForEach(obj => obj.enabled = false);

            ShowNotification("NavMesh successfully built.");
        }


        /// <summary>
        /// Shows a SceneView notification.
        /// </summary>
        public static void ShowNotification(string text)
        {
            GetSceneView().ShowNotification(new GUIContent(text));
        }


        /// <summary>
        /// Gets the active SceneView or creates one.
        /// </summary>
        public static SceneView GetSceneView()
        {
            SceneView view = SceneView.currentDrawingSceneView;
            if (view == null)
                view = EditorWindow.GetWindow<SceneView>();

            return view;
        }
    }
}
