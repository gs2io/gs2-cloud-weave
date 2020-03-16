using System;

namespace Gs2.Weave.Login
{
    public interface IAccountRepository
    {
        bool IsExistsAccount();

        void SaveAccount(PersistAccount account);

        PersistAccount LoadAccount();

        void DeleteAccount();
    }
}