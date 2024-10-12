using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node
{
	public Vector3 pos;
	public float cost;
	public int numVisits;
	public Vector3 prev;
}

public class PQueue
{
	
}
public class DStarAlgorithm : MonoBehaviour {

	//3D grid of obstacles
	public bool[,,] obstacles;
	//3D grid of nodes
	public Node[,,] nodes;

	//max dimensions of 
	public int length;
	public int width;
	public int height;

	

	//starting location of the drone
	public Vector3 start;
	//target location of the drone
	public Vector3 target;
	


	//drone GameObject
	public GameObject drone;
	public GameObject goal;
	//wall Prefab
	public GameObject wall;
	// number of wall we want to initialize
	public int num_walls;
	void Start () {
		// Get the GameObject's Transform component
		Transform droneTransform = drone.GetComponent<Transform>();

		// Set the position of the GameObject to the start location
		droneTransform.position = start;

		// Get the GameObject's Transform component
		Transform goalTransform = goal.GetComponent<Transform>();

		// Set the position of the GameObject to the target location
		goalTransform.position = target;

		//set size pf obstacles array
		obstacles = new bool[length + 1, width + 1, height + 1];
		nodes = new Node[length + 1, width + 1, height + 1];


		// InitializeWalls(num_walls);
		BuildAWall();
		InitializeCosts();
	}
	
	// Update is called once per frame
	void Update () {
		Transform droneTransform = drone.GetComponent<Transform>();
		if (Input.GetKeyDown("space") && droneTransform.position!=target)
		{
			// Get the GameObject's Transform component
			Transform transform = drone.GetComponent<Transform>();

			// Set the position of the GameObject to the start location
			// transform.position += new Vector3(0, 2, 0);
			UpdateCosts();
			Debug.Log("space key was pressed");

			// Recalculate next lowest cost and move to that position

		}
	}

	void InitializeWalls(int num_walls)
	{

		// Loop through the number of objects to allocate
		for (int i = 0; i < num_walls; i++)
		{
			// Generate random position within the map boundaries
			Vector3 randomPosition = new Vector3(Random.Range(0, width), Random.Range(0, height), Random.Range(0, length));

			// Check if the position is already occupied by another object
			Collider[] colliders = Physics.OverlapSphere(randomPosition, 0.5f);
			if (colliders.Length == 0 && randomPosition != start && randomPosition != target) // If no colliders (objects) are found at the position
			{
				// Instantiate the object at the random position
				Instantiate(wall, randomPosition, Quaternion.identity);
				obstacles[(int)randomPosition.x, (int)randomPosition.y, (int)randomPosition.z] = true;
			}
			else
			{
				// Try again to find a new random position
				i--; // Decrement i to try again for the current object
			}
		}
	}

	void BuildAWall()
	{
		for (int i = 1; i < length - 1; i++)
		{
			for (int j = 1; j < width - 1; j++)
			{
				for (int k = 1; k < height - 1; k++)
				{
					if (!(((i >= 2 && i < length - 1) && (j >= 2 && j < width - 2) && (k >= 2 && k < height - 2))
						&& ((i == 5 && j == 5) || (i == 5 && k == 5) || (j == 5 && k == 5))))
					{
						Vector3 randomPosition = new Vector3(i, j, k);
						Instantiate(wall, randomPosition, Quaternion.identity);
						obstacles[(int)randomPosition.x, (int)randomPosition.y, (int)randomPosition.z] = true;
					}
				}
			}
		}
		

	}

	void InitializeCosts()
	{
		Transform transform = drone.GetComponent<Transform>();
		for (int x = 0; x <= length; x++)
		{
			for (int y = 0; y <= width; y++)
			{
				for (int z = 0; z <= height; z++)
				{
					nodes[x, y, z] = new Node();
					nodes[x, y, z].pos = new Vector3(x, y, z);
					nodes[x, y, z].cost = int.MaxValue; // Set costs of all nodes to infinity
					nodes[x, y, z].numVisits = 0;
				}
			}
		}
		
		nodes[(int)transform.position.x, (int)transform.position.y, (int)transform.position.z].cost = int.MaxValue; // Cost of Max to drone so that it doesn't stand still
		
	}

