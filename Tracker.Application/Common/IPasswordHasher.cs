namespace Tracker.Application.Common;
public interface IPasswordHasher
{
    string Hash(string password);

}
