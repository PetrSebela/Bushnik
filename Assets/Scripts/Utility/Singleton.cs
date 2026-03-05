using Unity.VisualScripting;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Class simplifying creation of singleton classes
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : Object
    {
        /// <summary>
        /// Private singleton reference and lookup cache
        /// </summary>
        private static T _instance;
        
        /// <summary>
        /// Singleton getter
        /// </summary>
        public static T Instance
        {
            get
            {
                if(_instance == null)
                    _instance = FindAnyObjectByType<T>();

                return _instance;
            }
        }
    }
}
