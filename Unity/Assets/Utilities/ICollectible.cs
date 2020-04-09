using UnityEngine;

public interface ICollectible
{
     
     bool IsReachable(Ray ray, float distance);
     void Collect();
     void Emphase(bool isEmphased);

}
