using UnityEngine;

[System.Serializable]
public class BehaviorParam
{
    [SerializeField]
    bool active;
    public bool Active
    {
        get { return active; }
        set { active = value; }
    }

    [SerializeField]
    SteeringBehaviors behavior;

    public SteeringBehaviors Behavior
    {
        get { return behavior; }
    }

    [SerializeField]
    float weight;
    public float Weight
    {
        get { return weight; }
        set { weight = value; }
    }

    [SerializeField]
    float radius;
    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }

    public bool HasRadius()
    {
        return radius != -1;
    }

    public BehaviorParam(SteeringBehaviors behavior, bool active, float weight)
    {
        this.behavior = behavior;
        this.active = active;
        this.weight = weight;
        this.radius = -1;
    }

    public BehaviorParam(SteeringBehaviors behavior, bool active, float radius, float weight)
    {
        this.behavior = behavior;
        this.active = active;
        this.weight = weight;
        this.radius = radius;
    }

    public BehaviorParam Clone()
    {
        return new BehaviorParam(behavior, active, radius, weight); 
    }
}