using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTModel
{
    [System.Serializable]
    public class BehaviourTree
    {
        public string version;
        public string scope;
        public string id;
        public string title;
        public string description;
        public string root;

        public Dictionary<string, BehaviourTreeNode> nodes;
        public BehaviourTreeNode[] debugNodes;
    }

    [System.Serializable]
    public class BehaviourTreeNode
    {
        public string id;
        public string name;
        public string title;
        public string description;
        public Dictionary<string, object> properties;
        public string child;
        public List<string> children = new List<string>();
    }
}