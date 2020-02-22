using UnityEngine;
using UnityEngine.UI;
public class HitMark : MonoBehaviour
{
   
    [Tooltip("Hit mark image time before disappear.")][SerializeField]
    private float lifeTime = 1.5f;

    private Image image; // hitmark image UIElement
    private Color temp; // temporary color variable because we can't directly assign image.tintColor.a

    private void Awake()
    {
        image = GetComponentInChildren<Image>();
        temp = image.color;
        temp.a = 1;
    }

    private void Update()
    {
        temp.a = AnimationCurve.EaseInOut(0f, 1f, lifeTime, 0f).Evaluate(Time.time);;
        //temp.a = Mathf.Lerp(image.color.a, 0, Time.deltaTime/lifeTime);
        image.color = temp;
        if(temp.a >= 0) Destroy(gameObject);
    }
}
