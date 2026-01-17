using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Reflection;


public class StateBase : MonoBehaviour
{
    public HashSet<baseTransition> GlobalTransitons = new HashSet<baseTransition>();

    protected Dictionary<Type, State> States = new Dictionary<Type, State>();

    protected State currentState;
    private int shit;

    public State this[Type type]
    {
        get
        {
            return States[type];
        }
    }
    public T Get<T>() where T : State
        => (T)this[typeof(T)];

    private void Start()
    {
        GlobalTransitons = new HashSet<baseTransition>();
    }
    public void addGlobalTrans(Type to, Func<bool> condition)
    {
        GlobalTransitons.Add(new baseTransition(to, condition));
    }

    //sets the first state in the list as the defualt pram
    public void StartState<TScript>(TScript scriptInstance)
    {
        Type baseGeneric = typeof(StateAuto<,>);

        var types = Assembly.GetAssembly(baseGeneric)
                             .GetTypes()
                             .Where(type => type.BaseType != null
                                 && type.BaseType.IsGenericType
                                 && type.BaseType.GetGenericTypeDefinition() == baseGeneric
                                 && type.BaseType.GenericTypeArguments[1] == typeof(TScript)
                                 && !type.IsAbstract)
                             .ToList();

        foreach (var type in types)
        {
            var rawInstance = Activator.CreateInstance(type);

            if (rawInstance is State stateInstance)
            {
                var scriptField = type.GetField("script");

                if (scriptField != null)
                {
                    scriptField.SetValue(stateInstance, scriptInstance);
                }
                this.States[type] = stateInstance;
            }
        }
        var defaultState = this.States.Values.FirstOrDefault(s => s.IsDefault) ?? this.States.Values.First();
        this.currentState = defaultState;
        this.currentState.enter();
    }
    public int checkTrans()
    {
        foreach (baseTransition tran in GlobalTransitons)
        {
            if (tran.Evalutate)
            {
                changeState(tran.ToState);
                return 0;
            }
        }

        foreach (baseTransition tran in currentState.stateTrans)
        {
            if (tran.Evalutate)
            {

                changeState(tran.ToState);
                return 0;
            }
        }
        return 0;
    }
    public void changeState(Type newState)
    {
        currentState.exit();
        currentState = States[newState];
        currentState.enter();
    }
    public void update()
    {
        currentState.update();
        checkTrans();
    }

}
public abstract class MachineAuto<T> : StateBase where T : StateBase, new()
{
    public static T Auto => new T();
}
public class StateMachine : MachineAuto<StateMachine>
{
    //sets the first state in the list as the defualt pram
    public StateMachine StartStateWithAuto<TScript>(TScript scriptInstance)
    {
        Type baseGeneric = typeof(StateAuto<,>);

        var types = Assembly.GetAssembly(baseGeneric)
                             .GetTypes()
                             .Where(type => type.BaseType != null
                                 && type.BaseType.IsGenericType
                                 && type.BaseType.GetGenericTypeDefinition() == baseGeneric
                                 && type.BaseType.GenericTypeArguments[1] == typeof(TScript)
                                 && !type.IsAbstract)
                             .ToList();

        var stateMachine = Auto;

        foreach (var type in types)
        {
            var rawInstance = Activator.CreateInstance(type);

            if (rawInstance is State stateInstance)
            {
                var scriptField = type.GetField("script");

                if (scriptField != null)
                {
                    scriptField.SetValue(stateInstance, scriptInstance);
                }
                stateMachine.States[type] = stateInstance;
            }
        }

        // 5. Pick the default state
        var defaultState = stateMachine.States.Values.FirstOrDefault(s => s.IsDefault) ?? stateMachine.States.Values.First();
        stateMachine.currentState = defaultState;
        stateMachine.currentState.enter();

        return stateMachine;
    }


}


public struct baseTransition
{
    public Type ToState { get; }

    private Func<bool> condition { get; set; }

    public bool Evalutate => condition?.Invoke() ?? false;

    public baseTransition(Type to, Func<bool> _Condition)
    {
        ToState = to;
        condition = _Condition;
    }
}


public interface sItemState
{
    void enter();
    void update();
    void exit();
}
public class State : sItemState
{
    public virtual bool IsDefault => false;
    public HashSet<baseTransition> stateTrans { get; } = new HashSet<baseTransition>();
    virtual public void enter() { }
    virtual public void update() { }
    virtual public void exit() { }
    public void addTrans(Type to, Func<bool> condition)
    {
        stateTrans.Add(new baseTransition(to, condition));
    }
}
public abstract class StateAuto<T, S> : State where T : StateAuto<T, S>, new()
{
    public static Type type => typeof(T);

    public S script;

    public static T Auto => new T();

    static public T Script(S _script)
    {
        T instance = new T();
        instance.script = _script;
        return instance;
    }
}
public class FuntionLibray : MonoBehaviour
{
    static public bool IsPointerOverThis(GameObject obj)
    {
        return IsPointerOverThis(Input.mousePosition, obj);
    }
    static public bool IsPointerOverThis(Vector2 screenPosition, GameObject obj)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject == obj.gameObject)
                return true;
        }

        return false;
    }
    static public bool PlayTransition(float MoveTimer, float MoveDuration, Vector3 StartingPos, Vector3 TargetPos, Transform ObjectTransfrom)
    {
        MoveTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(MoveTimer / MoveDuration);
        ObjectTransfrom.localPosition = Vector3.Lerp(StartingPos, TargetPos, progress);

        if (progress >= MoveDuration)
        {
            return false;
        }
        return false;
    }

}
