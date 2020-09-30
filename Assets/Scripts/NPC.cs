using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class NPC : MonoBehaviour
{
    // Let's start with a simple wander
    public Transform wanderParent;
    public Transform foodParent;

    public TextAsset behaviourTree;
    public BTModel.BehaviourTree loadedTree;
    private BehaviourTree.BaseNode root;

    // Practise Variables
    public GameObject hungry_bar;

    public List<Transform> wanderNodes = new List<Transform>();
    public List<Transform> foodNodes = new List<Transform>();

    public Transform nextPosition = null;

    public Animator player_animator;

    public UnityEngine.AI.NavMeshAgent agent;

    

    enum States
    {
        Moving,
        Waiting,
        Eating,
    }
    States currentState;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        wanderNodes.AddRange(wanderParent.GetComponentsInChildren<Transform>());
        foodNodes.AddRange(foodParent.GetComponentsInChildren<Transform>());

        wanderNodes.Remove(wanderParent);
        foodNodes.Remove(foodParent);

        player_animator = GetComponent<Animator>();

        ParseBehaviour();

        // StartCoroutine(MovingState());
    }

    // Update is called once per frame
    void Update()
    {
        if (root != null)
        {
            root.Execute();
        }
    }

    #region State machine
    private IEnumerator MovingState()
    {
        currentState = States.Moving;

        var destination = wanderNodes[Random.Range(0, wanderNodes.Count)];
        agent.destination = destination.position;
        agent.isStopped = false;

        var distance = transform.position - destination.position;
        distance.y = 0;

        while (distance.sqrMagnitude > 1)
        {
            yield return null;

            distance = transform.position - destination.position;
            distance.y = 0;
        }

        agent.isStopped = true;
        StartCoroutine(WaitingState());
    }

    private IEnumerator WaitingState()
    {
        currentState = States.Waiting;

        yield return new WaitForSeconds(1);

        StartCoroutine(MovingState());
    }
    #endregion

    #region Behaviour tree loading
    private void ParseBehaviour()
    {
        JsonSerializer js = new JsonSerializer();
        loadedTree = js.Deserialize<BTModel.BehaviourTree>(new JsonTextReader(new System.IO.StringReader(behaviourTree.text)));
        loadedTree.debugNodes = loadedTree.nodes.Values.ToArray();

        // create real tree

        // create nodes
        List<BehaviourTree.BaseNode> nodes = new List<BehaviourTree.BaseNode>();
        foreach (var n in loadedTree.nodes)
        {
            nodes.Add(CreateNode(n.Value));
        }

        // assamble nodes
        root = nodes.Find(n => n.Id == loadedTree.root);

        // set relationships
        foreach (var n in loadedTree.nodes)
        {
            var modelNode = n.Value;

            BehaviourTree.BaseNode currentNode = nodes.Find(n2 => n2.Id == modelNode.id);
            if (modelNode.child != null)
            {
                var child = nodes.Find(n3 => n3.Id == modelNode.child);
                AddChild(currentNode, child);
            }
            else if (modelNode.children.Count > 0)
            {
                foreach (string childId in modelNode.children)
                {
                    var child = nodes.Find(n3 => n3.Id == childId);

                    if (!(child is BehaviourTree.NoopNode))
                    {
                        AddChild(currentNode, child);
                    }
                    else
                    {
                        AddChild(currentNode, child);
                        // Debug.Log("New type: " + currentNode.GetType().ToString());
                    }
                }
            }
        }

        List<BehaviourTree.BaseNode> traverseNodes = new List<BehaviourTree.BaseNode>();

        root = nodes.Find(n => n.Id == loadedTree.root);
        traverseNodes.Add(root);

        if (root is BehaviourTree.RepeatUntilFail)
        {
            Debug.Log("Debugger went nuts!");
        }

        while (traverseNodes.Count > 0)
        {
            var topNode = traverseNodes[0];
            traverseNodes.RemoveAt(0);

            Debug.Log(topNode.GetType().ToString());

            if (topNode is BehaviourTree.RepeatUntilFail)
            {
                traverseNodes.Add(((BehaviourTree.RepeatUntilFail)topNode).Node);
            }
            else if (topNode is BehaviourTree.BaseNodeList)
            {
                foreach (var n in ((BehaviourTree.BaseNodeList)topNode).Nodes)
                {
                    traverseNodes.Add(n);
                }
            }
            else if (topNode is BehaviourTree.Inverter)
            {
                traverseNodes.Add(((BehaviourTree.Inverter)topNode).Node);
            }
            else if (topNode is BehaviourTree.Succeder)
            {
                traverseNodes.Add(((BehaviourTree.Succeder)topNode).Node);
            }
        }
    }

    private BehaviourTree.BaseNode CreateNode(BTModel.BehaviourTreeNode data)
    {
        switch (data.name.ToLower())
        {
            case "repeatuntilfailure":
                return new BehaviourTree.RepeatUntilFail(data.id);

            case "sequence":
                return new BehaviourTree.Sequence(data.id);

            case "priority":
                return new BehaviourTree.Selector(data.id);

            case "wait":
                int ms = System.Convert.ToInt32(data.properties["milliseconds"]);
                return new Wait(data.id, ms);

            case "getnextposition":
                return new GetNextPosition(data.id, this);

            case "movetoposition":
                return new MoveToPosition(data.id, this);

            case "onplayerhungry":
                return new onPlayerHungry(data.id, hungry_bar);

            case "isfoodavailable":
                return new isFoodAvailable(data.id, this);

            case "onplayerfindfood":
                return new onPlayerFindFood(data.id, this);

            case "onplayereat":
                return new onPlayerEat(data.id, this, player_animator,hungry_bar);

            default:
                return new BehaviourTree.NoopNode(data.id, data.name);
        }
    }

    private void AddChild(BehaviourTree.BaseNode parent, BehaviourTree.BaseNode n)
    {
        // throw new System.Exception("Error! Can't add child to node " + parent.Id + " (" + parent.GetType().ToString() + ")");

        // UGLY!!!!
        if (parent is BehaviourTree.RepeatUntilFail)
        {
            AddChild((BehaviourTree.RepeatUntilFail)parent, n);
        }
        else if (parent is BehaviourTree.BaseNodeList)
        {
            AddChild((BehaviourTree.BaseNodeList)parent, n);
        }
        else if (parent is BehaviourTree.Inverter)
        {
            AddChild((BehaviourTree.Inverter)parent, n);
        }
        else
        {
            throw new System.Exception("Error! Can't add child to node " + parent.Id + " (" + parent.GetType().ToString() + ")");
        }
    }

    private void AddChild(BehaviourTree.RepeatUntilFail parent, BehaviourTree.BaseNode n)
    {
        parent.Node = n;
    }

    private void AddChild(BehaviourTree.BaseNodeList parent, BehaviourTree.BaseNode n)
    {
        // Debug.Log("Trying to add child [" + n.Id + ":" + n.GetType().ToString() + "] to node " + parent.Id + ":" + parent.GetType().ToString());
        parent.AddNode(n);
    }

    private void AddChild(BehaviourTree.Inverter parent, BehaviourTree.BaseNode n)
    {
        parent.Node = n;
    }

    private void AddChild(BehaviourTree.Succeder parent, BehaviourTree.BaseNode n)
    {
        parent.Node = n;
    }
    #endregion
}
