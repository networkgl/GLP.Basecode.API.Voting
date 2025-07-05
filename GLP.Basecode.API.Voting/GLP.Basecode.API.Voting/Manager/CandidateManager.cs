using GLP.Basecode.API.Voting.Models;
using GLP.Basecode.API.Voting.Repository;

namespace GLP.Basecode.API.Voting.Manager
{
    public class CandidateManager
    {
        private readonly VotingAppDbContext _dbContext;
        private readonly BaseRepository<Candidate> _candidateRepo;
        private readonly BaseRepository<FilePath> _filePathRepo;

        public CandidateManager(
            VotingAppDbContext dbContext,
            BaseRepository<Candidate> candidateRepo,
            BaseRepository<FilePath> filePathRepo
            )
        {
            _dbContext = dbContext;
            _candidateRepo = candidateRepo;
            _filePathRepo = filePathRepo;
        }
    }
}
