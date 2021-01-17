using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Yugen.DJ.Interfaces
{
    public interface IBPMService
    {
        double BPM { get; }

        double Detect(float[] buffer, int sampleRate, double totalMinutes);
        Task<double> Detect(IStorageFile file);
        double Detect(Stream stream);
    }
}