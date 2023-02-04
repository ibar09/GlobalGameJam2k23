using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
public class Player : MonoBehaviour
{
    public enum Direction
    {
        UP, DOWN, LEFT, RIGHT
    }
    [SerializeField]
    private float x = 0f;
    Direction direction;
    public Transform headTransform;
    public SpriteShapeController spriteShapeController;
    private float speed = 0.005f;
    private Vector2? targetPosition = null;

    // Start is called before the first frame update
    void Start()
    {

        spriteShapeController = GetComponent<SpriteShapeController>();
        gameManager.players.Add(this);
        headTransform = transform.GetChild(0);
    }
    private void Update()
    {
        if (targetPosition != null)
        {
            Vector2 currentPosition = spriteShapeController.spline.GetPosition(0);
            if (currentPosition != targetPosition)
            {
                Vector3 nextPosition = Vector2.MoveTowards(currentPosition, (Vector2)targetPosition, speed);
                if (Vector3.Distance(spriteShapeController.spline.GetPosition(1), nextPosition) <= 0.2f)
                {
                    nextPosition = spriteShapeController.spline.GetPosition(1);
                    spriteShapeController.spline.RemovePointAt(0);
                }

                spriteShapeController.spline.SetPosition(0, nextPosition);
                headTransform.localPosition = nextPosition;

            }
            if (currentPosition == targetPosition)
            {
                targetPosition = null;
                gameManager.NextTurn();
            }
        }
    }


    public Direction DirectionRng()
    {
        float downWeight = 0.25f + x;
        float percentage = Random.Range(0, 1f);
        Debug.Log(percentage);
        if (percentage <= downWeight)
        {
            x = 0;
            return Direction.DOWN;
        }
        else
        {
            x += 0.05f;
            int newRng = Random.Range(1, 4);
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


    void MoveBy(int steps, Direction direction)
    {
        switch (direction)
        {
            case Direction.UP:
                targetPosition = spriteShapeController.spline.GetPosition(0) + new Vector3(0, steps, 0);
                break;
            case Direction.DOWN:
                targetPosition = spriteShapeController.spline.GetPosition(0) + new Vector3(0, -steps, 0);
                break;
            case Direction.LEFT:
                targetPosition = spriteShapeController.spline.GetPosition(0) + new Vector3(-steps, 0, 0);
                break;
            case Direction.RIGHT:
                targetPosition = spriteShapeController.spline.GetPosition(0) + new Vector3(steps, 0, 0);
                break;
        }
        Debug.Log(((Vector2)targetPosition).y);
        if (((Vector2)targetPosition).y > -1)
        {
            targetPosition = new Vector2(((Vector2)targetPosition).x, -1);
            Debug.Log(((Vector2)targetPosition).y);
        }


        if ((direction == Direction.UP || direction == Direction.DOWN) != (this.direction == Direction.UP || this.direction == Direction.DOWN))
        {
            spriteShapeController.spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            spriteShapeController.spline.InsertPointAt(0, Vector2.MoveTowards(spriteShapeController.spline.GetPosition(0), (Vector2)targetPosition, 0.2f));
        }
        this.direction = direction;
    }

    public GameManager gameManager;
    public int id;

    public void Move(int steps)
    {

        MoveBy(steps, DirectionRng());
    }


}
