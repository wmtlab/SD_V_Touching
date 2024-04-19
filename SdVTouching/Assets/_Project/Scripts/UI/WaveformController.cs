using UnityEngine;
using XCharts.Runtime;

namespace SdVTouching
{
    public class WaveformController
    {
        private Line _line;
        private const int WindowSize = 100;
        private float[] _data;
        private int _head = 0;
        private readonly int _updateDump = 1;
        private int _updateCount = 0;

        public WaveformController(Transform canvas, float min, float max, int updateDump)
        {
            _updateDump = Mathf.Max(updateDump, 1);
            _data = new float[WindowSize];
            for (int i = 0; i < WindowSize; i++)
            {
                _data[i] = 0;
            }
            var xCharts = new GameObject("WaveformChart");
            xCharts.transform.SetParent(canvas);

            var chart = xCharts.AddComponent<LineChart>();
            chart.Init();
            chart.SetSize(1000, 200);

            var rect = chart.GetComponent<RectTransform>();
            rect.anchoredPosition3D = Vector3.zero;
            rect.localScale = Vector3.one;
            rect.pivot = Vector2.zero;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;

            var title = chart.EnsureChartComponent<Title>();
            title.text = "Geomagic Touch Vibration Waveform";
            var xAxis = chart.EnsureChartComponent<XAxis>();
            xAxis.type = Axis.AxisType.Value;
            xAxis.axisLabel.show = false;
            var yAxis = chart.EnsureChartComponent<YAxis>();
            yAxis.type = Axis.AxisType.Value;
            yAxis.minMaxType = Axis.AxisMinMaxType.Custom;
            yAxis.min = min;
            yAxis.max = max;

            chart.RemoveData();
            var line = chart.AddSerie<Line>("wave");
            line.lineType = LineType.Smooth;
            line.animation.enable = false;
            line.symbol.show = false;
            for (int i = 0; i < WindowSize; i++)
            {
                line.AddData(i, _data[i]);
            }

            _line = line;
        }

        public void Update(float signal)
        {
            if (_updateCount % _updateDump == 0)
            {
                _data[_head] = signal;
                _head = (_head + 1) % WindowSize;
                for (int i = 0; i < WindowSize; i++)
                {
                    _line.UpdateYData(i, _data[(_head + i) % WindowSize]);
                }
            }
            _updateCount++;
        }
    }
}