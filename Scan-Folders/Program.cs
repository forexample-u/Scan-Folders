using DirectoryScans;

Console.WriteLine("\nВпишите один из аттрибутов или нажмите Enter для сканирования");
Console.WriteLine("-q (--quite) - вывод только в файл;");
Console.WriteLine("-p (--path) - путь к папке для обхода;");
Console.WriteLine("-o (--output) - путь к тестовому файлу, для записи результата расчёта");
Console.WriteLine("-h (--humanread) - печать размер файла в читаемом форме");
Command command = new Command();
while (true)
{
    command.Input();
    command.Run();
}