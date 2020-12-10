//TODO: ENERGY MAX VALUE CHANGE
using UnityEngine;
using UnityEngine.UI;
using Eccentric;
using TmUnity.Node;
namespace TmUnity
{
    class UIController : MonoBehaviour
    {
        [SerializeField] Slider bossHPSlider = null;
        [SerializeField] Text bossHPText = null;
        [SerializeField] Text atkText = null;
        [SerializeField] Text chargeAtkText = null;
        [SerializeField] Text defText = null;
        [SerializeField] Text timeText = null;
        [SerializeField] Slider timeSlider = null;
        [SerializeField] Text chargeCountText = null;
        [SerializeField] Slider chargeSlider = null;
        [SerializeField] Slider playerHPSlider = null;
        [SerializeField] Text playerHPText = null;
        [SerializeField] Text comboText = null;
        int maxHP = 0;
        int maxChargeNum = 0;
        void OnEnable()
        {
            DomainEvents.Register<OnPlayerStatsInit>(HandlePlayerStatsInit);
            DomainEvents.Register<OnAtkChanged>(HandleAtkChanged);
            DomainEvents.Register<OnChargeAtkChanged>(HandleChargeAtkChanged);
            DomainEvents.Register<OnDefChanged>(HandleDefChanged);
            DomainEvents.Register<OnEnergyChanged>(HandleEnergyChanged);
            DomainEvents.Register<OnChargeCountChange>(HandleChargeCountChanged);
            DomainEvents.Register<OnPlayerHPChanged>(HandlePlayerHPChanged);
            DomainEvents.Register<OnComboChange>(HandleComboChange);
        }

        void OnDisable()
        {
            DomainEvents.UnRegister<OnPlayerStatsInit>(HandlePlayerStatsInit);
            DomainEvents.UnRegister<OnAtkChanged>(HandleAtkChanged);
            DomainEvents.UnRegister<OnChargeAtkChanged>(HandleChargeAtkChanged);
            DomainEvents.UnRegister<OnDefChanged>(HandleDefChanged);
            DomainEvents.UnRegister<OnEnergyChanged>(HandleEnergyChanged);
            DomainEvents.UnRegister<OnChargeCountChange>(HandleChargeCountChanged);
            DomainEvents.UnRegister<OnPlayerHPChanged>(HandlePlayerHPChanged);
            DomainEvents.UnRegister<OnComboChange>(HandleComboChange);
        }

        void HandlePlayerStatsInit(OnPlayerStatsInit e)
        {
            maxHP = e.MaxHP;
            maxChargeNum = e.MaxChargeNum;
            chargeSlider.maxValue = maxChargeNum;
            playerHPSlider.maxValue = maxHP;
        }

        void HandleAtkChanged(OnAtkChanged e) => atkText.text = $"ATK:{e.NewAtk}";

        void HandleChargeAtkChanged(OnChargeAtkChanged e) => chargeAtkText.text = $"CHA:{e.NewAtk}";

        void HandleDefChanged(OnDefChanged e) => defText.text = $"DEF:{e.NewDef}";

        void HandleEnergyChanged(OnEnergyChanged e)
        {
            timeText.text = $"{e.NewEnergy} sec";
            timeSlider.value = e.NewEnergy;
        }

        void HandleChargeCountChanged(OnChargeCountChange e)
        {
            chargeSlider.value = e.Current;
            chargeCountText.text = $"{e.Current}/{maxChargeNum}";
        }

        void HandlePlayerHPChanged(OnPlayerHPChanged e)
        {
            playerHPSlider.value = e.NewHP;
            playerHPText.text = $"{e.NewHP}/{maxHP}";
        }

        void HandleComboChange(OnComboChange e)
        {
            if (e.Combos == 0)
                return;
            //     comboText.text = "";
            // else
            comboText.text = $"{e.Combos} COMBOS";
        }

    }

}
