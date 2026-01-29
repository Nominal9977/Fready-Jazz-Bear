using UnityEditor;
using UnityEngine;
using static ChannelNames;


public class PlayerMovScript : MonoBehaviour
{

    [SerializeField] public float mMoveSpeed = 5f;
    [SerializeField] public GameObject mPlayer;
    [SerializeField] public GameObject mFloor;
    [SerializeField] public Vector3 mBossLastKnownPostion;
    public Vector3 mNewPostion;
    public int mHealth = 0;

    public StateMachine mStateMachine;

    // Start is called before the first frame update
    void Start()
    {
        mPlayer = this.gameObject;
        mStateMachine = mStateMachine.StartStateWithAuto(this);

        mStateMachine[sCanMove.mtype].addTrans(sCantMove.mtype, () => 
        { 
            return !CheckInsideGround(mNewPostion);
        });

        mStateMachine[sCantMove.mtype].addTrans(sCanMove.mtype, () =>
        {
            return CheckInsideGround(mNewPostion);
        });

        EventManager.cPlayer.eOnHealthChanged.Get().AddListener(UpdateHealth);
    }

    // Update is called once per frame
    void Update()
    {
        mStateMachine.update();
        mHealth++;
        EventManager.cPlayer.eOnHealthChanged.Get().Invoke(this, mHealth);
    }

    //Is called when the event Listener is triggered
    private void UpdateHealth(Component component, int health)
    {
        Debug.Log("EVENT SYSTEM RECIEVED HEATLH " + health.ToString());
    }


    //Checks if the players move to postion is inside the ground area
    public bool CheckInsideGround(Vector3 NewPos)
    {
        Vector3 FloorCenterPos = mFloor.transform.position;
        Vector3 sphereWidth = mFloor.transform.localScale;

        Vector3 center_to_player = NewPos - FloorCenterPos;
        if(center_to_player.magnitude > sphereWidth.z/2 - 1) return false; else return true;

       

    }

    //Checks where the player wants to move based on input
    public void CheckNewPostion()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(moveX, moveY);

        mNewPostion = new Vector3(mPlayer.transform.position.x + move.x * mMoveSpeed * Time.deltaTime,
                                      mPlayer.transform.position.y,
                                      mPlayer.transform.position.z + move.y * mMoveSpeed * Time.deltaTime);
    }
}



public class sCanMove: StateAuto<sCanMove, PlayerMovScript>
{
    public override bool mIsDefault => true;
    public override void enter()
    {
        Debug.Log("Entered Can Move State");
    }

    public override void update()
    {
        mScript.CheckNewPostion();
        mScript.mPlayer.transform.position = mScript.mNewPostion;
    }

    public override void exit()
    {
        Debug.Log("Exited Can Move State");
    }
}

public class sCantMove : StateAuto<sCantMove, PlayerMovScript>
{

    public override void enter()
    {
        Debug.Log("Entered Can not Move State");
    }

    public override void update()
    {
        mScript.CheckNewPostion();
    }

    public override void exit()
    {
        Debug.Log("Exited Can not Move State");
    }
}
