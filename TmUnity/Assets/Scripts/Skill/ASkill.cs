using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Eccentric.Utils;
using Eccentric;
using TmUnity.Node;
using TmUnity;

namespace TmUnity.Skill
{
    abstract class ASkill : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        const float expositionCountDown = .5f;
        [SerializeField] string skillName = "";
        [SerializeField] protected int manaCost = 0;
        [SerializeField] string exposition = "";
        [SerializeField] Color disableColor = new Color(.5f, .5f, .5f, 1f);
        RawImage image = null;
        ScaledTimer expositionPanelTimer = null;
        GameObject expositonPanel = null;
        bool isUseable = false;
        protected GameController controller { get; private set; } = null;
        protected Text expositionText { get; private set; } = null;
        void Awake()
        {
            expositionText = GetComponentInChildren<Text>();
            expositionPanelTimer = new ScaledTimer(expositionCountDown, false);
            expositonPanel = transform.GetChild(0).gameObject;
            image = GetComponent<RawImage>();
        }

        void Start()
        {
            expositonPanel.SetActive(false);
            SetExpositionText(expositionText, skillName, manaCost, exposition);
        }

        virtual public void Init(GameController controller) => this.controller = controller;

        public void OnPointerClick(PointerEventData e)
        {
            if (isUseable)
            {
                controller?.UseSkill(skillName, manaCost);
                UseSkill();
            }
        }

        public void OnPointerEnter(PointerEventData e) => expositonPanel.SetActive(true);

        public void OnPointerExit(PointerEventData e) => expositonPanel.SetActive(false);

        void HandleManaChanged(OnManaChanged e)
        {
            isUseable = e.NewMana >= manaCost;
            image.color = isUseable ? Color.white : disableColor;
        }

        virtual protected void SetExpositionText(Text expositionText, string skillName, int manaCost, string exposition) => expositionText.text = $"{skillName} ({manaCost})\n {exposition}";

        abstract protected void UseSkill();

        void OnEnable() => DomainEvents.Register<OnManaChanged>(HandleManaChanged);

        void OnDisable() => DomainEvents.UnRegister<OnManaChanged>(HandleManaChanged);

    }
}

