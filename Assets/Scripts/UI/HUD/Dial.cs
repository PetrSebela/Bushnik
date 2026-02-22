using TMPro;
using UnityEngine;

namespace UI.HUD
{
    public class Dial : MonoBehaviour
    {
        [SerializeField] private TMP_Text readoutText;
        [SerializeField] private Transform needle;
        [SerializeField] private GameObject indent;
        [SerializeField] private float maxAngle;
        [SerializeField] private float offsetAngle;
        [SerializeField] private float maxValue;
        [SerializeField] private bool clockWise;
        [SerializeField] private float radius;
        [SerializeField] private int indentCount;
        
        private float _value;
        
        void Start()
        {
            for (int i = 0; i < indentCount; i++)
            {
                float angle = offsetAngle + maxAngle / (indentCount - 1) * i * (clockWise ? -1 : 1);
                var indentInstance = Instantiate(indent, transform);
                indentInstance.transform.localPosition = Quaternion.AngleAxis(-angle, Vector3.forward) * Vector3.right * radius;
                indentInstance.GetComponent<TMP_Text>().text = (maxValue * ((float)i / (indentCount-1))).ToString();
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
