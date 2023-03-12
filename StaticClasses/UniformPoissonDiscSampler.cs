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
        public const int DefaultNumPoints = 30;
        static readonly float SquareRootTwo = (float)Math.Sqrt(2);
        public static Random r = new Random();


        struct Grid
        {
            public Vector2 TopLeft, BottomRight, Centre;
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
            return SampleCircle(centre, radius, minDistance, DefaultNumPoints);
        }

        public static List<Vector2> SampleCircle(Vector2 centre, float radius, float minDistance, int pointsPerIteration)
        {
            return Sample(centre - new Vector2(radius), centre + new Vector2(radius), radius, minDistance, pointsPerIteration);
        }

        public static List<Vector2> SampleRectangle(Vector2 topLeft, Vector2 lowerRight, float minDistance)
        {
            return SampleRectangle(topLeft, lowerRight, minDistance, DefaultNumPoints);
        }

        public static List<Vector2> SampleRectangle(Vector2 topLeft, Vector2 lowerRight, float minDistance, int pointsPerIteration)
        {
            return Sample(topLeft, lowerRight, null, minDistance, pointsPerIteration);
        }

        /// <summary>
        /// Generates all the points in the grid
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <param name="rejectionDistance"></param>
        /// <param name="minDistance"></param>
        /// <param name="numPoints"></param>
        /// <returns></returns>
        private static List<Vector2> Sample(Vector2 topLeft, Vector2 bottomRight, float? rejectionDistance, float minDistance, int numPoints)
        {
            Grid grid = new Grid()
            {
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Dimensions = bottomRight - topLeft,
                Centre = (topLeft + bottomRight) / 2,
                CellSize = minDistance / SquareRootTwo,
                MinDistance = minDistance,
                RejectionSqDistance = rejectionDistance == null ? null : rejectionDistance * rejectionDistance
            };

            grid.GridWidth = (int)(grid.Dimensions.X / grid.CellSize) + 1;
            grid.GridHeight = (int)(grid.Dimensions.Y / grid.CellSize) + 1;

            State state = new State()
            {
                Grid = new Vector2?[grid.GridWidth, grid.GridHeight],
                ActivePoints = new List<Vector2>(),
                Points = new List<Vector2>()
            };

            AddFirstPoint(ref grid, ref state);

            //Creates all the rest of the points in the grid
            while (state.ActivePoints.Count != 0)
            {
                int listIndex = r.Next(state.ActivePoints.Count);
                Vector2 point = state.ActivePoints[listIndex];
                bool found = false;

                // Generates the rest of the points in the grid
                for (int i = 0; i < numPoints; i++)
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
                double increaseMultiplier = r.NextDouble();
                double xr = grid.TopLeft.X + grid.Dimensions.X * increaseMultiplier;

                increaseMultiplier = r.NextDouble();
                double yr = grid.TopLeft.Y + grid.Dimensions.Y * increaseMultiplier;

                Vector2 p = new Vector2((float)xr, (float)yr);

                if (grid.RejectionSqDistance != null && Vector2.DistanceSquared(grid.Centre, p) > grid.RejectionSqDistance)
                    continue;
                added = true;

                Vector2 index = Denormalise(p, grid.TopLeft, grid.CellSize);

                state.Grid[(int)index.X, (int)index.Y] = p;

                state.ActivePoints.Add(p);
                state.Points.Add(p);
            }
        }

        /// <summary>
        /// Creates a new point in the grid
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="grid"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private static bool AddNextPoint(Vector2 startPoint, ref Grid grid, ref State state)
        {
            bool found = false;

            Vector2 point = GenerateRandomAround(startPoint, grid.MinDistance);

            if
               (
                point.X >= grid.TopLeft.X &&
                point.X < grid.BottomRight.X &&
                point.Y > grid.TopLeft.Y &&
                point.Y < grid.BottomRight.Y &&
                (grid.RejectionSqDistance == null || Vector2.DistanceSquared(grid.Centre, point) <= grid.RejectionSqDistance)
               )
            {
                Vector2 cell = Denormalise(point, grid.TopLeft, grid.CellSize);
                bool tooClose = false;

                // Checks that the generated point isn't too close to already existing points or in the same cell as another point
                for (int i = (int)Math.Max(0, cell.X - 2); i < Math.Min(grid.GridWidth, cell.X + 3) && !tooClose; i++)
                {
                    for (int j = (int)Math.Max(0, cell.Y - 2); j < Math.Min(grid.GridHeight, cell.Y + 3) && !tooClose; j++)
                    {
                        if (state.Grid[i, j].HasValue && Vector2.Distance(state.Grid[i, j].Value, point) < grid.MinDistance)
                            tooClose = true;
                    }
                }

                // If the point isn't too close then it is generated
                if (!tooClose)
                {
                    found = true;
                    state.ActivePoints.Add(point);
                    state.Points.Add(point);
                    state.Grid[(int)cell.X, (int)cell.Y] = point;
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
            double increaseMultiplyer = r.NextDouble();
            double radius = minDistance + minDistance * increaseMultiplyer;

            increaseMultiplyer = r.NextDouble();
            double angle = Math.PI * 2 * increaseMultiplyer;

            double newX = radius * Math.Cos(angle);
            double newY = radius * Math.Sin(angle);

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
