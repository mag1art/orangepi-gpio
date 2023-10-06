using System;

namespace OrangePiGpio
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Настройка PWM...");
            var pwm = new PWM(0, 0, 50, 0); // Пример для chip=0, pin=0, frequency=50Hz и dutyCycle=0%

            while (true)
            {
                Console.WriteLine("Введите команду и значение (например, '1 50') или 'exit' для выхода:");
                var command = Console.ReadLine();

                if (string.IsNullOrEmpty(command))
                    continue;

                if (command == "exit")
                    break;

                if (command == "help")
                {
                    DisplayHelp();
                    continue;
                }

                var parts = command.Split(' ');
                var cmd = parts[0];

                try
                {
                    switch (cmd)
                    {
                        case "1":
                            var dutyCycle = double.Parse(parts[1]);
                            pwm.SetDutyCycle(dutyCycle);
                            Console.WriteLine($"Duty cycle изменен на {dutyCycle}%");
                            break;

                        case "2":
                            var frequency = double.Parse(parts[1]);
                            pwm.ChangeFrequency(frequency);
                            Console.WriteLine($"Частота изменена на {frequency}Hz");
                            break;

                        case "3":
                            pwm.TogglePolarity();
                            Console.WriteLine($"Полярность изменена на {(pwm.InvertPolarity ? "inversed" : "normal")}");
                            break;

                        case "4":
                            pwm.StartPWM();
                            Console.WriteLine("PWM запущен");
                            break;

                        case "5":
                            pwm.StopPWM();
                            Console.WriteLine("PWM остановлен");
                            break;

                        default:
                            Console.WriteLine("Неизвестная команда");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }

            pwm.Close();
            Console.WriteLine("PWM закрыт");
        }

        static void DisplayHelp()
        {
            Console.WriteLine("Доступные команды:");
            Console.WriteLine("1 [0-100] - Установить Duty Cycle в процентах");
            Console.WriteLine("2 [частота] - Изменить частоту PWM");
            Console.WriteLine("3 - Переключить полярность PWM");
            Console.WriteLine("4 - Запустить PWM");
            Console.WriteLine("5 - Остановить PWM");
            Console.WriteLine("help - Показать эту справку");
            Console.WriteLine("exit - Выйти из программы");
        }
    }
}
