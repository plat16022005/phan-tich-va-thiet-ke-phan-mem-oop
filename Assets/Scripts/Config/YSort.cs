using UnityEngine;
using UnityEngine.Rendering;

public class YSort : MonoBehaviour
{
    private SortingGroup sortingGroup;

    void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
    }

    void LateUpdate()
    {
        sortingGroup.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
}