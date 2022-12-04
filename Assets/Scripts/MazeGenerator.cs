using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    public GameObject CeilingPrefab;

    public GameObject FloorParent;
    public GameObject WallParent;
    public GameObject CeilingParent;
    public GameObject PlayerController;

    // usar un techo
    public bool GenerateCeiling = false;
    public bool RandomCeiling = true;
    public bool GenerateFullCeiling = true;
    public bool GenerateFullFloor = true;
    // tamaño del laberinto
    public Vector2Int MazeSize = new(3, 3);

    public float BlockScale = 1.0f;

    // un array de tipo 2D que contiene el laberinto
    private bool[,] _maze;

    // cuadros a quitar
    private int _tilesToRemove = 1;

    private Vector2Int _mazeCoords = new(4, 1);

    private bool _playerPlaced = false;

    private bool[,] _spawnPoints;
    // the unsafe spawn points may not be reachable, goals should not be placed in them
    private bool[,] _unsafeSpawnPoints;

    public void Start()
    {
        if (FloorPrefab == null)
        {
            
        }

        bool createCeiling = false;
        if (GenerateCeiling)
        {
            createCeiling = true;
            if (RandomCeiling)
            {
            createCeiling = Random.value < 0.5;
            }
        }

        // asegurar que la cantidad de tiles a quitar existen en el laberinto
        if ((MazeSize.x - 2) * (MazeSize.y - 2) < _tilesToRemove)
        {
            return;
        }

        _maze = GenerateMaze();

		for (int x = 0; x < MazeSize.x; x++)
        {
			for (int y = 0; y < MazeSize.y; y++)
            {
                if (!_maze[x, y] || GenerateFullFloor)
                {
				    CreateChildPrefab(FloorPrefab, FloorParent, y * BlockScale, 0 * BlockScale, x * BlockScale);
                }

				if (_maze[x, y])
                {
					CreateChildPrefab(WallPrefab, WallParent, y * BlockScale, 1 * BlockScale, x * BlockScale);
					CreateChildPrefab(WallPrefab, WallParent, y * BlockScale, 2 * BlockScale, x * BlockScale);
					//CreateChildPrefab(WallPrefab, WallParent, y * BlockScale, 3 * BlockScale, x * BlockScale);
				}
                else if (!_playerPlaced && _spawnPoints[x, y])
                {
					PlayerController.transform.SetPositionAndRotation(
						new Vector3(x * BlockScale, 1 * BlockScale, y * BlockScale), Quaternion.identity
					);

					_playerPlaced = true;
				}

				if (createCeiling)
                {
                    if (!_maze[x, y] || GenerateFullCeiling)
                    {
					    CreateChildPrefab(CeilingPrefab, CeilingParent, y * BlockScale, 3 * BlockScale, x * BlockScale);
                    }
				}
			}
		}
    }

    private bool[,] GenerateMaze()
    {
        bool[,] maze = new bool[MazeSize.x, MazeSize.y];
        _spawnPoints = new bool[MazeSize.x, MazeSize.y];

        _tilesToRemove = Mathf.FloorToInt((MazeSize.x - 2) * (MazeSize.y - 2) * 0.40f);

        // inicializar con todos los muros
        for (int x = 0; x < MazeSize.x; x++)
        {
            for (int y = 0; y < MazeSize.y; y++)
            {
                maze[x, y] = true;
                _spawnPoints[x, y] = false;
            }
        }

        int tilesRemoved = 0;

        while (tilesRemoved < _tilesToRemove)
        {
            int xDirection = 0;
            int yDirection = 0;

            if (Random.value < 0.5)
            {
                xDirection = Random.value < 0.5 ? 1 : -1;
            }
            else
            {
                yDirection = Random.value < 0.5 ? 1 : -1;
            }

            int tilesToTraverse = xDirection > 0 ? Random.Range(1, MazeSize.x / 2 - 1) : Random.Range(1, MazeSize.y / 2 - 1);

            for (int i = 0; i < tilesToTraverse; i++)
            {
                _mazeCoords.x = System.Math.Clamp(_mazeCoords.x + xDirection, 1, MazeSize.x - 2);
                _mazeCoords.y = System.Math.Clamp(_mazeCoords.y + yDirection, 1, MazeSize.y - 2);

                if (maze[_mazeCoords.x, _mazeCoords.y])
                {
                    maze[_mazeCoords.x, _mazeCoords.y] = false;
                    _spawnPoints[_mazeCoords.x, _mazeCoords.y] = true;
                    tilesRemoved++;
                }
            }
        }

        _unsafeSpawnPoints = _spawnPoints;

        // optimize maze by removing excess blocks
        for (int x = 1; x < MazeSize.x - 1; x++)
        {
            for (int y = 1; y < MazeSize.y - 1; y++)
            {
                if (maze[x - 1, y] && maze[x + 1, y] && maze[x, y - 1] && maze[x, y + 1])
                {
                    // a wall surrounded by 4 other walls
                    RemoveXTillCorridor(maze, new(x, y));
                }
            }
        }

        return maze;
    }

    private bool[,] RemoveXTillCorridor(bool[,] maze, Vector2Int startPoint)
    {
        int x = startPoint.x;
        while (x < MazeSize.x - 1 && maze[x + 1, startPoint.y])
        {
            if (maze[x, startPoint.y + 1] && maze[x, startPoint.y - 1] && maze[x + 1, startPoint.y])
            {
                maze[x, startPoint.y] = false;
                _unsafeSpawnPoints[x, startPoint.y] = true;
                maze = RemoveYTillCorridor(maze, new(x, startPoint.y));
            }
            x++;
        }

        return maze;
    }

    // this function intentionally doesn't traverse the whole grid so that it can make possible rooms
    private bool[,] RemoveYTillCorridor(bool[,] maze, Vector2Int startPoint)
    {
        int y = startPoint.y + 1;
        while (y < MazeSize.y - 1 && maze[startPoint.x, y + 1])
        {
            maze[startPoint.x, y] = false;
            _unsafeSpawnPoints[startPoint.x, y] = true;
            y++;
        }

        return maze;
    }

    private void CreateChildPrefab(GameObject prefab, GameObject parent, float x, float y, float z) {
		var myPrefab = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
		myPrefab.transform.parent = parent.transform;
	}
}
