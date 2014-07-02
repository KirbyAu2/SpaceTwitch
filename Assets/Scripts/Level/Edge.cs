using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Edge {

    private Vector3 _front;
    private Vector3 _back;
    private List<Edge> _neighbors;
    private Vector3 _normal;
    private Edge _right;
    private Edge _left;

    public Edge(Vector3 front, Vector3 back) {
        _front = front;
        _back = back;
        _normal = Vector3.zero;
        _neighbors = new List<Edge>();
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

    public Edge Right {
        get {
            return _right;
        }
    }

    public Edge Left {
        get {
            return _left;
        }
    }

    public Vector3 Normal {
        get {
            return _normal;
        }
        set {
            _normal = Vector3.Normalize(value);
        }
    }

    public void addNeighbor(Edge e) {
        if (_neighbors.Count >= 2) {
            Debug.LogError("Edges can't have more than one neighbor!");
        }
        _neighbors.Add(e);
        if (e.Front.z > _front.z) {
            _right = e;
        } else {
            _left = e;
        }
    }

    public void addNeighbor(Edge e, bool left) {
        if (_neighbors.Count >= 2) {
            Debug.LogError("Edges can't have more than one neighbor!");
            return;
        }
        _neighbors.Add(e);
        if (!left) {
            _right = e;
        } else {
            _left = e;
        }
    }
}
