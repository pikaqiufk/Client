using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum AnalyzeType
{
    AT_UNDEFINED,
    AT_SCENE,
    AT_PREFAB,
    AT_MAT,
    AT_FBX,
    AT_CS,
    AT_ASSET,
    AT_PNG, 
    AT_TGA,
    AT_JPG,
    AT_ANIM,
    AT_TIF,
    AT_JS,
    AT_SHADER,
    AT_PSD,
    AT_OGG,
    AT_WAV,
    AT_TXT,
    AT_GUISKIN,
    AT_TTF, 
    AT_MP3,
    AT_LUA

}

public enum AnalyzeFlag
{
    AF_UNFLAG     = 0,
    AF_QUEUED     = 1 << 0,
    AF_LOST       = 1 << 1
}


public class BaseResourceUnit
{
    public string mResName;
    public string mResExtension;
    public string mResPath;

    public int mMemSize;
    public bool mResChanged;

    public BaseResourceUnit ( )
    {
        mResName = mResExtension = mResPath = null;
        mMemSize = 0;
        mResChanged = false;
    }

    public BaseResourceUnit ( string resPathName )
    {
        mResName = Path.GetFileName( resPathName );

        mResPath = Path.GetDirectoryName( resPathName );

        mResExtension = Path.GetExtension( resPathName );

        mMemSize = 0;
        mResChanged = false;
    }

    public void ClearInfo ( )
    {
        mMemSize = 0;
        mResChanged = false;
    }

    public static int SortByResName ( BaseResourceUnit a, BaseResourceUnit b )
    {
        return string.Compare( a.mResName, b.mResName );
    }

    public static int SortByResPath ( BaseResourceUnit a, BaseResourceUnit b )
    {
        return string.Compare( a.mResPath, b.mResPath );
    }

    public static int SortByResExtension ( BaseResourceUnit a, BaseResourceUnit b )
    {
        return string.Compare( a.mResExtension, b.mResExtension );
    }

    public static int SortByResMemSize ( BaseResourceUnit a, BaseResourceUnit b )
    {
        if ( a.mMemSize > b.mMemSize )
        {
            return -1;
        }
        else if ( a.mMemSize < b.mMemSize )
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    //public static bool operator == ( BaseResourceUnit a, BaseResourceUnit b )
    //{
    //    if ( a.mResPath == b.mResPath && a.mResName == b.mResName )
    //    {
    //        return true;
    //    }

    //    return false;
    //}

    //public static bool operator != ( BaseResourceUnit a, BaseResourceUnit b )
    //{
    //    return !( a == b );
    //}
}

public class TextureResourceUnit : BaseResourceUnit
{
    public int mWidth, mHeight;
    public bool mReadWriteEnabled;
    public bool mMipmapEnabled;
    public bool mIsPowerOfTwo;
    public string mTextureType;

    public Texture2D mTexture;
    public string mTextureFormat;

    public TextureResourceUnit ( )
        : base()
    {
        mWidth = mHeight = 0;
        mReadWriteEnabled = false;
        mMipmapEnabled = true;
        mTexture = null;
        mTextureFormat = null;
        mIsPowerOfTwo = true;
        mTextureType = null;
    }

    public TextureResourceUnit ( string resPathName )
        : base( resPathName )
    {
        mWidth = mHeight = 0;
        mReadWriteEnabled = false;
        mMipmapEnabled = true;
        mTexture = null;
        mTextureFormat = null;
        mIsPowerOfTwo = true;
        mTextureType = null;
    }

    public void ClearInfo ( )
    {
        base.ClearInfo( );

        mWidth = mHeight = 0;
        mReadWriteEnabled = false;
        mMipmapEnabled = true;
        mTexture = null;
        mTextureFormat = null;
        mIsPowerOfTwo = true;
        mTextureType = null;
    }

