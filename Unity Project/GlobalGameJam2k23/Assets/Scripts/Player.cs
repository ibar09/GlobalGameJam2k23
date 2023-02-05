using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.U2D;
using TMPro;
public class Player : MonoBehaviour
{
    public enum Direction
    {
        UP, DOWN, LEFT, RIGHT
    }
    [SerializeField]
    public float x = 0f;
    public Transform headTransform;
    public SpriteShapeController spriteShapeController;
    public EdgeCollider2D edgeCollider;
    private float speed = 0.01f;
    private Vector2? targetPosition = null;
    public GameManager gameManager;
    public float animationSpeed;
    public TextMeshProUGUI directionText;
    public bool isImmune = false;
    public bool isDownBoosted = false;
    public int immunityCD = 0;
    public int downBoostCD = 0;

    Direction direction;

    // Start is called before the first frame update
    void Start()
    {

        spriteShapeController = GetComponent<SpriteShapeController>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        gameManager.players.Add(this);
        headTransform = transform.GetChild(0);
    }
    private void Update()
    {
        if (targetPosition == null)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                direction = Direction.UP;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                direction = Direction.DOWN;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                direction = Direction.LEFT;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                direction = Direction.RIGHT;
            }
        }

        if (targetPosition != null)
        {
            Vector2 currentPosition = spriteShapeController.spline.GetPosition(0);
            if (currentPosition != targetPosition)
            {
                Vector3 nextPosition = Vector2.MoveTowards(currentPosition, (Vector2)targetPosition, speed);
                Vector3 prevPointPosition = spriteShapeController.spline.GetPosition(1);
                if (Vector3.Distance(prevPointPosition, nextPosition) <= 0.2f)
                {
                    if (prevPointPosition == targetPosition)
                    {
                        nextPosition = prevPointPosition;
                        spriteShapeController.spline.RemovePointAt(0);
                    }
                    else
                    {
                        nextPosition = Vector2.MoveTowards(prevPointPosition, (Vector2)targetPosition, 0.2f);
                        Direction currentDirection = ComputeDirection(spriteShapeController.spline.GetPosition(2), prevPointPosition);
                        Direction nextDirection = ComputeDirection(prevPointPosition, nextPosition);
                        setTangent(1, currentDirection, nextDirection);
                    }
                }

                spriteShapeController.spline.SetPosition(0, nextPosition);
                currentPosition = nextPosition;
                updateEdgeCollider();
                headTransform.localPosition = nextPosition;
            }
            if (currentPosition == targetPosition)
            {
                OnMoveEnd();
            }
        }
    }

    void OnMoveEnd()
    {
        targetPosition = null;
        gameManager.NextTurn();
        gameManager.ActivateControls();
    }
    public void SetPosition(Vector2 position)
    {
        var localPosition = position - (Vector2)transform.position;
        spriteShapeController.spline.SetPosition(0, localPosition);
        headTransform.localPosition = localPosition;
        updateEdgeCollider();
    }

    public Direction DirectionRng()
    {
        float downWeight = 0.25f + x;
        float percentage = UnityEngine.Random.Range(0, 1f);
        Debug.Log(percentage);
        if (percentage <= downWeight)
        {
            x = 0;
            return Direction.DOWN;
        }
        else
        {
            x += 0.05f;
            int newRng = UnityEngine.Random.Range(1, 4);
            Debug.Log(newRng);
            switch (newRng)
            {
                case 1:
                    return Direction.UP;

                case 2:
                    return Direction.LEFT;

                case 3:
                    return Direction.RIGHT;
                default:
                    throw new System.Exception();

            }
        }
    }


    public void MoveBy(int steps, Direction direction)
    {
        Vector3 currentPosition = spriteShapeController.spline.GetPosition(0);
        switch (direction)
        {
            case Direction.UP:
                targetPosition = currentPosition + new Vector3(0, steps, 0);
                break;
            case Direction.DOWN:
                targetPosition = currentPosition + new Vector3(0, -steps, 0);
                break;
            case Direction.LEFT:
                targetPosition = currentPosition + new Vector3(-steps, 0, 0);
                break;
            case Direction.RIGHT:
                targetPosition = currentPosition + new Vector3(steps, 0, 0);
                break;
        }
        targetPosition = new Vector2(Mathf.Clamp(((Vector2)targetPosition).x, gameManager.GetCenter().x - transform.position.x - gameManager.width / 2 + 0.5f, gameManager.GetCenter().x - transform.position.x + gameManager.width / 2 - 0.5f), ((Vector2)targetPosition).y);

        if (((Vector2)targetPosition).y > -1)
        {
            targetPosition = new Vector2(((Vector2)targetPosition).x, -1);
        }

        if (targetPosition == currentPosition)
        {
            OnMoveEnd();
            return;
        }

        Debug.Log("targetPosition: x: " + ((Vector2)targetPosition).x + " y: " + ((Vector2)targetPosition).y);

        Vector3 prevPointPosition = spriteShapeController.spline.GetPosition(1);
        Direction currentDirection = ComputeDirection(prevPointPosition, currentPosition);

        setTangent(0, currentDirection, direction);

        Debug.Log("----------------");
        Debug.Log("currentDirection: " + currentDirection + " direction: " + direction);
        Debug.Log("prevPointPosition: x: " + prevPointPosition.x + " y: " + prevPointPosition.y + " currentPosition: x: " + currentPosition.x + " y: " + currentPosition.y);
        Debug.Log("----------------");

        if (!areSimilarDirections(currentDirection, direction))
        {
            spriteShapeController.spline.InsertPointAt(0, Vector2.MoveTowards(spriteShapeController.spline.GetPosition(0), (Vector2)targetPosition, 0.2f));
        }
    }

    private static bool areSimilarDirections(Direction prevDirection, Direction nextDirection)
    {
        return (prevDirection == Direction.UP || prevDirection == Direction.DOWN) == (nextDirection == Direction.UP || nextDirection == Direction.DOWN);
    }

    void setTangent(int index, Direction prevDirection, Direction nextDirection)
    {
        Debug.Log("prevDirection: " + prevDirection + " nextDirection: " + nextDirection);
        if (!areSimilarDirections(prevDirection, nextDirection))
        {
            spriteShapeController.spline.SetTangentMode(index, ShapeTangentMode.Continuous);
            if ((prevDirection == Direction.UP && nextDirection == Direction.RIGHT) || (prevDirection == Direction.RIGHT && nextDirection == Direction.UP))
            {
                spriteShapeController.spline.SetLeftTangent(index, new Vector3(0.25f, 0.25f, 0));
                spriteShapeController.spline.SetRightTangent(index, new Vector3(-0.25f, -0.25f, 0));
            }
            else if ((prevDirection == Direction.LEFT && nextDirection == Direction.DOWN) || (prevDirection == Direction.DOWN && nextDirection == Direction.LEFT))
            {
                spriteShapeController.spline.SetLeftTangent(index, new Vector3(-0.25f, -0.25f, 0));
                spriteShapeController.spline.SetRightTangent(index, new Vector3(0.25f, 0.25f, 0));
            }
            else if ((prevDirection == Direction.DOWN && nextDirection == Direction.RIGHT) || (prevDirection == Direction.RIGHT && nextDirection == Direction.DOWN))
            {
                spriteShapeController.spline.SetLeftTangent(index, new Vector3(0.25f, -0.25f, 0));
                spriteShapeController.spline.SetRightTangent(index, new Vector3(-0.25f, 0.25f, 0));
            }
            else if ((prevDirection == Direction.UP && nextDirection == Direction.LEFT) || (prevDirection == Direction.LEFT && nextDirection == Direction.UP))
            {
                spriteShapeController.spline.SetLeftTangent(index, new Vector3(-0.25f, 0.25f, 0));
                spriteShapeController.spline.SetRightTangent(index, new Vector3(0.25f, -0.25f, 0));
            }
        }
    }

    private static Direction ComputeDirection(Vector3 prevPosition, Vector3 nextPosition)
    {
        if (MathF.Abs(nextPosition.x - prevPosition.x) < MathF.Abs(nextPosition.y - prevPosition.y))
        {
            if (nextPosition.y < prevPosition.y)
            {
                return Direction.DOWN;
            }
            else
            {
                return Direction.UP;
            }
        }
        else
        {
            if (nextPosition.x < prevPosition.x)
            {
                return Direction.LEFT;
            }
            else
            {
                return Direction.RIGHT;
            }
        }
    }

    public void Move(int steps)
    {
        MoveBy(steps, direction);
        // StartCoroutine(DirectionChooser(steps));
    }




    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isImmune && gameManager.currentPlayer != this && other.gameObject.CompareTag("Head") && other.gameObject != headTransform.gameObject)
        {
            Vector2 collisionPoint = other.gameObject.transform.position - transform.position;
            Debug.Log("Collision point, x: " + collisionPoint.x + " y: " + collisionPoint.y);
            for (int i = spriteShapeController.spline.GetPointCount() - 1; i > 0; i--)
            {
                // check if collisionPoint is between i and i-1 with a margin of 0.2f
                Vector2 point1 = spriteShapeController.spline.GetPosition(i);
                Vector2 point2 = spriteShapeController.spline.GetPosition(i - 1);
                Debug.Log("Point1, x: " + point1.x + " y: " + point1.y);
                Debug.Log("Point2, x: " + point2.x + " y: " + point2.y);
                if (MathF.Abs(point1.x - point2.x) < 0.2f)
                {
                    if ((collisionPoint.x < (point1.x + 0.2f)) && (collisionPoint.x > (point1.x - 0.2f)))
                    {
                        collisionPoint.x = point1.x;
                    }
                }
                else if (MathF.Abs(point1.y - point2.y) < 0.2f)
                {
                    if ((collisionPoint.y < (point1.y + 0.2f)) && (collisionPoint.y > (point1.y - 0.2f)))
                    {
                        collisionPoint.y = point1.y;
                    }
                }
                Debug.Log("Collision point, x: " + collisionPoint.x + " y: " + collisionPoint.y);
                if (Vector2.Distance(point1, collisionPoint) + Vector2.Distance(point2, collisionPoint) <= Vector2.Distance(point1, point2) + 0.01f)
                {
                    int removeAt;
                    // create a new point at collisionPoint if it's not equal to point1 or point2
                    if (collisionPoint != point1 && collisionPoint != point2)
                    {
                        spriteShapeController.spline.InsertPointAt(i, collisionPoint);
                        removeAt = i - 1;
                    }
                    else if (collisionPoint == point1)
                    {
                        removeAt = i - 1;
                    }
                    else
                    {
                        removeAt = i - 2;
                    }
                    Debug.Log("Remove at: " + removeAt);
                    // remove all points before removeAt (inclusive)
                    while (removeAt >= 0)
                    {
                        spriteShapeController.spline.RemovePointAt(removeAt);
                        removeAt--;
                    }

                    headTransform.localPosition = collisionPoint;
                    updateEdgeCollider();
                    break;
                }
            }
        }
    }

    void updateEdgeCollider()
    {
        Vector2[] points = new Vector2[spriteShapeController.spline.GetPointCount()];
        for (int i = 0; i < spriteShapeController.spline.GetPointCount(); i++)
        {
            points[i] = spriteShapeController.spline.GetPosition(i);
        }
        edgeCollider.points = points;
    }




    IEnumerator DirectionChooser(int steps)
    {
        float x = gameManager.animationSpeed;
        gameManager.DisactivateControls();
        while (gameManager.animationSpeed < 0.65f)
        {
            foreach (string i in Enum.GetNames(typeof(Direction)))
            {
                gameManager.directionText.text = i;
                yield return new WaitForSeconds(gameManager.animationSpeed);
                gameManager.animationSpeed *= 1.2f;

            }


        }
        gameManager.animationSpeed = x;
        Direction d = DirectionRng();
        gameManager.directionText.text = d.ToString();
        MoveBy(steps, d);

    }



}
