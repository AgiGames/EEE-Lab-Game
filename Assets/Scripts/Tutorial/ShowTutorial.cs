using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayImageOnClick : MonoBehaviour
{
    public Button button;  // Reference to the button
    public Image image;    // Reference to the image
    public TextMeshProUGUI buttonText;  // Reference to the TextMeshProUGUI text
    bool on = false;
    public AudioSource audioSource;     // Reference to the AudioSource component
    public AudioClip clickSound;        // Reference to the sound effect

    void Start()
    {
        // Add a listener to the button's onClick event
        button.onClick.AddListener(OnButtonClick);
        // Set the audio clip to the AudioSource if it's not assigned in the Inspector
        if (audioSource != null && clickSound != null)
        {
            audioSource.clip = clickSound;
        }
    }

    void OnButtonClick()
    {
        // Play the sound when the button is clicked
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        on = !on;
        if (on)
        {
            // Set the image scale to its normal size (1, 1, 1)
            image.rectTransform.localScale = new Vector3(1, 1, 1);
            // Change the button text
            buttonText.text = "Close Tutorial";
        }
        else
        {
            // Set the image scale to zero size (0, 0, 1)
            image.rectTransform.localScale = new Vector3(0, 0, 1);
            // Change the button text
            buttonText.text = "Tutorial";
        }
    }
}
