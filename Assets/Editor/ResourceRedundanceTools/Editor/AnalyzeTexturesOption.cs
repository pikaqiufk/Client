using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using Object = UnityEngine.Object;

public class AnalyzeTexturesOption
{
    private Dictionary<TextureImporterType, string> mImporterType = new Dictionary<TextureImporterType, string>();

    private string[] mTextureAttributeStrings = { "Name", "Type", "Format", "Path", "Width", "Height", "Is Power Of Two", "Read/Write Enabled", "Mipmap Enabled", "MemSize(Bytes)" };
    private int mSelectedAttributeString = 0;
    private List<TextureResourceUnit> mCurrentTextureSelectionList = new List<TextureResourceUnit>();

    private float mAttributeWidth = 0;
    private Vector2 mScrollPosition;

    private int mTexNumPerPage = 50;
    private int mCurrentPage = 1;
    private int mTotalPage = 1;

    private string mStateStr;
    private string mSearchName = "";
    private bool mIsSearchCaseSensitive = false;

    private string[] mTextureExtensionArray = new string[] { "*.jpg", "*.png", "*.psd", "*.tiff", "*.tga", "*.gif", "*.bmp", "*.iff", "*.pict", "*.exr", "*.tif" };
    private int mAssetPathStartIndex = Application.dataPath.IndexOf("Assets");

    bool mIsShowMainInterface = true;

    string[] mOptionSelectedStr = { "当前选择", "满足筛选条件" };
    public static int mOptionSelectedIndex = 0;

    string[] mOptionStr = { "把纹理的Read/Write Enable属性置为False"
                            , "把纹理的MipMap Enable属性置为False"
                            , "把纹理的类型改为自动压缩类型"
                            , "把纹理格式为RGBA32的纹理的格式改为DXT5, 只修改类型为Advanced的纹理"
                            , "把纹理高或宽大于1024的纹理缩放为1024"
                            };

    static bool mIsOptimizeReadWriteEnable = false;
    static bool mIsOptimizeMipMapEnable = false;
    static bool mIsOptimizeType = false;
    static bool mIsOptimizeFormat = false;
    static bool mIsOptimizeSize = false;

    public AnalyzeTexturesOption()
    {
        mStateStr = "Initialize Success!";

        mImporterType.Add(TextureImporterType.Image, "Texture");
        mImporterType.Add(TextureImporterType.Bump, "Normal map");
        mImporterType.Add(TextureImporterType.GUI, "GUI");
        mImporterType.Add(TextureImporterType.Reflection, "Reflection");
        mImporterType.Add(TextureImporterType.Cookie, "Cookie");
        mImporterType.Add(TextureImporterType.Advanced, "Advanced");
        mImporterType.Add(TextureImporterType.Lightmap, "Lightmap");
        mImporterType.Add(TextureImporterType.Cursor, "Cursor");
        //mImporterType.Add( TextureImporterType.Sprite, "Sprite" );
    }

    private Dictionary<string, bool> mIsDisplayFormats = new Dictionary<string, bool>();

    public List<TextureResourceUnit> mTextureResourcesList = new List<TextureResourceUnit>();
    public List<TextureResourceUnit> mIsDisplayTextureResourcesList = new List<TextureResourceUnit>();

