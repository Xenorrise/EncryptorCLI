using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EncryptorCLI.FactoryInterface;

namespace EncryptorCLI.PluginManagement;

[RequiresUnreferencedCode("Reflection is used by methods of this class")]
public static class PluginManager
{
    private static readonly List<EncryptorPluginFactory> _factories = [];
	//private static readonly Dictionary<EncryptorPluginFactory, IEncryptorPlugin> _activePlugins = [];
    private static readonly string defaultPath = Path.Combine(AppContext.BaseDirectory, "plugins");

	public static IEncryptorPlugin UseFactory(int pluginIndex) => _factories[pluginIndex].CreatePlugin();
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

    public static void UninstallPlugin(string pluginName)
    {
        var factory = _factories.FirstOrDefault(p => p.PluginName == pluginName);
        if (factory != null)
        {
            _factories.Remove(factory);
			// Добавить выгрузку плагинов
            Console.WriteLine($"Плагин {pluginName} выгружен (деактивирован).");
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