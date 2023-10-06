using System;
using System.IO;
using System.Threading;

namespace OrangePiGpio
{
    public class PwmController
    {
        private const int WAIT_PERMISSION_TIMEOUT = 1000; // in milliseconds
        private const string GPIO_PATH = "/sys/class/gpio";
        private const string PWM_PATH = "/sys/class/pwm";

        private void AwaitPermissions(string path)
        {
            DateTime startTime = DateTime.Now;
            while (!File.Exists(path) && (DateTime.Now - startTime).TotalMilliseconds < WAIT_PERMISSION_TIMEOUT)
            {
                Thread.Sleep(100);
            }
        }

        private void WriteToFile(string path, string value)
        {
            AwaitPermissions(path);
            File.WriteAllText(path, value);
        }

        private string ReadFromFile(string path)
        {
            AwaitPermissions(path);
            return File.ReadAllText(path);
        }

        public void Export(int pin) => WriteToFile($"{GPIO_PATH}/export", pin.ToString());

        public void Unexport(int pin) => WriteToFile($"{GPIO_PATH}/unexport", pin.ToString());

        public void Direction(int pin, string dir) => WriteToFile($"{GPIO_PATH}/gpio{pin}/direction", dir);

        public int Input(int pin) => int.Parse(ReadFromFile($"{GPIO_PATH}/gpio{pin}/value").Trim());

        public void Output(int pin, int value) => WriteToFile($"{GPIO_PATH}/gpio{pin}/value", value.ToString());

        public void Edge(int pin, string trigger) => WriteToFile($"{GPIO_PATH}/gpio{pin}/edge", trigger);

        public void PWM_Export(int chip, int pin) => WriteToFile($"{PWM_PATH}/pwmchip{chip}/export", pin.ToString());

        public void PWM_Unexport(int chip, int pin) => WriteToFile($"{PWM_PATH}/pwmchip{chip}/unexport", pin.ToString());

        public void PWM_Enable(int chip, int pin) => WriteToFile($"{PWM_PATH}/pwmchip{chip}/pwm{pin}/enable", "1");

        public void PWM_Disable(int chip, int pin) => WriteToFile($"{PWM_PATH}/pwmchip{chip}/pwm{pin}/enable", "0");

        public void PWM_Polarity(int chip, int pin, bool invert) => WriteToFile($"{PWM_PATH}/pwmchip{chip}/pwm{pin}/polarity", invert ? "inversed" : "normal");

        public void PWM_Period(int chip, int pin, long pwmPeriod) => WriteToFile($"{PWM_PATH}/pwmchip{chip}/pwm{pin}/period", pwmPeriod.ToString());

        public void PWM_Frequency(int chip, int pin, double pwmFrequency) => PWM_Period(chip, pin, (long)(1e9 / pwmFrequency));

        public void PWM_DutyCyclePercent(int chip, int pin, double dutyCyclePercent) => WriteToFile($"{PWM_PATH}/pwmchip{chip}/pwm{pin}/duty_cycle", ((long)(dutyCyclePercent / 100.0 * long.Parse(ReadFromFile($"{PWM_PATH}/pwmchip{chip}/pwm{pin}/period")))).ToString());

        public void PWM_DutyCycle(int chip, int pin, long dutyCycle)
        {
            if (dutyCycle > long.Parse(ReadFromFile($"{PWM_PATH}/pwmchip{chip}/pwm{pin}/period")))
            {
                throw new InvalidOperationException($"Duty cycle {dutyCycle} exceeds current period");
            }
            WriteToFile($"{PWM_PATH}/pwmchip{chip}/pwm{pin}/duty_cycle", dutyCycle.ToString());
        }
    }
}
