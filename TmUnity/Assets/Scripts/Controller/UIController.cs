using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Eccentric;
using TmUnity.Node;
namespace TmUnity
{
    class UIController : MonoBehaviour
    {
        [SerializeField] int defAnimDelayBetweenHPSlider = 2000;
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
        [SerializeField] Text manaText = null;
        [SerializeField] Text recoverText = null;
        [SerializeField] Text timeText = null;
        [SerializeField] Slider timeSlider = null;
        [SerializeField] Slider manaSlider = null;
        [SerializeField] Text comboText = null;
        //TODO:[SerializeField] Text comboFactorText = null;
        [SerializeField] RawImage gameEndImage = null;
        [SerializeField] Text resultText = null;
        [SerializeField] Text recordText = null;
        [SerializeField] Text pressText = null;
        [SerializeField] Text totalDamageText = null;
        [SerializeField] Text defensePlusText = null;
        [SerializeField] GameObject skillPanel = null;
        Animation comboAnim = null;
        Animation totalDamageAnim = null;
        Animation cardAnim = null;
        Animation defenseAnim = null;
        Animation recoverAnim = null;
        int maxMana = 0;
        int maxHP = 0;
        int currentHP = 0;
        int nextAttack = 0;
        int currentDef = 0;
        int currentHPRecover = 0;

        void Start()
        {
            comboAnim = comboText.GetComponent<Animation>();
            totalDamageAnim = totalDamageText.GetComponent<Animation>();
            cardAnim = cardText.transform.parent.GetComponent<Animation>();
            defenseAnim = defensePlusText.GetComponent<Animation>();
            recoverAnim = recoverText.GetComponent<Animation>();
        }

        async void DefAnimFinAE()
        {
            DomainEvents.Raise<OnDefAnimFin>(new OnDefAnimFin());
            await Task.Delay(defAnimDelayBetweenHPSlider);
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

        void UpdateLeftText()
        {
            defensePlusText.text = currentDef.ToString();
            recoverText.text = $"+{currentHPRecover}";
        }

        // Start State
        void HandleGameStart(OnGameStart e)
        {
            gameEndImage.gameObject.SetActive(true);
            resultText.text = "Tasu Bomb";
            pressText.text = "Touch Screen to Start";
            recordText.text = "Do your best to get best point";
        }

        // Start State
        void HandlePlayerStatsInit(OnPlayerStatsInit e)
        {
            maxHP = e.MaxHP;
            maxMana = e.BasicMana;
            manaSlider.maxValue = maxMana;
            manaSlider.value = maxMana;
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
            if (int.Parse(totalDamageText.text) != e.NewAtk)
                totalDamageAnim.Play(PlayMode.StopAll);
            totalDamageText.text = $"{e.NewAtk}";
        }

        void HandleManaChanged(OnManaChanged e)
        {
            if (e.NewMana > maxMana)
            {
                maxMana = e.NewMana;
                manaSlider.maxValue = maxMana;
            }
            manaSlider.value = e.NewMana;
            manaText.text = $"{e.NewMana}/{maxMana}";
        }

        void HandleDefChanged(OnDefChanged e)
        {
            if (currentDef != e.NewDef)
                defenseAnim.Play();
            currentDef = e.NewDef;
            UpdateLeftText();
            UpdateHPSlider();
        }

        void HandleEnergyChanged(OnEnergyChanged e)
        {
            timeText.text = $"{e.NewEnergy.ToString("0.0")} sec";
            timeSlider.maxValue = e.NewEnergy;
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
            var oldHpRecover = currentHPRecover;
            currentHPRecover += e.Info.HPRecover;
            if (oldHpRecover != currentHPRecover)
                recoverAnim.Play();
            UpdateLeftText();
        }

        //TODO:
        // void HandleComboFactorChange(OnComboFactorChange e)
        // {
        //     comboFactorText.text = $"{e.Factor.ToString("0.00")}%";
        // }

        #endregion


        #region パズル移動前

        //End Enemy State
        void HandleEnemyAtkAnimFin(OnEnemyGetNewAttack e)
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
                    skillPanel.SetActive(true);
                    break;
                case GameState.ACTION:
                    skillPanel.SetActive(false);
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
            DomainEvents.Register<OnManaChanged>(HandleManaChanged);
            DomainEvents.Register<OnDefChanged>(HandleDefChanged);
            DomainEvents.Register<OnEnergyChanged>(HandleEnergyChanged);
            DomainEvents.Register<OnPlayerHPChanged>(HandlePlayerHPChanged);
            DomainEvents.Register<OnComboChange>(HandleComboChange);
            DomainEvents.Register<OnMaxTimeSet>(HandleMaxTimeSet);
            DomainEvents.Register<OnRemainTimeChanged>(HandleRemainTimeChanged);
            DomainEvents.Register<OnEnemyHPChanged>(HandleEnemyHPChanged);
            DomainEvents.Register<OnGameStart>(HandleGameStart);
            DomainEvents.Register<OnGameEnd>(HandleGameEnd);
            DomainEvents.Register<OnEnemyGetNewAttack>(HandleEnemyAtkAnimFin);
            DomainEvents.Register<OnNodeEliminate>(HandleNodeEliminate);
            DomainEvents.Register<OnGameStateChange>(HandleGameStateChange);
            //TODO:DomainEvents.Register<OnComboFactorChange>(HandleComboFactorChange);
        }

        void OnDisable()
        {
            DomainEvents.UnRegister<OnPlayerStatsInit>(HandlePlayerStatsInit);
            DomainEvents.UnRegister<OnAtkChanged>(HandleAtkChanged);
            DomainEvents.UnRegister<OnManaChanged>(HandleManaChanged);
            DomainEvents.UnRegister<OnDefChanged>(HandleDefChanged);
            DomainEvents.UnRegister<OnEnergyChanged>(HandleEnergyChanged);
            DomainEvents.UnRegister<OnPlayerHPChanged>(HandlePlayerHPChanged);
            DomainEvents.UnRegister<OnComboChange>(HandleComboChange);
            DomainEvents.UnRegister<OnMaxTimeSet>(HandleMaxTimeSet);
            DomainEvents.UnRegister<OnRemainTimeChanged>(HandleRemainTimeChanged);
            DomainEvents.UnRegister<OnEnemyHPChanged>(HandleEnemyHPChanged);
            DomainEvents.UnRegister<OnGameStart>(HandleGameStart);
            DomainEvents.UnRegister<OnGameEnd>(HandleGameEnd);
            DomainEvents.UnRegister<OnEnemyGetNewAttack>(HandleEnemyAtkAnimFin);
            DomainEvents.UnRegister<OnNodeEliminate>(HandleNodeEliminate);
            DomainEvents.UnRegister<OnGameStateChange>(HandleGameStateChange);
            //TODO:DomainEvents.UnRegister<OnComboFactorChange>(HandleComboFactorChange);
        }

    }

}
