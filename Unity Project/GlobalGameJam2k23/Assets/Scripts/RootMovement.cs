using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class RootMovement : MonoBehaviour
{
    public SpriteShapeController spriteShapeController;
    private Direction direction = Direction.Down;
    private float speed = 0.005f;
    private Vector2? targetPosition = null;

    // Start is called before the first frame update
    void Start()
    {
        spriteShapeController.spline.SetTangentMode(0, ShapeTangentMode.Continuous);
        spriteShapeController.spline.SetTangentMode(1, ShapeTangentMode.Continuous);
    }

    // Update is called once per frame
    void Update()
    {
        // test
        if (Input.GetKey(KeyCode.S) && targetPosition == null)
        {
            // move by random steps from 1 to 6 and random direction
            moveBy(Random.Range(1, 6), (Direction)Random.Range(0, 4));
        }

        if (targetPosition != null)
        {
            Vector2 currentPosition = spriteShapeController.spline.GetPosition(0);
            if (currentPosition != targetPosition)
            {
                Vector3 nextPosition = Vector2.MoveTowards(currentPosition, (Vector2)targetPosition, speed);
                if (spriteShapeController.spline.GetPosition(1) == nextPosition)
                {
                    spriteShapeController.spline.RemovePointAt(0);
                }
                else
                {
                    spriteShapeController.spline.SetPosition(0, nextPosition);
                }
            }
            if (currentPosition == targetPosition)
            {
                targetPosition = null;
            }
        }
    }

    void moveBy(int steps, Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                targetPosition = spriteShapeController.spline.GetPosition(0) + new Vector3(0, steps, 0);
                break;
            case Direction.Down:
                targetPosition = spriteShapeController.spline.GetPosition(0) + new Vector3(0, -steps, 0);
                break;
            case Direction.Left:
                targetPosition = spriteShapeController.spline.GetPosition(0) + new Vector3(-steps, 0, 0);
                break;
            case Direction.Right:
                targetPosition = spriteShapeController.spline.GetPosition(0) + new Vector3(steps, 0, 0);
                break;
        }

        if ((direction == Direction.Up || direction == Direction.Down) != (this.direction == Direction.Up || this.direction == Direction.Down))
        {
            spriteShapeController.spline.InsertPointAt(0, Vector2.MoveTowards(spriteShapeController.spline.GetPosition(0), (Vector2)targetPosition, 0.2f));
            spriteShapeController.spline.SetTangentMode(0, ShapeTangentMode.Continuous);
        }
        this.direction = direction;
    }
}
