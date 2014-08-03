using UnityEngine;
using System.Collections;

public class Crosshatch : Enemy {
    private const float LANE_CROSS_SPEED = 1f;
    private const float LANE_CLIMB_SPEED = .02f;

    public Level currentLevel { get; private set; }
    public Player player { get; private set; }

    private int _currentPlane = 1;
    private float _positionOnPlane = 0.5f; // between 0 (beginning) and 1 (end)

	// Use this for initialization
	void Start () {
	
	}

	public override void spawn(Lane spawnLane) {
        currentLevel = GameManager.Instance.CurrentLevel;
        player = GameManager.Instance.CurrentPlayerShips[0]; // should always be the actual player ship
    }
	// Update is called once per frame
	void Update () {
        // if current target player ship is destroyed (null), get another
        if (player == null) {
            player = GameManager.Instance.CurrentPlayerShips[0];
        }

        // move towards player
        float direction = 0; // 1 or -1
        float playerPlane = player.currentPlane;

        // get the direction closest to the player
        if (currentLevel.wrapAround) {
            int upper = _currentPlane + currentLevel.lanes.Count / 2;
            if (upper > currentLevel.lanes.Count) {
                upper -= currentLevel.lanes.Count;
                if (playerPlane < upper || playerPlane > _currentPlane) {
                    direction = 1;
                }
                else if (_currentPlane == playerPlane) { 
                    if (_positionOnPlane < .5f) {
                        direction = 1;
                    }
                    else {
                        direction = -1;
                    }
                }
                else {
                    direction = -1;
                }
            }
            else { 
                if (playerPlane < upper && playerPlane > _currentPlane) {
                    direction = 1;
                }
                else if (_currentPlane == playerPlane) { 
                    if (_positionOnPlane < .5f) {
                        direction = 1;
                    }
                    else {
                        direction = -1;
                    }
                }
                else {
                    direction = -1;
                }
            }
        }
        else {
            if (_currentPlane < playerPlane) { 
                direction = 1;
            }
            else if (_currentPlane > playerPlane) {
                direction = -1;
            }
            else { 
                if (_positionOnPlane < .5f) {
                    direction = 1;
                }
                else {
                    direction = -1;
                }
            }
        }

        _positionOnPlane += direction * LANE_CROSS_SPEED * Time.deltaTime;

        // calculate new position after movement
        if (_positionOnPlane < 0)
        {
            _currentPlane--;
            _positionOnPlane++;
        }
        else if (_positionOnPlane > 1)
        {
            _currentPlane++;
            _positionOnPlane--;
        }

        if (_currentPlane < 0)
        {
            if (currentLevel.wrapAround)
            {
                _currentPlane += currentLevel.lanes.Count;
            }
            else
            {
                _currentPlane = 0;
                _positionOnPlane = 0;
            }
        }
        else if (_currentPlane >= currentLevel.lanes.Count)
        {
            if (currentLevel.wrapAround)
            {
                _currentPlane -= currentLevel.lanes.Count;
            }
            else
            {
                _currentPlane = currentLevel.lanes.Count - 1;
                _positionOnPlane = 1;
            }
        }
        // update position
        transform.position = currentLevel.lanes[_currentPlane].Front;
        float angleUp = Vector3.Angle(Vector3.up, currentLevel.lanes[_currentPlane].Normal) - 90;
        float angleRight = Vector3.Angle(Vector3.forward, currentLevel.lanes[_currentPlane].Normal);
        float angleLeft = Vector3.Angle(Vector3.back, currentLevel.lanes[_currentPlane].Normal);
        if (angleRight < angleLeft)
        {
            angleUp = -angleUp;
        }
        transform.eulerAngles = new Vector3(angleUp, 180, 0);

	}
}
