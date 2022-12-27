using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject PortalPrefab;
    public GameObject GemPrefab;
    public GameObject TrapPrefab;
    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    public GameObject CeilingPrefab;

    public GameObject MainParent;
    public GameObject EnemyParent;
    public GameObject TrapParent;
    public GameObject FloorParent;
    public GameObject WallParent;
    public GameObject CeilingParent;

    public GameObject EnemyPrefab;

    // usar un techo
    public bool Enemies = true;
    public bool Traps = true;
    public bool Walls = true;
    public bool GenerateCeiling = false;
    public bool RandomCeiling = true;
    public bool GenerateFullCeiling = true;
    public bool GenerateFullFloor = true;
    // tamaño del laberinto
    public Vector2Int MazeSize = new(10, 10);
    public bool RandomMazeSize = true;

    public float BlockScale = 1.0f;

    // un array de tipo 2D que contiene el laberinto
    private bool[,] _maze;

    private Vector2Int _mazeCoords = new(4, 1);

    private SpawnEntities[,] _spawnPoints;
    // the unsafe spawn points may not be reachable, goals should not be placed in them
    private SpawnEntities[,] _unsafeSpawnPoints;

    private enum SpawnEntities {
        Blocked,
        None,
        Player,
        Gem,
        Enemy,
        Portal,
        Trap
    }

    public void Start()
    {
        bool createCeiling = false;
        if (GenerateCeiling)
        {
            createCeiling = true;
            if (RandomCeiling)
            {
                createCeiling = Random.value < 0.5;
            }
        }

        _maze = GenerateMaze();

        Vector2 playerSpawnPoint = new();
		for (int x = 0; x < MazeSize.x; x++)
        {
			for (int y = 0; y < MazeSize.y; y++)
            {
                if (!_maze[x, y] || GenerateFullFloor)
                {
				    CreateChildPrefab(FloorPrefab, FloorParent, y * BlockScale, 0 * BlockScale, x * BlockScale);
                }

				if (Walls && _maze[x, y])
                {
					CreateChildPrefab(WallPrefab, WallParent, y * BlockScale, 1 * BlockScale, x * BlockScale);
					CreateChildPrefab(WallPrefab, WallParent, y * BlockScale, 2 * BlockScale, x * BlockScale);
					//CreateChildPrefab(WallPrefab, WallParent, y * BlockScale, 3 * BlockScale, x * BlockScale);
				}
                else if (_spawnPoints[x, y] == SpawnEntities.Player)
                {
                    playerSpawnPoint.x = x;
                    playerSpawnPoint.y = y;
				}
                else if (EnemyPrefab && _spawnPoints[x, y] == SpawnEntities.Enemy)
                {
                    var enemy = CreateChildPrefab(EnemyPrefab, EnemyParent, 0, 0, 0);

                    float width = enemy.GetComponent<SphereCollider>().radius;
                    enemy.transform.position = new Vector3(y * BlockScale + (BlockScale / 2 - width), 1 * BlockScale, x * BlockScale + (BlockScale / 2));
                    enemy.GetComponent<Boo>().SetMaze(_maze, new Vector2Int(x, y), BlockScale);
                }
                else if (_spawnPoints[x, y] == SpawnEntities.Trap)
                {
                    CreateChildPrefab(TrapPrefab, MainParent, y * BlockScale, 4 * BlockScale, x * BlockScale);
                }
                else if (_spawnPoints[x, y] == SpawnEntities.Gem)
                {
                    CreateChildPrefab(GemPrefab, MainParent, y * BlockScale, 1 * BlockScale, x * BlockScale);
                }
                else if (_spawnPoints[x, y] == SpawnEntities.Portal)
                {
                    CreateChildPrefab(PortalPrefab, MainParent, y * BlockScale, (1 * BlockScale / 2) - 0.2f, x * BlockScale);
                }

				if (createCeiling)
                {
                    if (_spawnPoints[x, y] != SpawnEntities.Trap && (!_maze[x, y] || GenerateFullCeiling))
                    {
					    CreateChildPrefab(CeilingPrefab, CeilingParent, y * BlockScale, 3 * BlockScale, x * BlockScale);
                    }
				}
			}
		}

        CreateChildPrefab(PlayerPrefab, MainParent, playerSpawnPoint.y * BlockScale, 2.0f * BlockScale, playerSpawnPoint.x * BlockScale);
    }

    private bool[,] GenerateMaze()
    {
        if (RandomMazeSize)
        {
            MazeSize.x = Random.Range(10, MazeSize.x);
            MazeSize.y = Random.Range(10, MazeSize.y);
        }
        bool[,] maze = new bool[MazeSize.x, MazeSize.y];
        _spawnPoints = new SpawnEntities[MazeSize.x, MazeSize.y];

        int tilesToRemove = Mathf.FloorToInt((MazeSize.x - 2) * (MazeSize.y - 2) * 0.40f);

        int enemiesToAdd = 0;
        if (Enemies)
        {
            enemiesToAdd = Mathf.FloorToInt(tilesToRemove * 0.01f);
            if (enemiesToAdd == 0)
            {
                enemiesToAdd = 1;
            }
        }

        int trapsToAdd = 0;
        if (Traps)
        {
            trapsToAdd = Mathf.FloorToInt(tilesToRemove * 0.01f);
            if (trapsToAdd == 0)
            {
                trapsToAdd = 1;
            }
        }

        // inicializar con todos los muros
        for (int x = 0; x < MazeSize.x; x++)
        {
            for (int y = 0; y < MazeSize.y; y++)
            {
                maze[x, y] = true;
                _spawnPoints[x, y] = SpawnEntities.Blocked;
            }
        }

        bool playerAdded = false;
        int tilesRemoved = 0;
        while (tilesRemoved < tilesToRemove)
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
                    if (!playerAdded)
                    {
                        _spawnPoints[_mazeCoords.x, _mazeCoords.y] = SpawnEntities.Player;
                        playerAdded = true;
                    }
                    else
                    {
                        _spawnPoints[_mazeCoords.x, _mazeCoords.y] = SpawnEntities.None;
                    }
                    tilesRemoved++;
                }
            }
        }

        while (true)
        {
            _mazeCoords.x = Random.Range((MazeSize.x - 2) / 8, MazeSize.x - 2);
            _mazeCoords.y = Random.Range((MazeSize.y - 2) / 8, MazeSize.y - 2);
            if (_spawnPoints[_mazeCoords.x, _mazeCoords.y] == SpawnEntities.None)
            {
                _spawnPoints[_mazeCoords.x, _mazeCoords.y] = SpawnEntities.Gem;
                break;
            }
        }

        while (true)
        {
            _mazeCoords.x = Random.Range((MazeSize.x - 2) / 8, MazeSize.x - 2);
            _mazeCoords.y = Random.Range((MazeSize.y - 2) / 8, MazeSize.y - 2);
            if (_spawnPoints[_mazeCoords.x, _mazeCoords.y] == SpawnEntities.None)
            {
                _spawnPoints[_mazeCoords.x, _mazeCoords.y] = SpawnEntities.Portal;
                break;
            }
        }

        while (enemiesToAdd > 0)
        {
            _mazeCoords.x = Random.Range((MazeSize.x - 2) / 2, MazeSize.x - 2);
            _mazeCoords.y = Random.Range((MazeSize.y - 2) / 2, MazeSize.y - 2);
            if (_spawnPoints[_mazeCoords.x, _mazeCoords.y] == SpawnEntities.None)
            {
                _spawnPoints[_mazeCoords.x, _mazeCoords.y] = SpawnEntities.Enemy;
                enemiesToAdd--;
            }
        }

        while (trapsToAdd > 0)
        {
            _mazeCoords.x = Random.Range((MazeSize.x - 2) / 8, MazeSize.x - 2);
            _mazeCoords.y = Random.Range((MazeSize.y - 2) / 8, MazeSize.y - 2);
            if (_spawnPoints[_mazeCoords.x, _mazeCoords.y] == SpawnEntities.None)
            {
                _spawnPoints[_mazeCoords.x, _mazeCoords.y] = SpawnEntities.Trap;
                trapsToAdd--;
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
                _unsafeSpawnPoints[x, startPoint.y] = SpawnEntities.None;
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
            _unsafeSpawnPoints[startPoint.x, y] = SpawnEntities.None;
            y++;
        }

        return maze;
    }

    private GameObject CreateChildPrefab(GameObject prefab, GameObject parent, float x, float y, float z) {
		var myPrefab = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
		myPrefab.transform.parent = parent.transform;
        return myPrefab;
	}
}
