using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CordLabelController : MonoBehaviour
{
    public TMPro.TMP_Text labelGO;
    List<TMPro.TMP_Text> labels = new List<TMPro.TMP_Text>();
    PhysicController physicController;
    IEnumerator WaitForTarget()
    {
        while (GameController.Instance == null)
        {
            yield return null;
        }
        while (GameController.Instance.physicController.objects.Length == 0)
        {
            yield return null;
        }
        physicController = GameController.Instance.physicController;
        physicController.OnStopSimulationCallback += OnEndSimulate;
        physicController.OnStartSimulationCallback += StartSimulate;
        Debug.Log($"SHPT --> WaitForTarget");
        for (int i = 0; i < physicController.objects.Length; i++)
        {
            var label = Instantiate(labelGO, transform);
            labels.Add(label);
        }
        OnEndSimulate();
    }

    private void OnDestroy()
    {
        physicController.OnStopSimulationCallback -= OnEndSimulate;
        physicController.OnStartSimulationCallback -= StartSimulate;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForTarget());
    }

    void StartSimulate()
    {
        foreach (var label in labels)
        {
            label.gameObject.SetActive(false);
        }
    }
    void OnEndSimulate()
    {
        for (int i = 0; i < physicController.objects.Length; i++)
        {
            var physickObj = physicController.objects[i];
            if (!physickObj.gameObject.activeSelf)
            {
                continue;
            }
            var targetTrans = physickObj.transform.position;
            labels[i].transform.position = new Vector3(targetTrans.x, targetTrans.y - 0.3f, -5);
            labels[i].text = $"({targetTrans.x:0.##}, {targetTrans.y:0.##})";
            labels[i].gameObject.SetActive(true);
        }
    }
}
