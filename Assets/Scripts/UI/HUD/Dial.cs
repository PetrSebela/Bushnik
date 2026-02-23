using TMPro;
using UnityEngine;

namespace UI.HUD
{
    public class Dial : MonoBehaviour
    {
        [SerializeField] private TMP_Text readoutText;
        [SerializeField] private Transform needle;
        [SerializeField] private GameObject indent;
        [SerializeField] private GameObject majorIndent;
        [SerializeField] private GameObject minorIndent;
        [SerializeField] private float maxAngle;
        [SerializeField] private float offsetAngle;
        [SerializeField] private float maxValue;
        [SerializeField] private bool clockWise;
        [SerializeField] private float radius;
        
        [SerializeField] private float indentRadius;
        
        [SerializeField] private int indentCount;
        [SerializeField] private int minorIndentMultiplier = 5;
        
        private float _value;
        
        void Start()
        {
            for (int i = 0; i < indentCount; i++)
            {
                float angle = offsetAngle + maxAngle / (indentCount - 1) * i * (clockWise ? -1 : 1);
                
                var numberInstance = Instantiate(indent, transform);
                numberInstance.transform.localPosition = Quaternion.AngleAxis(-angle, Vector3.forward) * Vector3.right * radius;
                numberInstance.GetComponent<TMP_Text>().text = (maxValue * ((float)i / (indentCount-1))).ToString();
                
                var majorInstance = Instantiate(majorIndent, transform);
                majorInstance.transform.localPosition = Quaternion.AngleAxis(-angle, Vector3.forward) * Vector3.right * indentRadius;
                majorInstance.transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
            }

            for (int i = 0; i < (indentCount - 1) * minorIndentMultiplier; i++)
            {
                float angle = offsetAngle + maxAngle / ((indentCount - 1) * minorIndentMultiplier ) * i * (clockWise ? -1 : 1);
                var majorInstance = Instantiate(minorIndent, transform);
                majorInstance.transform.localPosition = Quaternion.AngleAxis(-angle, Vector3.forward) * Vector3.right * indentRadius;
                majorInstance.transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
            }
        }


        public void SetValue(float value)
        {
            _value = value;
            var progress = value / maxValue;
            var angle = offsetAngle + maxAngle * progress * (clockWise ? 1 : -1);
            needle.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            readoutText.text = ((int)value).ToString();
        }
    }
}
