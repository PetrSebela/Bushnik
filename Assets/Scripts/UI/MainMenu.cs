using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        public void LoadWorld()
        {
            SceneManager.LoadScene("World");
        }
    }
}
