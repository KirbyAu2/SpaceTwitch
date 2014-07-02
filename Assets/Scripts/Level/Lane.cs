using UnityEngine;
using System.Collections;

public class Lane {
    private const float HIGHLIGHT_OFFSET = 0.02f;

    private Edge _leftEdge;
    private Edge _rightEdge;
    private Lane _rightLane;
    private Lane _leftLane;
    private Vector3 _front;
    private Vector3 _back;
    private Vector3 _normal;
    private GameObject _highlightLane;

    public Lane(Edge leftEdge, Edge rightEdge) {
        _leftEdge = leftEdge;
        _rightEdge = rightEdge;

        _front = (_leftEdge.Front + _rightEdge.Front) / 2;
        _back = (_leftEdge.Back + _rightEdge.Back) / 2;
        _normal = _rightEdge.Normal;
        createPlane();
    }

    private void createPlane() {
        Mesh m = new Mesh();
        m.name = "Scripted_Plane_New_Mesh";
        m.vertices = new Vector3[]{ _leftEdge.Front + _normal*HIGHLIGHT_OFFSET, _rightEdge.Front + _normal*HIGHLIGHT_OFFSET, 
            _rightEdge.Back + _normal*HIGHLIGHT_OFFSET, _leftEdge.Back + _normal*HIGHLIGHT_OFFSET };
        m.uv = new Vector2[]{ new Vector2(0,0), new Vector2 (0, 1), new Vector2(1, 1), new Vector2 (1, 0) };
        m.triangles = new int[]{0, 1, 2, 0, 2, 3};
        m.RecalculateNormals();
        _highlightLane = new GameObject("LanePlane");
        MeshRenderer ren = _highlightLane.AddComponent<MeshRenderer>();
        ren.material = (Material)Resources.Load("Materials/LaneHighlight");
        MeshFilter mF = _highlightLane.AddComponent<MeshFilter>();
        mF.mesh = m;
        _highlightLane.SetActive(false);
    }

    public void setHighlight(bool v) {
        _highlightLane.SetActive(v);
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
