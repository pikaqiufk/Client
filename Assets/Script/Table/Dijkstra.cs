
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DataTable
{
    public class Coordinate
    {
        public int nId;
        public int SceneId;
        public float PosX;
        public float PosY;

        public Coordinate()
        {
            nId = Dijkstra.GetNextId();
        }
        public Coordinate(bool flag)
        {
        }
    }
    public static class Dijkstra
    {
        private static int id = 0;
        public static Dictionary<int, Coordinate> Points = new Dictionary<int, Coordinate>();           //id , Coordinate坐标
        //public static Dictionary<int, List<int>> ScenePoint = new Dictionary<int, List<int>>();         //Scene , List<int>(Id)
        public static Dictionary<int, double> Distances = new Dictionary<int, double>();                //from * 10000 + to , distance  点与点的到达时间
        public static Dictionary<int, List<int>> MoveList = new Dictionary<int, List<int>>();           //from * 10000 + to , moveList
        private static Dictionary<int, Dictionary<int, List<int>>> Wayss = new Dictionary<int, Dictionary<int, List<int>>>(); //fromid,(toid,list<pointid>)
        //初始化
        public static void Init()
        {
            Points.Clear();
            Distances.Clear();
            Table.ForeachTransfer(record =>
            {
                Coordinate f = new Coordinate() { SceneId = record.FromSceneId, PosX = record.FromX, PosY = record.FromY };
                Coordinate t = new Coordinate() { SceneId = record.ToSceneId, PosX = record.ToX, PosY = record.ToY };
                PushCoordinate(f);
                PushCoordinate(t);
                AddWay(f.nId, t.nId, record.NeedTime);
                return true;
            });
        }
        //查找路线
        public static List<Coordinate> FindWay(int FromSceneId, float FromPosX, float FromPoxY, int ToSceneId,
            float ToPosX, float ToPoxY)
        {
            Coordinate f = new Coordinate() { SceneId = FromSceneId, PosX = FromPosX, PosY = FromPoxY };
            Coordinate t = new Coordinate() { SceneId = ToSceneId, PosX = ToPosX, PosY = ToPoxY };
            List<Coordinate> r = FindWay(f, t);
            RemoveCoordinate(f.nId);
            RemoveCoordinate(t.nId);
            MoveList.Clear();
            Wayss.Remove(f.nId);
            id -= 2;
            return r;
        }
        //查找路线
        public static List<Coordinate> FindWay(int fromid, int toid)
        {
            List<Coordinate> newList = new List<Coordinate>();
            int key = GetKey(fromid, toid);
            List<int> temp;
            if (MoveList.TryGetValue(key, out temp))
            {
            }
            else
            {
                FindWay(fromid);
                temp = MoveList[key];
                //foreach (int i in MoveList[key])
                //{
                //    newList.Add(Points[i]);
                //}
            }
            {
                var __list1 = temp;
                var __listCount1 = __list1.Count;
                for (int __i1 = 0; __i1 < __listCount1; ++__i1)
                {
                    var i = (int)__list1[__i1];
                    {
                        newList.Add(Points[i]);
                    }
                }
            }            //Display(newList);
            return newList;
        }

        #region 不需要要看的
        //移除某个点
        private static void RemoveCoordinate(int id)
        {
            Coordinate remove;
            if (!Points.TryGetValue(id, out remove))
            {
                return;
            }
            List<int> removeList = new List<int>();
            {
                // foreach(var d in Distances)
                var __enumerator2 = (Distances).GetEnumerator();
                while (__enumerator2.MoveNext())
                {
                    var d = __enumerator2.Current;
                    {
                        if (d.Key / 10000 == id || d.Key % 10000 == id)
                        {
                            removeList.Add(d.Key);
                        }
                    }
                }
            }
            {
                var __list3 = removeList;
                var __listCount3 = __list3.Count;
                for (int __i3 = 0; __i3 < __listCount3; ++__i3)
                {
                    var i = (int)__list3[__i3];
                    {
                        Distances.Remove(i);
                    }
                }
            }
            removeList.Clear();
            //List<int> temp;
            //if (ScenePoint.TryGetValue(remove.SceneId, out temp))
            //{
            //    temp.Remove(id);
            //}
            Points.Remove(id);
        }
        public static int GetNextId()
        {
            return id++;
        }
        private static void Display(List<Coordinate> way)
        {
            Coordinate p0 = way[0];
            Coordinate pn = way[way.Count - 1];
            Console.WriteLine("最短路径如下: V{0} -> V{1} ====={2}", p0.nId, pn.nId, Distances[GetKey(p0.nId, pn.nId)]);

            for (int i = 0; i != way.Count; ++i)
            {
                Coordinate coordinate = way[i];
                if (i == way.Count - 1)
                {
                    Console.WriteLine("V{0} -- {1}:[{2},{3}]", coordinate.nId, coordinate.SceneId, coordinate.PosX, coordinate.PosY);
                }
                else
                {
                    double dd1 = GetDis(coordinate.nId, way[i + 1].nId);
                    //double dd2 = Distances[GetKey(coordinate.nId, way[i + 1].nId)];
                    Console.WriteLine("V{0} -- {1}:[{2},{3}] ====={4}", coordinate.nId, coordinate.SceneId, coordinate.PosX, coordinate.PosY, dd1);
                }
            }
            //foreach (Coordinate coordinate in way)
            //{
            //    Console.WriteLine("V{0} -- {1}:[{2},{3}]", coordinate.nId, coordinate.SceneId, coordinate.PosX, coordinate.PosY);
            //}
        }
        private static void Display(int from)
        {
            //------中心到各点的最短路径----------
            Console.WriteLine("中心到各点的最短路径如下: \n\n");
            int sum_d_index = 0;
            {
                // foreach(var mother in Wayss[from])
                var __enumerator4 = (Wayss[from]).GetEnumerator();
                while (__enumerator4.MoveNext())
                {
                    var mother = __enumerator4.Current;
                    {
                        int index = 0;
                        {
                            var __list8 = mother.Value;
                            var __listCount8 = __list8.Count;
                            for (int __i8 = 0; __i8 < __listCount8; ++__i8)
                            {
                                var child = (int)__list8[__i8];
                                {
                                    Console.Write("V{0} - ", child);
                                    index++;
                                }
                            }
                        }
                        Console.WriteLine("    路径长 {0}", GetDis(from, sum_d_index));
                        sum_d_index++;
                    }
                }
            }
        }

        private static int GetKey(int fromId, int toid)
        {
            return fromId * 10000 + toid;
        }

        private static Coordinate GetCoordinate(int id)
        {
            Coordinate coordinate;
            if (Points.TryGetValue(id, out coordinate))
            {
                return coordinate;
            }
            return null;
        }

        private static void SetDis(int fromId, int toId, double dis)
        {
            int key = GetKey(fromId, toId);
            double Olddis = GetDis(fromId, toId);
            if (Olddis > dis)
            {
                Distances[key] = dis;
            }
        }
        private static double GetDis(int fromId, int toId)
        {
            int key = GetKey(fromId, toId);
            double dis;
            if (!Distances.TryGetValue(key, out dis))
            {
                Coordinate f = GetCoordinate(fromId);
                if (f == null)
                {
                    return int.MaxValue;
                }
                Coordinate t = GetCoordinate(toId);
                if (t == null)
                {
                    return int.MaxValue;
                }
                if (f.SceneId == t.SceneId)
                {
                    double distance = (f.PosX - t.PosX) * (f.PosX - t.PosX) + (f.PosY - t.PosY) * (f.PosY - t.PosY);
                    distance = Math.Sqrt(distance);
                    distance = distance / 4;
                    dis = distance + 0.01;
                    Distances[key] = dis;
                    Distances[GetKey(toId, fromId)] = dis;
                    return dis;
                }
                else
                {
                    return int.MaxValue;
                }
            }
            return dis;
        }
        //私有方法
        private static void WaysIndexClear(int fromId)
        {
            Dictionary<int, List<int>> fromWays = GetWays(fromId);
            fromWays.Clear();
        }

        private static void WayClear(int fromId, int toId)
        {
            List<int> Way = GetWay(fromId, toId);
            Way.Clear();
        }
        private static Dictionary<int, List<int>> GetWays(int fromId)
        {
            Dictionary<int, List<int>> temp;
            if (!Wayss.TryGetValue(fromId, out temp))
            {
                temp = new Dictionary<int, List<int>>();
                Wayss[fromId] = temp;
            }
            return temp;
        }

        private static List<int> GetWay(int fromId, int toId)
        {
            Dictionary<int, List<int>> fromWays = GetWays(fromId);
            List<int> temp;
            if (!fromWays.TryGetValue(toId, out temp))
            {
                temp = new List<int>();
                fromWays[toId] = temp;
            }
            return temp;
        }
        private static void WaysAdd(int fromId, int toId, int addvalue)
        {
            List<int> Way = GetWay(fromId, toId);
            Way.Add(addvalue);
        }
        //增加路线
        private static void AddWay(int fromId, int toid, double distance)
        {
            int key = GetKey(fromId, toid);
            double nowdis;
            if (Distances.TryGetValue(key, out nowdis))
            {
                if (nowdis > distance)
                {//目前这个Key的距离比新的距离远
                    Distances[key] = distance;
                }
                else
                {
                    //为什么会传一个更大值进来呢
                }
            }
            else
            {//目前没有这个Key
                Distances[key] = distance;
            }
        }

        //添加某个点
        private static void PushCoordinate(Coordinate point)
        {
            if (Points.ContainsKey(point.nId))
            {
                return;
            }
            Points[point.nId] = point;
            //List<int> temp;
            //if (ScenePoint.TryGetValue(point.SceneId, out temp))
            //{
            //    temp.Add(point.nId);
            //}
        }

        //查找路线
        private static List<Coordinate> FindWay(Coordinate from, Coordinate to)
        {
            PushCoordinate(from);
            PushCoordinate(to);
            return FindWay(from.nId, to.nId);
        }
        //寻找路径
        private static void FindWay(int from)
        {
            WaysIndexClear(from);
            int nCount = Points.Count;
            for (int i = 0; i < nCount; i++)  //有row个点，则从中心到各点的路有row-1条
            {
                WaysAdd(from, i, from);
            }
            List<int> S = new List<int>(4);
            List<int> Sr = new List<int>(4);
            int[] Indexof_distance = new int[nCount];

            for (int i = 0; i < nCount; i++)
            {
                Indexof_distance[i] = i;
            }
            S.Add(Indexof_distance[from]);
            for (int i = 0; i < nCount; i++)
            {
                Sr.Add(Indexof_distance[i]);
            }
            Sr.RemoveAt(from);
            double[] D = new double[nCount];    //存放from点到每个点的距离

            //---------------以上已经初始化了，S和Sr(里边放的都是点的编号)------------------
            int Count = nCount - 1;
            while (Count > 0)
            {
                //假定中心点的编号是0的贪吃法求路径
                for (int i = 0; i < nCount; i++)
                {
                    D[i] = GetDis(from, i);//   this.distance[from * row + i];
                }

                int min_num = (int)Sr[0];  //距中心点的最小距离点编号
                double mindis = D[min_num];
                {
                    var __list5 = Sr;
                    var __listCount5 = __list5.Count;
                    for (int __i5 = 0; __i5 < __listCount5; ++__i5)
                    {
                        var s = (int)__list5[__i5];
                        {
                            if (mindis < 0)
                            {
                                min_num = s;
                                mindis = D[s];
                                continue;
                            }
                            if (D[s] < 0)
                            {
                                continue;
                            }
                            if (D[s] < D[min_num])
                            {
                                mindis = D[s];
                                min_num = s;
                            }
                        }
                    }
                }
                //以上可以排序优化
                S.Add(min_num);
                Sr.Remove(min_num);
                //-----------把最新包含进来的点也加到路径中-------------
                WaysAdd(from, min_num, min_num);
                {
                    var __list6 = Sr;
                    var __listCount6 = __list6.Count;
                    for (int __i6 = 0; __i6 < __listCount6; ++__i6)
                    {
                        var element = (int)__list6[__i6];
                        {
                            bool exchange = false;      //有交换标志
                            double dis = GetDis(min_num, element);//this.distance[position];
                            if (dis < 0)
                            {
                                continue;
                            }
                            if (D[min_num] < 0)
                            {
                                continue;
                            }
                            if (D[element] < D[min_num] + dis && D[element] > 0)
                            {
                                continue;
                            }
                            D[element] = dis + D[min_num];
                            exchange = true;
                            //修改距离矩阵
                            SetDis(from, element, D[element]);//this.distance[from * row + element] = D[element];
                            //position = element * this.row + from;
                            //this.distance[position] = D[element];

                            //修改路径---------------
                            if (exchange == true)
                            {
                                WayClear(from, element);
                                {
                                    var __list9 = Wayss[from][min_num];
                                    var __listCount9 = __list9.Count;
                                    for (int __i9 = 0; __i9 < __listCount9; ++__i9)
                                    {
                                        var i = (int)__list9[__i9];
                                        {
                                            WaysAdd(from, element, i);
                                        }
                                    }
                                }                                //((ArrayList)ways[element]).Clear();
                                //foreach (int point in (ArrayList)ways[min_num])
                                //    ((ArrayList)ways[element]).Add(point);
                            }
                        }
                    }
                }
                --Count;
            }
            {
                // foreach(var Ways in Wayss[from])
                var __enumerator7 = (Wayss[from]).GetEnumerator();
                while (__enumerator7.MoveNext())
                {
                    var Ways = (KeyValuePair<int, List<int>>)__enumerator7.Current;
                    {
                        MoveList[GetKey(from, Ways.Key)] = Ways.Value;
                    }
                }
            }
        }
        //根据表格造一个点
        //private static Coordinate CreateCoordinate(CoordinateRecord tb)
        //{
        //    Coordinate result = new Coordinate()
        //    {
        //        nId = tb.Id,
        //        SceneId = tb.SceneId,
        //        PosX = tb.X,
        //        PosY = tb.Y,
        //    };
        //    return result;
        //}
        #endregion
    }
}
