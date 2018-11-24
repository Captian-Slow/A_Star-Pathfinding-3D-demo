using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingScript : MonoBehaviour {

    GridScript grid;
    public Transform seeker;
    public Transform target;

    void Update()
    {
        FindPath(seeker.position, target.position);
    }

    void Awake()
    {
        grid = GetComponent<GridScript>();    
    }

    void FindPath (Vector3 startPos, Vector3 targetPos)
    {
        NodeScript startNode = grid.getNodeFromWorldPoint(startPos);
        NodeScript targetNode = grid.getNodeFromWorldPoint(targetPos);

        List<NodeScript> openNodes = new List<NodeScript>();
        HashSet<NodeScript> closedNodes = new HashSet<NodeScript>();

        openNodes.Add(startNode);

        while (openNodes.Count > 0)
        {
            // Initially set the current node to the startNode.
            NodeScript currentNode = openNodes[0];

            // Find and assign new currentNode.
            for (int i = 1; i < openNodes.Count; i++)
            {
                if (openNodes[i].fCost < currentNode.fCost || (openNodes[i].fCost == currentNode.fCost  &&  openNodes[i].hCost < currentNode.hCost))
                {
                    currentNode = openNodes[i];
                }
            }

            // Remove the new currentNode from openNodes and add it to the closed nodes.
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            // Target reached
            if (currentNode == targetNode)
            {                
                RetracePath(startNode, targetNode);
                return;
            }

            // Loop through each of the neihbouring nodes of the current node.
            foreach (NodeScript n in grid.getNeighbours(currentNode))
            {
                if (!n.walkable || closedNodes.Contains(n))
                    continue;

                int newMovementCostToNeighbour = currentNode.gCost + getDistance(currentNode, n);
                if (newMovementCostToNeighbour < n.gCost  ||  !openNodes.Contains(n))
                {
                    n.gCost = newMovementCostToNeighbour;
                    n.hCost = getDistance(n, targetNode);
                    n.parent = currentNode;

                    if (!openNodes.Contains(n))
                        openNodes.Add(n);
                }
            }
        
        }
    }

    void RetracePath(NodeScript startNode, NodeScript targetNode)
    {
        List<NodeScript> path = new List<NodeScript>();
        NodeScript currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        // Debug.Log(path.Count);
        path.Reverse();
        grid.path = path;
    }

    int getDistance(NodeScript nodeA, NodeScript nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
        {
            return distY * 14 + 10 * (distX - distY);
        }

        return distX * 14 + 10 * (distY - distX);
    }
}
