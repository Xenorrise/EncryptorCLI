using System.Diagnostics.CodeAnalysis;
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
            int choice = (int)GetUserInput("Выберите опцию: ", "Ошибка: опции под таким номером нет", true);
            if(choice == 0)
			{
				break;
			}
            else if(choice == 1)
			{
                if(PluginManager.PluginCount > 0)
                {
				    PluginUsage();
                }
				else
				{
					Console.WriteLine("В программе нет загруженных плагинов. Попробуйте загрузить хотя бы один");
				}
			} 
            else if(choice == 2)
			{
				PluginInstallation();
			} 
            else if(choice == 3)
			{
                if(PluginManager.PluginCount > 0)
                {
				    PluginDeinstallation();
                }
				else
				{
					Console.WriteLine("В программе нет загруженных плагинов, чтобы их удалять");
				}
			}
		}
    }

    private static void PluginUsage()
	{
		Console.WriteLine(PluginManager.ShowInstalledPlugins());
        int pluginChoice = (int)GetUserInput("Выберите шифрование, которое хотите использовать: ", "Ошибка: плагина под таким номером нет", true) - 1;
        string inputPath = (string)GetUserInput("Укажите абсолютный путь до файла, над которым нужно провести операцию: ", "Ошибка: некорректный путь файла");
        Console.WriteLine("Выберите тип операции: ");
        Console.WriteLine("1. Шифрование");
        Console.WriteLine("2. Дешифрование");
        int choice = (int)GetUserInput("", "Ошибка: операции под таким номером нет", true);
        byte[] key = BitConverter.GetBytes((int)GetUserInput("Укажите ключ шифрования: ", "Ошибка: некорректный ключ шифрования", true));
        var encryptor = PluginManager.UseFactory(pluginChoice, key);
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

    private static void PluginInstallation()
	{
		Console.WriteLine("Выберите метод загрузки: ");
        Console.WriteLine("1. Один плагин");
        Console.WriteLine("2. Все плагины из папки");
        int choice = (int)GetUserInput("", "Ошибка: операции под таким номером нет", true);
        if(choice == 1){
            string filePath = (string)GetUserInput("Введите абсолютный путь до файла плагина: ", "Ошибка: некорректный путь файла");
            if(filePath != null)
                PluginManager.InstallPlugin(filePath);
        }
        else if(choice == 2)
        {
            string? folderPath = (string?)GetUserInput("Введите абсолютный путь до папки с плагинами: ", "Ошибка: некорректный путь папки");
            if(folderPath != null)
                PluginManager.InstallPluginsFromFolder(folderPath);
        }
	}

    private static void PluginDeinstallation()
	{
        Console.WriteLine(PluginManager.ShowInstalledPlugins());
        int pluginChoice = (int)GetUserInput("Введите номер плагина, который хотите удалить: ", "Ошибка: операции под таким номером нет", true) - 1;
        PluginManager.UninstallPlugin(pluginChoice);
	}

    private static object GetUserInput(string message, string errorMessage, bool isNum = false)
    {
        bool success;
        int numValue = 0;
        string? text = "";
        do
        {
            Console.Write(message);
            text = Console.ReadLine();
            success = isNum ? int.TryParse(text, out numValue) : !string.IsNullOrWhiteSpace(text);
            if (!success)
            {
                Console.WriteLine(errorMessage);
            }
        } while (!success);

        if(isNum)
            return numValue;
		else
			return text!;
    }
}