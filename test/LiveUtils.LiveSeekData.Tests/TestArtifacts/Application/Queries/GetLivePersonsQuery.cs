using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiveUtils.LiveSeekData.Tests.TestArtifacts.Application.Data;
using LiveUtils.LiveSeekData.Tests.TestArtifacts.Application.Domain;
using LiveUtils.Seek;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiveUtils.LiveSeekData.Tests.TestArtifacts.Application.Queries
{
    public class GetLivePersonsQuery : IRequest<List<Person>>
    {
        public int Page { get; }
        public int Size { get; }
        public long LastId { get; }

        public GetLivePersonsQuery(int page, int size,long lastId)
        {
            Page = page < 0 ? 1 : page;
            Size = size < 0 ? 1 : size;
            LastId = lastId;
        }
    }

    public class GetLivePersonsQueryHandler:IRequestHandler<GetLivePersonsQuery,List<Person>>
    {
        private readonly TestDbContext _context;

        public GetLivePersonsQueryHandler(TestDbContext context)
        {
            _context = context;
        }

        public async Task<List<Person>> Handle(GetLivePersonsQuery request, CancellationToken cancellationToken)
        {
            var liveSeek = new LiveSeek(request.Size, request.LastId);

            var next = liveSeek.LiveCursors.FirstOrDefault(x => x.Index == request.Page);
            
            var result = _context.Persons
                .AsNoTracking()
                .OrderBy(x => x.LiveRowId)
                .Where(x=>x.LiveRowId>=next.BeginRow)
                .Take(request.Size)
                .ToList();
            
            return result;
        }
    }
}