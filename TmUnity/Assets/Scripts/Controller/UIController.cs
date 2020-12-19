using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Eccentric;
using TmUnity.Node;
namespace TmUnity
{
    class UIController : MonoBehaviour
    {
        [SerializeField] Animation gamePanelAnim = null;
        [SerializeField] GameObject leftInfoObject = null;
        [SerializeField] GameObject rightInfoObject = null;
        [SerializeField] Text enemyInfoText = null;
        [SerializeField] Text enemyInfoShadowText = null;
        [SerializeField] Text cardText = null;
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
        [SerializeField] Text chargeNumPlusText = null;
        [SerializeField] Text defensePlusText = null;
        Animation comboAnim = null;
        Animation totalDamageAnim = null;
        Animation cardAnim = null;
        int maxChargeNum = 0;
        int maxHP = 0;
        int currentHP = 0;
        int nextAttack = 0;
        int currentDef = 0;
        int currentHPRecover = 0;
        int chargeNumPlus = 0;

        void Start()
        {
            comboAnim = comboText.GetComponent<Animation>();
            totalDamageAnim = totalDamageText.GetComponent<Animation>();
            cardAnim = cardText.transform.parent.GetComponent<Animation>();
        }

        async void DefAnimFinAE()
        {
            DomainEvents.Raise<OnDefAnimFin>(new OnDefAnimFin());
            await Task.Delay(2000);
            UpdateHPSlider();
        }

        void EnablePlayerInfo(bool isEnable = true)
        {
            rightInfoObject.SetActive(isEnable);
            leftInfoObject.SetActive(isEnable);
        }

        void UpdateHPSlider()
        {
            playerHPText.text = $"{currentHP}/{maxHP}";
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

        void UpdateLeftText()
        {
            chargeNumPlusText.text = chargeNumPlus.ToString();
            defensePlusText.text = currentDef.ToString();
            recoverText.text = $"+{currentHPRecover}";
        }

        // Start State
        void HandleGameStart(OnGameStart e)
        {
            gameEndImage.gameObject.SetActive(true);
            resultText.text = "Game Start";
            pressText.text = "Touch Screen to Start";
            recordText.text = "Do your best to get best point";
        }

        // Start State
        void HandlePlayerStatsInit(OnPlayerStatsInit e)
        {
            maxHP = e.MaxHP;
            maxChargeNum = e.MaxChargeNum;
            chargeNumPlus = 0;
            chargeSlider.maxValue = maxChargeNum;
            currenHPSlider.maxValue = maxHP;
            gameEndImage.gameObject.SetActive(false);
            UpdateHPSlider();
            UpdateLeftText();
        }

        // End State Init
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

        #region Statsから

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

        void HandleDefChanged(OnDefChanged e)
        {
            currentDef = e.NewDef;
            UpdateLeftText();
            UpdateHPSlider();
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

        void HandleComboChange(OnComboChange e)
        {
            if (e.Combos != 0)
                comboAnim.Play(PlayMode.StopAll);
            comboText.text = $"{e.Combos} Combo";
        }

        #endregion



        #region パズル移動後

        //Init Animate State
        void HandleEnemyHPChanged(OnEnemyHPChanged e)
        {
            enemyHPSlider.maxValue = e.MaxHP;
            enemyHPSlider.value = e.NewHP;
            enemyHPText.text = $"{e.NewHP}/{e.MaxHP}";
        }



        // Init Animate State
        void HandleNodeEliminate(OnNodeEliminate e)
        {
            currentHPRecover += e.Info.HPRecover;
            chargeNumPlus += e.Info.ChargeNum;
            UpdateLeftText();
        }

        #endregion


        #region パズル移動前

        //End Enemy State
        void HandleEnemyAtkAnimFin(OnEnemyAtkAnimFin e)
        {
            nextAttack = e.Attr.Atk;
            enemyInfoText.text = $"{e.Attr.AnimTrigger} coming\n{e.Attr.Atk} damage";
            enemyInfoShadowText.text = $"{e.Attr.AnimTrigger} coming\n{e.Attr.Atk} damage";
            gamePanelAnim.Play();
        }

        //NOTE: this is call before start new round can init value of current round at this
        void HandleMaxTimeSet(OnMaxTimeSet e)
        {
            timeSlider.maxValue = e.MaxTime;
            timeSlider.value = e.MaxTime;
            timeText.text = $"{e.MaxTime.ToString("0.0")}s";
            currentHPRecover = 0;
            chargeNumPlus = 0;
            UpdateLeftText();

        }

        void HandleRemainTimeChanged(OnRemainTimeChanged e)
        {
            timeText.text = $"{e.Remain.ToString("0.0")}s";
            timeSlider.value = e.Remain;
        }

        #endregion

        void HandleGameStateChange(OnGameStateChange e)
        {
            switch (e.NewState)
            {
                case GameState.WAIT:
                    EnablePlayerInfo(false);
                    enemyInfoText.enabled = true;
                    enemyInfoShadowText.enabled = true;
                    cardText.text = "YOUR TURN";
                    cardAnim.Play(PlayMode.StopAll);
                    break;
                case GameState.ACTION:
                    enemyInfoText.enabled = false;
                    enemyInfoShadowText.enabled = false;
                    EnablePlayerInfo(true);
                    break;
                case GameState.ANIMATE:
                    break;
                case GameState.ENEMY:
                    cardText.text = "ENEMY TURN";
                    cardAnim.Play(PlayMode.StopAll);

                    break;
            }
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
            DomainEvents.Register<OnGameStateChange>(HandleGameStateChange);
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
            DomainEvents.UnRegister<OnGameStateChange>(HandleGameStateChange);
        }

    }

}
