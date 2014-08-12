using UnityEngine;
using System.Collections;

/*
 * The Confetti enemy spawns on a random lane and moves up lanes
 * Confetti will then pick a random lane and move from lane to lane to that lane
 * If Confetti runs into the player ship, both will die
 */
public class Confetti : Enemy {
    public const int MAX_LANE_MOVE = 5;
    public const float DEFAULT_SPEED_PER_UNIT = 1.0f;
    public const float DURATION_IN_MIDDLE = 1.5f;
    public const float DURATION_FOR_FAILSAFE = 1.0f;

    private bool _isInMiddle = false;
    private bool _atBackOfLane = false;
    private bool _goingRight = false;
    private bool _goingDownLane = false;
    private float _currentSpeed;
    private Vector3 _currentStartPoint;
    private Vector3 _currentTargetPoint;
    private Lane _destinatioLane;
    private Vector3 _destination;
    private float _startTime = 0;
    private Vector3 _failsafePos;
    private float _failsafeTime = 0;

    void Start () {
        _score = 300;
        _failsafePos = gameObject.transform.position;
        _failsafeTime = Time.time;
        randomEnemyDrop();
    }
	/*
     * Moves the Confetti down the lane or from lane to lane 
     */
    void Update () {
        handleFailsafe();
        if (gameObject.transform.position ==_currentTargetPoint) {
            getNextPoint();
        }
        if (!_goingDownLane) {
            gameObject.transform.position = Vector3.Lerp(_currentStartPoint, _currentTargetPoint, (Time.time - _startTime) / DEFAULT_SPEED_PER_UNIT);
        } else {
            gameObject.transform.position = Vector3.Lerp(_currentStartPoint, _currentTargetPoint, (Time.time - _startTime) / (DEFAULT_SPEED_PER_UNIT * 3.0f));
        }
    }

    /**
     * Failsafe for incase the fixes to confetti didn't fix the problem with it getting stuck
     */
    private void handleFailsafe() {
        if (_failsafeTime + DURATION_FOR_FAILSAFE < Time.time) {
            _failsafeTime = Time.time;
            if (_failsafePos == gameObject.transform.position) {
                explode();
            }
        }
    }

    /*
     * Looks to see the next it moves to depending on the dierection it is moving
     */
    private void getNextPoint() {
        _startTime = Time.time;
        _currentStartPoint = _currentTargetPoint;

        if (_goingDownLane) {
            _atBackOfLane = !_atBackOfLane;
            getNewDestination();
            _goingDownLane = false;
        }

        if (_currentTargetPoint == _destination) {
            _currentTargetPoint = (_atBackOfLane) ? _destinatioLane.Front : _destinatioLane.Back;
            _goingDownLane = true;
            _destination = _currentTargetPoint;
            return;
        }

        if (_atBackOfLane) {
            if (_isInMiddle) {
                _currentTargetPoint = _goingRight ? _currentLane.RightEdge.Back : _currentLane.LeftEdge.Back;
            } else {
                _currentLane = _goingRight ? _currentLane.RightLane : _currentLane.LeftLane;
                _currentTargetPoint = _currentLane.Back;
            }
        } else {
            if (_isInMiddle) {
                _currentTargetPoint = _goingRight ? _currentLane.RightEdge.Front : _currentLane.LeftEdge.Front;
            } else {
                _currentLane = _goingRight ? _currentLane.RightLane : _currentLane.LeftLane;
                _currentTargetPoint = _currentLane.Front;
            }
        }
        _isInMiddle = !_isInMiddle;
    }

