using System;

namespace Cimplicity.Views.Data.Context
{
    public interface IDataContext : IDisposable
    {
        void Commit();
        void RollBack();

        IDataContext Create();
    }
}