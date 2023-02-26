using QSB.Player;
using UnityEngine;

namespace QSBGameModes.GameManagement;

public class GameBase{

    public float gameStartTime = 0f;
    
    public PlayerManagement.PlayerState stateOnJoinLate{ get; }
    public PlayerManagement.PlayerState stateOnJoinEarly{ get; }
    
    public void OnStateChange(GameState state){
        switch (state){
            case GameState.Starting:
                gameStartTime = Time.time;
                OnStarting();
                break;
            case GameState.Waiting:
                OnWaiting();
                break;
            case GameState.InProgress:
                OnInProgress();
                break;
            case GameState.Ending:
                OnEnding();
                break;
            case GameState.Stopped:
                OnStopped();
                break;
            
        }
    }

    public virtual void Init(){ }

    public virtual void OnCatch(GameModeInfo seekerPlayer){}
    public virtual void OnStarting(){}
    public virtual void OnInProgress(){}
    public virtual void OnEnding(){}
    public virtual void OnStopped(){}
    public virtual void OnWaiting(){}

    public virtual void OnJoin(){}
}