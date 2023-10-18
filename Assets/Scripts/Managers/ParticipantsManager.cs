using Photon.Pun;
using Photon.Realtime;
using System;
using Weapon.Model;

namespace Managers
{
    public class ParticipantsManager : MonoBehaviourPunCallbacks
    {
        public CharacterManager CharacterManager { get; private set; }
        private PlayerInfo _defaultPlayerInfo;

        private void Awake()
        {
            _defaultPlayerInfo = new PlayerInfo("", "PlayerImage/human", new WeaponData());
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            CharacterManager.ChangePlayerInfo(newPlayer.ActorNumber, _defaultPlayerInfo);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            CharacterManager.RemovePlayerInfo(otherPlayer.ActorNumber);
        }
    }
}