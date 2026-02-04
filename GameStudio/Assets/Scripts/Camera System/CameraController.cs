using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour

{

    [SerializeField] public Camera mCamera;
    [SerializeField] public StateMachine mStateMachine;

    [HideInInspector] public Vector3 mPlayerpostion;
    [HideInInspector] public Vector3 mBosspostion;

    void Start()
    {
        
        mCamera = GetComponent<Camera>();

        if(mCamera == null)
        {
            Debug.LogError("Camera component not found on the GameObject.");
        }

       

        EventManager.cBoss.ePostionChange.Get().AddListener(UpdateBossPostion);
        EventManager.cBoss.ePostionChange.Get().AddListener((c, pos) =>
        {
            mStateMachine.TriggerTransition(sMoveToNewPostion.mtype);
        });


        EventManager.cPlayer.ePostionChange.Get().AddListener(UpdatePlayerPostion);
        EventManager.cPlayer.ePostionChange.Get().AddListener((c, pos) =>
        {
                mStateMachine.TriggerTransition(sMoveToNewPostion.mtype);
        });
    }

    #region Event Sytem Listen Methods

    // Updates the Player Postion and calls the Change Camera Postion Method
    public void UpdatePlayerPostion(Component component, Vector3 newPostion)
    {
        mPlayerpostion = newPostion;

        //StartCoroutine(ChangeCameraPostion());

    }

    // Updates the Boss Postion and calls the Change Camera Postion Method
    public void UpdateBossPostion(Component component, Vector3 newPostion)
    {
        mBosspostion = newPostion;
        //StartCoroutine(ChangeCameraPostion());
    }

    #endregion

    #region Change Camera Postion


    // Changes the Camera Postion based on the Player and Boss Postions
    IEnumerator ChangeCameraPostion()
    {
        yield return null;
    }

    #endregion


    #region States

    public class sMoveToNewPostion : StateAuto<sMoveToNewPostion, CameraController>
    {
        public override bool mIsDefault => true;


        public override void update()
        {
            
        }
    }
    #endregion

}
