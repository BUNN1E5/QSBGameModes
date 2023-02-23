namespace QSBGameModes.GameManagement.GameTypes;

public class GameBase{

    public void OnStateChange(GameState state){
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

    public virtual void OnCatch(){}
    public virtual void OnStarting(){}
    public virtual void OnInProgress(){}
    public virtual void OnEnding(){}
    public virtual void OnStopped(){}
    public virtual void OnWaiting(){}

    public virtual void OnJoin(){}
}