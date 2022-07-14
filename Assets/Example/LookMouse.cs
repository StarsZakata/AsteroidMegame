using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookMouse : MonoBehaviour
{
    [Range(1, 3)]
    public int sposob = 1;


    public float speed = 5f;

    void Update()
    {
        if (sposob == 1)
            OneMouse();
        if (sposob == 2)
            TwoMouse();
        if (sposob == 3)
            ThreeMouse();
    }


    private void OneMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 direction = new Vector2(
                                mousePosition.x - transform.position.x,
                                mousePosition.y - transform.position.y
        );
        transform.up = direction;
    }

    private void TwoMouse()
    {
        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void ThreeMouse()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, speed * Time.deltaTime);
    }
}
