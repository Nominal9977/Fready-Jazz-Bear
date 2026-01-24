using UnityEditor;
using UnityEngine;
using static ChannelNames;


public class PlayerMovScript : MonoBehaviour
{

    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject Floor;
    [SerializeField] public Vector3 BossLastKnownPostion;
    public Vector3 newPostion;
    public int health = 0;

    public StateMachine stateMachine;
    void Start()
    {
        player = this.gameObject;
        stateMachine = stateMachine.StartStateWithAuto(this);

        stateMachine[sCanMove.type].addTrans(sCantMove.type, () => 
        { 
            return !CheckInsideGround(newPostion);
        });

        stateMachine[sCantMove.type].addTrans(sCanMove.type, () =>
        {
            return CheckInsideGround(newPostion);
        });

        EventManager.Player.OnHealthChanged.Get().AddListener(UpdateHealth);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.update();
        health++;
        EventManager.Player.OnHealthChanged.Get(Default).Invoke(this, health);
    }

    private void UpdateHealth(Component component, int health)
    {
        Debug.Log("EVENT SYSTEM RECIEVED HEATLH " + health.ToString());
    }



    public bool CheckInsideGround(Vector3 NewPos)
    {
        Vector3 FloorCenterPos = Floor.transform.position;
        Vector3 sphereWidth = Floor.transform.localScale;

        Vector3 center_to_player = NewPos - FloorCenterPos;
        if(center_to_player.magnitude > sphereWidth.z/2 - 1) return false; else return true;

       

    }

    public void CheckNewPostion()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(moveX, moveY);

        newPostion = new Vector3(player.transform.position.x + move.x * moveSpeed * Time.deltaTime,
                                      player.transform.position.y,
                                      player.transform.position.z + move.y * moveSpeed * Time.deltaTime);
    }
}



public class sCanMove: StateAuto<sCanMove, PlayerMovScript>
{
    public override bool IsDefault => true;
    public override void enter()
    {
        Debug.Log("Entered Can Move State");
    }

    public override void update()
    {
        script.CheckNewPostion();
        script.player.transform.position = script.newPostion;
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
        script.CheckNewPostion();
    }

    public override void exit()
    {
        Debug.Log("Exited Can not Move State");
    }
}
