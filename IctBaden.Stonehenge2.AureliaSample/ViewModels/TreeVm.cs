using System.Collections.Generic;
using IctBaden.Stonehenge2.Core;
using IctBaden.Stonehenge2.ViewModel;

namespace IctBaden.Stonehenge2.Aurelia.Sample.ViewModels
{
    public class TreeVm : ActiveViewModel
    {
        private readonly TreeNode _world;
        public List<TreeNode> RootNodes => new List<TreeNode>() { _world };

        public TreeVm(AppSession session) : base (session)
        {
            _world = new TreeNode(null)
            {
                Name = "world",
                IsExpanded = true
            };

            var america = new TreeNode(_world) { Name = "America" };
            america.Children = new List<TreeNode>
            {
                new TreeNode(america) {Name = "North America"},
                new TreeNode(america) {Name = "South America"}
            };
            var eurasia = new TreeNode(_world) { Name = "Eurasia" };
            eurasia.Children = new List<TreeNode>
            {
                new TreeNode(eurasia) {Name = "Europe"},
                new TreeNode(eurasia) {Name = "Asia"}
            };

            _world.Children = new List<TreeNode>
            {
                america,
                eurasia,
                new TreeNode(_world) {Name = "Africa"},
                new TreeNode(_world) {Name = "Australia"},
                new TreeNode(_world) {Name = "Antarctica"}
            };
        }

        [ActionMethod]
        public void TreeToggle(string nodeId)
        {
            var node = RootNodes[0].FindNodeById(nodeId);
            if (node == null) return;

            node.IsExpanded = !node.IsExpanded;
        }

        [ActionMethod]
        public void TreeSelect(string nodeId)
        {
            var node = _world.FindNodeById(nodeId);
            if (node == null) return;

            foreach (var treeNode in _world.AllNodes())
            {
                treeNode.IsSelected = false;
            }
            node.IsSelected = true;
        }

    }
}
