using Terrain;
using Terrain.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using User;

namespace Game.World
{
    /// <summary>
    /// Class responsible for management of general game behavior such as spawning the player of pause/unpause logic
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Reference to player rigidbody
        /// </summary>
        [SerializeField] private Rigidbody player;
        
        /// <summary>
        /// Is the game paused
        /// </summary>
        private bool _isPaused = false;
        
        /// <summary>
        /// Is the game paused
        /// </summary>
        public bool IsPaused => _isPaused;
        
        /// <summary>
        /// Loads the terrain and spawns the player
        /// </summary>
        void Start()
        {
            player.isKinematic = true;
            Loader.Instance.AfterPregeneration += SpawnPlayer;
            Loader.Instance.AfterLoading += AfterLoad;
            Loader.Instance.Load();

            InputProvider.Instance.Input.UI.Back.performed += TogglePause;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        /// <summary>
        /// Places player on the desired spot
        /// </summary>
        void SpawnPlayer()
        {
            var runway = TerrainFeatureManager.Instance.Runways[0];
            player.position = runway.ApproachThreshold + Vector3.up;
        }

        /// <summary>
        /// Enables player movement
        /// </summary>
        void AfterLoad()
        {
            player.isKinematic = false;
        }
        
        /// <summary>
        /// Sets timescale back to one to avoid issues while transitioning to other scenes
        /// </summary>
        void OnDestroy()
        {
            Time.timeScale = 1;
        }
        
        /// <summary>
        /// Toggles game pause
        /// </summary>
        /// <param name="context"></param>
        void TogglePause(InputAction.CallbackContext context)
        {
            if (!Loader.Instance.IsLoaded)
                return;

            if (UIManager.Instance.State != UIManager.UIState.Game && UIManager.Instance.State != UIManager.UIState.Pause)
            {
                UIManager.Instance.Back();
                return;
            }
            
            UIManager.Instance.Back();
            
            _isPaused = !_isPaused;

            if (_isPaused)
            {
                InputProvider.Instance.DisableAircraftControls();
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                InputProvider.Instance.EnableAircraftControls();
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        /// <summary>
        /// Forcefully unpauses the game 
        /// </summary>
        public void Unpause()
        {
            if (!_isPaused)
                return;
            
            _isPaused = false;
            UIManager.Instance.Back();
            InputProvider.Instance.EnableAircraftControls();
            
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        /// <summary>
        /// Loads main menu scene
        /// </summary>
        public void GoToMainMenu()
        {
            SceneManager.LoadScene("Main menu");
        }
        
        /// <summary>
        /// Quits the application
        /// </summary>
        public void Quit()
        {
            Application.Quit();
        }
    }
}
