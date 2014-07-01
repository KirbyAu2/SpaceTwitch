using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EdgeGeneration : MonoBehaviour {
    public GameObject front;
    public bool debugDraw = false;

    private MeshFilter _filter;
    private MeshRenderer _renderer;
    private Dictionary<Vector3, float> _angleDict;
    private List<Vector3> _candidates;
    private Dictionary<Vector3, Vector3> _normalDict;
    private Dictionary<Vector3, List<Vector3>> _neighbors;
    private Dictionary<Vector3, Edge> _edgeDict;
    private Lane _spawnLane;
    private List<Edge> _newEdges;
    private List<Vector3> _edgeList;
    private List<Lane> _laneList;
    private Dictionary<Edge, bool> _traversed;

    /**
     * Initialization
     */
    void Start () {
        _laneList = new List<Lane>();
        _traversed = new Dictionary<Edge, bool>();
        _newEdges = new List<Edge>();
        _candidates = new List<Vector3>();
        _normalDict = new Dictionary<Vector3, Vector3>();
        _angleDict = new Dictionary<Vector3, float>();
        _neighbors = new Dictionary<Vector3, List<Vector3>>();
        _filter = GetComponent<MeshFilter>();
        _renderer = GetComponent<MeshRenderer>();
        _edgeDict = new Dictionary<Vector3, Edge>();
        if (_filter == null || _renderer == null) {
            Debug.LogError("No mesh renderer or filter on edge generation object!");
            return;
        }
        initGeneration();
    }

    public Lane SpawnLane {
        get {
            return _spawnLane;
        }
    }

    /**
     * Go through all of the triangles and verticies for
     * a mesh level and generate the correct edges
     */
    private void initGeneration() {
        int[] triangles = _filter.mesh.triangles;
        Vector3[] verts = _filter.mesh.vertices;

        for(int i = 0; i < triangles.Length;) {
            getAngleForVert(verts[triangles[i]], verts[triangles[i + 1]], verts[triangles[i + 2]]);
            getAngleForVert(verts[triangles[i + 1]], verts[triangles[i]], verts[triangles[i + 2]]);
            getAngleForVert(verts[triangles[i + 2]], verts[triangles[i]], verts[triangles[i + 1]]);
            i += 3;
        }

        foreach (KeyValuePair<Vector3, float> entry in _angleDict) {
            if (entry.Value < 358) {
                Vector3 v = transform.TransformPoint(entry.Key);
                bool keep = true;
                foreach (Vector3 prevCandidate in _candidates) {
                    float dist = Vector3.Distance(prevCandidate, v);
                    if (dist < 0.5f) {
                        keep = false;
                        break;
                    }
                }
                if (keep) {
                    _candidates.Add(v);
                }
            }
        }

        _edgeList = new List<Vector3>();
        foreach (Vector3 v1 in _candidates) {
            float closestDist = float.MaxValue;
            Vector3 partner = Vector3.zero;
            foreach (Vector3 v2 in _candidates) {
                if (v1 == v2) {
                    continue;
                }
                
                float dist = Vector3.Distance(v1, v2);
                if (dist > renderer.bounds.size.x - 0.1f && dist < renderer.bounds.size.x + 0.1f &&
                    dist < closestDist && !_edgeList.Contains(v1) && !_edgeList.Contains(v2)) {
                    closestDist = dist;
                    partner = v2;
                }

                if (dist < 1.1f && dist > 0.9f) {
                    if (!_neighbors.ContainsKey(v1)) {
                        _neighbors[v1] = new List<Vector3>();
                    }
                    _neighbors[v1].Add(v2);
                }
            }
            if(partner != Vector3.zero) {
                _edgeList.Add(v1);
                _edgeList.Add(partner);
            }
        }

        Vector3 xDir = new Vector3(1, 0, 0);
        foreach (KeyValuePair<Vector3, List<Vector3>> entry in _neighbors) {
            Vector3 norm;
            if (entry.Key.z < entry.Value[0].z) {
                norm = Vector3.Cross(entry.Value[0] - entry.Key, xDir);
            } else {
                norm = Vector3.Cross(entry.Key - entry.Value[0], xDir);
            }
            _normalDict[entry.Key] = norm;
        }

        separateEdges();
        separateLanes();
        getSpawnNode();
    }
    
    /**
     * Generates all of the lanes in order
     */
    private void separateLanes() {

    }

    /**
     * Gets the farthest left and bottom edge
     * 
     */
    private Edge getFarLeftEdge() {
        Edge farthestLeft = null;
        float mostLeft = float.MaxValue;
        float mostBottom = float.MaxValue;
        foreach (Edge e in _newEdges) {
            if(e.Front.z < mostLeft) {
                mostLeft = e.Front.z;
                mostBottom = e.Front.y;
                farthestLeft = e;
            } else if (e.Front.z == mostLeft && e.Front.y < mostBottom) {
                mostBottom = e.Front.y;
                farthestLeft = e;
            }
            if (e.Front.x < mostLeft) {
                mostLeft = e.Front.x;
                farthestLeft = e;
            }
        }
        return farthestLeft;
    }

    /**
     * Goes through all of the implicit edges and converts them to actual
     * Edges for later use. Saves the edges into the level. Gets all neighbors
     * for each edge.
     */
    private void separateEdges() {
        Vector3 frontPos = front.transform.position;
        for (int i = 0; i < _edgeList.Count;) {
            Edge e;
            if (Vector3.Distance(_edgeList[i], frontPos) < Vector3.Distance(_edgeList[i + 1], frontPos)) {
                e = new Edge(_edgeList[i], _edgeList[i + 1]);
            } else {
                e = new Edge(_edgeList[i + 1], _edgeList[i]);
            }
            _newEdges.Add(e);
            _edgeDict[e.Front] = e;

            i += 2;
        }

        Edge farthestLeft = getFarLeftEdge();
        if (farthestLeft == null) {
            Debug.LogError("Farthest left is null!");
            return;
        }

        if (_neighbors[farthestLeft.Front].Count > 1) {
            if (_neighbors[farthestLeft.Front][0].y < farthestLeft.Front.y) {
                farthestLeft.addNeighbor(_edgeDict[_neighbors[farthestLeft.Front][0]], false);
            } else {
                farthestLeft.addNeighbor(_edgeDict[_neighbors[farthestLeft.Front][1]], false);
            }
        } else {
            farthestLeft.addNeighbor(_edgeDict[_neighbors[farthestLeft.Front][0]], false);
        }

        farthestLeft.Normal = Vector3.Cross(farthestLeft.Right.Front - farthestLeft.Front, new Vector3(1,0,0));
        _normalDict[farthestLeft.Front] = farthestLeft.Normal;
        Edge previousEdge = farthestLeft;
        Edge currentEdge = farthestLeft.Right;

        while (!_traversed.ContainsKey(currentEdge)) {
            currentEdge.addNeighbor(previousEdge,true);
            if(_neighbors[currentEdge.Front].Count > 1 && currentEdge != farthestLeft) {
                if (currentEdge.Left == _edgeDict[_neighbors[currentEdge.Front][0]]) {
                    currentEdge.addNeighbor(_edgeDict[_neighbors[currentEdge.Front][1]],false);
                } else {
                    currentEdge.addNeighbor(_edgeDict[_neighbors[currentEdge.Front][0]],false);
                }
            }

            currentEdge.Normal = Vector3.Cross(currentEdge.Front - currentEdge.Left.Front, new Vector3(1,0,0));

            _normalDict[currentEdge.Front] = currentEdge.Normal;
            _normalDict[currentEdge.Back] = currentEdge.Normal;

            _laneList.Add(new Lane(currentEdge.Left, currentEdge));

            _traversed[currentEdge] = true;
            previousEdge = currentEdge;
            currentEdge = previousEdge.Right;
            if(currentEdge == null) {
                break;
            }
        }
        Level l = GetComponent<Level>();
        l.lanes = _laneList;
        l.edges = _newEdges;
    }

    /**
     * Add to a running total to check the angle, this is used to determine where an edge is
     */
    private void getAngleForVert(Vector3 targetVert, Vector3 o1, Vector3 o2) {
        Vector3 d1 = o1 - targetVert;
        Vector3 d2 = o2 - targetVert;
        if (!_angleDict.ContainsKey(targetVert)) {
            _angleDict[targetVert] = 0f;
        }
        _angleDict[targetVert] += Vector3.Angle(d1, d2);
    }

    /**
     * Quick way to get the spawn location for the ship
     * 
     */
    private void getSpawnNode() {
        float lowest = float.MaxValue;
        foreach (Lane l in _laneList) {
            if (l.Front.y < lowest) {
                _spawnLane = l;
                lowest = l.Front.y;
            }
        }
    }
	
    void Update () {
	
    }

    void OnDrawGizmos() {
        if (!debugDraw) {
            return;
        }
        if (_candidates == null) { return; }
        foreach (Vector3 v in _candidates) {
            Gizmos.DrawSphere(v, 0.05f);
            Gizmos.color = Color.red;
            if (_neighbors.ContainsKey(v)) {
                foreach (Vector3 n in _neighbors[v]) {
                    Gizmos.DrawLine(n, v);
                }
            }
        }

        bool flip = false;
        for (int i = 0; i < _edgeList.Count; ) {
            if (flip) {
                Gizmos.color = Color.green;
            } else {
                Gizmos.color = Color.blue;
            }
            flip = !flip;
            Gizmos.DrawSphere(_edgeList[i], 0.1f);

            Gizmos.DrawLine(_edgeList[i], _edgeList[i] + _normalDict[_edgeList[i]]);

            Gizmos.DrawSphere(_edgeList[i + 1], 0.1f);

            Gizmos.DrawLine(_edgeList[i+1], _edgeList[i+1] + _normalDict[_edgeList[i+1]]);

            Gizmos.DrawLine(_edgeList[i], _edgeList[i + 1]);
            i += 2;

        }
    }
}
