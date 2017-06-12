using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Timers;
using System.Threading;
using DataContract;
using System.Collections.Generic;
using System.Linq;


public class AnimationClipTool
{
    //找到animator中的animationclip
    private static AnimationClip FindAnimation(GameObject obj)
    {
        string name = null;
        AnimationClip clip = new AnimationClip();
        Animator anim = obj.GetComponent<Animator>();


        if (anim != null)
        {
            UnityEditorInternal.AnimatorController ac =
                anim.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
            if (null == ac)
            {
                return null;
            }
            if (null == ac.GetLayer(0))
            {
                return null;
            }
            UnityEditorInternal.StateMachine sm = ac.GetLayer(0).stateMachine;

            for (int i = 0; i < sm.stateCount; i++)
            {
                UnityEditorInternal.State state = sm.GetState(i);

                clip = state.GetMotion() as AnimationClip;
                if (clip != null)
                {
                    name = clip.name;
                    return clip;
                }

            }

        }

        return null;
    }
    //把animator变为animation
    private static bool ChangeAnimator2Animation(GameObject o)
    {
        if (o.GetComponent<Animator>())
        {
            o.AddComponent<Animation>();
            var animation = o.GetComponent<Animation>();

            AnimationClip mCilp = FindAnimation(o);

            if (null != mCilp)
            {
                AnimationUtility.SetAnimationType(mCilp, ModelImporterAnimationType.Legacy);
                animation.clip = mCilp;


                AnimationUtility.SetAnimationClips(animation, new AnimationClip[] { mCilp });


                UnityEditor.EditorUtility.SetDirty(animation);
                UnityEditor.EditorUtility.SetDirty(mCilp);
            }

            MonoBehaviour.DestroyImmediate(o.GetComponent<Animator>(), true);
            UnityEditor.EditorUtility.SetDirty(o);

            return true;
        }
        return false;
    }

    public static void AnimationOptimizing(Animator go)
    {
        var g = go.gameObject;
        if (go.runtimeAnimatorController == null)
        {
            MonoBehaviour.DestroyImmediate(g.GetComponent<Animator>(), true);
            UnityEditor.EditorUtility.SetDirty(g);

        }
        else
        {
            if (FindAnimation(go.gameObject) != null)
            {
                ChangeAnimator2Animation(g);
            }
            else
            {
                MonoBehaviour.DestroyImmediate(g.GetComponent<Animator>(), true);
                UnityEditor.EditorUtility.SetDirty(g);
            }
        }
    }

    [MenuItem("Tools/Animation/Change Animator To Animation")]
    //Animator如果不带Controller或是带Controller但是没有设置动作的 也直接去掉，不用再添加Animation
    public static bool ChangeAnimatorToAnimation()
    {
        string str = "";
        Debug.Log("ChangeAnimatorToAnimation----------------begin");
        int i = 0;
        {
            // foreach(var go in EnumAssets.EnumComponentRecursiveInCurrentSelection<Animator>())
            var __enumerator1 = (EnumAssets.EnumComponentRecursiveInCurrentSelection<Animator>()).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var go = __enumerator1.Current;
                {
                    var g = go.gameObject;

                    AnimationOptimizing(go);


                    str += "[" + g.transform.FullPath() + "]\n";
                    i++;
                }
            }
        }
        if (!string.IsNullOrEmpty(str))
        {
            Debug.Log(str);
        }



