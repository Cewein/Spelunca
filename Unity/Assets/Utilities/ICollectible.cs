using UnityEngine;

public interface ICollectible
{
     
     bool IsReachable(Ray ray, float distance);
     void Collect(GameObject container);
     void Emphase(bool isEmphased);

}
