#region using

using System.IO;
using UnityEngine;

#endregion

public class TerrainManager
{
    public TerrainManager()
    {
        RawDataOK = false;
    }

    private float m_fTerrainLenRate = 1.0f; //地图真是x坐标在高度图中的位置换算比例
    private float m_fTerrainWidRate = 1.0f; //地图真是z坐标在高度图中的位置换算比例
    private int m_nTerrainHeightMapLength; //地形长度
    private int m_nTerrainHeightMapWidth; //地形宽度
    private int m_nTerrainHeightMax; //地形高度最大值
    private byte[,] m_TerrianRawData;
    public bool RawDataOK { get; private set; }

    public float GetTerrianHeight(Vector3 pos)
    {
        if (m_TerrianRawData == null)
        {
            return 0;
        }

        var nX = (int) (pos.x*m_fTerrainLenRate);
        var nZ = (int) (pos.z*m_fTerrainWidRate);
        if (nX < 0 || nX > m_nTerrainHeightMapLength || nZ < 0 || nZ > m_nTerrainHeightMapWidth)
        {
            return 0;
        }

        //8bit灰度图可表示数值0~255,对应高度具体数值的0~m_nTerrainHeightMax
        //所以根据所在灰度图区间来映射到对应高度
        return ((float) (m_TerrianRawData[nX, nZ])/255)*m_nTerrainHeightMax;
    }

    public float GetTerrianHeightSample(Vector3 pos)
    {
        if (m_TerrianRawData == null)
        {
            return 0;
        }

        var nX = (int) (pos.x*m_fTerrainLenRate);
        var nZ = (int) (pos.z*m_fTerrainWidRate);
        var nX1 = nX + 1;
        var nZ1 = nZ + 1;
        if (nX < 0 || nX > m_nTerrainHeightMapLength || nZ < 0 || nZ > m_nTerrainHeightMapWidth)
        {
            return 0;
        }

        if (nX1 > m_nTerrainHeightMapLength)
        {
            nX1 = nX;
        }

        if (nZ1 > m_nTerrainHeightMapWidth)
        {
            nZ1 = nZ;
        }

        var LeftBottomHeight = ((float) (m_TerrianRawData[nX, nZ])/255)*m_nTerrainHeightMax;
        var LeftTopHeight = ((float) (m_TerrianRawData[nX, nZ1])/255)*m_nTerrainHeightMax;
        var RightBottomHeight = ((float) (m_TerrianRawData[nX1, nZ])/255)*m_nTerrainHeightMax;
        var RightTopHeight = ((float) (m_TerrianRawData[nX1, nZ1])/255)*m_nTerrainHeightMax;
        var retValue = LeftBottomHeight;

        if (nX1 - pos.x > pos.z - nZ)
        {
            if (nX1 == pos.x)
            {
                retValue = LeftBottomHeight + (LeftTopHeight - LeftBottomHeight)*(pos.z - nZ);
            }
            else
            {
                var k = (nZ1 - pos.z)/(nX1 - pos.x);
                var b = nZ1 - k*nX1;
                var nZTarget = k*nX + b;
                var precent = nZTarget - nZ;
                retValue = LeftBottomHeight + (LeftTopHeight - LeftBottomHeight)*precent;
            }
        }
        else
        {
            if (pos.x == nX)
            {
                retValue = RightBottomHeight + (RightTopHeight - RightBottomHeight)*(pos.z - nZ);
            }
            else
            {
                var k = (pos.z - nZ)/(pos.x - nX);
                var b = nZ - k*nX;
                var nZTarget = k*nX1 + b;
                var precent = nZTarget - nZ;
                retValue = RightBottomHeight + (RightTopHeight - RightBottomHeight)*precent;
            }
        }
        return retValue;
    }

    //从地形Raw文件读取地形高度数据
    public bool InitTerrianData(string path,
                                int nHeightMapLength,
                                int nHeightMapWidth,
                                int nHeightMax,
                                int nMapRealLength,
                                int nMapRealWidth)
    {
        //如果m_TerrianRawData不为null
        m_TerrianRawData = null;
        RawDataOK = false;

        //只能初始化一次，所以RawData必须为空
        //if (nHeightMapLength <= 0 || nHeightMapWidth <= 0 || m_TerrianRawData != null)
        if (nHeightMapLength <= 0 || nHeightMapWidth <= 0)
        {
            //数据发生错误，重新获取数据
            return false;
        }

        m_nTerrainHeightMapLength = nHeightMapLength;
        m_nTerrainHeightMapWidth = nHeightMapWidth;
        m_nTerrainHeightMax = nHeightMax;

        //得到长和宽的换算比例
        if (nMapRealLength > 0)
        {
            m_fTerrainLenRate = ((float) m_nTerrainHeightMapLength)/nMapRealLength;
        }
        if (nMapRealWidth > 0)
        {
            m_fTerrainWidRate = ((float) m_nTerrainHeightMapWidth)/nMapRealWidth;
        }

        m_TerrianRawData = new byte[m_nTerrainHeightMapLength, m_nTerrainHeightMapWidth];
#if UNITY_ANDROID && !UNITY_EDITOR
//         GameManager.gameManager.rawDataCallback = null;
//         // Raw 资源加载.回调方法
//         GameManager.gameManager.rawDataCallback = RawDataComplete;
//         GameManager.gameManager.GetRawData(path);
#else
        return IOSAndPCReadRaw(path);
#endif

        return true;
    }

    private bool IOSAndPCReadRaw(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                Logger.Info("Can't Open Raw File " + path);
                return false;
            }

            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var nSeek = 0;
            var byteData = new byte[1];
            //读数据
            for (var lenIdx = 0; lenIdx < m_nTerrainHeightMapLength; ++lenIdx)
            {
                for (var widIdx = 0; widIdx < m_nTerrainHeightMapWidth; ++widIdx)
                {
                    fileStream.Seek(nSeek, SeekOrigin.Begin);
                    fileStream.Read(byteData, 0, 1);
                    nSeek++;

                    //写入short
                    m_TerrianRawData[widIdx, lenIdx] = byteData[0];
                }
            }

            RawDataOK = true;
            fileStream.Close();
        }
        catch (IOException e)
        {
            Logger.Info("An IO exception has been thrown!");
            Logger.Info(e.ToString());

            return false;
        }

        return true;
    }

    public void RawDataComplete(byte[] bytes)
    {
        //GameManager.gameManager.rawDataCallback -= RawDataComplete;

        if (bytes == null || bytes.Length <= 0)
        {
            //数据发生失败后，重新提交申请数据
            return;
        }

        var nSeek = 0;
        //byte[] byteData = new byte[1];
        //读数据
        for (var lenIdx = 0; lenIdx < m_nTerrainHeightMapLength; ++lenIdx)
        {
            nSeek = lenIdx*m_nTerrainHeightMapWidth;

            for (var widIdx = 0; widIdx < m_nTerrainHeightMapWidth; ++widIdx)
            {
                //写入short
                m_TerrianRawData[widIdx, lenIdx] = bytes[nSeek + widIdx];
            }
        }

        RawDataOK = true;
    }
}