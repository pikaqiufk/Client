using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UnityEditor.XCodeEditor
{
    public partial class XCPlist
    {
        string plistPath;
        bool plistModified;

        // URLTypes constant --- plist
        const string BundleUrlTypes = "CFBundleURLTypes";
        const string BundleTypeRole = "CFBundleTypeRole";
        const string BundleUrlName = "CFBundleURLName";
        const string BundleUrlSchemes = "CFBundleURLSchemes";

        // URLTypes constant --- projmods
        const string PlistUrlType = "urltype";
        const string PlistRole = "role";
        const string PlistEditor = "Editor";
        const string PlistName = "name";
        const string PlistSchemes = "schemes";


        public void Process(Hashtable plist)
        {
            if (null == plist)
                return;

            Dictionary<string, object> dict = (Dictionary<string, object>)PlistCS.Plist.readPlist(plistPath);
            {
                // foreach(var entry in plist)
                var __enumerator1 = (plist).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var entry = (DictionaryEntry)__enumerator1.Current;
                    {
                        this.AddPlistItems((string)entry.Key, entry.Value, dict);
                    }
                }
            }
            if (plistModified)
            {
                PlistCS.Plist.writeXml(dict, plistPath);
            }
        }

        public void AddPlistItems(string key, object value, Dictionary<string, object> dict)
        {
            Debug.Log("AddPlistItems: key=" + key);

            if (key.CompareTo(PlistUrlType) == 0)
            {
                processUrlTypes((ArrayList)value, dict);
            }
            else
            {
                Debug.LogWarning("unknown plist key : " + key);
            }
        }

        private void processUrlTypes(ArrayList urltypes, Dictionary<string, object> dict)
        {
            List<object> bundleUrlTypes;
            if (dict.ContainsKey(BundleUrlTypes))
            {
                bundleUrlTypes = (List<object>)dict[BundleUrlTypes];
            }
            else
            {
                bundleUrlTypes = new List<object>();
            }
            {
                // foreach(var table in urltypes)
                var __enumerator2 = (urltypes).GetEnumerator();
                while (__enumerator2.MoveNext())
                {
                    var table = (Hashtable)__enumerator2.Current;
                    {
                        string role = (string)table[PlistRole];
                        if (string.IsNullOrEmpty(role))
                        {
                            role = PlistEditor;
                        }
                        string name = (string)table[PlistName];
                        ArrayList shcemes = (ArrayList)table[PlistSchemes];

                        // new schemes
                        List<object> urlTypeSchemes = new List<object>();
                        {
                            // foreach(var s in shcemes)
                            var __enumerator4 = (shcemes).GetEnumerator();
                            while (__enumerator4.MoveNext())
                            {
                                var s = (string)__enumerator4.Current;
                                {
                                    urlTypeSchemes.Add(s);
                                }
                            }
                        }

                        Dictionary<string, object> urlTypeDict = this.findUrlTypeByName(bundleUrlTypes, name);
                        if (urlTypeDict == null)
                        {
                            urlTypeDict = new Dictionary<string, object>();
                            urlTypeDict[BundleTypeRole] = role;
                            urlTypeDict[BundleUrlName] = name;
                            urlTypeDict[BundleUrlSchemes] = urlTypeSchemes;
                            bundleUrlTypes.Add(urlTypeDict);
                        }
                        else
                        {
                            urlTypeDict[BundleTypeRole] = role;
                            urlTypeDict[BundleUrlSchemes] = urlTypeSchemes;
                        }
                        plistModified = true;
                    }
                }
            }
            dict[BundleUrlTypes] = bundleUrlTypes;
        }

        private Dictionary<string, object> findUrlTypeByName(List<object> bundleUrlTypes, string name)
        {
            if ((bundleUrlTypes == null) || (bundleUrlTypes.Count == 0))
                return null;
            {
                var __list3 = bundleUrlTypes;
                var __listCount3 = __list3.Count;
                for (int __i3 = 0; __i3 < __listCount3; ++__i3)
                {
                    var dict = (Dictionary<string, object>)__list3[__i3];
                    {
                        string _n = (string)dict[BundleUrlName];
                        if (string.Compare(_n, name) == 0)
                        {
                            return dict;
                        }
                    }
                }
            }
            return null;
        }
    }
}
