using UnityEngine;

public class Camera : MonoBehaviour
{

    [SerializeField] public Camera mCamera;
    void Start()
    {
        
        mCamera = GetComponent<Camera>();

        if(mCamera == null)
        {
            Debug.LogError("Camera component not found on the GameObject.");
        }
    }

    void Update()
    {
        
    }

    #region

    public void UpdatePlayerPostion(Component component, Vector3 newPostion)
    {

    }

    #endregion
}
