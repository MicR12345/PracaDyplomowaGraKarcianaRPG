using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FancyCardMover : MonoBehaviour
{
    static public float speed = 0.30f;
    static public float rotationSpeed = 0.1f;
    public Vector3 moveCardTo = new Vector3(0f,0f,0f);
    public Vector3 rotateTo = new Vector3(0f, 0f, 0f);
    void Update()
    {
        Vector3 position = this.transform.position;
        if (this.transform.position != moveCardTo)
        {
            Vector3 newScreenPosition = Vector3.Lerp(position, moveCardTo, speed);
            this.transform.position = new Vector3(newScreenPosition.x, newScreenPosition.y, newScreenPosition.z);
        }
        if (this.transform.rotation != Quaternion.Euler(rotateTo))
        {
            Quaternion newRotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(rotateTo), rotationSpeed);
            this.transform.rotation = newRotation;
        }
    }
}
