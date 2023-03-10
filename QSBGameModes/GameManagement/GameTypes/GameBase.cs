using QSB.Messaging;
using QSB.Utility.Messages;
using QSBGameModes.GameManagement.PlayerManagement;

namespace QSBGameModes.GameManagement;

public class GameBase{

    public float stateTime = 0f;
    protected GameState currentState = GameState.Stopped;

    public virtual PlayerManagement.PlayerState StateOnJoinLate() => PlayerManagement.PlayerState.None;
    public virtual PlayerManagement.PlayerState StateOnJoinEarly() => PlayerManagement.PlayerState.None;
    
    public NotificationData catcheeNotification = new(NotificationTarget.All, "You are a catchee");
    public NotificationData catcherNotification = new(NotificationTarget.All, "You are a catcher");
    public NotificationData spectatorNotification = new(NotificationTarget.All, "You are a spectator");

    public void OnStateChange(GameState state){
        Utils.WriteLine($"Game now on state {state}!");
        switch (state){
            case GameState.Starting:
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
        currentState = state;
    }
    
    public virtual void Init(){ }

    public virtual void OnCatch(GameModeInfo seekerPlayer){}
    public virtual void OnStarting(){}
    public virtual void OnInProgress(){}

    public virtual void OnEnding(){
        new DebugTriggerSupernovaMessage().Send();
    }

    public virtual void OnStopped(){
        PlayerManager.SetAllPlayerStates(PlayerManagement.PlayerState.None);
        if (currentState == GameState.InProgress){
            new DebugTriggerSupernovaMessage().Send();
        }
    }
    public virtual void OnWaiting(){}

    public virtual void OnJoin(){}
}