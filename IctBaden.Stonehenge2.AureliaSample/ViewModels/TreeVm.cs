using System;
using System.Collections.Generic;
using IctBaden.Stonehenge2.Core;
using IctBaden.Stonehenge2.ViewModel;

namespace IctBaden.Stonehenge2.AureliaSample.ViewModels
{
    public class TreeVm : ActiveViewModel
    {
        private readonly TreeNode world;
        public List<TreeNode> RootNodes => new List<TreeNode>() { world };

        public TreeVm(AppSession session) : base (session)
        {
            world = new TreeNode(null)
            {
                Name = "world",
                IsExpanded = true
            };

            var america = new TreeNode(world) { Name = "America" };
            america.Children = new List<TreeNode>
            {
                new TreeNode(america) {Name = "North America"},
                new TreeNode(america) {Name = "South America"}
            };
            var eurasia = new TreeNode(world) { Name = "Eurasia" };
            eurasia.Children = new List<TreeNode>
            {
                new TreeNode(eurasia) {Name = "Europe"},
                new TreeNode(eurasia) {Name = "Asia"}
            };

            world.Children = new List<TreeNode>
            {
                america,
                eurasia,
                new TreeNode(world) {Name = "Africa"},
                new TreeNode(world) {Name = "Australia"},
                new TreeNode(world) {Name = "Antarctica"}
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
            var node = world.FindNodeById(nodeId);
            if (node == null) return;

            foreach (var treeNode in world.AllNodes())
            {
                treeNode.IsSelected = false;
            }
            node.IsSelected = true;
        }

    }
}
