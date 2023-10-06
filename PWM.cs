using System;

namespace OrangePiGpio
{
    public class PWM
    {
        private readonly PwmController _controller;
        public int Chip { get; }
        public int Pin { get; }
        public double Frequency { get; private set; }
        public double DutyCyclePercent { get; private set; }
        public bool InvertPolarity { get; private set; }

        public PWM(int chip, int pin, double frequency, double dutyCyclePercent, bool invertPolarity = false)
        {
            _controller = new PwmController();
            Chip = chip;
            Pin = pin;
            Frequency = frequency;
            DutyCyclePercent = dutyCyclePercent;
            InvertPolarity = invertPolarity;

            _controller.PWM_Export(Chip, Pin);
            _controller.PWM_Polarity(Chip, Pin, InvertPolarity);
            _controller.PWM_Enable(Chip, Pin);
            _controller.PWM_Frequency(Chip, Pin, Frequency);
        }

        public void StartPWM() => _controller.PWM_DutyCyclePercent(Chip, Pin, DutyCyclePercent);

        public void StopPWM() => _controller.PWM_DutyCyclePercent(Chip, Pin, 0);

        public void ChangeFrequency(double newFrequency)
        {
            double oldFrequency = Frequency;
            Frequency = newFrequency;

            if (newFrequency > oldFrequency)
            {
                _controller.PWM_Frequency(Chip, Pin, Frequency);
                _controller.PWM_DutyCyclePercent(Chip, Pin, DutyCyclePercent);
            }
            else
            {
                _controller.PWM_DutyCyclePercent(Chip, Pin, DutyCyclePercent);
                _controller.PWM_Frequency(Chip, Pin, Frequency);
            }
        }

        public void SetDutyCycle(double dutyCyclePercent)
        {
            if (dutyCyclePercent < 0 || dutyCyclePercent > 100)
                throw new ArgumentException("Duty cycle must be between 0 and 100.");

            DutyCyclePercent = dutyCyclePercent;
            _controller.PWM_DutyCyclePercent(Chip, Pin, DutyCyclePercent);
        }

        public void TogglePolarity()
        {
            _controller.PWM_Disable(Chip, Pin);
            InvertPolarity = !InvertPolarity;
            _controller.PWM_Polarity(Chip, Pin, InvertPolarity);
            _controller.PWM_Enable(Chip, Pin);
        }

        public void Close() => _controller.PWM_Unexport(Chip, Pin);
    }
}
