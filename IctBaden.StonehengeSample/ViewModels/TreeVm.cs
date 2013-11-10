using System;
using System.Collections.Generic;
using System.Linq;
using IctBaden.Stonehenge;
using IctBaden.Stonehenge.Tree;

namespace IctBaden.StonehengeSample.ViewModels
{
  public class TreeVm : ActiveViewModel
  {
    public List<TreeNode> DemoTreeData { get; private set; }
    public TreeSettings DemoTreeSettings { get; private set; }
    public int DemoTreeSelected { get; set; }

    public TreeVm()
    {
      // http://www.ztree.me/v3/main.php#_zTreeInfo

      var tree = new List<TreeNode>
      {
        new TreeNode("test1"), 
        new TreeNode("test2"), 
        new TreeNode("test3"), 
        new TreeNode("test4"), 
        new TreeNode("test5"),
        new TreeNode("test1"), 
        new TreeNode("test2"), 
        new TreeNode("test3"), 
        new TreeNode("test4"), 
        new TreeNode("test5"),
        new TreeNode("test1"), 
        new TreeNode("test2"), 
        new TreeNode("test3"), 
        new TreeNode("test4"), 
        new TreeNode("test5"),
        new TreeNode("test1"), 
        new TreeNode("test2"), 
        new TreeNode("test3"), 
        new TreeNode("test4"), 
        new TreeNode("test5"),
      };

      var child = new TreeNode("child1") { @checked = true };
      tree[2].AddChild(child);

      DemoTreeData = tree; 
      DemoTreeSettings = new TreeSettings
      {
        check = new TreeCheck
        {
          enable = true, 
          chkStyle = "radio",
          chkboxType = new Dictionary<string,string> { {"Y", ""}, {"N", ""} }
        }
      };

    }

    [ActionMethod]
    public void OnTreeChecked()
    {
      DemoTreeData.ForEach(n => n.Check(DemoTreeSelected));
    }
  }
}