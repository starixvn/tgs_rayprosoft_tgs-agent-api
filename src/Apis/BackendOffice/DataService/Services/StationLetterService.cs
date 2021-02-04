using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Nats.Model;

namespace TGS.Apis.BackendOffice.DataService.Services
{
    public interface IStationLetterService
    {
        Task CreateStationLetterAsync(StationLetter stationLetter, StationLetterPlayer[] stationLetterPlayers);
    }

    public class StationLetterService : IStationLetterService
    {
        private const string Pattern = @"\b(http(s?):)([/|:|.|\w|-])*\.(?:jpg|gif|png|jpeg)\b";
        private readonly INatsManager _natsManager;
        private readonly ITgsRepository _repository;

        public StationLetterService(ITgsRepository repository, INatsManager natsManager)
        {
            _repository = repository;
            _natsManager = natsManager;
        }

        async Task IStationLetterService.CreateStationLetterAsync(StationLetter stationLetter,
            StationLetterPlayer[] stationLetterPlayers)
        {
            var imageNames = GetImageNames(stationLetter.Content);

            await using var tran = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            stationLetter.Players = stationLetterPlayers;
            await _repository.StationLetters.AddAsync(stationLetter);

            await _repository.SaveChangesAsync();

            var file = new ObjectFileEventModel
            {
                Id = stationLetter.Id.ToString(),
                FileNames = imageNames
            };

            if (imageNames.Any())
            {
                await _natsManager.PublishAsync(nameof(ObjectFileEventModel), file);
            }

            await tran.CommitAsync();
        }

        private string[] GetImageNames(string content)
        {
            var rgx = new Regex(Pattern);

            return rgx.Matches(content).Select(x =>
                x.Value[(x.Value.LastIndexOf("/", StringComparison.Ordinal) + 1)..]).ToArray();
        }
    }
}