    public HashSet<string> mUITextures = new HashSet<string>();
    protected void CollectAndAnalyzeTextures()
    {
        {
            var allFiles = new DirectoryInfo(Path.Combine(Application.dataPath, "Res/UI")).GetFiles("*.prefab",
                SearchOption.AllDirectories);
            var dep = AssetDatabase.GetDependencies(allFiles.Select(item => item.FullName.Remove(0, mAssetPathStartIndex)).ToArray());
            {

                var __array = (dep);
                var __arrayLength = __array.Length;
                for (int __i = 0; __i < __arrayLength; ++__i)
                {
                    var s = __array[__i];
                    if (Path.GetExtension(s).Equals(".png", StringComparison.InvariantCultureIgnoreCase))
                    {
                        mUITextures.Add(s);
                    }
                }
            }
        }

        {
            var allFiles = new DirectoryInfo(Path.Combine(Application.dataPath, "Res")).GetFiles("*.prefab",
                SearchOption.AllDirectories);
            var dep = AssetDatabase.GetDependencies(allFiles.Select(item => item.FullName.Remove(0, mAssetPathStartIndex)).ToArray());
            {
                var __array = (dep);
                var __arrayLength = __array.Length;
                for (int __i = 0; __i < __arrayLength; ++__i)
                {
                    var s = __array[__i];
                    {
                        {
                            var __array15 = mTextureExtensionArray;
                            var __arrayLength15 = __array15.Length;
                            for (int __i15 = 0; __i15 < __arrayLength15; ++__i15)
                            {
                                var ext = __array15[__i15];
                                {
                                    if (ext.Contains(Path.GetExtension(s)))
                                    {
                                        TextureResourceUnit textureResourceUnit = new TextureResourceUnit(s);

                                        try
                                        {
                                            SetTexUnitInfo(textureResourceUnit);
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.LogError("Analyze " + s + " Error. \n" + ex.ToString());
                                        }

                                        mTextureResourcesList.Add(textureResourceUnit);

                                        if (!mIsDisplayFormats.ContainsKey(textureResourceUnit.mResExtension))
                                        {
                                            mIsDisplayFormats.Add(textureResourceUnit.mResExtension, true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        {
            var allFiles = new DirectoryInfo(Path.Combine(Application.dataPath, "Res")).GetFiles("*.unity",
                SearchOption.AllDirectories);
            var dep = AssetDatabase.GetDependencies(allFiles.Select(item => item.FullName.Remove(0, mAssetPathStartIndex)).ToArray());
            {
                var __array = (dep);
                var __arrayLength = __array.Length;
                for (int __i = 0; __i < __arrayLength; ++__i)
                {
                    var s = __array[__i];
                    {
                        {
                            var __array16 = mTextureExtensionArray;
                            var __arrayLength16 = __array16.Length;
                            for (int __i16 = 0; __i16 < __arrayLength16; ++__i16)
                            {
                                var ext = __array16[__i16];
                                {
                                    if (ext.Contains(Path.GetExtension(s)))
                                    {
                                        TextureResourceUnit textureResourceUnit = new TextureResourceUnit(s);

                                        try
                                        {
                                            SetTexUnitInfo(textureResourceUnit);
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.LogError("Analyze " + s + " Error. \n" + ex.ToString());
                                        }

                                        mTextureResourcesList.Add(textureResourceUnit);

                                        if (!mIsDisplayFormats.ContainsKey(textureResourceUnit.mResExtension))
                                        {
                                            mIsDisplayFormats.Add(textureResourceUnit.mResExtension, true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //string [] textureAssets = AssetDatabase.FindAssets( "t:Texture", null );

        //foreach ( string textureAssetsIt in textureAssets )
        //{
        //    string textureResPath = AssetDatabase.GUIDToAssetPath( textureAssetsIt );
        //    TextureResourceUnit textureResourceUnit = new TextureResourceUnit( textureResPath );

        //    SetTexUnitInfo( textureResourceUnit );

        //    mTextureResourcesList.Add( textureResourceUnit );

        //    if ( !mIsDisplayFormats.ContainsKey( textureResourceUnit.mResExtension ) )
        //    {
        //        mIsDisplayFormats.Add( textureResourceUnit.mResExtension, true );
        //    }
        //}

        mStateStr = "Collect And Analyze Textures Success!";
    }

    void OnSelectTexUnit(TextureResourceUnit textureResourceUnit)
    {
        Selection.activeObject = textureResourceUnit.mTexture;

        bool isCtrlDown = false;
        bool isShiftDown = false;

        if (Event.current.control)
        {
            isCtrlDown = true;
        }

        if (Event.current.shift)
        {
            isShiftDown = true;
        }

        if (isCtrlDown)
        {
            mCurrentTextureSelectionList.Add(textureResourceUnit);
        }
        else if (isShiftDown)
        {
            if (mCurrentTextureSelectionList.Count > 0)
            {
                TextureResourceUnit lastSelectd = mCurrentTextureSelectionList[mCurrentTextureSelectionList.Count - 1];

                int startPos = mIsDisplayTextureResourcesList.IndexOf(lastSelectd);
                int endPos = mIsDisplayTextureResourcesList.IndexOf(textureResourceUnit);

                List<TextureResourceUnit> selectList = mIsDisplayTextureResourcesList.GetRange(startPos, endPos - startPos + 1);
                {
                    var __list4 = selectList;
                    var __listCount4 = __list4.Count;
                    for (int __i4 = 0; __i4 < __listCount4; ++__i4)
                    {
                        var texResUnit = (TextureResourceUnit)__list4[__i4];
                        {
                            if (!mCurrentTextureSelectionList.Contains(texResUnit))
                            {
                                mCurrentTextureSelectionList.Add(texResUnit);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            mCurrentTextureSelectionList.Clear();
            mCurrentTextureSelectionList.Add(textureResourceUnit);
        }
    }

    private void DrawRow(TextureResourceUnit textureResourceUnitIt)
    {
        GUI.color = (mCurrentTextureSelectionList.Contains(textureResourceUnitIt)) ? new Color(1, 0.5f, 1) : Color.white;

        GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));

        if (GUILayout.Button(textureResourceUnitIt.mResName, EditorStyles.label, GUILayout.Width(mAttributeWidth)))
        {
            OnSelectTexUnit(textureResourceUnitIt);

            mStateStr = textureResourceUnitIt.mResName;
        }

        if (GUILayout.Button(textureResourceUnitIt.mTextureType, EditorStyles.label, GUILayout.Width(mAttributeWidth)))
        {
            OnSelectTexUnit(textureResourceUnitIt);

            mStateStr = textureResourceUnitIt.mTextureType;
        }

        if (GUILayout.Button(textureResourceUnitIt.mTextureFormat, EditorStyles.label, GUILayout.Width(mAttributeWidth)))
        {
            OnSelectTexUnit(textureResourceUnitIt);

            mStateStr = textureResourceUnitIt.mTextureFormat;
        }

        if (GUILayout.Button(textureResourceUnitIt.mResPath, EditorStyles.label, GUILayout.Width(mAttributeWidth)))
        {
            OnSelectTexUnit(textureResourceUnitIt);

            mStateStr = textureResourceUnitIt.mResPath;
        }

        if (GUILayout.Button(textureResourceUnitIt.mWidth.ToString(), EditorStyles.label, GUILayout.Width(mAttributeWidth)))
        {
            OnSelectTexUnit(textureResourceUnitIt);

            mStateStr = textureResourceUnitIt.mWidth.ToString();
        }

        if (GUILayout.Button(textureResourceUnitIt.mHeight.ToString(), EditorStyles.label, GUILayout.Width(mAttributeWidth)))
        {
            OnSelectTexUnit(textureResourceUnitIt);

            mStateStr = textureResourceUnitIt.mHeight.ToString();
        }

        if (GUILayout.Button(textureResourceUnitIt.mIsPowerOfTwo.ToString(), EditorStyles.label, GUILayout.Width(mAttributeWidth)))
        {
            OnSelectTexUnit(textureResourceUnitIt);

            mStateStr = textureResourceUnitIt.mIsPowerOfTwo.ToString();
        }

        if (GUILayout.Button(textureResourceUnitIt.mReadWriteEnabled.ToString(), EditorStyles.label, GUILayout.Width(mAttributeWidth)))
        {
            OnSelectTexUnit(textureResourceUnitIt);

            mStateStr = textureResourceUnitIt.mReadWriteEnabled.ToString();
        }

        if (GUILayout.Button(textureResourceUnitIt.mMipmapEnabled.ToString(), EditorStyles.label, GUILayout.Width(mAttributeWidth)))
        {
            OnSelectTexUnit(textureResourceUnitIt);

            mStateStr = textureResourceUnitIt.mMipmapEnabled.ToString();
        }

        string memSizeStr = textureResourceUnitIt.mMemSize.ToString();

        string pattern = @"(?<=\d)(?=(\d{3})+$)";

        Regex rgx = new Regex(pattern);

        memSizeStr = rgx.Replace(memSizeStr, ",");

        if (GUILayout.Button(memSizeStr, EditorStyles.label, GUILayout.Width(mAttributeWidth)))
        {
            OnSelectTexUnit(textureResourceUnitIt);

            mStateStr = textureResourceUnitIt.mMemSize.ToString();
        }

        GUILayout.EndHorizontal();

        GUI.color = Color.white;
    }

    void UpdateSelectedTexture()
    {
        if (mCurrentTextureSelectionList.Count > 0)
        {
            {
                var __list5 = mCurrentTextureSelectionList;
                var __listCount5 = __list5.Count;
                for (int __i5 = 0; __i5 < __listCount5; ++__i5)
                {
                    var texResUnit = (TextureResourceUnit)__list5[__i5];
                    {
                        SetTexUnitInfo(texResUnit);

                        Selection.activeObject = texResUnit.mTexture;
                    }
                }
            }
        }
    }
    void AlphaTransparency()
    {
        int i = 0;
        int total = mTextureResourcesList.Count;
        if (mTextureResourcesList.Count > 0)
        {
            {
                var __list17 = mTextureResourcesList;
                var __listCount17 = __list17.Count;
                for (int __i17 = 0; __i17 < __listCount17; ++__i17)
                {
                    var data = __list17[__i17];
                    {
                        EditorUtility.DisplayProgressBar("Change Animator to Animation in Path", data.mResName, i * 1.0f / total);
                        var texResUnit = data;
                        {
                            AlphaTransparencyUnitInfo(texResUnit);
                        }
                        i++;
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }
    }

    private void AlphaTransparencyUnitInfo(TextureResourceUnit texUnit)
    {

        if (texUnit.mTextureType != null && !texUnit.mTextureType.Equals("Advanced"))
        {
            return;
        }
        var texture2Ddata =
            Resources.LoadAssetAtPath<Texture2D>(texUnit.mResPath + "/" + texUnit.mResName);
        if (texture2Ddata == null)
            return;

        if (!IsAlpha(texture2Ddata))
        {
            return;
        }

        string[] s = texUnit.mResPath.Split(new char[] { '/' });
        if (s[s.Length - 1] == "Terrain")
        {
            return;
        }
        var TextureImporterdata = AssetImporter.GetAtPath(texUnit.mResPath + "/" + texUnit.mResName) as TextureImporter;
        if (TextureImporterdata == null)
            return;

        TextureImporterdata.alphaIsTransparency = true;
        //UnityEditor.EditorUtility.SetDirty(TextureImporterdata);
        AssetDatabase.ImportAsset(texUnit.mResPath + "/" + texUnit.mResName);
    }

    private bool IsAlpha(Texture2D texture2Ddata)
    {

        var dataformat = texture2Ddata.format;
        switch (dataformat)
        {
            case TextureFormat.ASTC_RGBA_10x10:
            case TextureFormat.ASTC_RGBA_12x12:
            case TextureFormat.ASTC_RGBA_4x4:
            case TextureFormat.ASTC_RGBA_5x5:
            case TextureFormat.ASTC_RGBA_8x8:
            case TextureFormat.ASTC_RGBA_6x6:
            case TextureFormat.ATC_RGBA8:
            case TextureFormat.ATF_RGBA_JPG:
            case TextureFormat.BGRA32:
            case TextureFormat.ETC2_RGBA1:
            case TextureFormat.ETC2_RGBA8:
            case TextureFormat.PVRTC_RGB2:
            case TextureFormat.PVRTC_RGBA4:
            case TextureFormat.RGBA32:
            case TextureFormat.RGBA4444:
            case TextureFormat.DXT5:
                {
                    return true;
                }
            default:
                {
                    return false;
                }
        }

    }

    bool ExportInfo()
    {
        string path = EditorUtility.SaveFilePanel("Save Texture Info", "", "TextureInfo.txt", "txt");

        if (path.Length == 0)
        {
            mStateStr = "Save File Failed!";
            return false;
        }

        string textureInfoBufferStr = "Texture Name\tTexture Extension\tTexture Type\tTexture Format\tTexture Path\tTexture Width\tTexture Height\tTexutre Is Power Of Two\tTexture Read/Write Enabled\tTexture MipMap Enabled\tTexture MemSize(byte)";
        {
            var __list6 = mIsDisplayTextureResourcesList;
            var __listCount6 = __list6.Count;
            for (int __i6 = 0; __i6 < __listCount6; ++__i6)
            {
                var textureResourceUnitIt = (TextureResourceUnit)__list6[__i6];
                {
                    textureInfoBufferStr += "\n";

                    textureInfoBufferStr += textureResourceUnitIt.mResName;
                    textureInfoBufferStr += "\t";

                    textureInfoBufferStr += textureResourceUnitIt.mResExtension;
                    textureInfoBufferStr += "\t";

                    textureInfoBufferStr += textureResourceUnitIt.mTextureType;
                    textureInfoBufferStr += "\t";

                    textureInfoBufferStr += textureResourceUnitIt.mTextureFormat;
                    textureInfoBufferStr += "\t";

                    textureInfoBufferStr += textureResourceUnitIt.mResPath;
                    textureInfoBufferStr += "\t";

                    textureInfoBufferStr += textureResourceUnitIt.mWidth;
                    textureInfoBufferStr += "\t";

                    textureInfoBufferStr += textureResourceUnitIt.mHeight;
                    textureInfoBufferStr += "\t";

                    textureInfoBufferStr += textureResourceUnitIt.mIsPowerOfTwo.ToString();
                    textureInfoBufferStr += "\t";

                    textureInfoBufferStr += textureResourceUnitIt.mReadWriteEnabled.ToString();
                    textureInfoBufferStr += "\t";

                    textureInfoBufferStr += textureResourceUnitIt.mMipmapEnabled.ToString();
                    textureInfoBufferStr += "\t";

                    textureInfoBufferStr += textureResourceUnitIt.mMemSize.ToString();
                    textureInfoBufferStr += "\t";
                }
            }
        }
        byte[] textureInfoBytes = System.Text.Encoding.UTF8.GetBytes(textureInfoBufferStr);

        File.WriteAllBytes(path, textureInfoBytes);

        mStateStr = "Save File Success!";

        return true;
    }

    string GetAttributeStr(TextureResourceUnit textureResourceunit, int index)
    {
        switch (index)
        {
            case 0:
                return textureResourceunit.mResName;
            case 1:
                return textureResourceunit.mTextureType;
            case 2:
                return textureResourceunit.mTextureFormat;
            case 3:
                return textureResourceunit.mResPath;
            case 4:
                return textureResourceunit.mWidth.ToString();
            case 5:
                return textureResourceunit.mHeight.ToString();
            case 6:
                return textureResourceunit.mIsPowerOfTwo.ToString();
            case 7:
                return textureResourceunit.mReadWriteEnabled.ToString();
            case 8:
                return textureResourceunit.mMipmapEnabled.ToString();
            case 9:
                return textureResourceunit.mMemSize.ToString();
        }

        return null;
    }

    private void SetTexUnitInfo(TextureResourceUnit texUnit)
    {
        texUnit.ClearInfo();

        string path = texUnit.mResPath + "/" + texUnit.mResName;

        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

        texUnit.mReadWriteEnabled = textureImporter.isReadable;
        texUnit.mMipmapEnabled = textureImporter.mipmapEnabled;
        texUnit.mTextureFormat = textureImporter.textureFormat.ToString();
        mImporterType.TryGetValue(textureImporter.textureType, out texUnit.mTextureType);

        Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));

        int memSize = Profiler.GetRuntimeMemorySize(texture);

        if (texture != null)
        {
            texUnit.mWidth = Math.Min(textureImporter.maxTextureSize, texture.width);
            texUnit.mHeight = Math.Min(textureImporter.maxTextureSize, texture.height);

            if (!Mathf.IsPowerOfTwo(texUnit.mWidth) || !Mathf.IsPowerOfTwo(texUnit.mHeight))
            {
                texUnit.mIsPowerOfTwo = false;
            }

            texUnit.mTexture = texture;
            texUnit.mMemSize = memSize;
            //texUnit.mTextureFormat = texture.format.ToString( );
            Resources.UnloadAsset(texture);
        }
    }

    private TextureImporterFormat GetTexFormatWhenTypeChanged(TextureImporterFormat oldTextureImporterFormat)
    {
        switch (oldTextureImporterFormat)
        {
            case TextureImporterFormat.ARGB32:
            case TextureImporterFormat.RGB24:
            case TextureImporterFormat.Alpha8:
            case TextureImporterFormat.RGBA32:
            case TextureImporterFormat.AutomaticTruecolor:
                return TextureImporterFormat.AutomaticTruecolor;

            case TextureImporterFormat.Automatic16bit:
            case TextureImporterFormat.RGB16:
            case TextureImporterFormat.ARGB16:
            case TextureImporterFormat.RGBA16:
                return TextureImporterFormat.Automatic16bit;

            default:
                return TextureImporterFormat.AutomaticCompressed;
        }
    }

    public void OnGUI()
    {
        if (mIsShowMainInterface)
        {
            mAttributeWidth = Screen.width / (float)mTextureAttributeStrings.Length;

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Analyze All Textures"))
            {
                mTextureResourcesList.Clear();

                CollectAndAnalyzeTextures();

                mTextureResourcesList.Sort(TextureResourceUnit.SortByResName);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Search Texture's " + mTextureAttributeStrings[mSelectedAttributeString] + ": ");

            string searchName = GUILayout.TextField(mSearchName, GUILayout.Width(100));
            if (mSearchName != searchName)
            {
                mCurrentPage = mTotalPage = 1;
                mSearchName = searchName;
            }

            GUILayout.Space(10);
            if (mIsSearchCaseSensitive != GUILayout.Toggle(mIsSearchCaseSensitive, "区分大小写"))
            {
                mIsSearchCaseSensitive = !mIsSearchCaseSensitive;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            Dictionary<string, bool> tempDisplayFormats = new Dictionary<string, bool>();
            {
                // foreach(var format in mIsDisplayFormats)
                var __enumerator7 = (mIsDisplayFormats).GetEnumerator();
                while (__enumerator7.MoveNext())
                {
                    var format = (KeyValuePair<string, bool>)__enumerator7.Current;
                    {
                        if (format.Value != GUILayout.Toggle(format.Value, format.Key))
                        {
                            mCurrentPage = mTotalPage = 1;
                            tempDisplayFormats.Add(format.Key, !format.Value);
                        }
                    }
                }
            }
            {
                // foreach(var format in tempDisplayFormats)
                var __enumerator8 = (tempDisplayFormats).GetEnumerator();
                while (__enumerator8.MoveNext())
                {
                    var format = (KeyValuePair<string, bool>)__enumerator8.Current;
                    {
                        mIsDisplayFormats[format.Key] = format.Value;
                    }
                }
            }

            GUILayout.EndHorizontal();

            int currentSelectID = GUILayout.Toolbar(mSelectedAttributeString, mTextureAttributeStrings);

            if (currentSelectID != mSelectedAttributeString)
            {
                mSelectedAttributeString = currentSelectID;

                switch (mSelectedAttributeString)
                {
                    case 0:
                        mTextureResourcesList.Sort(TextureResourceUnit.SortByResName);
                        break;
                    case 1:
                        mTextureResourcesList.Sort(TextureResourceUnit.SortByTexType);
                        break;
                    case 2:
                        mTextureResourcesList.Sort(TextureResourceUnit.SortByTexFormat);
                        break;
                    case 3:
                        mTextureResourcesList.Sort(TextureResourceUnit.SortByResPath);
                        break;
                    case 4:
                        mTextureResourcesList.Sort(TextureResourceUnit.SortByTextureWidth);
                        break;
                    case 5:
                        mTextureResourcesList.Sort(TextureResourceUnit.SortByTextureHeight);
                        break;
                    case 6:
                        mTextureResourcesList.Sort(TextureResourceUnit.SortByTextureIsPowerOfTwo);
                        break;
                    case 7:
                        mTextureResourcesList.Sort(TextureResourceUnit.SortByTextureReadWriteEnabled);
                        break;
                    case 8:
                        mTextureResourcesList.Sort(TextureResourceUnit.SortByTextureMipMapEnabled);
                        break;
                    case 9:
                        mTextureResourcesList.Sort(TextureResourceUnit.SortByResMemSize);
                        break;
                }
            }

            mScrollPosition = GUILayout.BeginScrollView(mScrollPosition);

            int index = 0;
            mIsDisplayTextureResourcesList.Clear();
            {
                var __list9 = mTextureResourcesList;
                var __listCount9 = __list9.Count;
                for (int __i9 = 0; __i9 < __listCount9; ++__i9)
                {
                    var textureResourceIt = (TextureResourceUnit)__list9[__i9];
                    {
                        bool isOn = true;
                        mIsDisplayFormats.TryGetValue(textureResourceIt.mResExtension, out isOn);

                        if (!isOn)
                        {
                            continue;
                        }

                        string attributeValue = GetAttributeStr(textureResourceIt, mSelectedAttributeString);

                        if (!mIsSearchCaseSensitive)
                        {
                            attributeValue = attributeValue.ToLower();
                            mSearchName = mSearchName.ToLower();
                        }

                        if (!string.IsNullOrEmpty(mSearchName) && !attributeValue.Contains(mSearchName))
                        {
                            continue;
                        }

                        mIsDisplayTextureResourcesList.Add(textureResourceIt);

                        index++;
                        if (index >= (mCurrentPage - 1) * mTexNumPerPage && index < mCurrentPage * mTexNumPerPage)
                        {
                            DrawRow(textureResourceIt);
                        }
                    }
                }
            }
            UnityEngine.Object[] selObjects = new UnityEngine.Object[mCurrentTextureSelectionList.Count];
            for (int i = 0; i < mCurrentTextureSelectionList.Count; ++i)
            {
                selObjects[i] = mCurrentTextureSelectionList[i].mTexture;
            }

            Selection.objects = selObjects;

            GUILayout.EndScrollView();

            // Page Begin
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            mTotalPage = mIsDisplayTextureResourcesList.Count / mTexNumPerPage + 1;

            if (GUILayout.Button("Front Page"))
            {
                if (mCurrentPage > 1)
                {
                    mCurrentPage--;
                }
            }

            GUILayout.Space(10);
            GUILayout.Label(mCurrentPage.ToString() + "/" + mTotalPage.ToString());
            GUILayout.Space(10);

            if (GUILayout.Button("Next Page"))
            {
                if (mCurrentPage < mTotalPage)
                {
                    mCurrentPage++;
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            // Page End

            GUILayout.BeginHorizontal();

            GUILayout.Label("Textures Num: " + mIsDisplayTextureResourcesList.Count.ToString(), EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();
            GUILayout.Label(mStateStr, EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Update Selected Textures"))
            {
                UpdateSelectedTexture();
            }
            if (GUILayout.Button("Alpha Transparency"))
            {
                AlphaTransparency();
            }
            if (GUILayout.Button("Export Info"))
            {
                ExportInfo();
            }

            if (GUILayout.Button("Optimize"))
            {
                List<TextureResourceUnit> tempList = new List<TextureResourceUnit>();

                if (mOptionSelectedIndex == 0)
                {
                    tempList = mCurrentTextureSelectionList;
                }
                else
                {
                    tempList = mIsDisplayTextureResourcesList;
                }

                if (mIsOptimizeReadWriteEnable)
                {
                    {
                        var __list10 = tempList;
                        var __listCount10 = __list10.Count;
                        for (int __i10 = 0; __i10 < __listCount10; ++__i10)
                        {
                            var textureResourceUnitIt = (TextureResourceUnit)__list10[__i10];
                            {
                                TextureImporter textureImporter = AssetImporter.GetAtPath(textureResourceUnitIt.mResPath
                                    + "/" + textureResourceUnitIt.mResName) as TextureImporter;

                                if (textureImporter.isReadable)
                                {
                                    textureImporter.isReadable = false;
                                    textureResourceUnitIt.mResChanged = true;
                                }
                            }
                        }
                    }
                }

                if (mIsOptimizeMipMapEnable)
                {
                    {
                        var __list11 = tempList;
                        var __listCount11 = __list11.Count;
                        for (int __i11 = 0; __i11 < __listCount11; ++__i11)
                        {
                            var textureResourceUnitIt = (TextureResourceUnit)__list11[__i11];
                            {
                                TextureImporter textureImporter = AssetImporter.GetAtPath(textureResourceUnitIt.mResPath
                                    + "/" + textureResourceUnitIt.mResName) as TextureImporter;

                                if (textureImporter.mipmapEnabled)
                                {
                                    textureImporter.mipmapEnabled = false;
                                    textureResourceUnitIt.mResChanged = true;
                                }
                            }
                        }
                    }
                }

                if (mIsOptimizeType)
                {
                    {
                        var __list12 = tempList;
                        var __listCount12 = __list12.Count;
                        for (int __i12 = 0; __i12 < __listCount12; ++__i12)
                        {
                            var textureResourceUnitIt = (TextureResourceUnit)__list12[__i12];
                            {
                                var resPath = textureResourceUnitIt.mResPath + "/" + textureResourceUnitIt.mResName;
                                TextureImporter textureImporter = AssetImporter.GetAtPath(resPath)
                                    as TextureImporter;

                                if (mUITextures.Contains(resPath))
                                {
                                    var oldReadable = textureImporter.isReadable;
                                    var oldMipmap = textureImporter.mipmapEnabled;

                                    textureImporter.isReadable = true;
                                    AssetDatabase.ImportAsset(resPath);

                                    // pading and compressed
                                    var tex = Resources.LoadAssetAtPath(resPath,
                                        typeof(Texture2D)) as Texture2D;
                                    if (tex.width != tex.height || !IsPowerOfTwo(tex.width) || !IsPowerOfTwo(tex.height))
                                    {
                                        var s = GetNearestPowerOfTwo(Math.Max(tex.width, tex.height));

                                        var ext = Path.GetExtension(textureResourceUnitIt.mResName);
                                        if (ext.Equals(".png", StringComparison.InvariantCultureIgnoreCase) ||
                                            ext.Equals(".tga", StringComparison.InvariantCultureIgnoreCase))
                                        {

                                            Texture2D t = new Texture2D(s, s, TextureFormat.RGBA32, false);
                                            var ss = s * s;
                                            var texColors = new Color32[ss];
                                            for (int i = 0; i < ss; i++)
                                            {
                                                texColors[i] = Color.clear;
                                            }

                                            t.SetPixels32(texColors);
                                            t.Apply();
                                            t.SetPixels(0, s - tex.height, tex.width, tex.height,
                                                tex.GetPixels(0, 0, tex.width, tex.height));
                                            t.Apply();
                                            File.WriteAllBytes(resPath, t.EncodeToPNG());
                                            Object.DestroyImmediate(t);
                                        }
                                        else
                                        {
                                            Texture2D t = new Texture2D(s, s, TextureFormat.RGB24, false);
                                            t.SetPixels(0, s - tex.height, tex.width, tex.height,
                                                tex.GetPixels(0, 0, tex.width, tex.height));
                                            t.Apply();
                                            File.WriteAllBytes(resPath, t.EncodeToJPG());
                                            Object.DestroyImmediate(t);
                                        }

                                        Object.DestroyImmediate(tex);

                                    }

                                    textureImporter.isReadable = oldReadable;
                                    textureImporter.mipmapEnabled = oldMipmap;
                                    textureImporter.textureFormat = TextureImporterFormat.AutomaticCompressed;

                                }
                                else
                                {
                                    textureImporter.textureFormat = TextureImporterFormat.AutomaticCompressed;
                                }

                                textureResourceUnitIt.mResChanged = true;
                            }
                        }
                    }
                }

                if (mIsOptimizeFormat)
                {
                    {
                        var __list13 = tempList;
                        var __listCount13 = __list13.Count;
                        for (int __i13 = 0; __i13 < __listCount13; ++__i13)
                        {
                            var textureResourceUnitIt = (TextureResourceUnit)__list13[__i13];
                            {
                                if (textureResourceUnitIt.mTextureType == "Advanced"
                                && textureResourceUnitIt.mTextureFormat == "RGBA32")
                                {
                                    TextureImporter textureImporter = AssetImporter.GetAtPath(textureResourceUnitIt.mResPath
                                        + "/" + textureResourceUnitIt.mResName) as TextureImporter;

                                    textureImporter.textureFormat = TextureImporterFormat.DXT5;
                                    textureResourceUnitIt.mResChanged = true;
                                }
                            }
                        }
                    }
                }

                if (mIsOptimizeSize)
                {
                    {
                        var __list14 = tempList;
                        var __listCount14 = __list14.Count;
                        for (int __i14 = 0; __i14 < __listCount14; ++__i14)
                        {
                            var textureResourceUnitIt = (TextureResourceUnit)__list14[__i14];
                            {
                                if (textureResourceUnitIt.mWidth > 1024 || textureResourceUnitIt.mHeight > 1024)
                                {
                                    TextureImporter textureImporter = AssetImporter.GetAtPath(textureResourceUnitIt.mResPath
                                        + "/" + textureResourceUnitIt.mResName) as TextureImporter;

                                    textureImporter.maxTextureSize = 1024;
                                    textureImporter.textureFormat = GetTexFormatWhenTypeChanged(textureImporter.textureFormat);

                                    textureResourceUnitIt.mResChanged = true;
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < tempList.Count; ++i)
                {
                    TextureResourceUnit tempTextureResourceUnit = tempList[i];
                    if (tempTextureResourceUnit.mResChanged)
                    {
                        string texPath = tempTextureResourceUnit.mResPath + "/" + tempTextureResourceUnit.mResName;

                        //TextureImporter textureImporter = AssetImporter.GetAtPath( texPath ) as TextureImporter;

                        //TextureImporterSettings tis = new TextureImporterSettings( );
                        //textureImporter.ReadTextureSettings( tis );
                        //textureImporter.SetTextureSettings( tis );
                        AssetDatabase.ImportAsset(texPath);

                        SetTexUnitInfo(tempTextureResourceUnit);
                    }
                }
            }

            if (GUILayout.Button("Optimize Options"))
            {
                mIsShowMainInterface = false;
            }

            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Optimize Options");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("优化");

            mOptionSelectedIndex = EditorGUILayout.Popup(mOptionSelectedIndex, mOptionSelectedStr);
            GUILayout.Label("的纹理");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginVertical();

            GUILayout.Label("优化内容:");

            if (mIsOptimizeReadWriteEnable != GUILayout.Toggle(mIsOptimizeReadWriteEnable, mOptionStr[0]))
            {
                mIsOptimizeReadWriteEnable = !mIsOptimizeReadWriteEnable;
            }

            if (mIsOptimizeMipMapEnable != GUILayout.Toggle(mIsOptimizeMipMapEnable, mOptionStr[1]))
            {
                mIsOptimizeMipMapEnable = !mIsOptimizeMipMapEnable;
            }

            if (mIsOptimizeType != GUILayout.Toggle(mIsOptimizeType, mOptionStr[2]))
            {
                mIsOptimizeType = !mIsOptimizeType;
            }

            //             if (mIsOptimizeFormat != GUILayout.Toggle(mIsOptimizeFormat, mOptionStr[3]))
            //             {
            //                 mIsOptimizeFormat = !mIsOptimizeFormat;
            //             }
            // 
            //             if (mIsOptimizeSize != GUILayout.Toggle(mIsOptimizeSize, mOptionStr[4]))
            //             {
            //                 mIsOptimizeSize = !mIsOptimizeSize;
            //             }

            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Main Interface"))
            {
                mIsShowMainInterface = true;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        Resources.UnloadUnusedAssets();
    }
    bool IsPowerOfTwo(int x)
    {
        return (x & (x - 1)) == 0;
    }

    private static int[] sPowerOfTwo = { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096 };
    int GetNearestPowerOfTwo(int x)
    {
        for (int i = 0; i < sPowerOfTwo.Length; i++)
        {
            if (x <= sPowerOfTwo[i])
            {
                return sPowerOfTwo[i];
            }
        }

        throw new Exception("Texture size too big.");
    }
}