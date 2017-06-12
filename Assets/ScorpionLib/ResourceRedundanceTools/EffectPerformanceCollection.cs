#if UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;



public class EffectPerformanceCollection : MonoBehaviour 
{
    public class EffectPerformanceData
    {
        public string mPathFileName;
        public int mAverageParticleNum= 0;
        public float mAverageParticleSize = 0.0f;
        public float mAverageFPS = 0.0f;

        public List<SubEffectInfo> mSubEffectInfoList = new List<SubEffectInfo>();

        public EffectPerformanceData(string pathFileName)
        {
            mPathFileName = pathFileName;
        }

        public static int SortByAverageParticleNum(EffectPerformanceData a, EffectPerformanceData b)
        {
            if (a.mAverageParticleNum > b.mAverageParticleNum)
            {
                return -1;
            }
            else if (a.mAverageParticleNum < b.mAverageParticleNum)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static int SortByAverageParticleSize(EffectPerformanceData a, EffectPerformanceData b)
        {
            if (a.mAverageParticleSize > b.mAverageParticleSize)
            {
                return -1;
            }
            else if (a.mAverageParticleSize < b.mAverageParticleSize)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static int SortByAverageFPS(EffectPerformanceData a, EffectPerformanceData b)
        {
            if (a.mAverageFPS < b.mAverageFPS)
            {
                return -1;
            }
            else if (a.mAverageFPS > b.mAverageFPS)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    public string[] mEffectPathNameArray;

    public List<EffectPerformanceData> mEffectPerformanceDataList = new List<EffectPerformanceData>();
    private bool mFinish = false;

    private const float mInterval = 20.0f;
    private float mTime = 0.0f;
    private int mIndex = 0;

	// Use this for initialization
	void Start () 
    {
        foreach (string effectPathName in mEffectPathNameArray)
        {
            EffectPerformanceData data = new EffectPerformanceData(effectPathName);
            mEffectPerformanceDataList.Add(data);
        }

        mIndex = 0;
        mFinish = false;
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        while (mTime > mInterval && mIndex <= mEffectPerformanceDataList.Count)
        {
            if (null != gameObject.GetComponent<SingleEffectPerformance>())
            {
                SingleEffectPerformance comp = gameObject.GetComponent<SingleEffectPerformance>();

                mEffectPerformanceDataList[mIndex-1].mAverageParticleNum = comp.GetAverageParticleNum();
                mEffectPerformanceDataList[mIndex-1].mAverageParticleSize = comp.GetAverageParticleSize();
                mEffectPerformanceDataList[mIndex-1].mAverageFPS = comp.GetAverageFPS();

                List<SubEffectInfo> infos = comp.GetSubEffectInfoList();
                for (int i = 0; i < infos.Count; ++i)
                {
                    SubEffectInfo info = new SubEffectInfo();
                    info.CopyFrom(infos[i]);
                    mEffectPerformanceDataList[mIndex - 1].mSubEffectInfoList.Add(info);
                }

                comp.DestroyGameObjects();

                GameObject.DestroyImmediate(comp, true);
                System.GC.Collect();

                //Debug.Log("Average FPS : " + comp.GetAverageFPS());
                
            }

            if (mIndex == mEffectPerformanceDataList.Count)
            {
                ++mIndex;
                break;
            }

            SingleEffectPerformance sep = gameObject.AddComponent<SingleEffectPerformance>();
            sep.mEffectPrefabPathName = mEffectPerformanceDataList[mIndex].mPathFileName;

            mTime = 0.0f;
            ++mIndex;

        }

        if (mIndex > mEffectPerformanceDataList.Count)
        {
            mFinish = true;
        }

        mTime += Time.deltaTime;
	}

    public bool GetFinish()
    {
        return mFinish;
    }
}


#endif