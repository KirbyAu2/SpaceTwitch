using UnityEngine;
using System.Collections;

public class Confetti : Enemy {
    public const int MAX_LANE_MOVE = 5;
    public const float DEFAULT_SPEED_PER_UNIT = 1.0f;
    public const float DURATION_IN_MIDDLE = 1.5f;

    private bool _isInMiddle = false;
    private bool _atBackOfLane = false;
    private bool _goingRight = false;
    private bool _goingDownLane = false;
    private float _currentSpeed;
    private Vector3 _currentStartPoint;
    private Vector3 _currentTargetPoint;
    private Edge _destinatioEdge;
    private Vector3 _destination;
    private float _startTime = 0;

    void Start () {
	
    }
	
    void Update () {
        if (gameObject.transform.position ==_currentTargetPoint) {
            getNextPoint();
        }
        if (!_goingDownLane) {
            gameObject.transform.position = Vector3.Lerp(_currentStartPoint, _currentTargetPoint, (Time.time - _startTime) / DEFAULT_SPEED_PER_UNIT);
        } else {
            gameObject.transform.position = Vector3.Lerp(_currentStartPoint, _currentTargetPoint, (Time.time - _startTime) / (DEFAULT_SPEED_PER_UNIT * 3.0f));
        }
    }

    private void getNextPoint() {
        _startTime = Time.time;
        _currentStartPoint = gameObject.transform.position;

        if (_goingDownLane) {
            _atBackOfLane = !_atBackOfLane;
            getNewDestination();
            _goingDownLane = false;
            _isInMiddle = !_isInMiddle;
        }

        if (_currentTargetPoint == _destination) {
            _currentTargetPoint = (_atBackOfLane) ? _destinatioEdge.Front : _destinatioEdge.Back;
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

    private void getNewDestination() {
        int targetLaneIndex = int.MaxValue;
        Lane tempLane = null;
        while (Mathf.Abs(targetLaneIndex - GameManager.Instance.CurrentLevel.getLaneIndex(_currentLane)) > MAX_LANE_MOVE ||
            Mathf.Abs(targetLaneIndex - GameManager.Instance.CurrentLevel.getLaneIndex(_currentLane)) < 2) {
            tempLane = GameManager.Instance.CurrentLevel.getRandomLane();
            if (tempLane == _currentLane) {
                continue;
            }
            targetLaneIndex = GameManager.Instance.CurrentLevel.getLaneIndex(tempLane);
            if (GameManager.Instance.CurrentLevel.wrapAround) {
                if(Mathf.Abs(targetLaneIndex - GameManager.Instance.CurrentLevel.getLaneIndex(_currentLane)) > MAX_LANE_MOVE) {
                    targetLaneIndex = GameManager.Instance.CurrentLevel.lanes.Count - targetLaneIndex;
                }
            }
        }

        if (tempLane == null) {
            Debug.LogError("Not a valid lane!");
        }

        //Get random edge
        _destinatioEdge = (Random.value * 2.0f > 1.0f) ? tempLane.RightEdge : tempLane.LeftEdge;
        _destination = (_atBackOfLane) ? _destinatioEdge.Back : _destinatioEdge.Front;

        //Basic path finding dist check
        //Check left first
        int leftCount = 1;
        if(_currentLane.LeftLane == null) {
            leftCount = 0;
        }
        Lane checkLane = _currentLane.LeftLane;
        while (checkLane != _currentLane && checkLane != null) {
            checkLane = checkLane.LeftLane;
            leftCount++;
            if (checkLane == null) {
                leftCount = int.MinValue;
                break;
            }
            if (checkLane == tempLane) {
                break;
            }
        }

        //Check right first
        int rightCount = 1;
        if (_currentLane.LeftLane == null) {
            rightCount = 0;
        }
        checkLane = _currentLane.RightLane;
        while (checkLane != _currentLane && checkLane != null) {
            checkLane = checkLane.RightLane;
            rightCount++;
            if (checkLane == null) {
                rightCount = int.MinValue;
                break;
            }
            if (checkLane == tempLane) {
                break;
            }
        }

        _goingRight = rightCount > leftCount;

    }

    public override void spawn(Lane spawnLane) {
        _currentLane = spawnLane;
        _alive = true;
        getNewDestination();
        gameObject.transform.position = _currentLane.RightEdge.Back;
        _atBackOfLane = true;
        _isInMiddle = true;
        _currentTargetPoint = _currentLane.RightEdge.Back;
        _currentStartPoint = _currentLane.RightEdge.Back;
        _destination = _currentLane.RightEdge.Back;
        _destinatioEdge = _currentLane.RightEdge;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_destination, 0.3f);
        Gizmos.DrawSphere(_currentLane.Front, 0.5f);
    }
}
