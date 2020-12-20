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
    abstract class ASkill : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
    {
        const float expositionCountDown = .5f;
        const int panelCloseDelay = 100;
        [SerializeField] string skillName = "";
        [SerializeField] protected int manaCost = 0;
        [SerializeField] string exposition = "";
        [SerializeField] Color disableColor = new Color(.5f, .5f, .5f, 1f);
        RawImage image = null;
        bool isPointerDown = false;
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

        void Update()
        {
            if (isPointerDown && expositionPanelTimer.IsFinished)
                expositonPanel.SetActive(true);
        }

        virtual public void Init(GameController controller) => this.controller = controller;

        async public void OnPointerUp(PointerEventData e)
        {
            isPointerDown = false;
            await Task.Delay(panelCloseDelay);
            expositonPanel.SetActive(false);
        }

        public void OnPointerDown(PointerEventData e)
        {
            isPointerDown = true;
            expositionPanelTimer.Reset();
        }

        public void OnPointerClick(PointerEventData e)
        {
            if (!expositonPanel.activeSelf && isUseable)
            {
                controller?.UseSkill(skillName, manaCost);
                UseSkill();
            }
        }

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

