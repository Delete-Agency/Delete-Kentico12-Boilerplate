using System.Threading;
using DeleteBoilerplate.Common.Helpers;
using DeleteBoilerplate.Tasks.Models;

namespace DeleteBoilerplate.Tasks.Processor
{
    public class CalculatorProcessor
    {
        public void Process(CalculatorInfo parameters)
        {
            Thread.Sleep(3000);
            var answer = parameters.FirstNumber + parameters.SecondNumber;

            LogHelper.LogInformation(nameof(CalculatorProcessor), "SOLUTION", $"{parameters.FirstNumber} + {parameters.SecondNumber} = {answer}");
        }
    }
}