	/// <summary>
	/// 
	/// </summary>
	void UpdateCosts()
	{
		//InitializeCosts();
		Transform transform = drone.GetComponent<Transform>();
		int droneX = (int)transform.position.x;
		int droneY = (int)transform.position.y;
		int droneZ = (int)transform.position.z;
		List<Node> nbs = new List<Node>(); // valid neighbors
		List<Node> neighborObs = new List<Node>(); // valid neighbors who are obstacles
		

		// Calculate Costs for all neighbors
		if (CheckBounds(droneX + 1, droneY, droneZ))
		{
			nodes[droneX + 1, droneY, droneZ].cost = Vector3.Distance(nodes[droneX + 1, droneY, droneZ].pos, target);
			if (obstacles[droneX + 1, droneY, droneZ] == true)
			{
				//Add to temp list...if list size=5, then make current position an obstacle
				neighborObs.Add(nodes[droneX + 1, droneY, droneZ]);
				nodes[droneX + 1, droneY, droneZ].cost = float.MaxValue;
			}
			nbs.Add(nodes[droneX + 1, droneY, droneZ]);
		}
		if (CheckBounds(droneX - 1, droneY, droneZ))
		{
			nodes[droneX - 1, droneY, droneZ].cost = Vector3.Distance(nodes[droneX - 1, droneY, droneZ].pos, target);
			if (obstacles[droneX - 1, droneY, droneZ] == true)
			{
				//Add to temp list...if list size=5, then make current position an obstacle
				neighborObs.Add(nodes[droneX - 1, droneY, droneZ]);
				nodes[droneX - 1, droneY, droneZ].cost = float.MaxValue;
			}
			nbs.Add(nodes[droneX - 1, droneY, droneZ]);
		}
		if (CheckBounds(droneX, droneY + 1, droneZ))
		{
			nodes[droneX, droneY + 1, droneZ].cost = Vector3.Distance(nodes[droneX, droneY + 1, droneZ].pos, target);
			if (obstacles[droneX, droneY + 1, droneZ] == true)
			{
				//Add to temp list...if list size=5, then make current position an obstacle
				neighborObs.Add(nodes[droneX, droneY + 1, droneZ]);
				nodes[droneX, droneY + 1, droneZ].cost = float.MaxValue;
			}
			nbs.Add(nodes[droneX, droneY + 1, droneZ]);
		}
		if (CheckBounds(droneX, droneY - 1, droneZ))
		{
			nodes[droneX, droneY - 1, droneZ].cost = Vector3.Distance(nodes[droneX, droneY - 1, droneZ].pos, target);
			if (obstacles[droneX, droneY - 1, droneZ] == true)
			{
				//Add to temp list...if list size=5, then make current position an obstacle
				neighborObs.Add(nodes[droneX, droneY - 1, droneZ]);
				nodes[droneX, droneY - 1, droneZ].cost = float.MaxValue;
			}
			nbs.Add(nodes[droneX, droneY - 1, droneZ]);
		}
		if (CheckBounds(droneX, droneY, droneZ + 1))
		{

			nodes[droneX, droneY, droneZ + 1].cost = Vector3.Distance(nodes[droneX, droneY, droneZ + 1].pos, target);
			if (obstacles[droneX, droneY, droneZ + 1] == true)
			{
				//Add to temp list...if list size=5, then make current position an obstacle
				neighborObs.Add(nodes[droneX, droneY, droneZ + 1]);
				nodes[droneX, droneY, droneZ + 1].cost = float.MaxValue;
			}
			nbs.Add(nodes[droneX, droneY, droneZ + 1]);
			
		}
		if (CheckBounds(droneX, droneY, droneZ - 1))
		{
			nodes[droneX, droneY, droneZ - 1].cost = Vector3.Distance(nodes[droneX, droneY, droneZ - 1].pos, target);
			if (obstacles[droneX, droneY, droneZ - 1] == true)
			{
				//Add to temp list...if list size=5, then make current position an obstacle
				neighborObs.Add(nodes[droneX, droneY, droneZ - 1]);
				nodes[droneX, droneY, droneZ - 1].cost = float.MaxValue;
			}
			nbs.Add(nodes[droneX, droneY, droneZ - 1]);
		}





		// Find lowest cost of all neighbors and move drone to that position


		//Test making the current position an obstacle in this spot

		nodes[droneX, droneY, droneZ].cost = Vector3.Distance(nodes[droneX, droneY, droneZ].pos, target);
		float cst = int.MaxValue;
		Node tempNode = nodes[droneX, droneY, droneZ];
		foreach (Node z in nbs)
		{
			if (z.cost < cst)
			{
				tempNode = z;
				cst = z.cost;
			}
			
		}

		/*if (tempNode == nodes[droneX, droneY, droneZ])
		{

		}*/


		int drone2X = (int)tempNode.pos.x;
		int drone2Y = (int)tempNode.pos.y;
		int drone2Z = (int)tempNode.pos.z;
		// print("Number of Visits to This Node Before: " + nodes[drone2X, drone2Y, drone2Z].numVisits);


		// Algo 1
		//transform.position = tempNode.pos;

		//nodes[drone2X, drone2Y, drone2Z].numVisits += 1;
		// print("Number of Visits to This Node After: " + nodes[drone2X, drone2Y, drone2Z].numVisits);

		/*if (nodes[drone2X, drone2Y, drone2Z].numVisits >= 2)
		{
			obstacles[droneX, droneY, droneZ] = true;
			Instantiate(wall, new Vector3(droneX, droneY, droneZ), Quaternion.identity);
		}*/

		// Algo 2
		transform.position = tempNode.pos;
		/*if (transform.position == target)
		{
			return;
		}*/

		if (nodes[droneX, droneY, droneZ].cost < tempNode.cost)
		{
			obstacles[droneX, droneY, droneZ] = true;
			Instantiate(wall, new Vector3(droneX, droneY, droneZ), Quaternion.identity);
		}

		/*if (neighborObs.Count == (neighborObs.Count - 1))
		{
			//Add to temp list...if list size=5, then make current position an obstacle
			obstacles[droneX, droneY, droneZ] = true;
		}*/

		//Drone2 is after move positions




		/*if (obstacles[drone2X, drone2Y, drone2Z] == true)
		{
			print("Is Obstacle");
		}
		else
		{
			print("Not Obstacle");
		}*/


	}


	bool CheckBounds(int droneX, int droneY, int droneZ)
	{
		if (droneX <= length && droneY <= width && droneZ <= height && droneX >= 0 && droneY >= 0 && droneZ >= 0) // If no colliders (objects) are found at the position
		{
			// Instantiate the object at the random position
			return true;
		}
		return false;
	}

	void UpdateCostsBackup()
	{
		Transform transform = drone.GetComponent<Transform>();

		// Update Costs for all neighboring points
		for (int x = 0; x <= length; x++)
		{
			for (int y = 0; y <= width; y++)
			{
				for (int z = 0; z <= height; z++)
				{
					float distance = Vector3.Distance(nodes[x, y, z].pos, target);

					nodes[x, y, z].cost = 1; // Set costs of all nodes to infinity
				}
			}
		}


	}
}

