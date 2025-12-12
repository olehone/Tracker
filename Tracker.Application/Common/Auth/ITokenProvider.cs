using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Auth;

public interface ITokenProvider
{
    string Create(User user); 
}
