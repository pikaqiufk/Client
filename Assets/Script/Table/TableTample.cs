
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using System.IO;
using System.Collections;

namespace DataTable
{

    public static class Table_Tamplet
    {
        public static int Convert_Int(string _str)
        {
            int temp;
            if (Int32.TryParse(_str, out temp))
            {
                return temp;
            }
            //Logger.Error("Convert_Int Error!  {0}", _str);
            return 0;
        }

        public static float Convert_Float(string _str)
        {
            float temp;
            if (Single.TryParse(_str, out temp))
            {
                return temp;
            }
            //Logger.Error("Convert_Float Error!  {0}", _str);
            return 0;
        }

        public static double Convert_Double(string _str)
        {
            double temp;
            if (Double.TryParse(_str, out temp))
            {
                return temp;
            }
            //Logger.Error("Convert_Double Error!  {0}", _str);
            return 0;
        }
        public static string Convert_String(string _str)
        {
            return _str.Replace("\\n", "\n");
        }
        public static void Convert_Value(List<int> _col_name, string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            string[] temp = str.Split('|');
            {
                var __array1 = temp;
                var __arrayLength1 = __array1.Length;
                for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var s = __array1[__i1];
                    {
                        int temp_int = Convert.ToInt32(s);
                        _col_name.Add(temp_int);
                    }
                }
            }
        }
        public static void Convert_Value(List<List<int>> _col_name, string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            string[] temp1 = str.Split(';');
            Int16 i = 0;
            {
                var __array2 = temp1;
                var __arrayLength2 = __array2.Length;
                for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                {
                    var ss = (string)__array2[__i2];
                    {
                        _col_name.Add(new List<int>());
                        string[] temp2 = ss.Split(',');
                        {
                            var __array3 = temp2;
                            var __arrayLength3 = __array3.Length;
                            for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
                            {
                                var s = (string)__array3[__i3];
                                {
                                    _col_name[i].Add(Convert.ToInt32(s));
                                }
                            }
                        }
                        ++i;
                    }
                }
            }
        }
    }
    public interface IRecord
    {
        void Init(string[] strs);
        object GetField(string name);
    }

    public static class TableInit
    {
        //加载表格
        public static void Table_Init(byte[] tableBytes, Dictionary<int, IRecord> _table_name, TableType type)
        {
            _table_name.Clear();
            var stream = new System.IO.MemoryStream(tableBytes, false);

            System.IO.TextReader tr = null;
            try
            {
                tr = new System.IO.StreamReader(stream, Encoding.UTF8);
                Int32 state = 1;
                string str = tr.ReadLine();
                var NewFunc = Table.NewTableRecord(type);
                while (str != null)
                {
                    string[] strs = str.Split('\t');
                    string first = strs[0];
                    if (state == 1 && first == "INT")
                    {
                        state = 2;
                    }
                    else if (first.Substring(0, 1) == "#" || first == "" || first == " ") //跳过此行加载
                    {

                    }
                    else if (state == 2)
                    {
                        state = 3;
                    }
                    else if (state == 3)
                    {
                        var t = NewFunc();
                        t.Init(strs);
                        _table_name[Convert.ToInt32(first)] = t;
                    }
                    str = tr.ReadLine();
                }
            }
            catch (Exception ex)
            {
                //加入表格加载错误提示
                Logger.Fatal("Load " + tableBytes + " Error!!");
                Logger.Fatal(ex.Message);
                throw ex;
            }
            finally
            {
                if (tr != null)
                {
                    tr.Close();
                }
            }
        }
    }

    public class AsyncResult<T>
    {
        public T Result { get; set; }
    }

    public class TableManager
    {
        public static void InitTable(string tableName, Dictionary<int, IRecord> dictionary, TableType type)
        {
            var path = "Table/" + tableName + ".txt";

            ResourceManager.PrepareResource<TextAsset>(path, asset =>
            {
                if (asset == null)
                {
                    Logger.Error("InitTable error! asset = null, assetname = {0}", path);
                    return;
                }

                TableInit.Table_Init((asset as TextAsset).bytes, dictionary, type);
                ResourceManager.Instance.RemoveFromCache( "Table/" + tableName + ".unity3d");
                
            }, true, false, true, true, true);
        }
    }
}

