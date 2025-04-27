using UnityEngine;

public class SyncCameraPosition : MonoBehaviour
{
    [SerializeField] GameObject Player;
    PlayerController playerController;
    [SerializeField] Transform parentPosition;
    [SerializeField] Transform childPosition;
    private float _crouchOffset;
    Vector3 crouchOffset = Vector3.down;
    Vector3 yOffset = Vector3.up;
    readonly float _yOffset = 0.6f; //TODO: edit this offset later to match the upcoming character model
    void Awake()
    {
        playerController = Player.GetComponent<PlayerController>();
        childPosition.localPosition = parentPosition.localPosition + yOffset * _yOffset ;
    }

    void FixedUpdate()
    {
        if (playerController.isCrouching) 
        {
            _crouchOffset = Mathf.Lerp(0f,0.47f,30f * Time.fixedDeltaTime); 
        }
        else { _crouchOffset = 0; }
        childPosition.localPosition = parentPosition.localPosition + yOffset * _yOffset + crouchOffset * _crouchOffset;
    }
}
