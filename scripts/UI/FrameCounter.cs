using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MC.UI
{
    public class FrameCounter : PanelContainer
    {
        private List<float> _frameTimes = new List<float>(100);
        private float _fps = 0;

        private Label _text;

        public override void _Ready()
        {
            _text = GetNode<Label>("Text");
        }

        public override void _Process(float delta)
        {
            var time = OS.GetTicksMsec();
            
            // Remove frameTimes older than one second
            while (_frameTimes.Count > 0 && _frameTimes[0] <= time - 1000)
            {
                _frameTimes.RemoveAt(0);
            }
            
            _frameTimes.Add(time);
            _fps = _frameTimes.Count;

            _text.Text = $"{_fps:N2} FPS";
        }
    }
}