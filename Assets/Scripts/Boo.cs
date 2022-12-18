using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boo : MonoBehaviour
{
    public float Speed = 5.0f;

    private bool[,] _maze;
    private Vector2Int _position;
    private float _blockScale = 2.0f;

    private enum Direction {
        None,
        Up,
        Down,
        Left,
        Right
    };

    private List<Direction> _possibleDirections;

    private Direction _direction = Direction.None;

    private Direction _previousDirection = Direction.Left;

    private Vector3 _target = new();

    // Start is called before the first frame update
    void Start()
    {
        _target = transform.position;

        transform.GetChild(0).transform.GetChild(0).GetComponent<Animation>()["Take 001"].speed = 10.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_position != null && _maze != null)
        {
            if (_direction == Direction.None)
            {
                _possibleDirections = new List<Direction>();

                if (_previousDirection != Direction.Right && !_maze[_position.x - 1, _position.y])
                {
                    _possibleDirections.Add(Direction.Left);
                }

                if (_previousDirection != Direction.Left && !_maze[_position.x + 1, _position.y])
                {
                    _possibleDirections.Add(Direction.Right);
                }

                if (_previousDirection != Direction.Up && !_maze[_position.x, _position.y - 1])
                {
                    _possibleDirections.Add(Direction.Down);
                }

                if (_previousDirection != Direction.Down && !_maze[_position.x, _position.y + 1])
                {
                    _possibleDirections.Add(Direction.Up);
                }

                if (_possibleDirections.Count == 0)
                {
                    if (!_maze[_position.x - 1, _position.y])
                    {
                        _possibleDirections.Add(Direction.Left);
                    }
                    else if (!_maze[_position.x + 1, _position.y])
                    {
                        _possibleDirections.Add(Direction.Right);
                    }
                    else if (!_maze[_position.x, _position.y - 1])
                    {
                        _possibleDirections.Add(Direction.Down);
                    }
                    else if (!_maze[_position.x, _position.y + 1])
                    {
                        _possibleDirections.Add(Direction.Up);
                    }
                    else
                    {
                        // The enemy is enclosed by 4 walls
                        return;
                    }
                }

                // Get a random direction from the 4 possible directions.
                _direction = _possibleDirections[Random.Range(0, _possibleDirections.Count)];

                var outerContainer = transform.GetChild(0);
                switch (_direction)
                {
                    case Direction.Left:
                        _position.x--;
                        _target = GetTarget(GetComponent<SphereCollider>().radius);
                        if (_previousDirection == Direction.Up)
                        {
                            Vector3 center = GetComponent<SphereCollider>().bounds.center;
                            outerContainer.transform.RotateAround(center, new Vector3(0f, 1f, 0f), 90);
                        }
                        else if (_previousDirection == Direction.Down)
                        {
                            Vector3 center = GetComponent<SphereCollider>().bounds.center;
                            outerContainer.transform.RotateAround(center, new Vector3(0f, 1f, 0f), -90);
                        }
                        else if (_previousDirection == Direction.Right)
                        {
                            Vector3 center = GetComponent<SphereCollider>().bounds.center;
                            outerContainer.transform.RotateAround(center, new Vector3(0f, 1f, 0f), 180);
                        }
                        break;
                    case Direction.Right:
                        _position.x++;
                        _target = GetTarget(GetComponent<SphereCollider>().radius);
                        if (_previousDirection == Direction.Up)
                        {
                            Vector3 center = GetComponent<SphereCollider>().bounds.center;
                            outerContainer.transform.RotateAround(center, new Vector3(0f, 1f, 0f), -90);
                        }
                        else if (_previousDirection == Direction.Down)
                        {
                            Vector3 center = GetComponent<SphereCollider>().bounds.center;
                            outerContainer.transform.RotateAround(center, new Vector3(0f, 1f, 0f), 90);
                        }
                        else if (_previousDirection == Direction.Left)
                        {
                            Vector3 center = GetComponent<SphereCollider>().bounds.center;
                            outerContainer.transform.RotateAround(center, new Vector3(0f, 1f, 0f), -180);
                        }
                        break;
                    case Direction.Up:
                        _position.y++;
                        _target = GetTarget(GetComponent<SphereCollider>().radius);
                        if (_previousDirection == Direction.Left)
                        {
                            Vector3 center = GetComponent<SphereCollider>().bounds.center;
                            outerContainer.transform.RotateAround(center, new Vector3(0f, 1f, 0f), -90);
                        }
                        else if (_previousDirection == Direction.Right)
                        {
                            Vector3 center = GetComponent<SphereCollider>().bounds.center;
                            outerContainer.transform.RotateAround(center, new Vector3(0f, 1f, 0f), 90);
                        }
                        else if (_previousDirection == Direction.Down)
                        {
                            Vector3 center = GetComponent<SphereCollider>().bounds.center;
                            outerContainer.transform.RotateAround(center, new Vector3(0f, 1f, 0f), 180);
                        }
                        break;
                    case Direction.Down:
                        _position.y--;
                        _target = GetTarget(GetComponent<SphereCollider>().radius);
                        if (_previousDirection == Direction.Left)
                        {
                            Vector3 center = GetComponent<SphereCollider>().bounds.center;
                            outerContainer.transform.RotateAround(center, new Vector3(0f, 1f, 0f), 90);
                        }
                        else if (_previousDirection == Direction.Right)
                        {
                            Vector3 center = GetComponent<SphereCollider>().bounds.center;
                            outerContainer.transform.RotateAround(center, new Vector3(0f, 1f, 0f), -90);
                        }
                        else if (_previousDirection == Direction.Up)
                        {
                            Vector3 center = GetComponent<SphereCollider>().bounds.center;
                            outerContainer.transform.RotateAround(center, new Vector3(0f, 1f, 0f), -180);
                        }
                        break;
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, _target, Speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _target) < 0.001f)
            {
                _previousDirection = _direction;
                _direction = Direction.None;
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            SceneManager.LoadScene("Dead");
        }
    }

    private Vector3 GetTarget(float radius)
    {
        return new Vector3(_position.y * _blockScale  + (_blockScale / 2 - radius), 1 * _blockScale, _position.x * _blockScale + (_blockScale / 2));
    }

    public void SetMaze(bool[,] maze, Vector2Int position, float blockScale)
    {
        _maze = maze;
        _position = position;
        _blockScale = blockScale;
    }
}
