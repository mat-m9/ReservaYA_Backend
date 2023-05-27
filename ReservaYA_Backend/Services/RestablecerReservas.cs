using Microsoft.EntityFrameworkCore;
using ReservaYA_Backend.Controllers;
using ReservaYA_Backend.Models;

namespace ReservaYA_Backend.Services
{
    public class RestablecerReservas : IHostedService, IDisposable
    {
        private Timer _timer;


        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Calcula la fecha y hora de la próxima ejecución
            DateTime nextRunTime = CalcularProximaEjecucion();

            // Calcula el tiempo restante hasta la próxima ejecución
            TimeSpan timeUntilNextRun = nextRunTime - DateTime.Now;

            // Inicia el temporizador
            _timer = new Timer(DoWork, null, timeUntilNextRun, TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            

            // Calcula la fecha y hora de la próxima ejecución
            DateTime nextRunTime = CalcularProximaEjecucion();

            // Calcula el tiempo restante hasta la próxima ejecución
            TimeSpan timeUntilNextRun = nextRunTime - DateTime.Now;

            // Reinicia el temporizador con el nuevo intervalo
            _timer.Change(timeUntilNextRun, TimeSpan.FromDays(1));
        }

        private DateTime CalcularProximaEjecucion()
        {
            // Obtiene la fecha y hora actual
            DateTime currentDate = DateTime.Now;

            // Calcula la fecha y hora de la próxima ejecución
            DateTime nextRunTime = currentDate.AddDays(1); // Inicialmente, establece la próxima ejecución para mañana

            // Verifica si la próxima ejecución no cae en domingo y ajusta la fecha si es necesario
            while (nextRunTime.DayOfWeek != DayOfWeek.Sunday)
            {
                nextRunTime = nextRunTime.AddDays(1);
            }

            return nextRunTime;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Detén el temporizador cuando se detenga el servicio
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // Libera los recursos utilizados por el temporizador
            _timer?.Dispose();
        }
    }
}
