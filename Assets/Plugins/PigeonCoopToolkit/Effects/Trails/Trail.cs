using UnityEngine;

namespace PigeonCoopToolkit.Effects.Trails
{
    [AddComponentMenu("Pigeon Coop Toolkit/Effects/Trail")]
    public class Trail : TrailRenderer_Base
    {
        public float MinVertexDistance = 0.5f;
        public int MaxNumberOfPoints = 20;
        private Vector3 _lastPosition;
        private float _distanceMoved;

        protected override void Start()
        {
            base.Start();
            _lastPosition = _t.position;
        }

        protected override void Update()
        {
            if(_emit)
            {
                _distanceMoved += Vector3.Distance(_t.position, _lastPosition);

                if (_distanceMoved != 0 && _distanceMoved >= MinVertexDistance)
                {
                    AddPoint(PCTrailPoint.New(), _t.position);
                    _distanceMoved = 0;
                }

                _lastPosition = _t.position;

            }

            base.Update();
        }


        [UnityEngine.ContextMenu("Toggle inspector size input method")]
        protected override void ToggleSizeInputStyle()
        {
            TrailData.UsingSimpleSize = !TrailData.UsingSimpleSize;
        }
        [UnityEngine.ContextMenu("Toggle inspector color input method")]
        protected override void ToggleColorInputStyle()
        {
            TrailData.UsingSimpleColor = !TrailData.UsingSimpleColor;
        }

        protected override void OnStartEmit()
        {
            _lastPosition = _t.position;
            _distanceMoved = 0;
        }

        protected override void Reset()
        {
            base.Reset();
            MinVertexDistance = 0.5f;
        }

        protected override void OnTranslate(Vector3 t)
        {
            _lastPosition += t;
        }

        protected override int GetMaxNumberOfPoints()
        {
            return MaxNumberOfPoints;
        }
    }
}
