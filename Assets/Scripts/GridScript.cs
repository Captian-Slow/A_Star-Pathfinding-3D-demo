using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour {

    public Vector2 gridWorldSize;
    public float nodeRadius;
    public LayerMask unwalkableMask;
    float nodeDiameter;

    // Number of nodes along x and y axis based on gridworldsize and node diameter
    int gridNodeSizeX, gridNodeSizeY;

    NodeScript[,] grid;

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridNodeSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridNodeSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new NodeScript[gridNodeSizeX, gridNodeSizeY];
    
        // Getting the bottom left corner of the grid
        Vector3 worldBottomLeft = transform.position - (Vector3.right * gridWorldSize.x / 2) - (Vector3.forward * gridWorldSize.y/2);

        // Collision check through all the nodes in the grid to check if the node is walkabke or not.
        for (int x = 0; x < gridNodeSizeX; x++)
        {
            for (int y = 0; y < gridNodeSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new NodeScript(walkable, worldPoint, x, y);
            }
        }
    }

    public NodeScript getNodeFromWorldPoint (Vector3 worldPoint) {

        float percentX = Mathf.Clamp01((worldPoint.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPoint.z + gridWorldSize.y / 2) / gridWorldSize.y);

        // Subtracting from y because of 0 indexing of the grid array

        int x = Mathf.RoundToInt((gridNodeSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridNodeSizeY - 1) * percentY);

        return grid[x, y];
    }

    public List<NodeScript> getNeighbours(NodeScript node)
    {
        List<NodeScript> neighbours = new List<NodeScript>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if ((checkX >= 0 && checkX < gridNodeSizeX) && (checkY >= 0 && checkY < gridNodeSizeY))
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    // List to trace out the path
    public List<NodeScript> path;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3( gridWorldSize.x, 1, gridWorldSize.y ));

        if (grid != null)
        {
            foreach (NodeScript n in grid)
            {
                Gizmos.color = (n.walkable) ? (Color.white) : (Color.red);              
                if (path != null)
                    if (path.Contains(n))
                        Gizmos.color = Color.black;

                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}
