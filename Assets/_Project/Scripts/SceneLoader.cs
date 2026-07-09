using UnityEngine;
using UnityEngine.SceneManagement;

namespace SemiconductorTeaching
{
    public sealed class SceneLoader : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            if (!string.IsNullOrWhiteSpace(sceneName))
                SceneManager.LoadScene(sceneName);
        }
    }
}
