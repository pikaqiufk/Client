using System.Collections.Generic;
using System.Linq;
using PigeonCoopToolkit.Utillities;
using UnityEngine;
using System;

namespace PigeonCoopToolkit.Effects.Trails
{
    public abstract class TrailRenderer_Base : MonoBehaviour
    {
        public PCTrailRendererData TrailData;
        public bool Emit = false;

        protected bool _emit;
        protected bool _noDecay;

        private PCTrail _activeTrail;
        private List<PCTrail> _fadingTrails;
        protected Transform _t;

        private static GameObject _renderObject;

        private static Dictionary<int, Dictionary<Material, GameObject>> _layer2Mat2Render =
            new Dictionary<int, Dictionary<Material, GameObject>>();
        private static Dictionary<int, Dictionary<Material, List<PCTrail>>> _layer2Mat2Trail =
            new Dictionary<int, Dictionary<Material, List<PCTrail>>>();

        private static Stack<CombineInstance> _combineInstances = new Stack<CombineInstance>();
        private static Stack<Mesh> _meshes = new Stack<Mesh>();

        private static bool _hasRenderer = false;

        protected virtual void Awake()
        {
            if (!TrailData.SharedMaterial)
            {
                TrailData.TrailMaterial = new Material(TrailData.TrailMaterial);
            }

            if (!_renderObject)
            {
                _renderObject = new GameObject("TrailRender");
            }

            _fadingTrails = new List<PCTrail>();
            _t = transform;
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDisable()
        {
            {
                // foreach(var a in _layer2Mat2Render)
                var __enumerator1 = (_layer2Mat2Render).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var a = __enumerator1.Current;
                    {
                        {
                            // foreach(var b in a.Value)
                            var __enumerator6 = (a.Value).GetEnumerator();
                            while (__enumerator6.MoveNext())
                            {
                                var b = __enumerator6.Current;
                                {
                                    if (b.Value && b.Value.renderer)
                                    {
                                        var filter = b.Value.renderer.GetComponent<MeshFilter>();
                                        if (filter && filter.sharedMesh)
                                        {
                                            if (filter.sharedMesh.name.StartsWith("combinedMesh"))
                                            {
                                                filter.sharedMesh.Clear();
                                                _meshes.Push(filter.sharedMesh);
                                                filter.sharedMesh = null;
                                            }
                                            else
                                            {
                                                filter.sharedMesh = null;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (_activeTrail != null)
                _activeTrail.MeshUpdated = false;
        }

        protected virtual void LateUpdate()
        {
            if (_hasRenderer)
                return;


            _hasRenderer = true;
            {
                // foreach(var layer in _layer2Mat2Trail)
                var __enumerator2 = (_layer2Mat2Trail).GetEnumerator();
                while (__enumerator2.MoveNext())
                {
                    var layer = __enumerator2.Current;
                    {
                        var _matToTrailList = layer.Value;
                        {
                            // foreach(var keyValuePair in _matToTrailList)
                            var __enumerator7 = (_matToTrailList).GetEnumerator();
                            while (__enumerator7.MoveNext())
                            {
                                var keyValuePair = (KeyValuePair<Material, List<PCTrail>>)__enumerator7.Current;
                                {
                                    try
                                    {

#if UNITY_EDITOR
                        keyValuePair.Key.shader = Shader.Find(keyValuePair.Key.shader.name);
#endif
                                        var count = keyValuePair.Value.Count(item => item.MeshUpdated);
                                        if (count == 0)
                                        {
                                            continue;
                                        }

                                        Dictionary<Material, GameObject> dict;
                                        if (!_layer2Mat2Render.TryGetValue(layer.Key, out dict))
                                        {
                                            dict = new Dictionary<Material, GameObject>();
                                            _layer2Mat2Render.Add(layer.Key, dict);
                                        }

                                        GameObject renderer;
                                        if (!dict.TryGetValue(keyValuePair.Key, out renderer))
                                        {
                                            renderer = new GameObject(LayerMask.LayerToName(layer.Key));
                                            renderer.AddComponent<MeshFilter>();
                                            renderer.AddComponent<MeshRenderer>();
                                            renderer.transform.parent = _renderObject.transform;
                                            renderer.layer = layer.Key;
                                            dict.Add(keyValuePair.Key, renderer);
                                        }

                                        if (!renderer)
                                        {
                                            renderer = new GameObject(LayerMask.LayerToName(layer.Key));
                                            renderer.AddComponent<MeshFilter>();
                                            renderer.AddComponent<MeshRenderer>();
                                            renderer.transform.parent = _renderObject.transform;
                                            renderer.layer = layer.Key;
                                            dict[keyValuePair.Key] = renderer;
                                        }

                                        renderer.renderer.sharedMaterial = keyValuePair.Key;
                                        var filter = renderer.GetComponent<MeshFilter>();
                                        if (filter.sharedMesh)
                                        {
                                            if (filter.sharedMesh.name.StartsWith("combinedMesh"))
                                            {
                                                filter.sharedMesh.Clear();
                                                _meshes.Push(filter.sharedMesh);
                                                filter.sharedMesh = null;
                                            }
                                            else
                                            {
                                                filter.sharedMesh = null;
                                            }
                                        }

                                        if (count == 1)
                                        {
                                            for (int i = 0; i < keyValuePair.Value.Count; i++)
                                            {
                                                if (keyValuePair.Value[i].MeshUpdated)
                                                {
                                                    filter.sharedMesh = keyValuePair.Value[i].Mesh;
                                                    keyValuePair.Value[i].MeshUpdated = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            CombineInstance[] combineInstances = new CombineInstance[count];
                                            int j = 0;
                                            for (int i = 0; i < keyValuePair.Value.Count; i++)
                                            {
                                                if (keyValuePair.Value[i].MeshUpdated)
                                                {
                                                    CombineInstance ci;
                                                    if (_combineInstances.Count > 0)
                                                    {
                                                        ci = _combineInstances.Pop();
                                                    }
                                                    else
                                                    {
                                                        ci = new CombineInstance();
                                                    }
                                                    ci.mesh = keyValuePair.Value[i].Mesh;
                                                    ci.subMeshIndex = 0;
                                                    ci.transform = Matrix4x4.identity;
                                                    combineInstances[j] = ci;
                                                    j++;
                                                    keyValuePair.Value[i].MeshUpdated = false;
                                                }
                                            }

                                            Mesh combinedMesh;
                                            if (_meshes.Count > 0)
                                            {
                                                combinedMesh = _meshes.Pop();
                                            }
                                            else
                                            {
                                                combinedMesh = new Mesh();
                                                combinedMesh.name = "combinedMesh";
                                            }

                                            combinedMesh.CombineMeshes(combineInstances, true, false);
                                            filter.sharedMesh = combinedMesh;

                                            for (int i = 0; i < combineInstances.Length; i++)
                                            {
                                                _combineInstances.Push(combineInstances[i]);
                                            }
                                        }

                                        renderer.renderer.enabled = true;
                                        keyValuePair.Value.Clear();
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                            }
                        }

                        layer.Value.Clear();
                    }
                }
            }

            _layer2Mat2Trail.Clear();
        }

        protected virtual void Update()
        {
            if (_hasRenderer)
            {
                _hasRenderer = false;
            }

            Dictionary<Material, List<PCTrail>> _matToTrailList;
            if (!_layer2Mat2Trail.TryGetValue(gameObject.layer, out _matToTrailList))
            {
                _matToTrailList = new Dictionary<Material, List<PCTrail>>();
                _layer2Mat2Trail.Add(gameObject.layer, _matToTrailList);
            }

            if (_matToTrailList.ContainsKey(TrailData.TrailMaterial) == false)
            {
                _matToTrailList.Add(TrailData.TrailMaterial, new List<PCTrail>());
            }

            if (_activeTrail != null)
            {
                UpdatePoints(_activeTrail, Time.deltaTime);
                UpdateTrail(_activeTrail, Time.deltaTime);
                GenerateMesh(_activeTrail);
                _matToTrailList[TrailData.TrailMaterial].Add(_activeTrail);
            }

            for (int i = _fadingTrails.Count - 1; i >= 0; i--)
            {
                if (_fadingTrails[i] == null || _fadingTrails[i].Points.Any(a => a.TimeActive() < TrailData.Lifetime) == false)
                {
                    if (_fadingTrails[i] != null)
                        _fadingTrails[i].Dispose();

                    _fadingTrails.RemoveAt(i);
                    continue;
                }

                UpdatePoints(_fadingTrails[i], Time.deltaTime);
                UpdateTrail(_fadingTrails[i], Time.deltaTime);
                GenerateMesh(_fadingTrails[i]);
                _matToTrailList[TrailData.TrailMaterial].Add(_fadingTrails[i]);
            }

            CheckEmitChange();
        }

        protected virtual void OnDestroy()
        {
            OnDisable();

            if (_activeTrail != null)
            {
                _activeTrail.Dispose();
                _activeTrail = null;
            }

            if (_fadingTrails != null)
            {
                {
                    var __list3 = _fadingTrails;
                    var __listCount3 = __list3.Count;
                    for (int __i3 = 0; __i3 < __listCount3; ++__i3)
                    {
                        var fadingTrail = (PCTrail)__list3[__i3];
                        {
                            if (fadingTrail != null)
                                fadingTrail.Dispose();
                        }
                    }
                }
                _fadingTrails.Clear();
            }
        }

        protected virtual void OnStopEmit()
        {

        }

        protected virtual void OnStartEmit()
        {
        }

        protected virtual void OnTranslate(Vector3 t)
        {
        }

        protected abstract int GetMaxNumberOfPoints();

        protected virtual void Reset()
        {
            if (TrailData == null)
                TrailData = new PCTrailRendererData();

            TrailData.Lifetime = 1;

            TrailData.UsingSimpleColor = true;
            TrailData.UsingSimpleSize = true;

            TrailData.ColorOverLife = new Gradient();
            TrailData.SimpleColorOverLifeStart = Color.white;
            TrailData.SimpleColorOverLifeEnd = new Color(1, 1, 1, 0);

            TrailData.SizeOverLife = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
            TrailData.SimpleSizeOverLifeStart = 1;
            TrailData.SimpleSizeOverLifeEnd = 0;
        }

        protected virtual void InitialiseNewPoint(PCTrailPoint newPoint)
        {

        }

        protected virtual void UpdateTrail(PCTrail trail, float deltaTime)
        {

        }

        protected void AddPoint(PCTrailPoint newPoint, Vector3 pos)
        {
            if (_activeTrail == null)
                return;

            newPoint.Position = pos;
            newPoint.PointNumber = _activeTrail.Points.Count == 0 ? 0 : _activeTrail.Points[_activeTrail.Points.Count - 1].PointNumber + 1;
            InitialiseNewPoint(newPoint);

            newPoint.SetDistanceFromStart(_activeTrail.Points.Count == 0
                                              ? 0
                                              : _activeTrail.Points[_activeTrail.Points.Count - 1].GetDistanceFromStart() + Vector3.Distance(_activeTrail.Points[_activeTrail.Points.Count - 1].Position, pos));

            if (TrailData.UseForwardOverride)
            {
                newPoint.Forward = TrailData.ForwardOverrideRelative
                                       ? _t.TransformDirection(TrailData.ForwardOverride.normalized)
                                       : TrailData.ForwardOverride.normalized;
            }

            _activeTrail.Points.Add(newPoint);
        }

        private void GenerateMesh(PCTrail trail)
        {
            Vector3 camForward = Camera.main != null ? Camera.main.transform.forward : Vector3.forward;

            if (TrailData.UseForwardOverride)
            {
                camForward = TrailData.ForwardOverride.normalized;
            }

            trail.activePointCount = NumberOfActivePoints(trail);

            if (trail.activePointCount < 2)
                return;

            int vertIndex = 0;
            for (int i = 0; i < trail.Points.Count; i++)
            {
                PCTrailPoint p = trail.Points[i];
                float timeAlong = p.TimeActive() / TrailData.Lifetime;

                if (p.TimeActive() > TrailData.Lifetime)
                {
                    continue;
                }

                if (TrailData.UseForwardOverride && TrailData.ForwardOverrideRelative)
                    camForward = p.Forward;

                Vector3 cross = Vector3.zero;

                if (i < trail.Points.Count - 1)
                {
                    cross =
                        Vector3.Cross((trail.Points[i + 1].Position - p.Position).normalized, camForward).
                            normalized;
                }
                else
                {
                    cross =
                        Vector3.Cross((p.Position - trail.Points[i - 1].Position).normalized, camForward).
                            normalized;
                }


                //yuck! lets move these into their own functions some time
                Color c = TrailData.StretchColorToFit ?
                    (TrailData.UsingSimpleColor ? Color.Lerp(TrailData.SimpleColorOverLifeStart, TrailData.SimpleColorOverLifeEnd, 1 - ((float)vertIndex / (float)trail.activePointCount / 2f)) : TrailData.ColorOverLife.Evaluate(1 - ((float)vertIndex / (float)trail.activePointCount / 2f))) :
                    (TrailData.UsingSimpleColor ? Color.Lerp(TrailData.SimpleColorOverLifeStart, TrailData.SimpleColorOverLifeEnd, timeAlong) : TrailData.ColorOverLife.Evaluate(timeAlong));

                float s = TrailData.StretchSizeToFit ?
                    (TrailData.UsingSimpleSize ? Mathf.Lerp(TrailData.SimpleSizeOverLifeStart, TrailData.SimpleSizeOverLifeEnd, 1 - ((float)vertIndex / (float)trail.activePointCount / 2f)) : TrailData.SizeOverLife.Evaluate(1 - ((float)vertIndex / (float)trail.activePointCount / 2f))) :
                    (TrailData.UsingSimpleSize ? Mathf.Lerp(TrailData.SimpleSizeOverLifeStart, TrailData.SimpleSizeOverLifeEnd, timeAlong) : TrailData.SizeOverLife.Evaluate(timeAlong));


                trail.verticies[vertIndex] = p.Position + cross * s;

                if (TrailData.MaterialTileLength <= 0)
                {
                    trail.uvs[vertIndex] = new Vector2((float)vertIndex / (float)trail.activePointCount / 2f, 0);
                }
                else
                {
                    trail.uvs[vertIndex] = new Vector2(p.GetDistanceFromStart() / TrailData.MaterialTileLength, 0);
                }

                trail.normals[vertIndex] = camForward;
                trail.colors[vertIndex] = c;
                vertIndex++;
                trail.verticies[vertIndex] = p.Position - cross * s;

                if (TrailData.MaterialTileLength <= 0)
                {
                    trail.uvs[vertIndex] = new Vector2((float)vertIndex / (float)trail.activePointCount / 2f, 1);
                }
                else
                {
                    trail.uvs[vertIndex] = new Vector2(p.GetDistanceFromStart() / TrailData.MaterialTileLength, 1);
                }

                trail.normals[vertIndex] = camForward;
                trail.colors[vertIndex] = c;

                vertIndex++;
            }

            Vector2 finalPosition = trail.verticies[vertIndex - 1];
            for (int i = vertIndex; i < trail.verticies.Length; i++)
            {
                trail.verticies[i] = finalPosition;
            }

            int indIndex = 0;
            for (int pointIndex = 0; pointIndex < 2 * (trail.activePointCount - 1); pointIndex++)
            {
                if (pointIndex % 2 == 0)
                {
                    trail.indicies[indIndex] = pointIndex;
                    indIndex++;
                    trail.indicies[indIndex] = pointIndex + 1;
                    indIndex++;
                    trail.indicies[indIndex] = pointIndex + 2;
                }
                else
                {
                    trail.indicies[indIndex] = pointIndex + 2;
                    indIndex++;
                    trail.indicies[indIndex] = pointIndex + 1;
                    indIndex++;
                    trail.indicies[indIndex] = pointIndex;
                }

                indIndex++;
            }

            int finalIndex = trail.indicies[indIndex - 1];
            for (int i = indIndex; i < trail.indicies.Length; i++)
            {
                trail.indicies[i] = finalIndex;
            }

            trail.Mesh.vertices = trail.verticies;
            trail.Mesh.SetIndices(trail.indicies, MeshTopology.Triangles, 0);
            trail.Mesh.uv = trail.uvs;
            trail.Mesh.normals = trail.normals;
            trail.Mesh.colors = trail.colors;

            trail.MeshUpdated = true;
        }

        private void DrawMesh(Mesh trailMesh, Material trailMaterial)
        {
            Graphics.DrawMesh(trailMesh, Matrix4x4.identity, trailMaterial, gameObject.layer);
        }

        private void UpdatePoints(PCTrail line, float deltaTime)
        {
            for (int i = 0; i < line.Points.Count; i++)
            {
                line.Points[i].Update(_noDecay ? 0 : deltaTime);
            }
        }

        [Obsolete("UpdatePoint is deprecated, you should instead override UpdateTrail and loop through the individual points yourself (See Smoke or Smoke Plume scripts for how to do this).", true)]
        protected virtual void UpdatePoint(PCTrailPoint pCTrailPoint, float deltaTime)
        {
        }

        private void CheckEmitChange()
        {
            if (_emit != Emit)
            {
                _emit = Emit;
                if (_emit)
                {
                    _activeTrail = new PCTrail(GetMaxNumberOfPoints());
                    _activeTrail.IsActiveTrail = true;

                    OnStartEmit();
                }
                else
                {
                    OnStopEmit();
                    if (_activeTrail != null)
                    {
                        _activeTrail.IsActiveTrail = false;
                        _fadingTrails.Add(_activeTrail);
                        _activeTrail = null;
                    }
                }
            }
        }

        private int NumberOfActivePoints(PCTrail line)
        {
            int count = 0;
            for (int index = 0; index < line.Points.Count; index++)
            {
                if (line.Points[index].TimeActive() < TrailData.Lifetime) count++;
            }
            return count;
        }

        [UnityEngine.ContextMenu("Toggle inspector size input method")]
        protected virtual void ToggleSizeInputStyle()
        {
            TrailData.UsingSimpleSize = !TrailData.UsingSimpleSize;
        }
        [UnityEngine.ContextMenu("Toggle inspector color input method")]
        protected virtual void ToggleColorInputStyle()
        {
            TrailData.UsingSimpleColor = !TrailData.UsingSimpleColor;
        }

        public void LifeDecayEnabled(bool enabled)
        {
            _noDecay = !enabled;
        }

        /// <summary>
        /// Translates every point in the vector t
        /// </summary>
        public void Translate(Vector3 t)
        {
            if (_activeTrail != null)
            {
                for (int i = 0; i < _activeTrail.Points.Count; i++)
                {
                    _activeTrail.Points[i].Position += t;
                }
            }

            if (_fadingTrails != null)
            {
                {
                    var __list4 = _fadingTrails;
                    var __listCount4 = __list4.Count;
                    for (int __i4 = 0; __i4 < __listCount4; ++__i4)
                    {
                        var fadingTrail = (PCTrail)__list4[__i4];
                        {
                            for (int i = 0; i < fadingTrail.Points.Count; i++)
                            {
                                fadingTrail.Points[i].Position += t;
                            }
                        }
                    }
                }
            }

            OnTranslate(t);
        }

        /// <summary>
        /// Insert a trail into this trail renderer. 
        /// </summary>
        /// <param name="from">The start position of the trail.</param>
        /// <param name="to">The end position of the trail.</param>
        /// <param name="distanceBetweenPoints">Distance between each point on the trail</param>
        public void CreateTrail(Vector3 from, Vector3 to, float distanceBetweenPoints)
        {
            float distanceBetween = Vector3.Distance(from, to);

            Vector3 dirVector = to - from;
            dirVector = dirVector.normalized;

            float currentLength = 0;

            CircularBuffer<PCTrailPoint> newLine = new CircularBuffer<PCTrailPoint>(GetMaxNumberOfPoints());
            int pointNumber = 0;
            while (currentLength < distanceBetween)
            {
                PCTrailPoint newPoint = PCTrailPoint.New();
                newPoint.PointNumber = pointNumber;
                newPoint.Position = from + dirVector * currentLength;
                newLine.Add(newPoint);
                InitialiseNewPoint(newPoint);

                pointNumber++;

                if (distanceBetweenPoints <= 0)
                    break;
                else
                    currentLength += distanceBetweenPoints;
            }

            PCTrailPoint lastPoint = PCTrailPoint.New();
            lastPoint.PointNumber = pointNumber;
            lastPoint.Position = to;
            newLine.Add(lastPoint);
            InitialiseNewPoint(lastPoint);

            PCTrail newTrail = new PCTrail(GetMaxNumberOfPoints());
            newTrail.Points = newLine;

            _fadingTrails.Add(newTrail);
        }

        /// <summary>
        /// Clears all active trails from the system.
        /// </summary>
        /// <param name="emitState">Desired emit state after clearing</param>
        public void ClearSystem(bool emitState)
        {
            if (_activeTrail != null)
            {
                _activeTrail.MeshUpdated = false;
                _activeTrail.Dispose();
                _activeTrail = null;
            }

            if (_fadingTrails != null)
            {
                {
                    var __list5 = _fadingTrails;
                    var __listCount5 = __list5.Count;
                    for (int __i5 = 0; __i5 < __listCount5; ++__i5)
                    {
                        var fadingTrail = (PCTrail)__list5[__i5];
                        {
                            if (fadingTrail != null)
                                fadingTrail.Dispose();
                        }
                    }
                }
                _fadingTrails.Clear();
            }

            Emit = emitState;
            _emit = !emitState;

            CheckEmitChange();
        }

        /// <summary>
        /// Get the number of active seperate trail segments.
        /// </summary>
        public int NumSegments()
        {
            int num = 0;
            if (_activeTrail != null && NumberOfActivePoints(_activeTrail) != 0)
                num++;

            num += _fadingTrails.Count;
            return num;
        }
    }

    public class PCTrail : System.IDisposable
    {
        public CircularBuffer<PCTrailPoint> Points;
        public Mesh Mesh;
        public bool MeshUpdated = false;

        public Vector3[] verticies;
        public Vector3[] normals;
        public Vector2[] uvs;
        public Color[] colors;
        public int[] indicies;
        public int activePointCount;

        public bool IsActiveTrail = false;

        public PCTrail(int numPoints)
        {
            Mesh = new Mesh();
            Mesh.name = "PCTrail";
            Mesh.MarkDynamic();

            verticies = new Vector3[2 * numPoints];
            normals = new Vector3[2 * numPoints];
            uvs = new Vector2[2 * numPoints];
            colors = new Color[2 * numPoints];
            indicies = new int[2 * (numPoints) * 3];

            Points = new CircularBuffer<PCTrailPoint>(numPoints);
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (Mesh != null)
            {
                if (Application.isEditor)
                    UnityEngine.Object.DestroyImmediate(Mesh, true);
                else
                    UnityEngine.Object.Destroy(Mesh);
            }

            for (int i = 0; i < Points.Count; i++)
            {
                PCTrailPoint.Release(Points[i]);
            }

            Points.Clear();
            Points = null;
        }

        #endregion
    }

    public class PCTrailPoint
    {
        public Vector3 Forward;
        public Vector3 Position;
        public int PointNumber;

        private float _timeActive = 0;
        private float _distance;

        private static Stack<PCTrailPoint> sPointStack = new Stack<PCTrailPoint>();
        public static PCTrailPoint New()
        {
            if (sPointStack.Count > 0)
            {
                var p = sPointStack.Pop();
                p.Forward = Vector3.forward;
                p.Position = Vector3.zero;
                p.PointNumber = 0;
                p._timeActive = 0;
                p._distance = 0;
                return p;
            }

            return new PCTrailPoint();
        }

        public static void Release(PCTrailPoint p)
        {
            sPointStack.Push(p);
        }

        public virtual void Update(float deltaTime)
        {
            _timeActive += deltaTime;
        }

        public float TimeActive()
        {
            return _timeActive;
        }

        public void SetTimeActive(float time)
        {
            _timeActive = time;
        }

        public void SetDistanceFromStart(float distance)
        {
            _distance = distance;
        }

        public float GetDistanceFromStart()
        {
            return _distance;
        }
    }

    [System.Serializable]
    public class PCTrailRendererData
    {
        public Material TrailMaterial;
        public bool SharedMaterial = true;
        public float Lifetime = 1;
        public bool UsingSimpleSize = true;
        public float SimpleSizeOverLifeStart;
        public float SimpleSizeOverLifeEnd;
        public AnimationCurve SizeOverLife = new AnimationCurve();
        public bool UsingSimpleColor = true;
        public Color SimpleColorOverLifeStart;
        public Color SimpleColorOverLifeEnd;
        public Gradient ColorOverLife;
        public bool StretchSizeToFit;
        public bool StretchColorToFit;
        public float MaterialTileLength = 0;
        public bool UseForwardOverride;
        public Vector3 ForwardOverride;
        public bool ForwardOverrideRelative;
    }
}


