using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Inventory System search functionality.
/// </summary>

public class InvFindItem : ScriptableWizard
{
    /// <summary>
    /// Private class used to return data from the Find function below.
    /// </summary>

    struct FindResult
    {
        public InvDatabase db;
        public InvBaseItem item;
    }

    string mItemName = "";
    List<FindResult> mResults = new List<FindResult>();

    /// <summary>
    /// Add a menu option to display this wizard.
    /// </summary>

    [MenuItem("Window/Find Item #&i")]
    static void FindItem()
    {
        ScriptableWizard.DisplayWizard<InvFindItem>("Find Item");
    }

    /// <summary>
    /// Draw the custom wizard.
    /// </summary>

    void OnGUI()
    {
        NGUIEditorTools.SetLabelWidth(80f);
        string newItemName = EditorGUILayout.TextField("Search for:", mItemName);
        NGUIEditorTools.DrawSeparator();

        if (GUI.changed || newItemName != mItemName)
        {
            mItemName = newItemName;

            if (string.IsNullOrEmpty(mItemName))
            {
                mResults.Clear();
            }
            else
            {
                FindAllByName(mItemName);
            }
        }

        if (mResults.Count == 0)
        {
            if (!string.IsNullOrEmpty(mItemName))
            {
                GUILayout.Label("No matches found");
            }
        }
        else
        {
            Print3("Item ID", "Item Name", "Path", false);
            NGUIEditorTools.DrawSeparator();
            {
                var __list1 = mResults;
                var __listCount1 = __list1.Count;
                for (int __i1 = 0; __i1 < __listCount1; ++__i1)
                {
                    var fr = (FindResult)__list1[__i1];
                    {
                        if (Print3(InvDatabase.FindItemID(fr.item).ToString(),
                            fr.item.name, NGUITools.GetHierarchy(fr.db.gameObject), true))
                        {
                            InvDatabaseInspector.SelectIndex(fr.db, fr.item);
                            Selection.activeGameObject = fr.db.gameObject;
                            EditorUtility.SetDirty(Selection.activeGameObject);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Helper function used to print things in columns.
    /// </summary>

    bool Print3(string a, string b, string c, bool button)
    {
        bool retVal = false;

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label(a, GUILayout.Width(80f));
            GUILayout.Label(b, GUILayout.Width(160f));
            GUILayout.Label(c);

            if (button)
            {
                retVal = GUILayout.Button("Select", GUILayout.Width(60f));
            }
            else
            {
                GUILayout.Space(60f);
            }
        }
        GUILayout.EndHorizontal();
        return retVal;
    }

    /// <summary>
    /// Find items by name.
    /// </summary>

    void FindAllByName(string partial)
    {
        partial = partial.ToLower();
        mResults.Clear();
        {
            var __array2 = InvDatabase.list;
            var __arrayLength2 = __array2.Length;
            for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
            {
                var db = (InvDatabase)__array2[__i2];
                {
                    {
                        // foreach(var item in db.items)
                        var __enumerator5 = (db.items).GetEnumerator();
                        while (__enumerator5.MoveNext())
                        {
                            var item = (InvBaseItem)__enumerator5.Current;
                            {
                                if (item.name.Equals(partial, System.StringComparison.OrdinalIgnoreCase))
                                {
                                    FindResult fr = new FindResult();
                                    fr.db = db;
                                    fr.item = item;
                                    mResults.Add(fr);
                                }
                            }
                        }
                    }
                }
            }
        }
        {
            var __array3 = InvDatabase.list;
            var __arrayLength3 = __array3.Length;
            for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
            {
                var db = (InvDatabase)__array3[__i3];
                {
                    {
                        // foreach(var item in db.items)
                        var __enumerator7 = (db.items).GetEnumerator();
                        while (__enumerator7.MoveNext())
                        {
                            var item = (InvBaseItem)__enumerator7.Current;
                            {
                                if (item.name.StartsWith(partial, System.StringComparison.OrdinalIgnoreCase))
                                {
                                    bool exists = false;
                                    {
                                        var __list10 = mResults;
                                        var __listCount10 = __list10.Count;
                                        for (int __i10 = 0; __i10 < __listCount10; ++__i10)
                                        {
                                            var res = (FindResult)__list10[__i10];
                                            {
                                                if (res.item == item)
                                                {
                                                    exists = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (!exists)
                                    {
                                        FindResult fr = new FindResult();
                                        fr.db = db;
                                        fr.item = item;
                                        mResults.Add(fr);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        {
            var __array4 = InvDatabase.list;
            var __arrayLength4 = __array4.Length;
            for (int __i4 = 0; __i4 < __arrayLength4; ++__i4)
            {
                var db = (InvDatabase)__array4[__i4];
                {
                    {
                        // foreach(var item in db.items)
                        var __enumerator9 = (db.items).GetEnumerator();
                        while (__enumerator9.MoveNext())
                        {
                            var item = (InvBaseItem)__enumerator9.Current;
                            {
                                if (item.name.ToLower().Contains(partial))
                                {
                                    bool exists = false;
                                    {
                                        var __list11 = mResults;
                                        var __listCount11 = __list11.Count;
                                        for (int __i11 = 0; __i11 < __listCount11; ++__i11)
                                        {
                                            var res = (FindResult)__list11[__i11];
                                            {
                                                if (res.item == item)
                                                {
                                                    exists = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (!exists)
                                    {
                                        FindResult fr = new FindResult();
                                        fr.db = db;
                                        fr.item = item;
                                        mResults.Add(fr);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
