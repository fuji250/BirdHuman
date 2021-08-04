using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyManager : MonoBehaviour
{
    public OVRInput.Controller controller;
    public GameObject leftHand;
    public GameObject rightHand;
    protected CharacterController Controller = null;

    // Start is called before the first frame update
    void Start()
    {
        Controller = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // ポジションが上のときかつフラグが立っていない時フラグを立てる
        if (OVRInput.GetLocalControllerPosition(controller).y >= 1)
        {
            if (FlagManager.Instance.flags[0] != true)
            {
                FlagManager.Instance.flags[0] = true;
                DebugUIBuilder.instance.AddLabel("上");
                // クォータニオン → オイラー角への変換
                Vector3 rotationLeftAngles = leftHand.gameObject.transform.rotation.eulerAngles;
                DebugUIBuilder.instance.AddLabel(rotationLeftAngles.ToString());
            }
        }

        // ポジションが下のときかつフラグが立っている時フラグを折る
        if (OVRInput.GetLocalControllerPosition(controller).y < 1)
        {
            if (FlagManager.Instance.flags[0] == true)
            {
                FlagManager.Instance.flags[0] = false;
                DebugUIBuilder.instance.AddLabel("下");
                // クォータニオン → オイラー角への変換
                Vector3 rotationRightAngles = rightHand.gameObject.transform.rotation.eulerAngles;
                DebugUIBuilder.instance.AddLabel(rotationRightAngles.ToString());

                Rigidbody rb = this.GetComponent<Rigidbody> ();  // rigidbodyを取得
                Vector3 force = new Vector3 (0.0f,0.01f,0.0f);    // 力を設定
                rb.AddForce (force);  // 力を加える
            }
        }

        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        //if (Input.GetMouseButtonDown(0))
        {
            DebugUIBuilder.instance.AddLabel("右人差し指トリガーを押した");
            //DebugUIBuilder.instance.AddLabel(leftHand.gameObject.transform.position.ToString());
            //DebugUIBuilder.instance.AddLabel(rightHand.gameObject.transform.position.ToString());
            DebugUIBuilder.instance.AddLabel(FlagManager.Instance.flags[0].ToString());
            Rigidbody rb = this.GetComponent<Rigidbody> ();  // rigidbodyを取得
            Vector3 force = new Vector3 (0.0f,0.01f,0.0f);    // 力を設定
            rb.AddForce (force);  // 力を加える
        }

        // もし右手が左手より0.8m低いなら
        if (leftHand.gameObject.transform.position.y - rightHand.gameObject.transform.position.y > 0.8)
        {
            DebugUIBuilder.instance.AddLabel("右回転");
            Rigidbody rb = this.GetComponent<Rigidbody> ();  // rigidbodyを取得
            Vector3 m_EulerAngleVelocity = new Vector3(0, 100, 0);
            Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.deltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);  // 右回転
        }

        // もし右手が左手より0.8m高いなら
        if (leftHand.gameObject.transform.position.y - rightHand.gameObject.transform.position.y < -0.8)
        {
            DebugUIBuilder.instance.AddLabel("左回転");
            Rigidbody rb = this.GetComponent<Rigidbody> ();  // rigidbodyを取得
            Vector3 m_EulerAngleVelocity = new Vector3(0, -100, 0);
            Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.deltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);  // 左回転
        }
    }
}
