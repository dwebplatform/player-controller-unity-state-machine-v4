using UnityEngine;
using UnityEngine.UI;
abstract public class BaseState {
    private string _name;
    private Text _text;
    public BaseState(string name){
        _name = name;
    }
    public virtual void Enter(){
        _text = GameObject.FindObjectOfType<Text>();
    }
    public virtual void HandleInput(){}
    public virtual void LogicUpdate(){
        Debug.Log(_name);
        _text.text = _name;
    }
    public virtual void PhysicsUpdate(){}
    public virtual void Exit(){}
}