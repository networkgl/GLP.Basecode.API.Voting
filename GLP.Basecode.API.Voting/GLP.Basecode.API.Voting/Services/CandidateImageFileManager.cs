using GLP.Basecode.API.Voting.Interfaces;
using GLP.Basecode.API.Voting.Models;
using GLP.Basecode.API.Voting.Repository;

namespace GLP.Basecode.API.Voting.Services
{
    public class CandidateImageFileManager
    {
        private readonly IWebHostEnvironment _env;

        public CandidateImageFileManager(IWebHostEnvironment env)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

    }
}
