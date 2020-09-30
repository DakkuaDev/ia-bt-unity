using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BehaviourTree
{
    /// <summary>
    /// This is a simple node. Base class for all the rest
    /// </summary>
    public abstract class BaseNode
    {
        private string id;
        public BaseNode(string _id)
        {
            id = _id;
        }

        /// <summary>
        /// Executes the action of the node. Can return Status.Failure, Status.Success or Status.Running
        /// </summary>
        /// <returns></returns>
        public abstract Status Execute();

        /// <summary>
        /// This is used for resetting the node. It is useful after ending the execution of a branch.
        /// </summary>
        public virtual void Reset() { }

        public string Id
        {
            get { return id; }
        }
    }

    /// <summary>
    /// This is a list of nodes. Base class for Sequense, Selector and others
    /// </summary>
    public abstract class BaseNodeList : BaseNode
    {
        protected List<BaseNode> sequence = new List<BaseNode>();

        public BaseNodeList(string _id, List<BaseNode> nodes) : base(_id)
        {
            sequence = nodes;
        }

        public void AddNode(BaseNode node)
        {
            sequence.Add(node);
        }

        public List<BaseNode> Nodes
        {
            get { return sequence; }
        }
    }

    // Debug porpose
    public class NoopNode : BaseNode
    {
        public string name;

        public NoopNode(string _id, string n) : base(_id)
        {
            name = n;
        }

        public override Status Execute()
        {
            return Status.Failure;
        }
    }

    /// <summary>
    /// Sequence node. This will execute all the nodes (from first to last) until one of them returns FAILURE or all of them return SUCCESS
    /// </summary>
    public class Sequence : BaseNodeList
    {
        private int currentNode = 0;
        private Status lastResult = Status.Running;


        public Sequence(string _id) : base(_id, new List<BaseNode>())
        { }
        
        public Sequence(string _id, List<BaseNode> nodes) : base(_id, nodes)
        { }

        public override Status Execute()
        {
            Status ret = Status.Success;
            if (currentNode < sequence.Count)
            {
                lastResult = ret = sequence[currentNode].Execute();
                if (ret == Status.Success)
                {
                    currentNode++;
                    if (currentNode < sequence.Count)
                    {
                        sequence[currentNode].Reset();
                    }

                    ret = currentNode < sequence.Count ? Status.Running : Status.Success;
                }
            }

            return ret;
        }

        public override void Reset()
        {
            currentNode = 0;
            lastResult = Status.Running;
            foreach (var node in sequence)
            {
                node.Reset();
            }
        }

        public override string ToString()
        {
            if (lastResult == Status.Running || lastResult == Status.Failure)
            {
                return sequence[currentNode].ToString();
            }

            return sequence[currentNode - 1].ToString();
        }
    }

    /// <summary>
    /// Selector node. This will execute all the nodes (from first to last) until one of them returns SUCCESS or all of them return FAILURE
    /// </summary>
    public class Selector : BaseNodeList
    {
        private int currentNode = 0;
        private Status lastResult = Status.Running;


        public Selector(string _id)
            : base(_id, new List<BaseNode>())
        { }

        public Selector(string _id, List<BaseNode> nodes)
            : base(_id, nodes)
        { }

        public override Status Execute()
        {
            Status ret = Status.Failure;
            if (currentNode < sequence.Count)
            {
                lastResult = ret = sequence[currentNode].Execute();

                if (ret == Status.Failure)
                {
                    currentNode++;
                    if (currentNode < sequence.Count)
                    {
                        sequence[currentNode].Reset();
                    }

                    ret = currentNode < sequence.Count ? Status.Running : Status.Failure;
                }
            }

            return ret;
        }

        public override void Reset()
        {
            currentNode = 0;
            lastResult = Status.Running;
            foreach (var node in sequence)
            {
                node.Reset();
            }
        }

        public override string ToString()
        {
            if (lastResult == Status.Running || lastResult == Status.Success)
            {
                return sequence[currentNode].ToString();
            }

            return sequence[currentNode - 1].ToString();
        }
    }

    /// <summary>
    /// Decorator node. This will execute his child and return the opposite of what his child has returned
    /// </summary>
    public class Inverter : BaseNode
    {
        private BaseNode node;
        private Status lastResult = Status.Running;

        public Inverter(string _id) : base(_id)
        {
            node = null;
        }
        
        public Inverter(string _id, BaseNode node) : base(_id)
        {
            this.node = node;
        }

        public override Status Execute()
        {
            if (lastResult != Status.Running)
            {
                node.Reset();
            }
            var ret = lastResult = node.Execute();

            if (ret == Status.Failure)
            {
                ret = Status.Success;
            }
            else if (ret == Status.Success)
            {
                ret = Status.Failure;
            }

            return ret;
        }

        public override void Reset()
        {
            node.Reset();
        }

        public override string ToString()
        {
            return node.ToString();
        }

        public BaseNode Node
        {
            get { return node; }
            set { node = value; }
        }
    }

    /// <summary>
    /// Decorator node. Execute his child and return Success or Running.
    /// </summary>
    public class Succeder : BaseNode
    {
        private BaseNode node;
        private Status lastResult = Status.Running;

        public Succeder(string _id) : base(_id)
        {
            node = null;
        }

        public Succeder(string _id, BaseNode node) : base(_id)
        {
            this.node = node;
        }

        public override Status Execute()
        {
            if (lastResult != Status.Running)
            {
                node.Reset();
            }
            var ret = lastResult = node.Execute();

            return ret != Status.Running ? Status.Success : Status.Running;
        }

        public override void Reset()
        {
            node.Reset();
        }

        public override string ToString()
        {
            return node.ToString();
        }

        public BaseNode Node
        {
            get { return node; }
            set { node = value; }
        }
    }

    /// <summary>
    /// Repeater node. Execute his child until the result is Failure (in that case, it returns Success)
    /// </summary>
    public class RepeatUntilFail : BaseNode
    {
        private BaseNode node;
        private Status lastResult = Status.Running;

        public RepeatUntilFail(string _id) : base(_id)
        {
            node = null;
        }

        public RepeatUntilFail(string _id, BaseNode node) : base(_id)
        {
            this.node = node;
        }

        public override Status Execute()
        {
            if (lastResult != Status.Running)
            {
                node.Reset();
            }

            var ret = lastResult = node.Execute();

            if (ret == Status.Failure)
            {
                ret = Status.Success;
            }
            else
            {
                ret = Status.Running;
            }

            return ret;
        }

        public override string ToString()
        {
            return node.ToString();
        }

        public BaseNode Node
        {
            get { return node; }
            set { node = value; }
        }
    }

    public class BehaviourTree
    {
        private BaseNode root;

        public BehaviourTree()
        {
            root = null;
        }

        public BehaviourTree(BaseNode root)
        {
            this.root = root;
        }

        public Status Execute()
        {
            return root.Execute();
        }

        public override string ToString()
        {
            return root.ToString();
        }

        public BaseNode Root
        {
            get { return root; }
            set { root = value; }
        }
    }

    public enum Status
    {
        Success,
        Failure,
        Running,
    }
}
