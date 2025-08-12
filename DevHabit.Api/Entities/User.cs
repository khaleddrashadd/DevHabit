namespace DevHabit.Api.Entities;

public sealed class User
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    ///<summary>
    /// we will use this field to store IdentityId from Identity Provider.
    /// this could be any identity provider like Azure AD, Google,Auth0, etc.
    ///</summary>

    public string IdentityId { get; set; }
}