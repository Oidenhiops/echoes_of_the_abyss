using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinding : MonoBehaviour
{
    WalkablePositionInfo[,] grid;
    int gridWidth, gridDepth;
    int minX, minZ;
    [System.NonSerialized] public List<Vector3> occupiedPositions = new List<Vector3>();

    public void GenerateWalkableGrid()
    {
        if (occupiedPositions == null || occupiedPositions.Count == 0)
        {
            return;
        }
        var (minPosX, maxPosX, minPosZ, maxPosZ) = CalculateBounds(occupiedPositions);
        minX = Mathf.FloorToInt(minPosX);
        minZ = Mathf.FloorToInt(minPosZ);
        gridWidth = Mathf.CeilToInt(maxPosX - minPosX) + 1;
        gridDepth = Mathf.CeilToInt(maxPosZ - minPosZ) + 1;
        grid = new WalkablePositionInfo[gridWidth, gridDepth];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                Vector3 posFinded = FindOccupiedPositions(new Vector2(x, z));
                grid[x, z] = new WalkablePositionInfo
                {
                    pos = new Vector3(minX + x, 0, minZ + z),
                    isWalkable = false
                };
            }
        }
        foreach (var pos in occupiedPositions)
        {
            int gridX = Mathf.FloorToInt(pos.x) - minX;
            int gridZ = Mathf.FloorToInt(pos.z) - minZ;

            if (gridX >= 0 && gridX < gridWidth && gridZ >= 0 && gridZ < gridDepth)
            {
                grid[gridX, gridZ].isWalkable = true;
            }
        }
    }
    Vector3 FindOccupiedPositions(Vector2 pos)
    {
        foreach(Vector3 posFinded in occupiedPositions)
        {
            if (posFinded.x == pos.x && posFinded.z == pos.y)
            {
                return posFinded;
            }
        }
        return Vector3.zero;
    }
    public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
    {
        int startX = Mathf.FloorToInt(startPos.x) - minX;
        int startZ = Mathf.FloorToInt(startPos.z) - minZ;
        int endX = Mathf.FloorToInt(endPos.x) - minX;
        int endZ = Mathf.FloorToInt(endPos.z) - minZ;
        if (!IsValidPosition(startX, startZ) || !IsValidPosition(endX, endZ) ||
            !grid[startX, startZ].isWalkable || !grid[endX, endZ].isWalkable)
        {
            return null;
        }
        var openList = new List<Node>();
        var closedList = new HashSet<Node>();
        Node startNode = new Node(startX, startZ);
        Node endNode = new Node(endX, endZ);
        openList.Add(startNode);
        while (openList.Count > 0)
        {
            openList.Sort((a, b) => a.F.CompareTo(b.F));
            Node currentNode = openList[0];
            if (currentNode.Equals(endNode))
                return ReconstructPath(currentNode);
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            foreach (var neighbor in GetNeighbors(currentNode))
            {
                if (closedList.Contains(neighbor) || !grid[neighbor.X, neighbor.Z].isWalkable)
                    continue;
                int tentativeG = currentNode.G + 1;
                if (!openList.Contains(neighbor) || tentativeG < neighbor.G)
                {
                    neighbor.G = tentativeG;
                    neighbor.H = Mathf.Abs(neighbor.X - endNode.X) + Mathf.Abs(neighbor.Z - endNode.Z);
                    neighbor.F = neighbor.G + neighbor.H;
                    neighbor.Parent = currentNode;
                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }
        return null;
    }
    List<Node> GetNeighbors(Node node)
    {
        var neighbors = new List<Node>();
        var directions = new (int, int)[]
        {
            (0, 1), (0, -1), (1, 0), (-1, 0), (1, 1), (-1, 1), (1, -1), (-1, -1)
        };
        foreach (var dir in directions)
        {
            int newX = node.X + dir.Item1;
            int newZ = node.Z + dir.Item2;

            if (IsValidPosition(newX, newZ))
                neighbors.Add(new Node(newX, newZ));
        }
        return neighbors;
    }
    bool IsValidPosition(int x, int z)
    {
        return x >= 0 && x < gridWidth && z >= 0 && z < gridDepth;
    }
    List<Vector3> ReconstructPath(Node node)
    {
        var path = new List<Vector3>();
        while (node != null)
        {
            path.Add(grid[node.X, node.Z].pos);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }
    (float minX, float maxX, float minZ, float maxZ) CalculateBounds(List<Vector3> positions)
    {
        float minX = positions[0].x, maxX = positions[0].x;
        float minZ = positions[0].z, maxZ = positions[0].z;

        foreach (var pos in positions)
        {
            minX = Mathf.Min(minX, pos.x);
            maxX = Mathf.Max(maxX, pos.x);
            minZ = Mathf.Min(minZ, pos.z);
            maxZ = Mathf.Max(maxZ, pos.z);
        }

        return (minX, maxX, minZ, maxZ);
    }
    public class WalkablePositionInfo
    {
        public Vector3 pos;
        public bool isWalkable;
    }
    private class Node
    {
        public int X, Z;
        public int G, H, F;
        public Node Parent;

        public Node(int x, int z)
        {
            X = x;
            Z = z;
        }
        public override bool Equals(object obj)
        {
            return obj is Node other && other.X == X && other.Z == Z;
        }
        public override int GetHashCode()
        {
            return (X, Z).GetHashCode();
        }
    }
}
