using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad;
    [SerializeField] private AudioClip clickSound;

    public void LoadScene()
    {
        // Проигрываем звук перед загрузкой сцены
        if (AudioManager.Instance != null && clickSound != null)
        {
            AudioManager.Instance.PlaySound(clickSound);
        }

        // Если возвращаемся на главную сцену
        if (sceneToLoad == "Main")
        {
            SceneManager.sceneLoaded += OnMainMenuLoaded;

            // Удаляем старый AudioManager, если он существует
            AudioManager[] audioManagers = FindObjectsOfType<AudioManager>();
            foreach (AudioManager am in audioManagers)
            {
                Destroy(am.gameObject);
            }
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnMainMenuLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main")
        {
            // Создаем новый AudioManager
            Instantiate(Resources.Load<GameObject>("AudioManager"));

            // Находим кнопки в главном меню
            var musicButton = GameObject.FindGameObjectWithTag("MusicButton")?.GetComponent<Button>();
            var sfxButton = GameObject.FindGameObjectWithTag("SFXButton")?.GetComponent<Button>();

            if (musicButton != null && AudioManager.Instance != null)
                AudioManager.Instance.SetMusicButtonReference(musicButton);
            if (sfxButton != null && AudioManager.Instance != null)
                AudioManager.Instance.SetSFXButtonReference(sfxButton);

            SceneManager.sceneLoaded -= OnMainMenuLoaded;
        }
    }
}