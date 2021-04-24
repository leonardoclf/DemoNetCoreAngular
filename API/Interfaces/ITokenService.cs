using API.Entities;

namespace API.Interfaces
{
    public interface ITokenService
    {
        
        string CreateToken(AppUser user);
    }
}

/*
    contract
    interface doesnt have logic only sign
*/