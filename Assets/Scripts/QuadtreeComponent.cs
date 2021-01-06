using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class QuadtreeComponent : MonoBehaviour
{
  public float size = 5;
  public int depth = 2;
  private Quadtree<bool> quadtree;

  public Quadtree<bool> Quadtree => quadtree;

  void Awake()
  {
    quadtree = new Quadtree<bool>(this.transform.position, size, depth);
  }

  private void OnDrawGizmos()
  {
    if (quadtree != null)
    {
      DrawNode(quadtree.GetRoot);
    }
  }

  private Color minColor = new Color(1, 1, 1, 1f);
  private Color maxColor = new Color(0, 0.5f, 1, 0.25f);

  private void DrawNode(Quadtree<bool>.QuadtreeNode<bool> node, int nodeDepth = 0)
  {
    if(!node.IsLeaf && node.Nodes != null)
    {
      foreach (var subNode in node.Nodes)
      {
        if (subNode != null)
        {
          DrawNode(subNode, nodeDepth + 1);
        }
      }
    }
    Gizmos.color = Color.Lerp(minColor, maxColor, nodeDepth / (float)depth);
    Gizmos.DrawWireCube(node.Position, new Vector3(1, 1, 0.1f) * node.Size);
  }
}
