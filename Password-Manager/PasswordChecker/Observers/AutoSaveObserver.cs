// observer
// concrete observer
// writes the encrypted vault to disk after 

    public class AutoSaveObserver : IVaultObserver{

        private readonly VaultFile _file;

        public AutoSaveObserver(VaultFile file){

            _file = file;
        }

        public void OnVaultChanged(Vault vault, string change){

            vault.SaveTo(_file);
        }
    }