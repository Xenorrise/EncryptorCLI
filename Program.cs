using System.Diagnostics.CodeAnalysis;
using EncryptorCLI.FactoryInterface;
using EncryptorCLI.PluginManagement;

namespace EncryptorCLI;

[RequiresUnreferencedCode("Reflection is used by methods of this class")]
class Program
{
    static void Main(string[] args)
    {
        PluginManager.LoadInstalledPlugins();
		while (true)
		{
            Console.WriteLine("EncryptorCLIApp---Menu");
            Console.WriteLine("0. Выйти");
            Console.WriteLine("Управление плагинами:");
            Console.WriteLine("1. Использование плагинов");
            Console.WriteLine("2. Загрузка плагинов");
            Console.WriteLine("3. Выгрузка плагинов");
			Console.WriteLine(PluginManager.ShowInstalledPlugins());
            int choice = Convert.ToInt32(Console.ReadLine());
            if(choice == 0)
			{
				break;
			} 
            else if(choice == 1)
			{
				PluginUsage();
			} 
            else if(choice == 2)
			{
				PluginInstallation();
			} 
            else if(choice == 3)
			{
				PluginDeinstallation();
			}
		}
    }

    static void PluginUsage()
	{
		Console.WriteLine(PluginManager.ShowInstalledPlugins());
        Console.Write("Выберите шифрование, которое хотите использовать: ");
        IEncryptorPlugin encryptor = PluginManager.UseFactory(Convert.ToInt32(Console.ReadLine()) - 1);
        Console.Write("Укажите абсолютный путь до файла, над которым нужно провести операцию: ");
        string? inputPath = Console.ReadLine();
        Console.WriteLine("Выберите тип операции: ");
        Console.WriteLine("1. Шифрование");
        Console.WriteLine("2. Дешифрование");
        int choice = Convert.ToInt32(Console.ReadLine());
        if(inputPath != null) {
            if(choice == 1)
            {
                File.WriteAllText(inputPath, encryptor.Encrypt(File.ReadAllText(inputPath)));
            }
            else if(choice == 2)
            {
                File.WriteAllText(inputPath, encryptor.Decrypt(File.ReadAllText(inputPath)));
            }
            else
            {
                Console.WriteLine("Операции под таким номером не существует");
            }
        }
        else
        {
            Console.WriteLine("Указанный путь пустой");
        }
	}

    static void PluginInstallation()
	{
		Console.WriteLine("Выберите метод загрузки: ");
        Console.WriteLine("1. Один плагин");
        Console.WriteLine("2. Все плагины из папки");
        int choice = Convert.ToInt32(Console.ReadLine());
        if(choice == 1){
            Console.Write("Введите абсолютный путь до файла плагина: ");
            string? filePath = Console.ReadLine();
            if(filePath != null)
                PluginManager.InstallPlugin(filePath);
        }
        else if(choice == 2)
        {
            Console.Write("Введите абсолютный путь до папки с плагинами: ");
            string? folderPath = Console.ReadLine();
            if(folderPath != null)
                PluginManager.InstallPluginsFromFolder(folderPath);
        }
	}

    static void PluginDeinstallation()
	{
		Console.Write("Введите абсолютный путь до файла плагина, который хотите удалить: ");
        string? pluginPath = Console.ReadLine();
        if(pluginPath != null)
            PluginManager.UninstallPlugin(pluginPath);
	}
}