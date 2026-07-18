using System.Data;

namespace NeuralForge.Application.Abstractions
{
    public interface ISqlConnectionFactory
    {
        public IDbConnection Create();
    }
}
