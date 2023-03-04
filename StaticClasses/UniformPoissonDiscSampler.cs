using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace EndlessRunner.StaticClasses
{
    public class UniformPoissonDiscSampler
    {
        public const int DefaultPointsPerIteration = 30;
        static readonly float SquareRootTwo = (float)Math.Sqrt(2);
        public static Random r = new Random();


        struct Grid
        {
            public Vector2 TopLeft, LowerRight, Centre;
            public Vector2 Dimensions;
            public float? RejectionSqDistance;
            public float MinDistance;
            public float CellSize;
            public int GridWidth, GridHeight;
        }

        struct State
        {
            public Vector2?[,] Grid;
            public List<Vector2> ActivePoints, Points;
        }

        public static List<Vector2> SampleCircle(Vector2 centre, float radius, float minDistance)
        {
            return SampleCircle(centre, radius, minDistance, DefaultPointsPerIteration);
        }

        public static List<Vector2> SampleCircle(Vector2 centre, float radius, float minDistance, int pointsPerIteration)
        {
            return Sample(centre - new Vector2(radius), centre + new Vector2(radius), radius, minDistance, pointsPerIteration);
        }

        public static List<Vector2> SampleRectangle(Vector2 topLeft, Vector2 lowerRight, float minDistance)
        {
            return SampleRectangle(topLeft, lowerRight, minDistance, DefaultPointsPerIteration);
        }

        public static List<Vector2> SampleRectangle(Vector2 topLeft, Vector2 lowerRight, float minDistance, int pointsPerIteration)
        {
            return Sample(topLeft, lowerRight, null, minDistance, pointsPerIteration);
        }

        /// <summary>
        /// Generates all the points in the grid
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="lowerRight"></param>
        /// <param name="rejectionDistance"></param>
        /// <param name="minDistance"></param>
        /// <param name="pointsPerIteration"></param>
        /// <returns></returns>
        private static List<Vector2> Sample(Vector2 topLeft, Vector2 lowerRight, float? rejectionDistance, float minDistance, int pointsPerIteration)
        {
            var grid = new Grid()
            {
                TopLeft = topLeft,
                LowerRight = lowerRight,
                Dimensions = lowerRight - topLeft,
                Centre = (topLeft + lowerRight) / 2,
                CellSize = minDistance / SquareRootTwo,
                MinDistance = minDistance,
                RejectionSqDistance = rejectionDistance == null ? null : rejectionDistance * rejectionDistance
            };

            grid.GridWidth = (int)(grid.Dimensions.X / grid.CellSize) + 1;
            grid.GridHeight = (int)(grid.Dimensions.Y / grid.CellSize) + 1;

            var state = new State()
            {
                Grid = new Vector2?[grid.GridWidth, grid.GridHeight],
                ActivePoints = new List<Vector2>(),
                Points = new List<Vector2>()
            };

            AddFirstPoint(ref grid, ref state);

            //Creates all the rest of the points in the grid
            while (state.ActivePoints.Count != 0)
            {
                var listIndex = r.Next(state.ActivePoints.Count);
                var point = state.ActivePoints[listIndex];
                bool found = false;

                // Generates the rest of the points in the grid
                for (int i = 0; i < pointsPerIteration; i++)
                    found |= AddNextPoint(point, ref grid, ref state);

                if (!found)
                    state.ActivePoints.RemoveAt(listIndex);
            }
            return state.Points;
        }

        /// <summary>
        /// Creates the first point in the grid
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="state"></param>
        private static void AddFirstPoint(ref Grid grid, ref State state)
        {
            bool added = false;

            while (!added)
            {
                var d = r.NextDouble();
                var xr = grid.TopLeft.X + grid.Dimensions.X * d;

                d = r.NextDouble();
                var yr = grid.TopLeft.Y + grid.Dimensions.Y * d;

                var p = new Vector2((float)xr, (float)yr);

                if (grid.RejectionSqDistance != null && Vector2.DistanceSquared(grid.Centre, p) > grid.RejectionSqDistance)
                    continue;
                added = true;

                var index = Denormalise(p, grid.TopLeft, grid.CellSize);

                state.Grid[(int)index.X, (int)index.Y] = p;

                state.ActivePoints.Add(p);
                state.Points.Add(p);
            }
        }

        /// <summary>
        /// Creates a new point in the grid
        /// </summary>
        /// <param name="point"></param>
        /// <param name="grid"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private static bool AddNextPoint(Vector2 point, ref Grid grid, ref State state)
        {
            bool found = false;

            var q = GenerateRandomAround(point, grid.MinDistance);

            if
               (
                q.X >= grid.TopLeft.X &&
                q.X < grid.LowerRight.X &&
                q.Y > grid.TopLeft.Y &&
                q.Y < grid.LowerRight.Y &&
                (grid.RejectionSqDistance == null || Vector2.DistanceSquared(grid.Centre, q) <= grid.RejectionSqDistance)
               )
            {
                var qIndex = Denormalise(q, grid.TopLeft, grid.CellSize);
                bool tooClose = false;

                // Checks that the generated point isn't too close to already existing points
                for (int i = (int)Math.Max(0, qIndex.X - 2); i < Math.Min(grid.GridWidth, qIndex.X + 3) && !tooClose; i++)
                {
                    for (int j = (int)Math.Max(0, qIndex.Y - 2); j < Math.Min(grid.GridHeight, qIndex.Y + 3) && !tooClose; j++)
                    {
                        if (state.Grid[i, j].HasValue && Vector2.Distance(state.Grid[i, j].Value, q) < grid.MinDistance)
                            tooClose = true;
                    }
                }

                // If the point isn't too close then it is generated
                if (!tooClose)
                {
                    found = true;
                    state.ActivePoints.Add(q);
                    state.Points.Add(q);
                    state.Grid[(int)qIndex.X, (int)qIndex.Y] = q;
                }
            }
            return found;
        }

        /// <summary>
        /// Generates a point a minimum distance from the previous point
        /// </summary>
        /// <param name="centre"></param>
        /// <param name="minDistance"></param>
        /// <returns></returns>
        private static Vector2 GenerateRandomAround(Vector2 centre, float minDistance)
        {
            var d = r.NextDouble();
            var radius = minDistance + minDistance * d;

            d = r.NextDouble();
            var angle = Math.PI * 2 * d;

            var newX = radius * Math.Sin(angle);
            var newY = radius * Math.Cos(angle);

            return new Vector2((float)(centre.X + newX), (float)(centre.Y + newY));
        }

        /// <summary>
        /// Calculates the cell that the point falls into
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <param name="cellSize"></param>
        /// <returns></returns>
        private static Vector2 Denormalise(Vector2 point, Vector2 origin, double cellSize)
        {
            return new Vector2((int)((point.X - origin.X) / cellSize), (int)((point.Y - origin.Y) / cellSize));
        }
    }
}