using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Object = UnityEngine.Object;

//在选中的资源中查找
public static class EnumAssets
{

    //枚举所有的T类型的资源 
    public static IEnumerable<T> EnumInCurrentSelection<T>()
    where T : Object
    {
        Object[] selectionAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        {
            var __array1 = selectionAsset;
            var __arrayLength1 = __array1.Length;
            for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
            {
                var s = __array1[__i1];
                {
                    var temp = s as T;
                    if (null != temp)
                    {
                        yield return temp;
                    }
                }
            }
        }
    }

    //枚举所有的GameObject类型的资源 
    public static IEnumerable<GameObject> EnumGameObjectInCurrentSelection()
    {
        {
            // foreach(var s in EnumInCurrentSelection<GameObject>())
            var __enumerator2 = (EnumInCurrentSelection<GameObject>()).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var s = __enumerator2.Current;
                {
                    yield return s;
                }
            }
        }

        yield break;
    }

    //递归枚举所有GameObject
    public static IEnumerable<GameObject> EnumGameObjectRecursiveInCurrentSelection()
    {
        {
            // foreach(var s in EnumInCurrentSelection<GameObject>())
            var __enumerator4 = (EnumInCurrentSelection<GameObject>()).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var s = __enumerator4.Current;
                {
                    foreach (var g in EnumGameObjectRecursive(s))
                    {
                        yield return g;
                    }
                }
            }
        }
    }

    public static IEnumerable<GameObject> EnumGameObjectRecursive(GameObject go)
    {
        yield return go;
        for (int i = 0; i < go.transform.childCount; i++)
        {
            {
                // foreach(var t in EnumGameObjectRecursive(go.transform.GetChild(i).gameObject))
                var __enumerator5 = (EnumGameObjectRecursive(go.transform.GetChild(i).gameObject)).GetEnumerator();
                while (__enumerator5.MoveNext())
                {
                    var t = __enumerator5.Current;
                    {
                        yield return t;
                    }
                }
            }
        }
    }

    //递归枚举所有Compoent
    public static IEnumerable<T> EnumComponentRecursiveInCurrentSelection<T>()
        where T : UnityEngine.Component
    {
        {
            // foreach(var go in EnumInCurrentSelection<GameObject>())
            var __enumerator7 = (EnumInCurrentSelection<GameObject>()).GetEnumerator();
            while (__enumerator7.MoveNext())
            {
                var go = __enumerator7.Current;
                {
                    var cs = go.GetComponentsInChildren<T>(true);
                    foreach (var c in cs)
                    {
                        yield return c;
                    }
                }
            }
        }
    }


    //枚举所有GameObject在这个目录
    //path是相对于Application.dataPath的 例如 Assets/Res/UI/
    public static IEnumerable<GameObject> EnumGameObjectAtPath(string path)
    {
        var guids = AssetDatabase.FindAssets("t:GameObject", new string[] { path });
        {
            // foreach(var guid in guids)
            var __enumerator8 = (guids).GetEnumerator();
            while (__enumerator8.MoveNext())
            {
                var guid = (string)__enumerator8.Current;
                {
                    var p = AssetDatabase.GUIDToAssetPath(guid);
                    var go = AssetDatabase.LoadAssetAtPath(p, typeof(GameObject)) as GameObject;
                    if (null != go)
                    {
                        yield return go;
                    }
                }
            }
        }
    }

	//枚举所有Object在这个目录
	//path是相对于Application.dataPath的 例如 Assets/Res/UI/
	public static IEnumerable<Object> EnumObjectAtPath(string path)
	{
		var guids = AssetDatabase.FindAssets("t:GameObject", new string[] { path });
		{
			// foreach(var guid in guids)
			var __enumerator8 = (guids).GetEnumerator();
			while (__enumerator8.MoveNext())
			{
				var guid = (string)__enumerator8.Current;
				{
					var p = AssetDatabase.GUIDToAssetPath(guid);
					var go = AssetDatabase.LoadAssetAtPath(p, typeof(Object)) as Object;
					if (null != go)
					{
						yield return go;
					}
				}
			}
		}
	}

    //枚举所有资源
    //path是相对于Application.dataPath的 例如 Assets/Res/UI/
    public static IEnumerable<T> EnumAssetAtPath<T>(string path)
        where T : Object
    {
        var guids = AssetDatabase.FindAssets("t:Object", new string[] { path });
        {
            // foreach(var guid in guids)
            var __enumerator9 = (guids).GetEnumerator();
            while (__enumerator9.MoveNext())
            {
                var guid = (string)__enumerator9.Current;
                {
                    var p = AssetDatabase.GUIDToAssetPath(guid);
                    var go = AssetDatabase.LoadAssetAtPath(p, typeof(System.Object)) as T;
                    if (null != go)
                    {
                        yield return go;
                    }
                }
            }
        }
    }

    //递归枚举这个目录下的GameObject的所有T类型组件
    //path是相对于Application.dataPath的 例如 Assets/Res/UI/
    public static IEnumerable<T> EnumComponentRecursiveAtPath<T>(string path)
        where T : UnityEngine.Component
    {
        var gos = EnumGameObjectAtPath(path);
        {
            // foreach(var go in gos)
            var __enumerator11 = (gos).GetEnumerator();
            while (__enumerator11.MoveNext())
            {
                var go = __enumerator11.Current;
                {
                    var cs = go.GetComponentsInChildren<T>(true);
                    foreach (var c in cs)
                    {
                        yield return c;
                    }
                }
            }
        }
    }

    //递归枚举这个目录下的GameObject
    //path是相对于Application.dataPath的 例如 Assets/Res/UI/
    public static IEnumerable<GameObject> EnumGameObjectRecursiveAtPath(string path)
    {
        var gos = EnumComponentRecursiveAtPath<Transform>(path);
        {
            // foreach(var go in gos)
            var __enumerator12 = (gos).GetEnumerator();
            while (__enumerator12.MoveNext())
            {
                var go = __enumerator12.Current;
                {
                    yield return go.gameObject;
                }
            }
        }
    }
    //递归枚举通过这个目录读出来的某个GObject以及其子节的gameobject
    public static IEnumerable<GameObject> EunmAllGameObjectRecursiveAtPath(string Path)
    {

        if (Path.Contains(".asset"))
            yield break;
        var objects = AssetDatabase.LoadAssetAtPath(Path, typeof(GameObject));


        if (objects != null)
        {
            if (objects.GetType().ToString() == "UnityEngine.GameObject")
            {
                GameObject mpGameObject = objects as GameObject;
                {
                    // foreach(var aa in EnumAssets.EnumGameObjectRecursive(mpGameObject))
                    var __enumerator13 = (EnumAssets.EnumGameObjectRecursive(mpGameObject)).GetEnumerator();
                    while (__enumerator13.MoveNext())
                    {
                        var aa = __enumerator13.Current;
                        {
                            if (aa != null)
                            {
                                yield return aa;
                            }
                        }
                    }
                }
            }
        }

        yield break;
    }
    //递归枚举GameObject以及其子节点所依赖的gameobject
    public static IEnumerable<GameObject> EnumAllGameObjectDependenciesRecursive(IEnumerable<UnityEngine.Object> gos)
    {

        int c = gos.Count();
        var modelArray = new string[c];
        int i = 0;
        {
            // foreach(var go in gos)
            var __enumerator14 = (gos).GetEnumerator();
            while (__enumerator14.MoveNext())
            {
                var go = __enumerator14.Current;
                {
                    modelArray[i++] = AssetDatabase.GetAssetPath(go.GetInstanceID());

                }
            }
        }
        string log = "";
        var dep = AssetDatabase.GetDependencies(modelArray);
        var processed = 0;
        var count = dep.Length;
        for (i = 0; i < count; i++)
        {
            var path = dep[i];

            if (string.IsNullOrEmpty(path))
            {
                continue;
            }

            if (path.Contains(".asset"))
                continue;

            var objects = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));

            if (objects != null)
            {
                GameObject mpGameObject = objects as GameObject;
                var temp = EnumAssets.EnumGameObjectRecursive(mpGameObject);
                {
                    // foreach(var aa in temp)
                    var __enumerator15 = (temp).GetEnumerator();
                    while (__enumerator15.MoveNext())
                    {
                        var aa = __enumerator15.Current;
                        {
                            if (aa != null)
                            {
                                yield return aa;
                            }
                        }
                    }
                }
            }
        }
    }
    //递归枚举GameObject以及其子节点所依赖的gameobject的组件T
    public static IEnumerable<T> EnumAllComponentDependenciesRecursive<T>(IEnumerable<UnityEngine.Object> gos)
        where T : UnityEngine.Component
    {

        int c = gos.Count();
        var modelArray = new string[c];
        int i = 0;
        {
            // foreach(var go in gos)
            var __enumerator16 = (gos).GetEnumerator();
            while (__enumerator16.MoveNext())
            {
                var go = __enumerator16.Current;
                {
                    modelArray[i++] = AssetDatabase.GetAssetPath(go.GetInstanceID());

                }
            }
        }
        string log = "";
        var dep = AssetDatabase.GetDependencies(modelArray);
        var processed = 0;
        var count = dep.Length;
        for (i = 0; i < count; i++)
        {
            var Path = dep[i];
            if (Path.Contains(".asset"))
                continue;
            var objects = AssetDatabase.LoadAssetAtPath(Path, typeof(GameObject));


            if (objects != null)
            {

                GameObject mpGameObject = objects as GameObject;
                if (mpGameObject != null)
                {
                    var cs = mpGameObject.GetComponentsInChildren<T>(true);
                    if (cs != null)
                    {
                        var __array17 = cs;
                        var __arrayLength17 = __array17.Length;
                        for (int __i17 = 0; __i17 < __arrayLength17; ++__i17)
                        {
                            var data = __array17[__i17];
                            {
                                yield return data;
                            }
                        }
                    }
                }


            }

        }
    }

	public static IEnumerable<UnityEngine.Object> EnumAllObjectDependentAtPath(string path)
	{
		var objs = EnumAssets.EnumObjectAtPath(path);
		List<string> pathList = new List<string>();
		foreach (var obj in objs)
		{
			var assetPath = AssetDatabase.GetAssetPath(obj.GetInstanceID());
			pathList.Add(assetPath);
		}

		var paths = AssetDatabase.GetDependencies(pathList.ToArray());
		foreach (var p in paths)
		{
			var res = AssetDatabase.LoadAssetAtPath(p, typeof(UnityEngine.Object));
			if (null != res)
			{
				yield return res;
			}
		}
	}


}
