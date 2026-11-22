// observer 
// subject
// keeps the observers and notifies all of them when the vault changes
// it lives as long as the program

public class VaultEvents{

    private readonly List<IVaultObserver> _observers = new();

    public void Subscribe(IVaultObserver observer){
        if(!IsSubscribed(observer)){

            _observers.Add(observer);
        }
    }

    public void Unsubscribe(IVaultObserver observer){
        _observers.Remove(observer);
    }

    public bool IsSubscribed(IVaultObserver observer){
        return _observers.Contain(observer);
    }

    public void Publish(Vault vault, string change){

        // a copy so an pbserver can unsubscribe wile being notified
        foreach(var observer in _observers.ToList()){
            observer.OnVaultChanges(vault, change);
        }
    }
}