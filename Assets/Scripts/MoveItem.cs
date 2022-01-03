using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MoveItem : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;

    public Vector3 StartPos;
    private int _triggerCount = 0;

    public PhotonView view;
    private bool _moveable;

    private void Start()
    {
        StartPos = gameObject.transform.position;
        view = GetComponent<PhotonView>();
        _moveable = true;
    }

    private void OnMouseDown()
    {
        if ((view.IsMine || PhotonNetwork.PlayerList.Length == 1) && _moveable)
        {
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            mOffset = gameObject.transform.position - GetMouseWorldPos();
        }
    }
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
        if ((view.IsMine || PhotonNetwork.PlayerList.Length == 1) && _moveable)
        {
            transform.position = GetMouseWorldPos() + mOffset;
            transform.position = new Vector3(gameObject.transform.position.x, 5, gameObject.transform.position.z);
        }
    }

    private void OnMouseUp()
    {
        if ((view.IsMine || PhotonNetwork.PlayerList.Length == 1) && _moveable)
        {
            /*if (_triggerCount == 1)
            {
                transform.position = new Vector3(gameObject.transform.position.x, 4, gameObject.transform.position.z);
            }
            else
            {
                transform.position = StartPos;
            }*/
            if (_triggerCount == 0)
            {
                transform.position = StartPos;
            }
        }
    }

    private void OnTriggerEnter()
    {
        _triggerCount++;
    }

    private void OnTriggerExit()
    {
        _triggerCount--;
    }

    public void EnableMovement()
    {
        _moveable = true;
    }

    public void DisableMovement()
    {
        _moveable = false;
    }
}
