using UnityEngine;
using UnityEngine.UI;
using Eccentric;
using TmUnity.Node;
namespace TmUnity
{
    class UIController : MonoBehaviour
    {
        [SerializeField] Slider enemyHPSlider = null;
        [SerializeField] Text enemyHPText = null;
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
            DomainEvents.Register<OnMaxTimeSet>(HandleMaxTimeSet);
            DomainEvents.Register<OnRemainTimeChanged>(HandleRemainTimeChanged);
            DomainEvents.Register<OnEnemyHPChanged>(HandleEnemyHPChanged);
            DomainEvents.Register<OnEnemyDead>(HandleEnemyDead);
            DomainEvents.Register<OnPlayerDead>(HandlePlayerDead);
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
            DomainEvents.UnRegister<OnMaxTimeSet>(HandleMaxTimeSet);
            DomainEvents.UnRegister<OnRemainTimeChanged>(HandleRemainTimeChanged);
            DomainEvents.UnRegister<OnEnemyHPChanged>(HandleEnemyHPChanged);
            DomainEvents.UnRegister<OnEnemyDead>(HandleEnemyDead);
            DomainEvents.UnRegister<OnPlayerDead>(HandlePlayerDead);
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
            timeText.text = $"{e.NewEnergy.ToString("0.0")} sec";
            timeSlider.maxValue = e.NewEnergy;
        }

        void HandleChargeCountChanged(OnChargeCountChange e)
        {
            chargeSlider.value = e.Current;
            chargeCountText.text = $"{e.Current}/{maxChargeNum}";
        }

        void HandlePlayerHPChanged(OnPlayerHPChanged e)
        {
            if (e.NewHP > maxHP)
            {
                maxHP = e.NewHP;
                playerHPSlider.maxValue = maxHP;
            }
            playerHPSlider.value = e.NewHP;
            playerHPText.text = $"{e.NewHP}/{maxHP}";
        }

        void HandleComboChange(OnComboChange e)
        {
            if (e.Combos == 0 && !e.IsZeroDisplay)
                return;
            comboText.text = $"{e.Combos} COMBOS";
        }

        void HandleMaxTimeSet(OnMaxTimeSet e)
        {
            timeSlider.maxValue = e.MaxTime;
            timeSlider.value = e.MaxTime;
            timeText.text = $"{e.MaxTime.ToString("0.0")}s";
        }

        void HandleRemainTimeChanged(OnRemainTimeChanged e)
        {
            timeText.text = $"{e.Remain.ToString("0.0")}s";
            timeSlider.value = e.Remain;
        }

        void HandleEnemyHPChanged(OnEnemyHPChanged e)
        {
            enemyHPSlider.maxValue = e.MaxHP;
            enemyHPSlider.value = e.NewHP;
            enemyHPText.text = $"{e.NewHP}/{e.MaxHP}";
        }

        void HandleEnemyDead(OnEnemyDead e) => enemyHPText.text = "DIE DIE DIE";

        void HandlePlayerDead(OnPlayerDead e) => playerHPText.text = "DIE DIE DIE";

    }

}
