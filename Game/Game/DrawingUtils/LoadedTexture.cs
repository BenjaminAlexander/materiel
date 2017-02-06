using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
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

        public void Draw(MyGraphicsClass graphics, Vector2 position, Vector2 origin, float rotation, Color color, float depth)
        {
            graphics.getSpriteBatch().Draw(texture, position, null, color, rotation, origin, 1, SpriteEffects.None, depth);
        }

        public Rectangle TransformBoundingRectangle(Vector2 position, Vector2 origin, float rotation)
        {
            return LoadedTexture.CalculateBoundingRectangle(this.BoundingRectangle, LoadedTexture.GetWorldTransformation(position, origin, rotation));
        }

        public Boolean Contains(Vector2 point, Vector2 position, Vector2 origin, float rotation)
        {
            if (this.TransformBoundingRectangle(position, origin, rotation).Contains(new Point((int)(point.X), (int)(point.Y))))
            {
                Matrix inversTransform = Matrix.Invert(LoadedTexture.GetWorldTransformation(position, origin, rotation));

                Vector2 texturePos = Vector2.Transform(point, inversTransform);
                int x = (int)Math.Round(texturePos.X);
                int y = (int)Math.Round(texturePos.Y);

                if (0 <= x && x < this.Texture.Width &&
                        0 <= y && y < this.Texture.Height)
                {
                    Color color = this.data[x + y * this.Texture.Width];
                    if (color.A != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Boolean CollidesWith(Vector2 position, Vector2 origin, float rotation, LoadedTexture other, Vector2 otherPosition, Vector2 otherOrigin, float otherRotation)
        {
            Rectangle tb = this.TransformBoundingRectangle(position, origin, rotation);
            Rectangle ob = other.TransformBoundingRectangle(otherPosition, otherOrigin, otherRotation);

            if (Utils.Vector2Utils.Intersects(tb, ob))
            {
                if (LoadedTexture.MyIntersectPixels(this, other, position, origin, rotation, otherPosition, otherOrigin, otherRotation))
                {
                    return true;
                }
            }
            return false;
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
                //TODO: why does they +2 seem to make the bounding rectangle most correct
                this.boundingRectangle = new Rectangle(minX, minY, maxX - minX + 2, maxY - minY + 2);
            }
        }

        // Returns a matrix tranformation that sends a texture into it's actual position in the game world.
        public static Matrix GetWorldTransformation(Vector2 position, Vector2 origin, float rotation)
        {
            Matrix originM = Matrix.CreateTranslation(-origin.X, -origin.Y, 0);
            Matrix rotationM = Matrix.CreateRotationZ(rotation);
            Matrix positionM = Matrix.CreateTranslation(position.X, position.Y, 0);
            Matrix returnM = originM * rotationM * positionM;
            return returnM;

        }

        /// <summary>
        /// Microsoft XNA Community Game Platform
        /// Copyright (C) Microsoft Corporation. All rights reserved.
        /// 
        /// Calculates an axis aligned rectangle which fully contains an arbitrarily
        /// transformed axis aligned rectangle.
        /// </summary>
        /// <param name="rectangle">Original bounding rectangle.</param>
        /// <param name="transform">World transform of the rectangle.</param>
        /// <returns>A new rectangle which contains the trasnformed rectangle.</returns>
        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle,
                                                           Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)Math.Floor(min.X), (int)Math.Floor(min.Y),
                                 (int)Math.Ceiling(max.X - min.X), (int)Math.Ceiling(max.Y - min.Y));
        }

        //This one is better because it only checks the part the bounding rectangeles that intersect instead of the whole texture
        public static bool MyIntersectPixels(LoadedTexture t1, LoadedTexture t2, Vector2 position1, Vector2 origin1, float rotation1, Vector2 position2, Vector2 origin2, float rotation2)
        {
            Rectangle d1Bound = t1.TransformBoundingRectangle(position1, origin1, rotation1);
            Rectangle d2Bound = t2.TransformBoundingRectangle(position2, origin2, rotation2);

            Rectangle intersectArea;
            Rectangle.Intersect(ref d1Bound, ref d2Bound, out intersectArea);

            Matrix inversTransform1 = Matrix.Invert(LoadedTexture.GetWorldTransformation(position1, origin1, rotation1));
            Matrix inversTransform2 = Matrix.Invert(LoadedTexture.GetWorldTransformation(position2, origin2, rotation2));

            //randomly selecting a pixels to check instead of iterating through rows would improve performance
            for (int worldX = intersectArea.X; worldX < intersectArea.X + intersectArea.Width; worldX++)
            {
                for (int worldY = intersectArea.Y; worldY < intersectArea.Y + intersectArea.Height; worldY++)
                {
                    Vector3 pos = new Vector3(worldX, worldY, 0);

                    Vector3 texture1Pos = Vector3.Transform(pos, inversTransform1);
                    Vector3 texture2Pos = Vector3.Transform(pos, inversTransform2);

                    int x1 = (int)Math.Round(texture1Pos.X);
                    int y1 = (int)Math.Round(texture1Pos.Y);

                    int x2 = (int)Math.Round(texture2Pos.X);
                    int y2 = (int)Math.Round(texture2Pos.Y);


                    if (0 <= x1 && x1 < t1.Texture.Width &&
                        0 <= y1 && y1 < t1.Texture.Height &&
                        0 <= x2 && x2 < t2.Texture.Width &&
                        0 <= y2 && y2 < t2.Texture.Height)
                    {
                        Color color1 = t1.data[x1 + y1 * t1.Texture.Width];
                        Color color2 = t2.data[x2 + y2 * t2.Texture.Width];

                        // If both pixels are not completely transparent,
                        if (color1.A != 0 && color2.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
