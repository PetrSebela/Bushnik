using Terrain;
using Terrain.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using User;

namespace Game.World
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Rigidbody player;
        
        private bool _isLoaded = false;
        private bool _isPaused = false;
        
        public bool IsLoaded => _isLoaded;
        
        public bool IsPaused => _isPaused;
        
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        void Awake()
        {
            _instance = this;
        }
        
        void Start()
        {
            player.isKinematic = true;
            Loader.Instance.AfterPregeneration += SpawnPlayer;
            Loader.Instance.AfterLoading += AfterLoad;
            Loader.Instance.Load();

            InputProvider.Instance.Input.UI.Back.performed += TogglePause;
        }

        void SpawnPlayer()
        {
            var runway = TerrainFeatureManager.Instance.Runways[0];
            player.position = runway.ApproachThreshold + Vector3.up;
        }

        void AfterLoad()
        {
            Debug.Log("After Load");
            player.isKinematic = false;
        }

        private bool CanPause()
        {
            return UIManager.Instance.State == UIManager.UIState.Game ||
                   UIManager.Instance.State == UIManager.UIState.Pause;
        }

        void OnDestroy()
        {
            Time.timeScale = 1;
        }
        
        void TogglePause(InputAction.CallbackContext context)
        {
            if (!Loader.Instance.IsLoaded)
                return;

            if (!CanPause())
            {
                UIManager.Instance.Back();
                return;
            }
            
            UIManager.Instance.Back();
            
            _isPaused = !_isPaused;

            if (_isPaused)
            {
                InputProvider.Instance.DisableAircraftControls();
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
                Cursor.visible = true;
            }
            else
            {
                InputProvider.Instance.EnableAircraftControls();
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
                Cursor.visible = false;
            }
        }

        public void Unpause()
        {
            if (!_isPaused)
                return;
            
            _isPaused = false;
            UIManager.Instance.Back();
            Time.timeScale = 1;
            InputProvider.Instance.EnableAircraftControls();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("Main menu");
        }
        
        public void Quit()
        {
            Application.Quit();
        }
    }
}
