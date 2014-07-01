using UnityEngine;
using System.Collections;

public class Lane {

    private Edge _leftEdge;
    private Edge _rightEdge;
    private Lane _rightLane;
    private Lane _leftLane;
    private Vector3 _front;
    private Vector3 _back;
    private Vector3 _normal;

    public Lane(Edge leftEdge, Edge rightEdge) {
        _leftEdge = leftEdge;
        _rightEdge = rightEdge;

        _front = (_leftEdge.Front + _rightEdge.Front) / 2;
        _back = (_leftEdge.Back + _rightEdge.Back) / 2;
        _normal = _rightEdge.Normal;
    }

    public Vector3 Normal {
        get {
            return _normal;
        }
    }

    public Vector3 Front {
        get {
            return _front;
        }
    }

    public Vector3 Back {
        get {
            return _back;
        }
    }

    public Edge LeftEdge {
        get {
            return _leftEdge;
        }
    }

    public Edge RightEdge {
        get {
            return _rightEdge;
        }
    }

    public Lane LeftLane {
        get {
            return _leftLane;
        }
    }

    public Lane RightLane {
        get {
            return _rightLane;
        }
    }

    /**
     * Adds a neighbor lane, set left to true if it will be clockwise from the lane
     */
    public void addNeighbor(Lane l, bool left) {
        if (left) {
            _leftLane = l;
        } else {
            _rightLane = l;
        }
    }
}
