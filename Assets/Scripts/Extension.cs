using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension  {

	public static Vector2 toVec2(this Transform trans)
    {
        return new Vector2(trans.position.x, trans.position.y);
    } 
  
}
