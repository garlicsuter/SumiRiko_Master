using System.Collections;
using UnityEngine;

public class SocketChildReset : MonoBehaviour
{
    public IEnumerator Wait( Socket socket, GameObject obj )
    {
        while (socket.IsGrabbed(obj))
        {
            yield return null;
        }

        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.transform.parent = null;
        Destroy(this);
    }
}