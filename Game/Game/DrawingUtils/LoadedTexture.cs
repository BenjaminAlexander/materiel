using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MyGame.Geometry;
using MyGame.Utils;

namespace MyGame.DrawingUtils
{
    // This class is a wrapper around a Texture2D that provides an interface to useful properties of the texture.
    public class LoadedTexture
    {
        private Texture2D texture;
        private Color[] data;
        private Vector2 centerOfMass;
        private int mass;
        private Circle boundingCircle;
        private Rectangle boundingRectangle;
        private List<Point> nonZeroPixels;

        public Texture2D Texture
        {
            get { return texture; }
        }

        public Color[] Data
        {
            get { return data; }
        }

        public Vector2 CenterOfMass
        {
            get { return centerOfMass; }
        }

        public int Mass
        {
            get { return mass; }
        }

        public Circle BoundingCircle
        {
            get { return boundingCircle; }
        }

        public Rectangle BoundingRectangle
        {
            get { return boundingRectangle; }
        }

        public List<Point> NonZeroPixels
        {
            get { return nonZeroPixels; }
        }

        public LoadedTexture(Texture2D t)
        {
            this.texture = t;
            this.data = new Color[this.texture.Width * this.texture.Height];
            this.texture.GetData(data);
            this.ComputeCenterOfMass();
            this.ComputeBoundingCircle();
            this.ComputeBoundingRectangle();
        }

        // Computes the center of mass of the texture.
        private void ComputeCenterOfMass()
        {
            nonZeroPixels = new List<Point>();
            Vector2 sum = new Vector2(0);
            mass = 0;
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    if (!ZeroPixel(x, y))
                    {
                        sum = sum + new Vector2(x, y);
                        mass++;
                        nonZeroPixels.Add(new Point(x, y));
                    }
                }
            }
            centerOfMass = sum / mass;
        }

        // Computes the bounding circle of the texture. (Useful for collision detection.) 
        //TODO:  This circle could be minimal
        private void ComputeBoundingCircle()
        {
            if (nonZeroPixels.Count == 0)
            {
                this.boundingCircle = new Circle(new Vector2(0), 0);
            }

            Point center = texture.Bounds.Center;

            Point farthestPoint = nonZeroPixels[0];
            foreach (Point pixel in nonZeroPixels)
            {
                if (MathUtils.Distance(farthestPoint, center) < MathUtils.Distance(pixel, center))
                {
                    farthestPoint = pixel;
                }
            }

            float radius = MathUtils.Distance(farthestPoint, center);
            this.boundingCircle = new Circle(new Vector2(center.X, center.Y), radius);
        }

        private Color GetData(int x, int y)
        {
            return data[x + y * texture.Width];
        }

        // Checks to see whether or not a point is within the rectangle bounds of the texture.
        private bool InBounds(int x, int y)
        {
            return (x >= 0 && x < texture.Width && y >= 0 && y < texture.Height);
        }
        
        // Overload for Point 
        private bool InBounds(Point p)
        {
            return InBounds(p.X, p.Y);
        }

        // Checks to see if the pixel is a zero pixel (has an alpha value of 0);
        private bool ZeroPixel(int x, int y)
        {
            if (InBounds(x, y))
            {
                return GetData(x, y).A == 0;
            }
            return true;
        }

        private void ComputeBoundingRectangle()
        {
            if (nonZeroPixels.Count == 0)
            {
                this.boundingRectangle = new Rectangle(0, 0, 0, 0);
            }
            else
            {
                int minX = nonZeroPixels[0].X;
                int maxX = nonZeroPixels[0].X;
                int minY = nonZeroPixels[0].Y;
                int maxY = nonZeroPixels[0].Y;

                foreach (Point pixel in nonZeroPixels)
                {
                    minX = Math.Min(minX, pixel.X);
                    maxX = Math.Max(maxX, pixel.X);
                    minY = Math.Min(minY, pixel.Y);
                    maxY = Math.Max(maxY, pixel.Y);
                }
                this.boundingRectangle = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
            }
        }
    }
}
