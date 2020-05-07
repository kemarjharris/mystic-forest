using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutableChainVisual
{
    readonly float width;
    static GameObject canvas;
    public GameObject parent;
    List<ExecutionVisual> visuals = new List<ExecutionVisual>();
    List<GameObject> connections = new List<GameObject>();
    int pos = -1;

    public ExecutableChainVisual(IEnumerator<IExecutable> chain)
    {
        GameObject connection = Resources.Load<GameObject>("Prefabs/ExecutionVisual/ExecutableConnector");
        width = connection.GetComponent<RectTransform>().rect.width * connection.transform.localScale.x;
        if (canvas == null) canvas = GameObject.Find("Canvas");
        parent = new GameObject("Attack Chain Execution Visual");
        parent.transform.SetParent(canvas.transform);
        int i = 0;
        while (chain.MoveNext())
        {
            visuals.Add(ExecutionVisualFactory.CreateVisual(chain.Current, new Vector3(i * width, 3), parent.transform));
            if (i > 0) connections.Add(Object.Instantiate(connection, new Vector3((i - 0.5f) * width, 3), Quaternion.identity, parent.transform));
            i++;
        }
        chain.Reset();
    }

    public ExecutableChainVisual(IEnumerator<IExecutable> chain, Vector3 position, Transform parent) : this(chain)
    {
        this.parent.transform.SetParent(parent);
        this.parent.transform.position = position;
    }

    public void MoveNext()
    {
        if (IsFinished()) return;
        if (pos >= 0)
        {
            Vector3 worldPos = visuals[pos].transform.position;
            visuals[pos].transform.SetParent(parent.transform);
            visuals[pos].transform.position = worldPos;
            parent.transform.position -= new Vector3(width, 0);
            visuals[pos].MarkFinished();
            if (pos < connections.Count) Object.Destroy(connections[pos].gameObject);
        }
        pos++;
        if (pos >= visuals.Count) Destroy();

    }

    public void Expand() => visuals.ForEach((v) => v.gameObject.SetActive(true));

    public void Destroy()
    {
        Object.Destroy(parent);
    }

    public bool IsFinished()
    {
        return parent == null || parent.transform.childCount <= 0;
    }
}
