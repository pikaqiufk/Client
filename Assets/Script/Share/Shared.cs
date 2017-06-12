#region using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DataContract;
using DataTable;
using LZ4s;
using ProtoBuf;
using UnityEngine;
using Random = System.Random;

#endregion

namespace Shared
{
    public static class GlobalVariable
    {
        public static string[] ServerNames =
        {
            "Login",
            "Logic",
            "Scene",
            "Chat",
            "Rank",
            "Activity"
        };

        public static int WaitToConnectTimespan = 2000; //time in second wait after listen.
    }

    /// <summary>
    ///     标记位集合
    /// </summary>
    public class BitFlag
    {
        /// <summary>
        ///     初始化BitFlag
        /// </summary>
        /// <param name="maxCount">位数</param>
        public BitFlag(int maxCount)
        {
            mFlag = new List<int>();
            Init(maxCount);
        }

        /// <summary>
        ///     初始化BitFlag
        /// </summary>
        /// <param name="maxCount">位数</param>
        /// <param name="defaultValue">默认值，如果0则所有位都是0，如果是个数字，则每个int都是数字</param>
        public BitFlag(int maxCount, int defaultValue)
        {
            mFlag = new List<int>();
            Init(maxCount, defaultValue);
        }

        public BitFlag(int maxCount, List<int> itmes)
        {
            mCount = maxCount;
            mFlag = itmes;
            var intCount = maxCount/32;
            if (maxCount%32 > 0)
            {
                ++intCount;
            }
            var nowCount = itmes.Count;
            for (var i = 0; i != intCount; ++i)
            {
                if (nowCount <= i)
                {
                    mFlag.Add(0);
                }
                //if (defaultValue == 0 && maxCount <= 32)
                //{
                //    string tempstr = "IniFlag ";
                //    for (int j = 0; j < 32; j++)
                //    {
                //        tempstr += GetFlag(j).ToString();
                //    }
                //    Logger.Info(tempstr);
                //}
            }
        }

        private int mCount;
        private readonly List<int> mFlag;

        /// <summary>
        ///     清除第nIndex位的标记
        /// </summary>
        /// <param name="nIndex"></param>
        public void CleanFlag(int nIndex)
        {
            if (nIndex < 0 || nIndex >= mCount)
            {
                return;
            }
            mFlag[nIndex/32] &= ~(1 << (nIndex%32));
            //if (mCount <= 32)
            //{
            //    string tempstr = "ClnFlag ";
            //    for (int j = 0; j < 32; j++)
            //    {
            //        tempstr += GetFlag(j).ToString();
            //    }
            //    Logger.Info(tempstr);
            //}
        }

        /// <summary>
        ///     获得两个int的“与”操作的结果
        /// </summary>
        /// <param name="data1">参数1</param>
        /// <param name="data2">参数2</param>
        /// <returns></returns>
        public static int GetAnd(int data1, int data2)
        {
            return data1 & data2;
        }

        public List<int> GetData()
        {
            return mFlag;
        }

        /// <summary>
        ///     获取第nIndex位的标记
        /// </summary>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public int GetFlag(int nIndex)
        {
            if (nIndex < 0 || nIndex >= mCount)
            {
                return -1;
            }

            return ((mFlag[nIndex/32] >> (nIndex%32)) & 1);
        }

        /// <summary>
        ///     data的低bit位是不是1
        /// </summary>
        /// <param name="data">要取数的数据</param>
        /// <param name="bit">低几位</param>
        /// <returns></returns>
        public static bool GetLow(int data, int bit)
        {
            return Convert.ToBoolean((data >> bit) & 1);
        }

        public void Init(List<int> itmes)
        {
//用于从数据库数据初始化
            mFlag.Clear();
            mCount = itmes.Count*32;
            mFlag.AddRange(itmes);
        }

        private void Init(int maxCount, int defaultValue = 0)
        {
            mCount = maxCount;
            var intCount = maxCount/32;
            if (maxCount%32 > 0)
            {
                ++intCount;
            }
            for (var i = 0; i != intCount; ++i)
            {
                mFlag.Add(defaultValue);
                //if (defaultValue == 0 && maxCount <= 32)
                //{
                //    string tempstr = "IniFlag ";
                //    for (int j = 0; j < 32; j++)
                //    {
                //        tempstr += GetFlag(j).ToString();
                //    }
                //    Logger.Info(tempstr);
                //}
            }
        }

        public static int IntSetFlag(int data, int nIndex, bool value = true)
        {
            if (nIndex < 0 || nIndex >= 32)
            {
                return data;
            }
            if (value)
            {
                data |= 1 << nIndex;
            }
            else
            {
                data &= ~(1 << nIndex);
            }
            return data;
        }

