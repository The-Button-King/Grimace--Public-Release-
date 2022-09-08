using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{

    [Category("GameObject")]
    public class IsActive : ConditionTask<Transform>
    {
        protected override string info =>  " is target Active";
        protected override bool OnCheck()
        {
            if( blackboard.GetVariableValue<GameObject>( "b_target" ) != null ) 
            {
                return blackboard.GetVariableValue<GameObject>( "b_target" ).activeInHierarchy;
            }
            return false;
        }
    }
}