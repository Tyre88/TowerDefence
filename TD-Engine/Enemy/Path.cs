using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TD_Engine
{
    public class Path
    {
        public List<Tile> Entries
        {
            get;
            protected set;
        }

        public Point Length
        {
            get;
            private set;
        }

        public Tile StartTile
        {
            get;
            set;
        }

        public Tile EndTile
        {
            get;
            set;
        }

        public PathFinding ShortestPath
        {
            get;
            private set;
        }

        public Tile this[int x, int y]
        {
            get { return Entries[y * Length.X + x]; }
        }

        public Path(Tile[,] tiles, Tile start, Tile end)
        {
            Entries = new List<Tile>(tiles.GetLength(0) * tiles.GetLength(1));
            Length = new Point(tiles.GetLength(0), tiles.GetLength(1));

            for (int y = 0; y < Length.Y; y++)
            {
                for (int x = 0; x < Length.X; x++)
                {
                    Entries.Add(tiles[x, y]);
                }
            }

            StartTile = start;
            EndTile = end;
        }

        public void CalculateShortestPath()
        {
            ShortestPath = new PathFinding();
            ShortestPath.AStar(StartTile, EndTile);
        }

        public bool Update(GameTime gameTime, Monster monster)
        {
            return false;
        }

    }
}
