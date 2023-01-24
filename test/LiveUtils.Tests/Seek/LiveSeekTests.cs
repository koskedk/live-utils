using LiveUtils.Seek;
using NUnit.Framework;

namespace LiveUtils.Tests.Seek
{
    [TestFixture]
    public class LiveSeekTests
    {
    
        [TestCase(2,4,2)]
        [TestCase(2,5,3)]
        [TestCase(3,10,4)]
        [TestCase(10,10,1)]
        public void should_Generate(long size,long lastRow, long count)
        {
            var seq = new LiveSeek(size, lastRow);
            Assert.That(seq.LiveCursors.Count,Is.EqualTo(count));

            foreach (var item in seq.LiveCursors)
            {
                Console.WriteLine($"{item}");
            }
        }
    
    }
}