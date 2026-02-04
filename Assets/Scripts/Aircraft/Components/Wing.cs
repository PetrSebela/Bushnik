using UnityEngine;

namespace Aircraft.Components
{
    /// <summary>
    /// Class modeling the aerodynamics of simple wing
    /// Resulting forces are dependent on the orientation of object
    /// This object assumes that the wind chord is parallel to the forward transform direction
    /// </summary>
    public class Wing : MonoBehaviour
    {
        /// <summary>
        /// Wing airfoil
        /// </summary>
        [SerializeField] private Airfoil.Airfoil airfoil;
        
        /// <summary>
        /// Wing chord length
        /// </summary>
        [SerializeField] private float chordLength;
        
        /// <summary>
        /// Wing span
        /// </summary>
        [SerializeField] private float wingSpan;
        
        /// <summary>
        /// Reference to aircraft rigidbody
        /// </summary>
        private Rigidbody _aircraftBody;
        
        /// <summary>
        /// Assignment of aircraft rigidbody
        /// </summary>
        void Awake()
        {
            _aircraftBody = Utility.Generic.LocateObjectTowardsRoot<Rigidbody>(transform.parent);
        }
        
        /// <summary>
        /// Wing area
        /// </summary>
        public float WingArea => wingSpan * chordLength;
    
        /// <summary>
        /// Wing velocity in world space
        /// </summary>
        private Vector3 Velocity => _aircraftBody.GetPointVelocity(transform.position);
        
        /// <summary>
        /// Wind velocity across the chord of the wing
        /// </summary>
        private Vector3 RelativeWindVelocity => Vector3.ProjectOnPlane(Velocity, transform.right);
    
        /// <summary>
        /// Direction of the lift based on wind direction
        /// </summary>
        private Vector3 LiftVector => Vector3.Cross(RelativeWindVelocity, transform.right).normalized;
    
        /// <summary>
        /// Direction of the drag
        /// </summary>
        private Vector3 DragVector => -RelativeWindVelocity.normalized;
    
        /// <summary>
        /// Angle between the wing chord and wind 
        /// </summary>
        private float AngleOfAttack => Vector3.SignedAngle(transform.forward, RelativeWindVelocity, transform.right);
    
        /// <summary>
        /// Velocity of the air flowing around the wing
        /// </summary>
        private float AirflowVelocity => Vector3.Dot(transform.forward,RelativeWindVelocity);
        
        /// <summary>
        /// Computes lift force of the wing (using plain velocity instead of dynamic pressure so that the forces are more smaller)
        /// </summary>
        /// <returns>Lift force in world coordinates</returns>
        private Vector3 GetLift()
        {
            var coefficients = airfoil.GetSample(AngleOfAttack);
            var density = Atmosphere.GetDensityAtPoint(transform.position);
            var lift = coefficients.Lift * density * AirflowVelocity  * WingArea;
            return LiftVector * lift;
        }
        
        /// <summary>
        /// Computes drag force of the wing (using plain velocity instead of dynamic pressure so that the forces are more smaller)
        /// </summary>
        /// <returns>Drag force in world coordinates</returns>
        private Vector3 GetDrag()
        {
            var coefficients = airfoil.GetSample(AngleOfAttack);
            var density = Atmosphere.GetDensityAtPoint(transform.position);
            var drag = coefficients.Drag * density * AirflowVelocity * WingArea;
            return DragVector * drag;
        }
        
        /// <summary>
        /// Applies wing lift and drag forces to the rigidbody
        /// </summary>
        public void FixedUpdate()
        {
            var lift = GetLift(); 
            _aircraftBody.AddForceAtPosition(lift, transform.position);
        
            var drag = GetDrag(); 
            _aircraftBody.AddForceAtPosition(drag, transform.position);
        }
    
        /// <summary>
        /// Draws basic debug gizmos (wing shape, lift and drag vectors)
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            
            Gizmos.color = GetComponent<Flap>() != null ? new Color(1, 0.25f, 0, 0.5f) : new Color(0, 0.25f, 1f, 0.5f);
            Gizmos.DrawCube(new Vector3(0, 0, 0), new Vector3(wingSpan, 0, chordLength));
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(new Vector3(0, 0, 0), new Vector3(wingSpan, 0, chordLength));
            
            Gizmos.matrix = Matrix4x4.identity;
            
            if (!Application.isPlaying)
                return;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + GetLift());
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + GetDrag());    
        }
    }
}