    public static int SortByTextureWidth ( TextureResourceUnit a, TextureResourceUnit b )
    {
        if ( a.mWidth > b.mWidth )
        {
            return -1;
        }
        else if ( a.mWidth < b.mWidth )
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public static int SortByTextureHeight ( TextureResourceUnit a, TextureResourceUnit b )
    {
        if ( a.mHeight > b.mHeight )
        {
            return -1;
        }
        else if ( a.mHeight < b.mHeight )
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public static int SortByTextureIsPowerOfTwo ( TextureResourceUnit a, TextureResourceUnit b )
    {
        if ( a.mIsPowerOfTwo == b.mIsPowerOfTwo )
        {
            return 0;
        }

        if ( a.mIsPowerOfTwo )
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public static int SortByTextureReadWriteEnabled ( TextureResourceUnit a, TextureResourceUnit b )
    {
        if ( a.mReadWriteEnabled == b.mReadWriteEnabled )
        {
            return 0;
        }

        if ( a.mReadWriteEnabled )
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public static int SortByTextureMipMapEnabled ( TextureResourceUnit a, TextureResourceUnit b )
    {
        if ( a.mMipmapEnabled == b.mMipmapEnabled )
        {
            return 0;
        }

        if ( a.mMipmapEnabled )
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public static int SortByTexFormat ( TextureResourceUnit a, TextureResourceUnit b )
    {
        return string.Compare( a.mTextureFormat, b.mTextureFormat );
    }

    public static int SortByTexType ( TextureResourceUnit a, TextureResourceUnit b )
    {
        return string.Compare( a.mTextureType, b.mTextureType );
    }

    //public static bool operator == ( TextureResourceUnit a, TextureResourceUnit b )
    //{
    //    if ( a.mResPath == b.mResPath && a.mResName == b.mResName )
    //    {
    //        return true;
    //    }

    //    return false;
    //}

    //public static bool operator != ( TextureResourceUnit a, TextureResourceUnit b )
    //{
    //    return !( a == b );
    //}
}

public class ResourceUnit
{
    static Dictionary<string, AnalyzeType> msAnalyzeTypeMap = new Dictionary<string, AnalyzeType>();
    static bool msInitAnalyzeType = false;

    private string mFileName;
    private string mPathName;
    private int mAnalyzeType;
    private int mAnalyzeFlag;
    private int mTableValidColmn = 0;
    private string mTablePathSectorName;

    public List<ResourceUnit> mResourcesReferrd = new List<ResourceUnit>();
    public List<ResourceUnit> mResourcesInclude = new List<ResourceUnit>();

    public ResourceUnit(string fileName, string pathName, 
        int analyzeType = (int)(AnalyzeType.AT_UNDEFINED),
        int analyzeFlag = (int)(AnalyzeFlag.AF_UNFLAG))
    {
        mFileName = fileName;
        mPathName = pathName;
        mAnalyzeType = analyzeType;
        mAnalyzeFlag = analyzeFlag;

    }

    public string GetFileName()
    {
        return mFileName;
    }

    public string GetPathName()
    {
        return mPathName;
    }

    public void SetResourceAnalyzeType(int type)
    {
        mAnalyzeType = type;
    }

    public int GetResourceAnalyzeType()
    {
        return mAnalyzeType;
    }

    public void SetQueued()
    {
        mAnalyzeFlag |= (int)(AnalyzeFlag.AF_QUEUED);
    }

    public bool IsQueued()
    {
        return 0 != (mAnalyzeFlag & (int)(AnalyzeFlag.AF_QUEUED));
    }

    public void SetLost()
    {
        mAnalyzeFlag |= (int)(AnalyzeFlag.AF_LOST);
    }

    public bool IsLost()
    {
        return 0 != (mAnalyzeFlag & (int)(AnalyzeFlag.AF_LOST));
    }

    public bool IsUsed()
    {
        return 0 != (mAnalyzeFlag & (int)(AnalyzeFlag.AF_QUEUED)) || 0 != mResourcesReferrd.Count;
    }

    public void SetTableValidColmn(int col)
    {
        mTableValidColmn = col;
    }

    public int GetTableValidColmn()
    {
        return mTableValidColmn;
    }

    public void SetTablePathSectorName(string secName)
    {
        mTablePathSectorName = secName;
    }

    public string GetTablePathSectorName()
    {
        return mTablePathSectorName;
    }

    public void AddReferrence(ResourceUnit res)
    {
        if (!mResourcesReferrd.Contains(res))
        {
            mResourcesReferrd.Add(res);
        }
    }

    public void AddInclude(ResourceUnit res)
    {
        if (!mResourcesInclude.Contains(res))
        {
            mResourcesInclude.Add(res);
        }
    }

    static public int GetAnalyzeType(string extName)
    {
        if (!msInitAnalyzeType)
        {
            msAnalyzeTypeMap.Add(".unity", AnalyzeType.AT_SCENE);
            msAnalyzeTypeMap.Add(".prefab", AnalyzeType.AT_PREFAB);
            msAnalyzeTypeMap.Add(".mat", AnalyzeType.AT_MAT);
            msAnalyzeTypeMap.Add(".fbx", AnalyzeType.AT_FBX);
            msAnalyzeTypeMap.Add(".cs", AnalyzeType.AT_CS);
            msAnalyzeTypeMap.Add(".asset", AnalyzeType.AT_ASSET);
            msAnalyzeTypeMap.Add(".png", AnalyzeType.AT_PNG);
            msAnalyzeTypeMap.Add(".tga", AnalyzeType.AT_TGA);
            msAnalyzeTypeMap.Add(".jpg", AnalyzeType.AT_JPG);
            msAnalyzeTypeMap.Add(".anim", AnalyzeType.AT_ANIM);
            msAnalyzeTypeMap.Add(".tif", AnalyzeType.AT_TIF);
            msAnalyzeTypeMap.Add(".js", AnalyzeType.AT_JS);
            msAnalyzeTypeMap.Add(".shader", AnalyzeType.AT_SHADER);
            msAnalyzeTypeMap.Add(".psd", AnalyzeType.AT_PSD);
            msAnalyzeTypeMap.Add(".ogg", AnalyzeType.AT_OGG);
            msAnalyzeTypeMap.Add(".wav", AnalyzeType.AT_WAV);
            msAnalyzeTypeMap.Add(".txt", AnalyzeType.AT_TXT);
            msAnalyzeTypeMap.Add(".guiskin", AnalyzeType.AT_GUISKIN);
            msAnalyzeTypeMap.Add(".ttf", AnalyzeType.AT_TTF);
            msAnalyzeTypeMap.Add(".mp3", AnalyzeType.AT_MP3);
            msAnalyzeTypeMap.Add(".lua", AnalyzeType.AT_LUA);

            msInitAnalyzeType = true;
        }

        if (msAnalyzeTypeMap.ContainsKey(extName))
        {
            return (int)(msAnalyzeTypeMap[extName]);
        }

        return (int)(AnalyzeType.AT_UNDEFINED);

    }


}   

