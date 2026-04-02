using UnityEngine;
using UnityEngine.Rendering;

public class YSort : MonoBehaviour
{
    private SortingGroup sortingGroup;

    void Awake()
    {
        if (!CompareTag("Player"))
        {
            enabled = false;
            return;
        }

        sortingGroup = GetComponent<SortingGroup>();
    }

    void LateUpdate()
    {
        sortingGroup.sortingOrder = -(int)(transform.position.y * 100);
    }
}