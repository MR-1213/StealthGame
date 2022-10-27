using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;

    public Transform targetTransform; //カメラが追従するオブジェクト(Player)
    public Transform cameraPivot; //上下方向の視点を実現するためのオブジェクト
    public Transform cameraTransform;　//最初のカメラの位置
    public float defaultPosition;
    public LayerMask collisionLayers;  //レイキャスト時に選択的に衝突を無視するために使う
    
    private Vector3 cameraFollowVelocity = Vector3.zero; //カメラがプレイヤーを追跡する速度
    private Vector3 cameraVectorPosition;

    public float cameraCollisionRadius = 2.0f;
    public float cameraCollisionOffSet = 0.2f;
    public float minimumCollisionOffSet = 0.2f;
    public float cameraFollowSpeed = 0.2f;
    public float cameraLookSpeed = 2.0f;
    public float cameraPivotSpeed = 2.0f;
    public float lookAngle; //上下方向のカメラ
    public float pivotAngle;//左右方向のカメラ
    public float minimumPivotAngle = -35.0f; //カメラの下方向の制限。
    public float maximumPivotAngle = 35.0f;　//カメラの上方向の制限。

    private void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        targetTransform = FindObjectOfType<PlayerManager>().transform; //プレイヤーの位置を取得
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z; //カメラがプレイヤーの後方何mの位置かを取得
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }

    private void FollowTarget()
    {
        /* 現在地(カメラのtransform.position)から目的地(プレイヤーのtransform.position)へ時間の経過とともに徐々にベクトルを変化させる。これによりプレイヤーが移動してもカメラが自然に追従する。 */
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);
        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed); //横方向の視点移動
        pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed); //上下方向の視点移動
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle); //上下方向の視点は制限内に視点の回転を抑える

        Vector3 rotation = Vector3.zero; //初期化
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation); //オイラー角でクオータニオンを指定できる関数
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position; //レイを飛ばす方向。カメラの高さを引くことで地面と水平かつカメラのある方向へレイを飛ばす。
        direction.Normalize();//長さを1にする

        /* 球型のレイを飛ばしてコライダーのついているオブジェクトとヒットするかどうかを調べる(壁などとの衝突を検知) */
        /* 参考 https://tsubakit1.hateblo.jp/entry/2016/02/25/025922 */
        if(Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition =- (distance - cameraCollisionOffSet);
        }

        if(Mathf.Abs(targetPosition) < minimumCollisionOffSet)
        {
            targetPosition = targetPosition - minimumCollisionOffSet;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
}
