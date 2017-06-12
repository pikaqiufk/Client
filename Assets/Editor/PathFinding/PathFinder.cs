using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using EpPathFinding.cs;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PathFinder : MonoBehaviour
{
    public SceneObstacle Obstacle;

    public GameObject Begin;
    public GameObject End;

    void Start()
    {
        Obstacle = new SceneObstacle(GetCurSceneObstacleFilePath());

        Begin = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Begin.name = "Begin";
        End = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        End.name = "End";
    }

    private string GetCurSceneObstacleFilePath()
    {
#if UNITY_EDITOR
        string curScene = EditorApplication.currentScene.Substring(EditorApplication.currentScene.LastIndexOf('/') + 1);
        curScene = curScene.Substring(0, curScene.IndexOf('.'));
        string filePath = Application.dataPath + "/../../Server/Scene/";
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        string obstacleFileName = curScene + ".path";
        string obstacleFilePath = filePath + obstacleFileName;
        return obstacleFilePath;
#endif
        return "";
    }

    private void OnDrawGizmos()
    {
        var TargetPos = Obstacle.FindPath(Begin.transform.position.xz(), End.transform.position.xz());
#if UNITY_EDITOR

    	Handles.color = Color.green;
        var p = Begin.transform.position.xz();
	    for (int i = 0; i < TargetPos.Count; i++)
	    {
            Gizmos.DrawLine(new Vector3(p.x, 50, p.y), new Vector3(TargetPos[i].x, 50, TargetPos[i].y));
	        p = TargetPos[i];
	    }
#endif
    }

    public class SceneObstacle
    {
        public enum ObstacleValue
        {
            Invalid = -1,		//无效值
            Unwalkable = 0,	//不可行走
            Walkable = 1,		//可行走
        }

        public struct ObstacleItem
        {
            public float Fx { get; set; }
            public float Fy { get; set; }
            public byte Value { get; set; }
        }

        public int Width { get; set; }
        public int Height { get; set; }

        private StaticGrid mSearchGrid;
        private JumpPointParam mParam;
        public SceneObstacle(string filename)
        {
            try
            {
                Load(filename);
            }
            catch (Exception ex)
            {
                Logger.Error("load server obstacle error, " + filename + ex.ToString());
            }
        }
        public void AddCollider(ICollider collider)
        {
            mSearchGrid.AddCollider(collider);
        }

        public void RemoveCollider(ICollider collider)
        {
            mSearchGrid.RemoveCollider(collider);
        }

        private void Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Logger.Error("path file do not exit. {0}", fileName);
                return;
            }

            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            int nSeek = 0;

            //先从文件头读取地图的长和宽
            byte[] byteLen = new byte[Marshal.SizeOf(typeof(int))];
            fileStream.Seek(nSeek, SeekOrigin.Begin);
            fileStream.Read(byteLen, 0, byteLen.Length);
            Height = System.BitConverter.ToInt32(byteLen, 0);
            nSeek += byteLen.Length;

            byte[] byteWid = new byte[Marshal.SizeOf(typeof(int))];
            fileStream.Seek(nSeek, SeekOrigin.Begin);
            fileStream.Read(byteWid, 0, byteWid.Length);
            Width = System.BitConverter.ToInt32(byteWid, 0);
            nSeek += byteWid.Length;

            int nReadLen = Marshal.SizeOf(typeof(ObstacleItem));
            byte[] read = new byte[nReadLen];

            mSearchGrid = new StaticGrid(Width * 2 + 1, Height * 2 + 1);

            byte[][] mat = new byte[Width * 2 + 1][];

            for (int i = 0; i < Width * 2 + 1; i++)
            {
                mat[i] = new byte[Height * 2 + 1];
            }

            for (int i = 0; i < (Height * 2 + 1) * (Width * 2 + 1); ++i)
            {
                fileStream.Seek(nSeek, SeekOrigin.Begin);
                fileStream.Read(read, 0, nReadLen);
                nSeek += nReadLen;
                ObstacleItem info = Byte2Struct(read);

                mat[(int)(info.Fx * 2)][(int)(info.Fy * 2)] = (byte)info.Value;
            }

            mSearchGrid.Reset(mat);

            mParam = new JumpPointParam(mSearchGrid, new GridPos(0, 0), new GridPos(0, 0), true, true, false,
                        HeuristicMode.MANHATTAN);
        }

        private static ObstacleItem Byte2Struct(byte[] arr)
        {
            int structSize = Marshal.SizeOf(typeof(ObstacleItem));
            IntPtr ptemp = Marshal.AllocHGlobal(structSize);
            Marshal.Copy(arr, 0, ptemp, structSize);
            ObstacleItem rs = (ObstacleItem)Marshal.PtrToStructure(ptemp, typeof(ObstacleItem));
            Marshal.FreeHGlobal(ptemp);
            return rs;
        }

        public static readonly List<Vector2> EmptyPath = new List<Vector2>();
        public List<Vector2> FindPath(Vector2 start, Vector2 end)
        {
            GridPos s = new GridPos((int)(start.x * 2), (int)(start.y * 2));
            GridPos e = new GridPos((int)(end.x * 2), (int)(end.y * 2));

            if (s.x < 0 || s.x >= mSearchGrid.width || s.y < 0 || s.y >= mSearchGrid.height ||
                e.x < 0 || e.x >= mSearchGrid.width || e.y < 0 || e.y >= mSearchGrid.height)
            {
                return EmptyPath;
            }

            mParam.Reset(s, e, mSearchGrid);

            var path = JumpPointFinder.FindPath(mParam);
            return path.Select(item => new Vector2(item.x / 2.0f, item.y / 2.0f)).Skip(1).ToList();
        }

        public ObstacleValue GetObstacleValue(float fx, float fy)
        {
            return mSearchGrid.IsWalkableAt((int)(fx * 2), (int)(fy * 2))
                ? ObstacleValue.Walkable
                : ObstacleValue.Unwalkable;
        }

    }
}
