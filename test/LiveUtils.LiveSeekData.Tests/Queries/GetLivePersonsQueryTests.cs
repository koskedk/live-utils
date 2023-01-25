using System.Threading.Tasks;
using LiveUtils.LiveSeekData.Tests.TestArtifacts.Application.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LiveUtils.LiveSeekData.Tests.Queries;

[TestFixture]
public class GetLivePersonsQueryTests
{
    private IMediator _mediator;
        
    [SetUp]
    public void Setup()
    {
        _mediator = TestInitializer.ServiceProvider.GetService<IMediator>();
    }

    [TestCase(1, 1000)]
    [TestCase(100, 1000)]
    [TestCase(500, 1000)]
    [TestCase(999, 1000)]
    public async Task should_Query_Live(int page, int size)
    {
        var res = await _mediator.Send(new GetLivePersonsQuery(page, size,TestInitializer.TotalItems));
        Assert.That(res.Count,Is.EqualTo(size));
    }
}