using System;
using System.Collections.Generic;
using System.Linq;

namespace IctBaden.Stonehenge2.Aurelia.Sample.ViewModels
{
    public class TreeNode
    {
        public string Id { get; }
        public string Name { get; set; }

        public List<TreeNode> Children { get; set; }

        public bool IsVisible => _parent?.IsExpanded ?? true;

        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }

        public bool HasChildren => Children.Count > 0;
        public string Icon => IsExpanded ? "fa fa-folder-open" : "fa fa-folder";
        public string Class => IsSelected ? "tree-selected" : "";

        private readonly TreeNode _parent;
        public TreeNode(TreeNode parentNode)
        {
            Id = Guid.NewGuid().ToString("N");
            Children = new List<TreeNode>();
            _parent = parentNode;
        }

        public IEnumerable<TreeNode> AllNodes()
        {
            yield return this;
            foreach (var node in Children.SelectMany(child => child.AllNodes()))
            {
                yield return node;
            }
        }

        public TreeNode FindNodeById(string id)
        {
            return AllNodes().FirstOrDefault(node => node.Id == id);
        }
    }
}