using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSoundController : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound;
    [Range(0, 1)] public float volume = 0.7f;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (AudioManager.Instance != null)
        {
            // »спользуем кастомный звук или стандартный из AudioManager
            AudioClip soundToPlay = clickSound != null ? clickSound : AudioManager.Instance.buttonClickSound;
            AudioManager.Instance.PlaySound(soundToPlay, volume);
        }
    }

    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}