using UnityEngine;

public class Eye : MonoBehaviour
{
    //Script attached to "London Eye" terrain objects to provide rotation with stationary platforms
    
    [SerializeField]  Transform[] platforms;

    void Update()
    {
        transform.eulerAngles +=  new Vector3(0,0,0.1f);
        foreach(Transform platform in platforms){
            platform.eulerAngles = Vector3.zero;
        }
    }
}
