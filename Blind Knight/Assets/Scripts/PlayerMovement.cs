using System;
using System.Collections;
using UnityEngine;
/// <summary>
/// This class detects target colliders for the player to move to when
/// their colliders collide with it (MoveLocation)
/// also has footstep sounds with slight volume variation
/// the picked number footstep sounds are defined by numberOfSteps
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Collider upCollider;
    [SerializeField]
    private Collider downCollider;
    [SerializeField]
    private Collider leftCollider;
    [SerializeField]
    private Collider rightCollider;
    
    [SerializeField]
    private float moveDelay = 0.2f;
    
    [Header("Footstep Sounds")]
    [SerializeField]
    private AudioClip[] footstepSounds;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private int numberOfSteps = 3;
    [SerializeField]
    private float timeBetweenSteps = 0.15f;
    [SerializeField]
    private float volumeRange = 0.2f;
    
    private float lastMoveTime;
    private Transform playerTransform;
    private Coroutine footstepCoroutine;

    private void Start()
    {
        playerTransform = transform;
        lastMoveTime = -moveDelay;
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    public void Update()
    {
        if (Time.time - lastMoveTime < moveDelay)
            return;
            
        if (Input.GetKeyDown(KeyCode.UpArrow) && upCollider)
        {
            TryMove(upCollider, Vector3.up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && downCollider)
        {
            TryMove(downCollider, Vector3.down);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && leftCollider)
        {
            TryMove(leftCollider, Vector3.left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && rightCollider)
        {
            TryMove(rightCollider, Vector3.right);
        }
    }
    
    private void TryMove(Collider sideCollider, Vector3 direction)
    {
        MoveLocation targetLocation = GetTouchingMoveLocation(sideCollider);
        
        if (targetLocation != null)
        {
            playerTransform.position = targetLocation.transform.position;
            lastMoveTime = Time.time;
            
            // Stop current footstep coroutine if playing
            if (footstepCoroutine != null)
            {
                StopCoroutine(footstepCoroutine);
            }
            
            // Start new footstep sequence
            footstepCoroutine = StartCoroutine(PlayFootstepSequence());
            
            Debug.Log($"Moved to {targetLocation.name}");
        }
    }
    
    private IEnumerator PlayFootstepSequence()
    {
        for (int i = 0; i < numberOfSteps; i++)
        {
            PlayRandomFootstepSound();
            yield return new WaitForSeconds(timeBetweenSteps);
        }
        footstepCoroutine = null;
    }
    
    private void PlayRandomFootstepSound()
    {
        if (footstepSounds == null || footstepSounds.Length == 0)
        {
            Debug.LogWarning("No footstep sounds assigned!");
            return;
        }
        
        float randomVolume = UnityEngine.Random.Range(1f - volumeRange, 1f);        
        audioSource.PlayOneShot(GetRandomFootstepSound(), randomVolume);
    }
    
    private AudioClip GetRandomFootstepSound()
    {
        int randomIndex = UnityEngine.Random.Range(0, footstepSounds.Length);
        return footstepSounds[randomIndex];
    }
    
    private MoveLocation GetTouchingMoveLocation(Collider collider)
    {
        Collider[] touchingColliders = Physics.OverlapBox(
            collider.bounds.center, 
            collider.bounds.extents, 
            collider.transform.rotation
        );
        
        foreach (Collider touchedCollider in touchingColliders)
        {
            if (touchedCollider.transform == transform)
                continue;
                
            MoveLocation moveLocation = touchedCollider.GetComponent<MoveLocation>();
            if (moveLocation != null)
                return moveLocation;
        }
        
        return null;
    }
}