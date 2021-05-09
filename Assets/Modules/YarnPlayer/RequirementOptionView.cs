using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Modules.YarnPlayer
{
    public class RequirementOptionView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        
        const float TimeToAppear = 1.0f;
        [SerializeField]
        private Color defaultColor;
        [SerializeField]
        private Color unsatisfiedColor;
        [SerializeField]
        private Image sprite;
        [SerializeField]
        private Tooltip tooltip;
        [SerializeField]
        private string pattern = "requires {0} {1} {2}. you have {3}";
        private Coroutine tooltipAwaits;
        public void Init(LineRequirement data)
        {
            tooltip.Text = string.Format(pattern,
                data.ResourceInfo.Name,
                data.Condition.op.AsText(),
                data.Condition.value,
                data.ActualValue
            );
            sprite.overrideSprite = data.ResourceInfo.Icon;
            sprite.color = data.IsSatisfied ? defaultColor : unsatisfiedColor;
            tooltip.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltipAwaits ??= StartCoroutine(WaitToAppear(TimeToAppear));
            Debug.Log("OnEnter");
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltipAwaits == null) return;
            StopCoroutine(tooltipAwaits);
            tooltipAwaits = null;
            tooltip.SetActive(false);
        }

        IEnumerator WaitToAppear(float time)
        {
           yield return new WaitForSecondsRealtime(time);
           tooltip.SetActive(true);
        }
    }
}
