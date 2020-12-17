using UnityEngine;
using UnityEngine.UI;
using Eccentric;
using TmUnity.Node;
namespace TmUnity
{
    class UIController : MonoBehaviour
    {
        [SerializeField] Slider currenHPSlider = null;
        [SerializeField] Slider newHPAndDefSlider = null;
        [SerializeField] Slider newHPSlider = null;
        [SerializeField] Text playerHPText = null;
        [SerializeField] Slider enemyHPSlider = null;
        [SerializeField] Text enemyHPText = null;
        [SerializeField] Text atkText = null;
        [SerializeField] Text chargeAtkText = null;
        [SerializeField] Text recoverText = null;
        [SerializeField] Text timeText = null;
        [SerializeField] Slider timeSlider = null;
        [SerializeField] Text chargeCountText = null;
        [SerializeField] Slider chargeSlider = null;
        [SerializeField] Text comboText = null;
        [SerializeField] RawImage gameEndImage = null;
        [SerializeField] Text resultText = null;
        [SerializeField] Text recordText = null;
        [SerializeField] Text pressText = null;
        [SerializeField] Text totalDamageText = null;
        Animation comboAnim = null;
        Animation totalDamageAnim = null;
        int maxChargeNum = 0;
        int maxHP = 0;
        int currentHP = 0;
        int nextAttack = 0;
        int currentDef = 0;
        int currentHPRecover = 0;

        void Start()
        {
            comboAnim = comboText.GetComponent<Animation>();
            totalDamageAnim = totalDamageText.GetComponent<Animation>();
        }

        void UpdateHPSlider()
        {
            playerHPText.text = $"({currentDef}){currentHP}/{maxHP}";
            currenHPSlider.maxValue = maxHP;
            newHPAndDefSlider.maxValue = maxHP;
            newHPSlider.maxValue = maxHP;
            currenHPSlider.value = currentHP;
            newHPAndDefSlider.value = Mathf.Clamp(currentHP - nextAttack + currentDef, 0, currentHP);
            newHPSlider.value = currentHP - nextAttack;
        }

        void UpdateTotalDamageText()
        {
            totalDamageText.text = (int.Parse(atkText.text) + int.Parse(chargeAtkText.text)).ToString();
            totalDamageAnim.Play(PlayMode.StopAll);
        }

        void UpdateRecoverText() => recoverText.text = $"+{currentHPRecover}";

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
            currenHPSlider.maxValue = maxHP;
            gameEndImage.gameObject.SetActive(false);
            UpdateHPSlider();
        }

        void HandlePlayerHPChanged(OnPlayerHPChanged e)
        {
            if (e.NewHP > maxHP)
            {
                maxHP = e.NewHP;
                currenHPSlider.maxValue = maxHP;
            }
            currentHP = e.NewHP;
            currenHPSlider.value = e.NewHP;

            UpdateHPSlider();
        }

        void HandleNodeEliminate(OnNodeEliminate e)
        {
            currentHPRecover += e.Info.HPRecover;
            UpdateRecoverText();
        }

        void HandleEnemyAtkAnimFin(OnEnemyAtkAnimFin e)
        {
            nextAttack = e.Attr.Atk;
            UpdateHPSlider();
        }

        void HandleDefChanged(OnDefChanged e)
        {
            currentDef = e.NewDef;
            UpdateHPSlider();
        }

        void HandleAtkChanged(OnAtkChanged e)
        {
            atkText.text = $"+{e.NewAtk}";
            UpdateTotalDamageText();
        }

        void HandleChargeAtkChanged(OnChargeAtkChanged e)
        {
            chargeAtkText.text = $"+{e.NewAtk}";
            UpdateTotalDamageText();
        }

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

        void HandleComboChange(OnComboChange e)
        {
            if (e.Combos != 0)
                comboAnim.Play(PlayMode.StopAll);
            comboText.text = $"{e.Combos} Combo";
        }

        //NOTE: this is call before start new round can init value of current round at this
        void HandleMaxTimeSet(OnMaxTimeSet e)
        {
            timeSlider.maxValue = e.MaxTime;
            timeSlider.value = e.MaxTime;
            timeText.text = $"{e.MaxTime.ToString("0.0")}s";
            currentHPRecover = 0;
            UpdateRecoverText();
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
            DomainEvents.Register<OnEnemyAtkAnimFin>(HandleEnemyAtkAnimFin);
            DomainEvents.Register<OnNodeEliminate>(HandleNodeEliminate);
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
            DomainEvents.UnRegister<OnEnemyAtkAnimFin>(HandleEnemyAtkAnimFin);
            DomainEvents.UnRegister<OnNodeEliminate>(HandleNodeEliminate);
        }

    }

}
