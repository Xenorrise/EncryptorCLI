using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EncryptorCLI.FactoryInterface;

namespace EncryptorCLI.PluginManagement;

[RequiresUnreferencedCode("Reflection is used by methods of this class")]
public static class PluginManager
{
    private static readonly List<EncryptorPluginFactory> _factories = [];
    private static readonly string defaultPath = Path.Combine(AppContext.BaseDirectory, "plugins");
    private static int _pluginCount = 0;
    public static int PluginCount => _pluginCount;
	public static IEncryptorPlugin UseFactory(int pluginIndex, byte[] key) => _factories[pluginIndex].CreatePlugin(key);
    public static void LoadInstalledPlugins()
	{
        LoadPlugins(defaultPath);
	}
	public static void InstallPlugin(string filePath)
	{
        LoadPlugin(filePath);
	}
    public static void InstallPluginsFromFolder(string folderPath)
	{
        LoadPlugins(folderPath);
	}
    public static void LoadPlugins(string path)
	{
        foreach(string dllPath in Directory.GetFiles(path, "*.dll"))
        {
            LoadPlugin(dllPath);
        }
	}

    private static void ProcessAssembly(Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (typeof(EncryptorPluginFactory).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            {
                EncryptorPluginFactory? factory = (EncryptorPluginFactory?)Activator.CreateInstance(type);
                if (factory != null) 
                {
                    if (!_factories.Any(f => f.PluginName == factory.PluginName))
                    {
                        _factories.Add(factory);
						_pluginCount++;
                    }
                    else
                    {
                        Console.WriteLine($"Плагин '{factory.PluginName}' уже загружен");
                    }
                }
            }
        }
    }
	public static void LoadPlugin(string filePath)
	{
        try
        {
            string destFile = Path.Combine(defaultPath, Path.GetFileName(filePath));
            if (File.Exists(destFile))
            {
                Assembly assembly = Assembly.LoadFile(destFile);
                ProcessAssembly(assembly);
            }
            else
            {
                Assembly assembly = Assembly.LoadFile(filePath);
                ProcessAssembly(assembly);
                Directory.CreateDirectory(defaultPath);
                File.Copy(filePath, destFile, overwrite: true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception while trying to load plugin from {filePath} : {ex.Message}");
        }
	}

    public static void UninstallPlugin(int pluginChoice)
    {
        var factory = _factories[pluginChoice];
        if (factory != null)
        {
            File.Delete(Path.Combine(defaultPath, factory.PluginName) + ".dll");
            Console.WriteLine($"Плагин {factory.PluginName} выгружен (деактивирован).");
            _factories.Remove(factory);
        }
    }

    public static List<string> GetPluginsNames() => _factories.Select(p => p.PluginName).ToList();

    public static string ShowInstalledPlugins()
	{
        string str = "Доступные плагины:";
        var pluginNames = GetPluginsNames();
        for(int i = 1; i < pluginNames.Count + 1; i++)
            str += $"\n{i}. {pluginNames[i - 1]}";
        if(pluginNames.Count == 0)
			str += $"\nПлагинов нет";
        return str;
	}
}