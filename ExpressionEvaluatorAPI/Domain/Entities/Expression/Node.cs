using Domain.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Expression;
 public class Node
{
    public Node? ParentNode { get; private set; }
    public Node? LeftChild { get; private set; }
    public Node? RightChild { get; private set; }

    public string Value { get; private set; } = string.Empty;
    public NodeTypeEnum Type { get; private set; }

    public Node(string value, NodeTypeEnum type) 
    {
        Value = value;
        Type = type;
    }

    public void Clear() 
    {
        ParentNode = null;
        LeftChild = null;
        RightChild = null;
        Value = string.Empty;
    }


    public void SetParent(Node? parent) 
    {
        this.ParentNode = parent;
    }

    public void SetLeftChild(Node? child)
    {
        this.LeftChild = child;
    }

    public void SetRightChild(Node? child)
    {
        this.RightChild = child;
    }

    public bool HasParent() 
    {
        return ParentNode != null;
    }

    public bool HasLeftChild()
    {
        return LeftChild != null;
    }

    public bool HasRightChild()
    {
        return RightChild != null;
    }


}
