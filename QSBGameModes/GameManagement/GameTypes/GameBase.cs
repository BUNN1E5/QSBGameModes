using QSB.Player;

namespace QSBGameModes.GameManagement.GameTypes;

public class GameBase{
    public GameState State;
    public void OnStateChange(GameState state){
        State = state;
        switch (state){
            case GameState.Starting:
                OnStarting();
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
            case GameState.Waiting:
                OnWaiting();
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