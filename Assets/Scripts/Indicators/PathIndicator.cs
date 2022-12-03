using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathIndicator : MonoBehaviour
{
    [SerializeField] private float _lineYOffset = 0.2f;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
    }

    public void UpdateLinePath(Vector3[] corners)
    {
        _lineRenderer.enabled = corners != null && corners.Length > 0;

        if (_lineRenderer.enabled == false) return;

        for (int i = 0; i < corners.Length; i++)
        {
            corners[i].y += _lineYOffset;
        }

        _lineRenderer.positionCount = corners.Length;
        _lineRenderer.SetPositions(corners);
    }
}
