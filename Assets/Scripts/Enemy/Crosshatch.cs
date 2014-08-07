using UnityEngine;
using System.Collections;

public class Crosshatch : Enemy {
    private const float LANE_CROSS_SPEED = 1f; // lanes per second
    private const float LANE_CLIMB_SPEED = 6f; // seconds to climb the lane

    public Level currentLevel { get; private set; }
    public Player player { get; private set; }

    private int _currentPlane = 1;
    private int _prevDirection = 1;
    private float _prevPosition = .5f;
    private float _positionOnPlane = 0.5f; // between 0 (beginning) and 1 (end)
    private float _climbPosition = 0f; // between 0 & 1

	// Use this for initialization
	void Start () {
        _score = 500;
        randomEnemyDrop();
	}

	public override void spawn(Lane spawnLane) {
        currentLevel = GameManager.Instance.CurrentLevel;
        player = GameManager.Instance.CurrentPlayerShips[0]; // should always be the actual player ship
        transform.position = spawnLane.Back;
        _currentPlane = currentLevel.GetIndexOfLane(spawnLane);
    }
	// Update is called once per frame
	void Update () {
        // if current target player ship is destroyed (null), get another
        if (player == null) {
            player = GameManager.Instance.CurrentPlayerShips[0];
        }
        // move towards player
        int direction = 0;

        // if reached center of lane
        if (_prevPosition <= .5f && _positionOnPlane > .5f || _prevPosition >= .5f && _positionOnPlane < .5f) {
            direction = GetDirectionToPlayer();
        }
        else {
            direction = _prevDirection;
        }

        _prevDirection = direction;
        _prevPosition = _positionOnPlane;
        _positionOnPlane += direction * LANE_CROSS_SPEED * Time.deltaTime;

        // calculate new position after movement
        if (_positionOnPlane < 0) {
            _currentPlane--;
            _positionOnPlane++;
        }
        else if (_positionOnPlane > 1) {
            _currentPlane++;
            _positionOnPlane--;
        }

        if (_currentPlane < 0) {
            if (currentLevel.wrapAround) {
                _currentPlane += currentLevel.lanes.Count;
            }
            else {
                _currentPlane = 0;
                _positionOnPlane = 0;
            }
        }
        else if (_currentPlane >= currentLevel.lanes.Count) {
            if (currentLevel.wrapAround) {
                _currentPlane -= currentLevel.lanes.Count;
            }
            else {
                _currentPlane = currentLevel.lanes.Count - 1;
                _positionOnPlane = 1;
            }
        }
        // update position
        if (_climbPosition < 1) { 
            _climbPosition += 1 / LANE_CLIMB_SPEED * Time.deltaTime;
        }
        
        Vector3 normal = .2f * currentLevel.lanes[_currentPlane].Normal;
        transform.position = Vector3.Lerp(currentLevel.lanes[_currentPlane].Back + normal, currentLevel.lanes[_currentPlane].Front + normal, _climbPosition);

        //update rotation
        float angleUp = Vector3.Angle(Vector3.up, currentLevel.lanes[_currentPlane].Normal) - 90;
        float angleRight = Vector3.Angle(Vector3.forward, currentLevel.lanes[_currentPlane].Normal);
        float angleLeft = Vector3.Angle(Vector3.back, currentLevel.lanes[_currentPlane].Normal);
        if (angleRight < angleLeft) {
            angleUp = -angleUp;
        }
        transform.forward = -currentLevel.lanes[_currentPlane].Normal;
        //transform.eulerAngles = new Vector3(angleUp, 180, 0);
        Vector3 rotationPoint;
        Vector3 rotationAxis = currentLevel.lanes[_currentPlane].Normal;
        if (_positionOnPlane < .5f) {
            rotationPoint = currentLevel.lanes[_currentPlane].LeftEdge.Front;
            rotationPoint.x = transform.position.x;
            float lerp = (.5f - _positionOnPlane) * 2;
            transform.RotateAround(rotationPoint, rotationAxis, -90 * lerp);
        }
        else {
            rotationPoint = currentLevel.lanes[_currentPlane].RightEdge.Front;
            rotationPoint.x = transform.position.x;
            float lerp = (_positionOnPlane - .5f) * 2;
            transform.RotateAround(rotationPoint, rotationAxis, 90 * lerp);
        }

	}

    int GetDirectionToPlayer()
    {
        int direction = 0; // 1 or -1
        int playerPlane = player.currentPlane;
        // get the direction closest to the player
        if (currentLevel.wrapAround)
        {
            int upper = _currentPlane + currentLevel.lanes.Count / 2;
            if (upper > currentLevel.lanes.Count)
            {
                upper -= currentLevel.lanes.Count;
                if (playerPlane < upper || playerPlane > _currentPlane)
                {
                    direction = 1;
                }
                else if (_currentPlane == playerPlane)
                {
                    if (_positionOnPlane < .5f)
                    {
                        direction = 1;
                    }
                    else
                    {
                        direction = -1;
                    }
                }
                else
                {
                    direction = -1;
                }
            }
            else
            {
                if (playerPlane < upper && playerPlane > _currentPlane)
                {
                    direction = 1;
                }
                else if (_currentPlane == playerPlane)
                {
                    if (_positionOnPlane < .5f)
                    {
                        direction = 1;
                    }
                    else
                    {
                        direction = -1;
                    }
                }
                else
                {
                    direction = -1;
                }
            }
        }
        else
        {
            if (_currentPlane < playerPlane)
            {
                direction = 1;
            }
            else if (_currentPlane > playerPlane)
            {
                direction = -1;
            }
            else
            {
                if (_positionOnPlane < .5f)
                {
                    direction = 1;
                }
                else
                {
                    direction = -1;
                }
            }
        }
        return direction;
    }
}
