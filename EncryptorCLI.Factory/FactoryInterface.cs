namespace EncryptorCLI.FactoryInterface;

public interface IEncryptorPlugin
{
	string Encrypt(string input);
    string Decrypt(string input);
    void Initialize(byte[] Key);
    string Terminate(string pluginName);
}

public abstract class EncryptorPluginFactory
{
    public abstract string PluginName { get; }
    public abstract IEncryptorPlugin CreatePlugin(byte[] Key);
}