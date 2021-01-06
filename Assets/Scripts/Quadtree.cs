using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum QuadtreeIndex
{
  // left / right on 1st bit
  // top / bottom on 2nd bit
  TopLeft = 0, //00
  TopRight = 1, //01
  BottomLeft = 2, //10
  BottomRight = 3, //11
}

public class Quadtree<TType>
{
  private QuadtreeNode<TType> node;
  private int depth;
  private TType data;

  public event EventHandler QuadtreeUpdated;
  public Quadtree(Vector2 position, float size, int depth)
  {
    node = new QuadtreeNode<TType>(position, size, depth);
    this.depth = depth;
  }

  public void Insert(TType value, Vector2 position)
  {
    var leafNode = node.Subdivide(value, position, depth);
    leafNode.Data = value;
    NotifyQuadtreeUpdate();
  }

  private void NotifyQuadtreeUpdate()
  {
    if (QuadtreeUpdated != null)
    {
      QuadtreeUpdated(this, new EventArgs());
    }
  }

  public class QuadtreeNode<TType> {
    Vector2 position;
    float size;
    QuadtreeNode<TType>[] subNodes;
    TType data;
    int depth;

    public QuadtreeNode(Vector2 position, float size, int depth, TType value = default)
    {
      this.position = position;
      this.size = size;
      this.depth = depth;
    }

    public IEnumerable<QuadtreeNode<TType>> Nodes => subNodes;

    public Vector2 Position => position;

    public TType Data
    {
      get { return data; }
      internal set
      {
        this.data = value;
      }
    }

    public float Size => size;

    public QuadtreeNode<TType> Subdivide(TType value, Vector2 targetPosition, int depth = 0)
    {
      if (depth == 0)
      {
        return this;
      }
      
      var subDivIndex = GetIndexOfPosition(targetPosition, position);
      if (subNodes == null)
      {
        subNodes = new QuadtreeNode<TType>[4];
        for (int i = 0; i < subNodes.Length; ++i)
        {
          Vector2 newPos = position;
          // examine bit & handle positioning
          if ((i & 2) == 2)
          {
            newPos.y -= size * 0.25f;
          }
          else
          {
            newPos.y += size * 0.25f;
          }

          if ((i & 1) == 1)
          {
            newPos.x += size * 0.25f;
          }
          else
          {
            newPos.x -= size * 0.25f;
          }

          // insert quadtree to subnode
          subNodes[i] = new QuadtreeNode<TType>(newPos, size * 0.5f, depth - 1);
        }
      }

      // walk up the tree & subdivide
      return subNodes[subDivIndex].Subdivide(value, targetPosition, depth - 1);
    }

    public bool IsLeaf {
      get {
        return depth == 0;
      }
    }

    public IEnumerable<QuadtreeNode<TType>> GetLeafNodes()
    {
      if(IsLeaf)
      {
        yield return this;
      }
      else
      {
        if (Nodes != null)
        {
          foreach (var node in Nodes)
          {
            foreach (var leaf in node.GetLeafNodes())
            {
              yield return leaf;
            }
          }
        }
       
      }
    }
  }

  private static int GetIndexOfPosition(Vector2 lookupPosition, Vector2 nodePosition)
  {
    int index = 0;

    index |= lookupPosition.y < nodePosition.y ? 2 : 0;
    index |= lookupPosition.x > nodePosition.x ? 1 : 0;

    return index;
  }

  public QuadtreeNode<TType> GetRoot => node;

  public IEnumerable<QuadtreeNode<TType>> GetLeafNodes ()
  {
    return node.GetLeafNodes();
  }
}
