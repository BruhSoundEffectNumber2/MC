using Godot;
using System;

namespace MC
{
    public class Player : Spatial
    {
        [Export()] 
        public float Speed = 8;
        [Export()] 
        public float FastSpeed = 20;
        [Export()] 
        public float MouseSensitivity = 0.05f;

        private float _speed;
        private Vector3 _velocity = Vector3.Zero;

        public override void _Ready()
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }

        public override void _Process(float delta)
        {
            var moveInput = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
            var heightInput = Input.GetAxis("move_down", "move_up");
            var fast = Input.IsActionPressed("sprint");

            _speed = fast ? FastSpeed : Speed;
            _velocity = CalculateVelocity(moveInput, heightInput, Transform.basis);

            Translation += _velocity * delta;

            if (Input.IsActionJustPressed("ui_home"))
            {
                GD.Print(Translation);
            }
        }

        private Vector3 CalculateVelocity(Vector2 move, float height, Basis aim)
        {
            var left = move.x * aim.x;
            var forward = move.y * aim.z;
            var up = height * aim.y;

            return (left + forward + up) * _speed;
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventMouseMotion motion)
            {
                var rot = RotationDegrees;

                rot.x = Mathf.Clamp(rot.x - motion.Relative.y * MouseSensitivity, -89, 89);
                rot.y -= motion.Relative.x * MouseSensitivity;

                RotationDegrees = rot;
            }
        }
    }
}
