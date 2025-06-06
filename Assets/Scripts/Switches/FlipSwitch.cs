using UnityEngine;
using System.Collections;

public class FlipSwitch : PressInputBase
{
    [SerializeField] private Transform targetObject;
    [SerializeField] private AudioClip onAudioClip;
    [SerializeField] private AudioClip offAudioClip;

    private AudioSource audioSource;
    public bool on = false;
    private bool canFlip = true;
    private float cooldownTime = 0.75f; // Cooldown duration in seconds

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            // Add an AudioSource component if it doesn't exist
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    protected override void OnPress(Vector3 position)
    {
        if (!canFlip) return; // Prevent flipping if cooldown is active

        Ray ray = Camera.main.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == targetObject)
        {
            StartCoroutine(FlipSwitchCooldown());
        }
    }

    private IEnumerator FlipSwitchCooldown()
    {
        canFlip = false;
        targetObject.rotation *= Quaternion.Euler(0, 180, 0);
        on = !on;
        audioSource.PlayOneShot(on ? onAudioClip : offAudioClip);

        yield return new WaitForSeconds(cooldownTime);
        canFlip = true;
    }
}