        Debug.Log("ChangeAnimatorToAnimation-Total=[" + i.ToString() + "]---------------end");
        return true;
    }
    [MenuItem("Tools/Animation/Change AnimationClips")]
    //Animator如果不带Controller或是带Controller但是没有设置动作的 也直接去掉，不用再添加Animation
    public static bool ChangeAnimationClips()
    {
        string str = "";
        Debug.Log("ChangeAnimatorToAnimation----------------begin");
        int i = 0;
        {
            // foreach(var go in EnumAssets.EnumComponentRecursiveInCurrentSelection<Animation>())
            var __enumerator2 = (EnumAssets.EnumComponentRecursiveInCurrentSelection<Animation>()).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var go = __enumerator2.Current;
                {
                    var g = go.gameObject;
                    var animation = g.GetComponent<Animation>();

                    AnimationClip mCilp = animation.clip;

                    if (null != mCilp)
                    {
                        AnimationUtility.SetAnimationType(mCilp, ModelImporterAnimationType.Legacy);

                        if (AnimationUtility.GetAnimationClips(animation).Length == 0)
                        {
                            AnimationUtility.SetAnimationClips(animation, new AnimationClip[] { mCilp });
                        }

                        UnityEditor.EditorUtility.SetDirty(mCilp);
                    }

                    UnityEditor.EditorUtility.SetDirty(g);

                    str += "[" + g.transform.FullPath() + "]\n";
                    i++;
                }
            }
        }
        if (!string.IsNullOrEmpty(str))
        {
            Debug.Log(str);
        }



        Debug.Log("ChangeAnimatorToAnimation-Total=[" + i.ToString() + "]---------------end");
        return true;
    }
    public static bool ChangeAnimatorToAnimationInPath(string ASSET_PATH)
    {
        EditorUtility.DisplayProgressBar("Change Animator to Animation in Path", "Collecting MeshRenderer Components", 0);
        string str = "";
        Debug.Log("ChangeAnimatorToAnimation----------------begin");
        int i = 0;
        var gos =
            EnumAssets.EnumAllComponentDependenciesRecursive<Animator>(
                EnumAssets.EnumAssetAtPath<UnityEngine.Object>(ASSET_PATH));

        int total = 0;
        {
            // foreach(var c in gos)
            var __enumerator3 = (gos).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var c = __enumerator3.Current;
                {
                    total++;
                }
            }
        }
        {
            // foreach(var go in gos)
            var __enumerator4 = (gos).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var go = __enumerator4.Current;
                {

                    EditorUtility.DisplayProgressBar("Change Animator to Animation in Path", go.gameObject.name, i * 1.0f / total);
                    var g = go.gameObject;

                    AnimationOptimizing(go);


                    str += "[" + g.transform.FullPath() + "]\n";
                    i++;
                }
            }
        }
        EditorUtility.ClearProgressBar();
        if (!string.IsNullOrEmpty(str))
        {
            Debug.Log(str);
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        Debug.Log("ChangeAnimatorToAnimation-Total=[" + i.ToString() + "]---------------end");
        return true;
    }
    [MenuItem("Tools/Animation/Remove Useless Animation")]
    //Animator如果不带Controller或是带Controller但是没有设置动作的 也直接去掉，不用再添加Animation
    public static bool RemoveUselessAnimation()
    {
        string str = "";
        Debug.Log("Remove Animation----------------begin");
        int i = 0;
        {
            // foreach(var c in EnumAssets.EnumComponentRecursiveInCurrentSelection<Animation>())
            var __enumerator5 = (EnumAssets.EnumComponentRecursiveInCurrentSelection<Animation>()).GetEnumerator();
            while (__enumerator5.MoveNext())
            {
                var c = __enumerator5.Current;
                {
                    var go = c.gameObject;
                    if (c.GetClipCount() <= 0)
                    {
                        MonoBehaviour.DestroyImmediate(c, true);
                    }

                    str += "[" + go.transform.FullPath() + "]\n";
                    i++;
                }
            }
        }
        if (!string.IsNullOrEmpty(str))
        {
            Debug.Log(str);
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        Debug.Log("Remove Useless Animation-Total=[" + i.ToString() + "]---------------end");
        return true;
    }

    [MenuItem("Tools/Animation/Find Use Animation Prefab")]
    //Animator如果不带Controller或是带Controller但是没有设置动作的 也直接去掉，不用再添加Animation
    public static bool FindUseAnimationPrefab()
    {
        string str = "";
        Debug.Log("Find Use Animation Prefab----------------begin");
        int i = 0;
        {
            // foreach(var c in EnumAssets.EnumComponentRecursiveInCurrentSelection<Animation>())
            var __enumerator6 = (EnumAssets.EnumComponentRecursiveInCurrentSelection<Animation>()).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                var c = __enumerator6.Current;
                {
                    if (c.GetClipCount() <= 0 || null == c.clip)
                    {
                        continue;
                    }

                    var go = c.gameObject;
                    str += AssetDatabase.GetAssetPath(go.GetInstanceID());
                    str += "[" + go.transform.FullPath() + "]\n";
                    i++;
                }
            }
        }
        if (!string.IsNullOrEmpty(str))
        {
            Debug.Log(str);
        }

        Debug.Log("Find Use Animation Prefab-Total=[" + i.ToString() + "]---------------end");
        return true;
    }

    //移除AnimationClip
    [MenuItem("Tools/Animation/Remove Animation Clip In Selection(don't recursive)")]
    public static bool RemoveAllAnimationClipInCurrentSelection()
    {
        string str = "";
        Debug.Log("Remove All AnimationClip----------------begin");
        var gos = EnumAssets.EnumGameObjectInCurrentSelection();

        int i = 0;
        int processed = 0;
        int count = gos.Count();
        {
            // foreach(var go in gos)
            var __enumerator9 = (gos).GetEnumerator();
            while (__enumerator9.MoveNext())
            {
                var go = __enumerator9.Current;
                {
                    EditorUtility.DisplayProgressBar("Remove Animation Clip In Selection", go.name, i * 1.0f / count);
                    i++;

                    var c = go.GetComponent<Animation>();

                    if (null == c)
                    {
                        continue;
                    }
                    c.clip = null;

                    if (c.GetClipCount() <= 0)
                    {
                        continue;
                    }

                    List<string> strList = new List<string>();
                    foreach (var clip in c)
                    {
                        var aniState = clip as AnimationState;
                        if (null != aniState)
                        {
                            strList.Add(aniState.clip.name);
                        }
                    }
                    foreach (var name in strList)
                    {
                        c.RemoveClip(name);
                    }

                    str += AssetDatabase.GetAssetPath(go.GetInstanceID());
                    str += "[" + go.transform.FullPath() + "]\n";


                    processed++;
                }
            }
        }

        EditorUtility.ClearProgressBar();

        if (!string.IsNullOrEmpty(str))
        {
            Debug.Log(str);
        }
        if (processed > 0)
        {
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        Debug.Log("Find Use Animation Prefab-Total=[" + processed.ToString() + "]---------------end");
        return true;
    }
}
