using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.IO;
using MyGame.IO.Events;

namespace MyGame
{
    public class Camera : IOObserver
    {
        private IOEvent up;
        private IOEvent down;
        private IOEvent left;
        private IOEvent right;

        private GraphicsDeviceManager graphics;
        private Vector2 position = new Vector2(0);
        private Vector2 positionRelativeToFocus = new Vector2(0);
        private float zoom = 1;
        private float targetZoom = 1;
        private float zoomInterpolationTime = .25f;
        private float currentZoomInterpolationTime = 0;
        private float rotation = 0;

        private float zoomIncrement = (float).001;
        private float maxZoom = 3;
        private float minZoom = (float).1;

        public Camera(Vector2 position, float zoom, float rotation, GraphicsDeviceManager graphics, InputManager ioManager)
        {
            this.graphics = graphics;
            this.position = position;
            this.zoom = zoom;
            this.rotation = rotation;

            up = new KeyDown(Keys.W);
            down = new KeyDown(Keys.S);
            left = new KeyDown(Keys.A);
            right = new KeyDown(Keys.D);

            ioManager.Register(up, this);
            ioManager.Register(down, this);
            ioManager.Register(left, this);
            ioManager.Register(right, this);
        }

        public void Update(float seconds)
        {
            int delta = IO.IOState.MouseWheelDelta;
            if (delta != 0)
            {
                targetZoom = targetZoom + targetZoom * zoomIncrement * IOState.MouseWheelDelta;
                currentZoomInterpolationTime = 0;

                if (targetZoom < minZoom)
                {
                    targetZoom = minZoom;
                }
                if (targetZoom > maxZoom)
                {
                    targetZoom = maxZoom;
                }

            }
            currentZoomInterpolationTime = currentZoomInterpolationTime + seconds;

            zoom = MathHelper.Lerp(zoom, targetZoom, currentZoomInterpolationTime / zoomInterpolationTime);
        }

        public Vector2 Position
        {
            get { return position; }

            set 
            {
                Vector2 viewSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight) / zoom;
                Vector2 cornerPosition = value - (viewSize / 2);

                position = cornerPosition + (viewSize / 2);

            }
        }

        public float X
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public float Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public float Zoom
        {
            get { return zoom; }
            set { zoom = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Matrix GetWorldToScreenTransformation()
        {
            float halfWidth = graphics.PreferredBackBufferWidth / 2;
            float halfHeight = (graphics.PreferredBackBufferHeight) / 2;

            Matrix stretch = Matrix.CreateScale(new Vector3(zoom, zoom, 1));

            return Matrix.CreateTranslation(-position.X, -position.Y, 0) * stretch * Matrix.CreateRotationZ(-rotation) * Matrix.CreateTranslation(halfWidth, halfHeight, 0);
        }

        public Matrix GetScreenToWorldTransformation()
        {
            return Matrix.Invert(GetWorldToScreenTransformation());
        }

        public Vector2 ScreenToWorldPosition(Vector2 vector)
        {
            return Vector2.Transform(vector, GetScreenToWorldTransformation());
        }

        public Vector2 WorldToScreenPosition(Vector2 vector)
        {
            return Vector2.Transform(vector, GetWorldToScreenTransformation());
        }

        public void UpdateWithIOEvent(IOEvent ioEvent)
        {
            if (ioEvent.Equals(up))
            {
                this.position = this.position + new Vector2(0, -10) / zoom;
            }
            else if (ioEvent.Equals(down))
            {
                this.position = this.position + new Vector2(0, 10) / zoom;
            }
            else if (ioEvent.Equals(left))
            {
                this.position = this.position + new Vector2(-10, 0) / zoom;
            }
            else if (ioEvent.Equals(right))
            {
                this.position = this.position + new Vector2(10, 0) / zoom;
            }
        }
    }
}
