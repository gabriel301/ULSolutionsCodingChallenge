using UL.Domain.Enumeration;

namespace UL.Domain.Entities.Expression;
public class Node : IDisposable, IEquatable<Node>
{

    private Guid _id;

    public Node? ParentNode { get; private set; }
    public Node? LeftChild { get; private set; }
    public Node? RightChild { get; private set; }

    public string Value { get; private set; } = string.Empty;
    public NodeTypeEnum Type { get; private set; }

    private bool disposed = false;

    public Node(string value, NodeTypeEnum type)
    {
        _id = Guid.NewGuid();
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

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                ParentNode = null;
                LeftChild = null;
                RightChild = null;
                Value = string.Empty;
            }
            disposed = true;
        }

    }

    public bool Equals(Node? other)
    {
        if (other == null || object.Equals(other, default)) return false;

        return this._id!.Equals(other._id);
    }

    public override int GetHashCode()
    {
        return _id.GetHashCode();
    }

    //Destructor
    ~Node()
    {
        Dispose(false);
    }
}
