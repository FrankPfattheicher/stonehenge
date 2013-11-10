using System.Linq;
using ServiceStack.Common.Extensions;

namespace IctBaden.Stonehenge.Tree
{
  public class TreeNode
  {
    private static int nextId = 1;

    public int id { get; set; }
    public int pId { get; set; }
    public string name { get; set; }
    public bool open { get; set; }
    public bool @checked { get; set; }
    public TreeNode[] children { get; set; }
    public string click { get; set; }

    private TreeNode Parent;

    public TreeNode(string text)
    {
      id = nextId++;
      name = text;
    }

    public void AddChild(TreeNode child)
    {
      var list = (children ?? new TreeNode[0]).ToList();
      list.Add(child);
      children = list.ToArray();
      child.Parent = this;
    }

    public void Check(int checkId)
    {
      @checked = id == checkId;
      if (@checked && (Parent != null))
      {
        Parent.open = true;
      }

      if (children == null)
        return;
      children.ForEach(child => child.Check(checkId));
    }
  }
}