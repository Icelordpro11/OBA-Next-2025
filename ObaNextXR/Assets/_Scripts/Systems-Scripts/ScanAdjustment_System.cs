using System.Collections.Generic;
using UnityEngine;

public class ScanAdjustment_System : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private List<MeshRenderer> meshrenderers = new();

    private bool _moveHeight;
    private Vector3 _newPosition;

    private void Start()
    {
        transform.localRotation = PlayerPrefs_GetQuaternion("Xrot", "Yrot", "Zrot", "Wrot");
        transform.localPosition = PlayerPrefs_GetPosition("Xcor", "Ycor", "Zcor");
    }

    private void Update()
    {
        SwitchHeightMovement();

        MovePositionScan();
        RotateScan();

        SaveAnchorPoint();
    }

    public void DisableRenderer()
    {
        foreach (MeshRenderer r in meshrenderers)
        {
            r.enabled = false;
        }
    }

    private void MovePositionScan()
    {
        _newPosition.x = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x * (Time.deltaTime * speed);
        if (_moveHeight) { _newPosition.y = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y; }
        _newPosition.z = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y * (Time.deltaTime * speed);

        transform.Translate(_newPosition);
    }

    private void RotateScan()
    {
        if (_moveHeight) return;
        transform.Rotate(Vector3.up, OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x * (Time.deltaTime * speed * 2));
    }

    private void SaveAnchorPoint()
    {
        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            PlayerPrefs.SetFloat("Xcor", transform.localPosition.x);
            PlayerPrefs.SetFloat("Ycor", transform.localPosition.y);
            PlayerPrefs.SetFloat("Zcor", transform.localPosition.z);

            PlayerPrefs.SetFloat("Xrot", transform.localRotation.x);
            PlayerPrefs.SetFloat("Yrot", transform.localRotation.y);
            PlayerPrefs.SetFloat("Zrot", transform.localRotation.z);
            PlayerPrefs.SetFloat("Wrot", transform.localRotation.w);
            PlayerPrefs.Save();
        }
    }

    private void SwitchHeightMovement()
    {
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            _moveHeight = !_moveHeight;

            //transform.localRotation = new Quaternion(PlayerPrefs.GetFloat("Xrot"), PlayerPrefs.GetFloat("Yrot"),
            //    PlayerPrefs.GetFloat("Zrot"), PlayerPrefs.GetFloat("Wrot"));
            //transform.localPosition = new Vector3(PlayerPrefs.GetFloat("Xcor"), PlayerPrefs.GetFloat("Ycor"),
            //    PlayerPrefs.GetFloat("Zcor"));
        }
    }

    private Quaternion PlayerPrefs_GetQuaternion(string x, string y, string z, string w)
    {
        return new Quaternion(PlayerPrefs.GetFloat(x), PlayerPrefs.GetFloat(y), PlayerPrefs.GetFloat(z), PlayerPrefs.GetFloat(w));
    }

    private Vector3 PlayerPrefs_GetPosition(string x, string y, string z)
    {
        return new Vector3(PlayerPrefs.GetFloat(x), PlayerPrefs.GetFloat(y), PlayerPrefs.GetFloat(z));
    }
}