    /*
     * Confetti moves down a lane, then randomly picks a lane and moves lane to lane to get to the destination
     * Basic pathfinding to find the shortest way to that lane
     */
    private void getNewDestination() {
        int targetLaneIndex = int.MaxValue;
        Lane tempLane = null;

        int backupCount = 0; //To make sure that there isn't a infinite while loop

        while (Mathf.Abs(targetLaneIndex - GameManager.Instance.CurrentLevel.getLaneIndex(_currentLane)) > MAX_LANE_MOVE ||
            Mathf.Abs(targetLaneIndex - GameManager.Instance.CurrentLevel.getLaneIndex(_currentLane)) < 2) {
            backupCount++;
            tempLane = GameManager.Instance.CurrentLevel.getRandomLane();
            if (tempLane == _currentLane) {
                continue;
            }

            targetLaneIndex = GameManager.Instance.CurrentLevel.getLaneIndex(tempLane);
            int leftPathLength = pathfindLanes(tempLane, true);
            int rightPathLength = pathfindLanes(tempLane, false);

            if (leftPathLength == -1 && rightPathLength == -1) {
                //get a new lane is no path
                continue;
            } else {
                if (rightPathLength > MAX_LANE_MOVE && leftPathLength > MAX_LANE_MOVE) {
                    //get a new lane if both are too far away from current plane
                    continue;
                }
                if (rightPathLength == -1 && leftPathLength != -1) {
                    if (leftPathLength > MAX_LANE_MOVE) {
                        continue;
                    }
                    _goingRight = false;
                } else if (rightPathLength != -1 && leftPathLength == -1) {
                    if (rightPathLength > MAX_LANE_MOVE) {
                        continue;
                    }
                    _goingRight = true;
                } else {
                    //if both paths are possible
                    _goingRight = rightPathLength < leftPathLength;
                }
            }

            if (backupCount > 50) {
                if (_currentLane.RightLane != null) {
                    tempLane = _currentLane.RightLane;
                    if (tempLane.RightLane != null) {
                        tempLane = tempLane.RightLane;
                    }
                } else {
                    tempLane = _currentLane.LeftLane;
                    if (tempLane.LeftLane != null) {
                        tempLane = tempLane.LeftLane;
                    }
                }
                break;
            }
        }

        backupCount = 0;
        if (tempLane == null) {
            Debug.LogError("Not a valid lane!");
        }

        _destinatioLane = tempLane;
        _destination = (_atBackOfLane) ? _destinatioLane.Back : _destinatioLane.Front;
    }

    /**
     * returns number of lanes over from current one the target lane is
     * 
     * returns -1 if there is no path
     */
    int pathfindLanes(Lane targetLane, bool goLeft) {
        /*if (GameManager.Instance.CurrentLevel.wrapAround) {
            if (Mathf.Abs(targetLaneIndex - GameManager.Instance.CurrentLevel.getLaneIndex(_currentLane)) > MAX_LANE_MOVE) {
                targetLaneIndex = GameManager.Instance.CurrentLevel.lanes.Count - targetLaneIndex;
            }
        }*/
        //sanity check
        if (targetLane == _currentLane) {
            return -1;
        }

        int length = 0;
        Lane tempLane = _currentLane;

        while (tempLane != targetLane && tempLane != null) {
            tempLane = (goLeft) ? tempLane.LeftLane : tempLane.RightLane;
            length++;
            if (targetLane == tempLane) {
                break;
            }
        }
        if (tempLane == null) {
            return -1;
        }
        return length;
    }

    /*
     * Spawns Confetti on alne and sets destination
     */
    public override void spawn(Lane spawnLane) {
        _currentLane = spawnLane;
        _alive = true;
        _isInMiddle = true;
        getNewDestination();
        gameObject.transform.position = _currentLane.Back;
        _atBackOfLane = true;
        _currentTargetPoint = _currentLane.RightEdge.Back;
        _currentStartPoint = _currentLane.RightEdge.Back;
        _destination = _currentLane.RightEdge.Back;
        _destinatioLane = _currentLane;
    }

   /*
    * Function that draws to help debugging 
    */
    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_destination, 0.3f);
        Gizmos.DrawSphere(_currentLane.Front, 0.5f);
    }
}
