using Game.Options;
using Terrain;
using Terrain.Data;
using Terrain.Interests;
using UI.Map;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using User;
using Utility;

namespace Game.World
{
    /// <summary>
    /// Class responsible for management of general game behavior such as spawning the player of pause/unpause logic
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private GameConfig gameConfig;
        /// <summary>
        /// Reference to player rigidbody
        /// </summary>
        private Rigidbody _aircraftRigidbody;
        public Rigidbody AircraftRigidbody => _aircraftRigidbody;

        private Aircraft.Aircraft _aircraft;
        public Aircraft.Aircraft Aircraft => _aircraft;
        
        /// <summary>
        /// Is the game paused
        /// </summary>
        private bool _isPaused = false;
        
        /// <summary>
        /// Is the game paused
        /// </summary>
        public bool IsPaused => _isPaused;
        
        public PointOfInterest landedAt = null;

        private void Awake()
        {
            var aircraft = Instantiate(gameConfig.defaultAircraft);
            _aircraftRigidbody = aircraft.GetComponent<Rigidbody>();
            _aircraft = aircraft.GetComponent<Aircraft.Aircraft>();
            Terrain.Terrain.Instance.player = aircraft.transform;
        }
        
        /// <summary>
        /// Loads the terrain and spawns the player
        /// </summary>
        private void Start()
        {
            _aircraftRigidbody.isKinematic = true;
            Loader.Instance.afterPregeneration.AddListener(MovePlayerToDefaultLocation);
            Loader.Instance.afterLoading.AddListener(AfterLoad);
            Loader.Instance.Load();
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            InputProvider.Instance.Input.Aircraft.Reset.performed += _ => FlipAircraft();
        }

        /// <summary>
        /// Flips aircraft to correct orientation
        /// </summary>
        private void FlipAircraft()
        {
            var heading = Vector3.ProjectOnPlane(_aircraft.transform.forward, Vector3.up).normalized;
            var correctOrientation = Quaternion.LookRotation(heading, Vector3.up);
            var position = _aircraft.transform.position + Vector3.up;
            _aircraftRigidbody.Move(position, correctOrientation);
        }
        
        /// <summary>
        /// Places player on the desired spot
        /// </summary>
        private void MovePlayerToDefaultLocation()
        {
            var spawnPoint = TerrainFeatureManager.Instance.Interests[0].TerrainAffectors[0].From;
            _aircraftRigidbody.position = spawnPoint + Vector3.up;
            MapMarkerUtility.Instance.PlacePlayerMarker(_aircraftRigidbody.transform);
        }

        /// <summary>
        /// Enables player movement
        /// </summary>
        private void AfterLoad()
        {
            _aircraftRigidbody.isKinematic = false;
        }
        
        /// <summary>
        /// Sets timescale back to one to avoid issues while transitioning to other scenes
        /// </summary>
        private void OnDestroy()
        {
            Time.timeScale = 1;
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
