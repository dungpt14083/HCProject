using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityExtensions {
  public static Transform InstantiateAsChild(this Transform parent, Transform original) {
    Transform result = GameObject.Instantiate(original, parent.position, parent.rotation) as Transform;

    result.SetParent(parent);

    result.localScale = original.localScale;
    result.localPosition = original.localPosition;

    return result;
  }

  public static GameObject InstantiateAsChild(this Transform parent, GameObject original) {
    GameObject result = GameObject.Instantiate(original, parent.position, parent.rotation) as GameObject;

    result.transform.SetParent(parent);

    result.transform.localScale = original.transform.localScale;
    result.transform.localPosition = original.transform.localPosition;

    return result;
  }

  public static Component InstantiateAsChild(this Transform parent, Component original) {
    Component result = GameObject.Instantiate(original, parent.position, parent.rotation) as Component;
    if (result != null) {
      result.transform.SetParent(parent);
      if (result.gameObject != null)
  

      result.transform.localScale = original.transform.localScale;
      result.transform.localPosition = original.transform.localPosition;
    }
    return result;
  }

  public static UnityEngine.Object InstantiateAsChild(this Transform parent, UnityEngine.Object original) {
    Transform originalTransform = null;
    {
      Component component = original as Component;
      if (component != null) {
        originalTransform = component.transform;
      } else {
        GameObject obj = original as GameObject;
        if (obj != null) {
          originalTransform = obj.transform;
        } else {
          Transform t = original as Transform;
          if (t != null) {
            originalTransform = t;
          }
        }
      }
    }

    UnityEngine.Object result = UnityEngine.Object.Instantiate(original, parent.position, parent.rotation);
    if (result != null) {
      Component component = result as Component;
      if (component != null) {
        component.transform.SetParent(parent);
        if (originalTransform != null) {
          component.transform.localScale = originalTransform.localScale;
          component.transform.localPosition = originalTransform.localPosition;
        }
      } else {
        GameObject obj = result as GameObject;
        if (obj != null) {
          obj.transform.SetParent(parent);
          if (originalTransform != null) {
            obj.transform.localScale = originalTransform.localScale;
            obj.transform.localPosition = originalTransform.localPosition;
          }
    
        } else {
          Transform t = result as Transform;
          if (t != null) {
            t.SetParent(parent);
            if (originalTransform != null) {
              t.localScale = originalTransform.localScale;
              t.localPosition = originalTransform.localPosition;
            }
          }
        }
      }
    }
    return result;
  }

  public static Transform InstantiateAsChild(this GameObject parent, Transform original) {
    Transform result = GameObject.Instantiate(original, parent.transform.position, parent.transform.rotation) as Transform;

    result.SetParent(parent.transform);

    result.localScale = original.localScale;
    result.localPosition = original.localPosition;

    return result;
  }

  public static GameObject InstantiateAsChild(this GameObject parent, GameObject original) {
    GameObject result = GameObject.Instantiate(original, parent.transform.position, parent.transform.rotation) as GameObject;

    result.transform.SetParent(parent.transform);

    result.transform.localScale = original.transform.localScale;
    result.transform.localPosition = original.transform.localPosition;

    return result;
  }

  public static Component InstantiateAsChild(this GameObject parent, Component original) {
    Component result = GameObject.Instantiate(original, parent.transform.position, parent.transform.rotation) as Component;
    if (result != null) {
      result.transform.SetParent(parent.transform);
      if (result.gameObject != null)
  

      result.transform.localScale = original.transform.localScale;
      result.transform.localPosition = original.transform.localPosition;
    }
    return result;
  }

  public static UnityEngine.Object InstantiateAsChild(this GameObject parent, UnityEngine.Object original) {
    Transform originalTransform = null;
    {
      Component component = original as Component;
      if (component != null) {
        originalTransform = component.transform;
      } else {
        GameObject obj = original as GameObject;
        if (obj != null) {
          originalTransform = obj.transform;
        } else {
          Transform t = original as Transform;
          if (t != null) {
            originalTransform = t;
          }
        }
      }
    }

    UnityEngine.Object result;
    if (originalTransform != null) {
      result = UnityEngine.Object.Instantiate(original, originalTransform.position, originalTransform.rotation);
    } else {
      result = UnityEngine.Object.Instantiate(original);
    }
    if (result != null) {
      Component component = result as Component;
      if (component != null) {
        component.transform.SetParent(parent.transform);
        if (originalTransform != null) {
          component.transform.localScale = originalTransform.localScale;
          component.transform.localPosition = originalTransform.localPosition;
        }
      } else {
        GameObject obj = result as GameObject;
        if (obj != null) {
          obj.transform.SetParent(parent.transform);
          if (originalTransform != null) {
            obj.transform.localScale = originalTransform.localScale;
            obj.transform.localPosition = originalTransform.localPosition;
          }
    
        } else {
          Transform t = result as Transform;
          if (t != null) {
            t.SetParent(parent.transform);
            if (originalTransform != null) {
              t.localScale = originalTransform.localScale;
              t.localPosition = originalTransform.localPosition;
            }
          }
        }
      }
    }
    return result;
  }
  
  public static void ChangeLayer(this GameObject obj, int layer) {
    foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true))
      trans.gameObject.layer = layer;

#if OLD_CODE
        foreach (UIWidget widget in obj.GetComponentsInChildren<UIWidget>(true))
            widget.ParentHasChanged();
#endif
  }
}
