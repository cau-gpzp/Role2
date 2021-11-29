using UnityEngine;

[RequireComponent(typeof(Collider))]
[AddComponentMenu("JU Voxel/Extras/Trigger Checker")]
public class TriggerCollisionChecker : MonoBehaviour
{
    public TypeOfDraw Draw;
    public bool Collliding;
    [HideInInspector]
    public GameObject Object = null;
    
    public enum TypeOfDraw
    {
        Box,
        Sphere
    }
    public void DisableColliding()
    {
        Collliding = false;
        Object = null;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != 10)
        {
            Collliding = true;
            Object = other.gameObject;
        }
        else
        {
            Collliding = false;
            Object = null;
        }
    }
    

    private void OnTriggerExit(Collider other)
    {
        Object = null;
        Collliding = false;
    }
  
    private void OnDrawGizmos()
    {
        if (Collliding == true)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        if (Draw == TypeOfDraw.Box)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z));
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, transform.lossyScale.z / 2);
        }
    }
}
