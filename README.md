# EncryptorCLI
It's a simple CLI project that you can use for data encryption. Project itself is an app that has a functionality to interact with external plugins. Each plugin is .NET project with two classes - encrypt class and factory class. 

For creating a plugin you need to use interfaces which you can find inside of subproject **CLIEncryptor.Factory**. Also you need to replace this subproject to another location to prevent conflicts during building process.

---
Это простой проект CLI, который можно использовать для шифрования данных. Сам проект представляет собой приложение, обладающее функциональностью для взаимодействия с внешними плагинами. Каждый плагин — это проект .NET с двумя классами: классом шифрования и классом фабрики.

Для создания плагина необходимо использовать интерфейсы, которые можно найти внутри подпроекта **CLIEncryptor.Factory**. Также необходимо перенести этот подпроект в другое место, чтобы избежать конфликтов в процессе сборки.
