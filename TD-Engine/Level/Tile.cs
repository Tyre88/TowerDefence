using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TD_Engine
{
    public enum DisplayRegion
    {
        Passable,
        Start,
        Finish,
        NonPassable,
        InPath,
        Closed
    }

    public enum TileType
    {
        Passable,
        NonPassable
    }

    public class Tile
    {
        public TileType Type
        {
            get;
            set;
        }

        public DisplayRegion Region
        {
            get;
            set;
        }

        public Point MapLocation
        {
            get;
            private set;
        }

        public List<Tile> AdjacencyList
        {
            get;
            private set;
        }

        public int DistanceFromStart
        {
            get;
            set;
        }

        public int DistanceToEnd
        {
            get;
            set;
        }

        public Tile Parent
        {
            get;
            set;
        }

        public int TileCode
        {
            get;
            private set;
        }

        public int Heuristic
        {
            get { return DistanceFromStart + DistanceToEnd; }
        }

        public Tile(Point location, int code)
        {
            MapLocation = location;
            Type = TileType.Passable;
            DistanceFromStart = 0;
            TileCode = code;
        }

        public Tile(TileType type, Point location, int code)
        {
            MapLocation = location;
            Type = type;
            DistanceFromStart = 0;
            TileCode = code;
        }

        public void AddToAdjecencyList(Tile t)
        {
            if (t == null)
            {
                return;
            }

            if (AdjacencyList == null)
            {
                AdjacencyList = new List<Tile>();
            }

            AdjacencyList.Add(t);
        }

        public override string ToString()
        {
            return MapLocation.ToString() + " a:" + AdjacencyList.Count + " c:" + TileCode.ToString();
        }
    }
}
