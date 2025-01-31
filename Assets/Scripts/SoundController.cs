using UnityEngine;
using System.IO;
using System.Linq;

public class SoundController : MonoBehaviour
{
    private AudioSource _audioSource;
    private string _folder;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogWarning("no tiene audiosource");
        }
    }

    public void PlaySound(string soundPath, float volume = 1f)
    {
        if (_audioSource != null)
        {
            AudioClip soundClip = Resources.Load<AudioClip>(soundPath);
            if (soundClip != null)
            {
                _audioSource.volume = volume;
                _audioSource.PlayOneShot(soundClip);
            }
            else
            {
                Debug.LogError($"no se pudo cargarrrrr: {soundPath}");
            }
        }
        else
        {
            Debug.LogWarning("audiosource no disponible :/");
        }
    }


    public string GetRandomSound(string _folder, string _format)
    {
        string fullPath = Path.Combine(Application.dataPath, "Resources", _folder);

        if (Directory.Exists(fullPath))
        {
            string[] files = Directory.GetFiles(fullPath, _format, SearchOption.TopDirectoryOnly);

            if (files.Length > 0)
            {
                string randomFile = files[Random.Range(0, files.Length)];

                string fileName = Path.GetFileNameWithoutExtension(randomFile);

                return $"{_folder}/{fileName}";
            }
            else
            {
                Debug.LogWarning($"Carpeta vacía {_folder}");
                return null;
            }
        }
        else
        {
            Debug.LogWarning($"Carpeta no existe {_folder}");
            return null;
        }
    }
}