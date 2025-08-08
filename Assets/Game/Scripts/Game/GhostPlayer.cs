using UnityEngine;
using DG.Tweening;

public class GhostPlayer : MonoBehaviour
{
    [Header("Animation Settings")]
    public float jumpPower = 2f;

    [Header("Effects")]
    public ParticleSystem collectEffect;
    public DissolveEffectController dissolveEffect;

    public PlatformManager ghostPlatformManager;

    public void Reset()
    {
        transform.localPosition = new Vector3(0, 1, 0);
        dissolveEffect.gameObject.SetActive(true);
    }

    public void ExecuteAction(PlayerAction action)
    {
        switch (action.actionType)
        {
            case ActionType.Jump:
                transform.DOLocalJump(
                    endValue: action.position,
                    jumpPower: jumpPower,
                    numJumps: 1,
                    duration: action.jumpDuration
                ).SetEase(Ease.InOutQuad);
                break;
            case ActionType.Collect:
                if (collectEffect != null)
                {
                    collectEffect.transform.localPosition = action.position;
                    collectEffect.Play();
                }
                Debug.Log($"[GhostPlayer] Received Collect action for ID: {action.collectibleID}");
                if (ghostPlatformManager != null)
                {
                    Collectible ghostCollectible = ghostPlatformManager.GetCollectibleByID(action.collectibleID);
                    if (ghostCollectible != null)
                    {
                        ghostCollectible.Collect();
                    }
                }
                break;
            case ActionType.Collide:
                if (dissolveEffect != null)
                {
                    dissolveEffect.StartDissolve();
                }
                break;
        }
    }
    
}