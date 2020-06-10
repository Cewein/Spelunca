using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RingMenu : MonoBehaviour
{
    [Header("Images")]
    [SerializeField] private Sprite ring;
    [SerializeField] private GameObject element;

    [Header("Content")]
    [SerializeField] private List<ScriptableObject> content;
    
    [Header("Format")]
    [SerializeField] private float gap = 0f;
    [SerializeField] private float iconDist = 300f;
    [SerializeField] private Vector3 normalScale;
    [SerializeField] private Vector3 emphaseScale;

    private float stepLength;
    private int activeElement;
    private float NormalizeAngle(float a) => (a + 360f) % 360f;
    private GameObject[] items;


    private void Awake()
    {
        stepLength = 360f/content.Count;
        GetComponent<Image>().sprite = ring;
        items = new GameObject[content.Count];
        if (normalScale.Equals(Vector3.zero)) normalScale = transform.localScale;
        if (emphaseScale.Equals(Vector3.zero)) emphaseScale = transform.localScale*.7f;

        for (int i = 0; i < content.Count; i++)
        {
            GameObject item = Instantiate(element, transform);
            items[i] = item;
            item.transform.localPosition = Vector3.zero;
            item.transform.localPosition = item.transform.localPosition
                                           + Quaternion.AngleAxis(i * stepLength, Vector3.forward)
                                           * Vector3.up * iconDist;
            item.transform.localRotation = Quaternion.Euler(0, 0, 0);
            item.transform.localRotation = Quaternion.Euler(0, 0, gap + i*stepLength);
        }
    }
    
    public void Open()
    {
        gameObject.SetActive(true);
        Debug.Log("erhgzfehhj");
        var mouseAngle = NormalizeAngle(Vector3.SignedAngle(Vector3.up, Input.mousePosition - transform.position, Vector3.forward) + stepLength / 2f);
        activeElement = (int)(mouseAngle / stepLength);
        for (int i = 0; i < items.Length; i++)
        {
            items[i].transform.localScale = (i == activeElement)  ? emphaseScale : normalScale;
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);

    }
}
