// observer
// interface
// gets told about every cahnge of the vault

public interface IVaultObserver{
    void OnVaultChanged(Vault vault, string change);
}