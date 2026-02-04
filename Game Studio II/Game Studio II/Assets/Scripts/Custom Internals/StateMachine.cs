using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;


public class StateBase : MonoBehaviour
{
    public HashSet<baseTransition> GlobalTransitons = new HashSet<baseTransition>();

    protected Dictionary<Type, State> States = new Dictionary<Type, State>();

    protected State currentState;

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
        var defaultState = this.States.Values.FirstOrDefault(s => s.mIsDefault) ?? this.States.Values.First();
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

        foreach (baseTransition tran in currentState.mStateTrans)
        {
            if (tran.Evalutate)
            {

                changeState(tran.ToState);
                return 0;
            }
        }
        return 0;
    }


    public void TriggerTransition(Type toState)
    {
        changeState(toState);
    }

    private void changeState(Type newState)
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

    public void fixedUpdate()
    { 
        currentState.fixedUpdate();
    }

}
public abstract class MachineAuto<T> : StateBase where T : StateBase, new()
{
    
}
public class StateMachine : MachineAuto<StateMachine>
{
    public void StartStateWithAuto<TScript>(TScript scriptInstance)
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

        States.Clear();
        GlobalTransitons.Clear();

        foreach (var type in types)
        {
            var rawInstance = Activator.CreateInstance(type);

            if (rawInstance is State stateInstance)
            {
                var scriptField = type.GetField("mScript");
                if (scriptField != null)
                    scriptField.SetValue(stateInstance, scriptInstance);

                States[type] = stateInstance;
            }
        }

        var defaultState = States.Values.FirstOrDefault(s => s.mIsDefault) ?? States.Values.First();
        currentState = defaultState;
        currentState.enter();
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


public interface sDefaultState
{
    void enter();
    void update();
    void fixedUpdate();
    void exit();
}
public class State : sDefaultState
{
    public virtual bool mIsDefault => false;
    public HashSet<baseTransition> mStateTrans { get; } = new HashSet<baseTransition>();
    virtual public void enter() { }
    virtual public void update() { }
    public virtual void fixedUpdate() { }
    virtual public void exit() { }
    public void addTrans(Type to, Func<bool> condition)
    {
        mStateTrans.Add(new baseTransition(to, condition));
    }
}
public abstract class StateAuto<T, S>: State where T : StateAuto<T, S>, new()
{
    public static Type mtype => typeof(T);

    public S mScript;

    public static T mAuto => new T();

    static public T Script(S _script)
    {
        T instance = new T();
        instance.mScript = _script;
        return instance;
    }
}