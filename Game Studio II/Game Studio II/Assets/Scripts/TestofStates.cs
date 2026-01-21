using UnityEngine;

public class TestofStates : MonoBehaviour
{
   StateMachine stateMachine;

    void Start()
    {
       StateMachine stateMachine = new StateMachine();
       stateMachine.StartStateWithAuto<TestofStates>(this);
        stateMachine.addGlobalTrans(sStartState, this.enabled = true);
    }

    void Update()
    {
        
    }
}
