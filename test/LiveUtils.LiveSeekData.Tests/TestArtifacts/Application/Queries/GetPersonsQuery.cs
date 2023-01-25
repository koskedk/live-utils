using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiveUtils.LiveSeekData.Tests.TestArtifacts.Application.Data;
using LiveUtils.LiveSeekData.Tests.TestArtifacts.Application.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiveUtils.LiveSeekData.Tests.TestArtifacts.Application.Queries
{
    public class GetPersonsQuery : IRequest<List<Person>>
    {
        public int Page { get; }
        public int Size { get; }
        public int Skip { get; }

        public GetPersonsQuery(int page, int size)
        {
            Page = page < 0 ? 1 : page;
            Size = size < 0 ? 1 : size;
            Skip = Page * Size;
        }
    }

    public class GetPersonsQueryHandler:IRequestHandler<GetPersonsQuery,List<Person>>
    {
        private readonly TestDbContext _context;

        public GetPersonsQueryHandler(TestDbContext context)
        {
            _context = context;
        }

        public async Task<List<Person>> Handle(GetPersonsQuery request, CancellationToken cancellationToken)
        {
            var result = _context.Persons
                .AsNoTracking()
                .OrderBy(x => x.LiveRowId)
                .Skip(request.Skip)
                .Take(request.Size)
                .ToList();
            
            return result;
        }
    }
}