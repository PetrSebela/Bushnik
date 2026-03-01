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
            Loader.Instance.AfterPregeneration.AddListener(SpawnPlayer);
            Loader.Instance.AfterLoading.AddListener(AfterLoad);
            Loader.Instance.Load();
            
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
        /// Toggles game pause state
        /// </summary>
        public void TogglePause()
        {
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
        /// Forcefully pauses the game
        /// </summary>
        public void Pause()
        {
            if (_isPaused)
                return;
            
            _isPaused = true;
            InputProvider.Instance.DisableAircraftControls();
            
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        /// <summary>
        /// Forcefully unpauses the game 
        /// </summary>
        public void Unpause()
        {
            if (!_isPaused)
                return;
            
            _isPaused = false;
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
