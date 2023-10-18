using Common;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Weapon;
using Weapon.Model;
using Text = TMPro.TextMeshProUGUI;

public class NextRoundUI : MonoBehaviour
{
    [SerializeField] private Slider waitingSlider;
    [SerializeField] private Text timeText;
    [SerializeField] private RectTransform playerInfoContainer;
    private EnhancementManager _enhanceManager;

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Unsubscribe()
    {
        if (_enhanceManager == null)
        {
            return;
        }

        _enhanceManager.OnTimeElapsed -= UpdateTimeUI;
        _enhanceManager.OnReadyToFight += Disappear;
    }

    public void Init(EnhancementManager manager)
    {
        _enhanceManager = manager;
        manager.OnTimeElapsed += UpdateTimeUI;
        manager.OnReadyToFight += Disappear;
        UpdatePlayerInfo();
    }

    private void Disappear()
    {
        Destroy(gameObject);
    }

    private void UpdatePlayerInfo()
    {
        //todo 
        foreach (var enhanceDataPair in _enhanceManager.CachedEnhancements)
        {
            CreateEnhancementInfo(enhanceDataPair.Key, enhanceDataPair.Value);
        }
    }

    private void CreateEnhancementInfo(int player, EnhancementData data)
    {
        RoundPlayerInfoUI roundPlayerInfoObj = Resources.Load<RoundPlayerInfoUI>("UI/RoundPlayerInfoUI");
        RoundPlayerInfoUI ui = Instantiate(roundPlayerInfoObj, playerInfoContainer, false);
        //todo get player image, player name from manager
        RoundPlayerInfoState state = new RoundPlayerInfoState(
            name: "name",
            playerIndex: player,
            iconUrl: "Sprite/Blood",
            data
        );
        ui.Init(state);
    }

    private void UpdateTimeUI(float time)
    {
        timeText.text = $"{time:N0}";
        waitingSlider.value = (Constants.TimeToNextRound - time) / Constants.TimeToNextRound;
    }
}