        public bool IsDirty()
        {
            {
                var __list1 = mFlag;
                var __listCount1 = __list1.Count;
                for (var __i1 = 0; __i1 < __listCount1; ++__i1)
                {
                    var i = __list1[__i1];
                    {
                        if (i != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        ///     清除所有标记
        /// </summary>
        public void ReSetAllFlag(bool dirty = false)
        {
            var mFlagCount0 = mFlag.Count;
            for (var i = 0; i < mFlagCount0; i++)
            {
                if (dirty)
                {
                    mFlag[i] = -1;
                }
                else
                {
                    mFlag[i] = 0;
                }
            }
        }

        /// <summary>
        ///     设置第nIndex位的标记
        /// </summary>
        /// <param name="nIndex"></param>
        public void SetFlag(int nIndex)
        {
            if (nIndex < 0 || nIndex >= mCount)
            {
                return;
            }
            mFlag[nIndex/32] |= 1 << (nIndex%32);

            //if (mCount <= 32)
            //{
            //    string tempstr = "SetFlag ";
            //    for (int j = 0; j < 32; j++)
            //    {
            //        tempstr += GetFlag(j).ToString();
            //    }
            //    Logger.Info(tempstr);
            //}
        }
    }


    //通用判断 100000+职业*100000+部位*1000+颜色*100+INT（装备等级/5））
    public static class CheckGeneral
    {
        /// <summary>
        ///     判断物品类型
        /// </summary>
        /// <param name="nId">物品ID</param>
        /// <param name="it"></param>
        /// <returns></returns>
        public static bool CheckItemType(int nId, eItemType it)
        {
            if (GetItemType(nId) == it)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///     获得物品类型
        /// </summary>
        /// <param name="nId"></param>
        /// <returns></returns>
        public static eItemType GetItemType(int nId)
        {
            if (nId >= 0 && nId < 10000)
            {
                return eItemType.Resources;
            }
            if (nId >= 20000 && nId < 30000)
            {
                return eItemType.BaseItem;
            }
            if (nId >= 30000 && nId < 40000)
            {
                return eItemType.Piece;
            }
            if (nId >= 40000 && nId < 50000)
            {
                return eItemType.Mission;
            }
            //if (nId >= 100000) 
            return eItemType.Equip;
            //return eItemType.Error;
        }
    }


    public static class VectorExtension
    {
        public static float Cross(this Vector2 A, Vector2 B)
        {
            return A.x*B.y - A.y*B.x;
        }

        public static float Dot(this Vector2 A, Vector2 B)
        {
            return A.x*B.x + A.y*B.y;
        }

        public static float GetAngle(this Vector2 A, Vector2 B)
        {
            // |A·B| = |A| |B| COS(θ)
            // |A×B| = |A| |B| SIN(θ)

            return Mathf.Atan2(Cross(A, B), Dot(A, B));
        }
    }

    //随机数
    public static class MyRandom
    {
        private static readonly Random r = new Random();

        public static int Random(int i)
        {
            return r.Next(i);
        }

        public static double Random()
        {
            return r.NextDouble();
        }

        public static int Random(int _min, int _max)
        {
            if (_min == _max)
            {
                return r.Next(_min, _max);
            }
            return r.Next(_min, _max + 1);
        }
    }

    public static class ListExtension
    {
        /// <summary>
        ///     取交集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a_list"></param>
        /// <param name="b_list"></param>
        /// <returns></returns>
        public static List<T> And<T>(this List<T> a_list, List<T> b_list)
        {
            var temp = new List<T>();
            var a_listCount1 = a_list.Count;
            for (var i = 0; i < a_listCount1; i++)
            {
                if (b_list.Contains(a_list[i]))
                {
                    temp.Add(a_list[i]);
                }
            }
            return temp;
        }

        /// <summary>
        ///     取交集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="b_list"></param>
        /// <param name="a_list"></param>
        /// <returns></returns>
        public static List<T> And<T>(this List<T> b_list, T[] a_list)
        {
            var temp = new List<T>();
            var a_listLength2 = a_list.Length;
            for (var i = 0; i < a_listLength2; i++)
            {
                if (b_list.Contains(a_list[i]))
                {
                    temp.Add(a_list[i]);
                }
            }
            return temp;
        }

        /// <summary>
        ///     用List的数据集生成字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static String GetDataString<T>(this List<T> list)
        {
            var strRet = "<";
            {
                var __list3 = list;
                var __listCount3 = __list3.Count;
                for (var __i3 = 0; __i3 < __listCount3; ++__i3)
                {
                    var iter = __list3[__i3];
                    {
                        strRet = strRet + iter + ",";
                    }
                }
            }
            strRet = strRet + ">";
            return strRet;
        }

        /// <summary>
        ///     获得List某个值的数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetValueCount<T>(this List<T> list, T key)
        {
            var comparer = EqualityComparer<T>.Default;
            return list.Count(a => comparer.Equals(a, key));
        }

        //往一个字典中增加某项的值
        public static int modifyValue<T>(this Dictionary<T, int> list, T key, int modifyValue)
        {
            int oldValue;
            if (list.TryGetValue(key, out oldValue))
            {
                oldValue = oldValue + modifyValue;
            }
            else
            {
                oldValue = modifyValue;
            }
            list[key] = oldValue;
            return oldValue;
        }

        /// <summary>
        ///     从[0,maxcount-1]中取随机几个[x0,x1...]
        /// </summary>
        /// <param name="maxcount"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static List<int> Random(int maxcount, int count)
        {
            var templist = new List<int>();
            if (count == 1)
            {
                templist.Add(MyRandom.Random(templist.Count));
                return templist;
            }
            for (var i = 0; i != maxcount; i++)
            {
                templist.Add(i);
            }
            if (maxcount <= count)
            {
                return templist;
            }
            var result = new List<int>();
            for (var i = 0; i != count; i++)
            {
                var r = MyRandom.Random(templist.Count);
                result.Add(templist[r]);
                templist.RemoveAt(r);
            }
            return result;
        }

        /// <summary>
        ///     从列表中的某个索引后，随机取count个数量，不改变原始List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="l"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<T> RandRange<T>(this List<T> l, int startIndex, int count)
        {
            var tempObjList = new List<T>();
            var tempIndex = Random(l.Count - startIndex, count);
            {
                var __list2 = tempIndex;
                var __listCount2 = __list2.Count;
                for (var __i2 = 0; __i2 < __listCount2; ++__i2)
                {
                    var i = __list2[__i2];
                    {
                        tempObjList.Add(l[i + startIndex]);
                    }
                }
            }
            return tempObjList;
        }

        /// <summary>
        ///     随机一个元素出来
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="l"></param>
        /// <returns></returns>
        public static T Range<T>(this List<T> l)
        {
            if (l == null)
            {
                throw new ArgumentNullException();
            }
            if (l.Count == 0)
            {
                throw new Exception("List is empty.");
            }

            return l[MyRandom.Random(l.Count)];
        }
    }

    public static class DictionayExtension
    {
        /// <summary>
        ///     随机一个迭代器
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static KeyValuePair<TKey, TValue> Random<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            if (dict == null)
            {
                throw new ArgumentNullException();
            }

            var index = MyRandom.Random(dict.Count);
            {
                // foreach(var value in dict)
                var __enumerator4 = (dict).GetEnumerator();
                while (__enumerator4.MoveNext())
                {
                    var value = __enumerator4.Current;
                    {
                        if (index == 0)
                        {
                            return value;
                        }

                        index--;
                    }
                }
            }

            throw new Exception("Dictionay is empty.");
        }
    }

    public static class StringExtension
    {
        public static bool CheckChatStr(string desc)
        {
            if (GetStringLength(desc) > 80)
            {
                return false;
            }
            return true;
        }

        public static string CheckSensitive(this string desc)
        {
            if (string.IsNullOrEmpty(desc))
            {
                return desc;
            }

            Table.ForeachSensitiveWord(recoard =>
            {
                desc = desc.Replace(recoard.Name, "****");
                return true;
            });

            return desc;
        }

        public static string GetSensitiveString(int count)
        {
            var ret = "";
            for (var i = 0; i < count; i++)
            {
                ret += "*";
            }
            return ret;
        }

        private static byte[] decodeBuffer = new byte[1024 * 32];
        public static int GetStringLength(this string desc)
        {
            if (string.IsNullOrEmpty(desc))
            {
                return 0;
            }
            if (desc.Length > 1000)
            {
                return 1000;
            }
            //去除聊私聊名字的长度，不能当成内容名字
            var nend1 = desc.IndexOf("/", 0, StringComparison.Ordinal);
            if (nend1 == 0)
            {
                var nend2 = desc.IndexOf(" ", 0, StringComparison.Ordinal);
                if (nend2 != -1)
                {
                    desc = desc.Substring(nend2 + 1, desc.Length - nend2 - 1);
                }
            }


            var nend = 0;
            var nbegin = 0;
            var token1 = "{!";
            var token2 = "!}";
            var result_str = "";
            var addCount = 0;
            while (nend != -1)
            {
                nend = desc.IndexOf(token1, nbegin, StringComparison.Ordinal);
                if (nend == -1)
                {
                    var temp1 = desc.Substring(nbegin, desc.Length - nbegin);
                    result_str = result_str + temp1;
                }
                else
                {
//拼接井号前的字符
                    var temp3 = desc.Substring(nbegin, nend - nbegin);
                    result_str = result_str + temp3;
                    nbegin = nend + token1.Length;
                    //查询结束符
                    var findend = true;
                    var midstr = "";
                    while (findend)
                    {
                        nend = desc.IndexOf(token2, nbegin, StringComparison.Ordinal);
                        if (nend == -1)
                        {
//没有找到
                            var temp1 = desc.Substring(nbegin, desc.Length - nbegin);
                            result_str = result_str + midstr + temp1;

                            result_str = Regex.Replace(result_str, @"[[][A-Fa-f0-9]{6}[]]", "");
                            result_str = result_str.Replace("[-]", "");
                            //Regex.Replace(result_str, @"^[[][A-Fa-f0-9]{6}[]]$", "");

                            var len = result_str.Length + addCount;
                            return len;
                        }
                        var temp2 = desc.Substring(nbegin, nend - nbegin);
                        midstr = midstr + temp2;

                        var bytes = Convert.FromBase64String(midstr);
                        var unwrapped = LZ4Codec.Decode32(bytes, 0, bytes.Length, decodeBuffer, 0, decodeBuffer.Length, false);

                        ChatInfoNodeData data = null;
                        using (var ms = new MemoryStream(decodeBuffer, 0, unwrapped, false))
                        {
                            data = Serializer.Deserialize<ChatInfoNodeData>(ms);
                        }

                        if (data == null)
                        {
                            addCount += midstr.Length;
                        }
                        else
                        {
                            switch ((eChatLinkType) data.Type)
                            {
                                case eChatLinkType.Face:
                                {
                                    addCount += 2;
                                }
                                    break;
                                case eChatLinkType.Postion:
                                {
                                    addCount += 4;
                                    var tbScene = Table.GetScene(data.ExData[0]);
                                    if (tbScene != null)
                                    {
                                        addCount += tbScene.Name.Length;
                                    }
                                    var x = data.ExData[1]/100;
                                    var y = data.ExData[2]/100;
                                    addCount += x.ToString().Length;
                                    addCount += y.ToString().Length;
                                }
                                    break;
                                case eChatLinkType.Equip:
                                {
                                    var tbItem = Table.GetItemBase(data.Id);
                                    if (tbItem != null)
                                    {
                                        addCount += 2;
                                        addCount += tbItem.Name.Length;
                                    }
                                }
                                    break;
                                case eChatLinkType.Dictionary:
                                {
                                    var str = GameUtils.GetDictionaryText(data.Id);
                                    addCount += str.Length;
                                }
                                    break;
                            }
                        }
                        findend = false;
                        nbegin = nend + token2.Length;
                    }
                }
            }
            result_str = Regex.Replace(result_str, @"[[][A-Fa-f0-9]{6}[]]", "");
            result_str = result_str.Replace("[-]", "");
            var len1 = result_str.Length + addCount;
            return len1;
        }

        public static string RemoveColorFalg(this string desc)
        {
            desc = Regex.Replace(desc, @"[[][A-Fa-f0-9]{6}[]]", "");
            desc = desc.Replace("[-]", "");
            return desc;
        }

        public static string SubStringLength(this string desc, int count)
        {
            if (string.IsNullOrEmpty(desc))
            {
                return "";
            }

            var nbegin = 0;
            var nend = 0;
            var token1 = "{!";
            var token2 = "!}";


            var resultStr = "";

            var realCount = 0;
            var color = @"[[][A-Fa-f0-9]{6}[]]";

            var regex = new Regex(color);

            var falgCount = 0;
            while (falgCount < desc.Length)
            {
                if (realCount >= count)
                {
                    break;
                }

                if (desc.Length >= falgCount + 8)
                {
//[000000]
                    var s = desc.Substring(falgCount, 8);
                    if (regex.IsMatch(s))
                    {
                        resultStr += s;
                        falgCount += 8;
                        continue;
                    }
                }

                if (desc.Length >= falgCount + 3)
                {
//[-]
                    var s = desc.Substring(falgCount, 3);
                    if (s == "[-]")
                    {
                        resultStr += s;
                        falgCount += 3;
                        continue;
                    }
                }

                if (desc.Length >= falgCount + 2)
                {
//"{!"
                    var s = desc.Substring(falgCount, 2);
                    if (s == token1)
                    {
                        nend = desc.IndexOf(token2, falgCount, StringComparison.Ordinal);
                        if (nend != -1)
                        {
                            var temp = desc.Substring(falgCount + token1.Length, nend - falgCount - token2.Length);

                            var bytes = Convert.FromBase64String(temp);
                            var unwrapped = LZ4Codec.Decode32(bytes, 0, bytes.Length, decodeBuffer, 0, decodeBuffer.Length, false);

                            ChatInfoNodeData data = null;
                            using (var ms = new MemoryStream(decodeBuffer, 0, unwrapped, false))
                            {
                                data = Serializer.Deserialize<ChatInfoNodeData>(ms);
                            }

                            if (data == null)
                            {
                                if (realCount + 2 > count)
                                {
                                    break;
                                }

                                resultStr += s;
                                falgCount += 2;
                                realCount += 2;
                            }
                            else
                            {
                                var addCount = 0;
                                switch ((eChatLinkType) data.Type)
                                {
                                    case eChatLinkType.Face:
                                    {
                                        addCount += 2;
                                    }
                                        break;
                                    case eChatLinkType.Postion:
                                    {
                                        addCount += 4;
                                        var tbScene = Table.GetScene(data.ExData[0]);
                                        if (tbScene != null)
                                        {
                                            addCount += tbScene.Name.Length;
                                        }
                                        var x = data.ExData[1]/100;
                                        var y = data.ExData[2]/100;
                                        addCount += x.ToString().Length;
                                        addCount += y.ToString().Length;
                                    }
                                        break;
                                    case eChatLinkType.Equip:
                                    {
                                        var tbItem = Table.GetItemBase(data.Id);
                                        if (tbItem != null)
                                        {
                                            addCount += 2;
                                            addCount += tbItem.Name.Length;
                                        }
                                    }
                                        break;
                                    case eChatLinkType.Dictionary:
                                    {
                                    }
                                        break;
                                }

                                if (realCount + addCount > count)
                                {
                                    break;
                                }
                                realCount += addCount;
                                falgCount = nend + token2.Length;
                                resultStr += (token1 + temp + token2);
                            }
                            continue;
                        }

                        if (realCount + 2 >= count)
                        {
                            break;
                        }

                        resultStr += s;
                        falgCount += 2;
                        realCount += 2;
                        continue;
                    }
                }

                resultStr += desc[falgCount];
                falgCount ++;
                realCount ++;
            }

            return resultStr;
        }

        public static int ToInt(this string str)
        {
            int ret;
            if (!string.IsNullOrEmpty(str) && Int32.TryParse(str, out ret))
            {
                return ret;
            }
            return -1;
        }

        public static ulong ToUlong(this string str)
        {
            ulong ret;
            if (!string.IsNullOrEmpty(str) && ulong.TryParse(str, out ret))
            {
                return ret;
            }
            return ulong.MaxValue;
        }
    }

	public class OperationActivityRewardItem
	{
		public int ItemId;
		public int ItemCount;

	}

	public class ActivityNormalItem
	{
		public int ItemId;
		public string ItemName;
		public int ConditionId;
		public int FlagId;
		public List<OperationActivityRewardItem> Reward = new List<OperationActivityRewardItem>();
	}

	public class ActivityRankItem
	{
		public int Idx { get; set; }
		public ulong CharacterId { get; set; }
		public string Name { get; set; }
		public int Value { get; set; }
	}
	public class OperationActivityTypeNormal
	{
		public List<ActivityNormalItem> List = new List<ActivityNormalItem>();
	}

	public class OperationActivityTypeRank
	{
		public int RankType { get; set; }
		public List<ActivityNormalItem> List = new List<ActivityNormalItem>();
		public List<ActivityRankItem> RankList = new List<ActivityRankItem>();
	}

	public class OperationActivityTypeTable
	{
		public List<OperationActivityTypeNormal> List = new List<OperationActivityTypeNormal>();
	}

	public class OperationActivityTermData
	{
		public int ActivityId;
		public string Name;
		public string StartTime;
		public string EndTime;
		public int BkgIconId;
		public string Desc;
		public int Type;
		public OperationActivityTypeNormal ActivityNormal = new OperationActivityTypeNormal();
		public OperationActivityTypeRank ActivityRank = new OperationActivityTypeRank();
		public OperationActivityTypeTable ActivityTable = new OperationActivityTypeTable();
	}

}