using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadtreePencil : MonoBehaviour
{
  public QuadtreeComponent quadtree;

  // Start is called before the first frame update
  void Start()
  {
    
  }

  // Update is called once per frame
  void Update()
  {
    var insertPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Input.GetMouseButton(0))
    {
      Debug.Log(insertPoint.origin);
      quadtree.Quadtree.Insert(false, insertPoint.origin);
    }
    else if (Input.GetMouseButton(1))
    {  
      quadtree.Quadtree.Insert(true, insertPoint.origin);
    }
  }
}
