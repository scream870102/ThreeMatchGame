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
        [SerializeField] RawImage gameEndImage = null;
        [SerializeField] Text resultText = null;
        [SerializeField] Text recordText = null;
        [SerializeField] Text pressText = null;
        int maxHP = 0;
        int maxChargeNum = 0;



        void HandleGameStart(OnGameStart e)
        {
            gameEndImage.gameObject.SetActive(true);
            resultText.text = "Game Start";
            pressText.text = "Touch Screen to Start";
            recordText.text = "Do your best to get best point";
        }

        void HandleGameEnd(OnGameEnd e)
        {
            gameEndImage.gameObject.SetActive(true);
            if (e.IsWin)
                resultText.text = "WIN WIN WIND";
            else
                resultText.text = "GAME OVER";
            pressText.text = "Touch Screen to Restart";
            recordText.text = $"Max Damage : {e.Result.MaxDamage} \n Elapsed Time : {e.Result.ElapsedTime.ToString("0.00")}s";
        }

        void HandlePlayerStatsInit(OnPlayerStatsInit e)
        {
            maxHP = e.MaxHP;
            maxChargeNum = e.MaxChargeNum;
            chargeSlider.maxValue = maxChargeNum;
            playerHPSlider.maxValue = maxHP;
            gameEndImage.gameObject.SetActive(false);
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

        void HandleComboChange(OnComboChange e) => comboText.text = $"{e.Combos} COMBOS";

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
            DomainEvents.Register<OnGameStart>(HandleGameStart);
            DomainEvents.Register<OnGameEnd>(HandleGameEnd);
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
            DomainEvents.UnRegister<OnGameStart>(HandleGameStart);
            DomainEvents.UnRegister<OnGameEnd>(HandleGameEnd);
        }

    }

}
