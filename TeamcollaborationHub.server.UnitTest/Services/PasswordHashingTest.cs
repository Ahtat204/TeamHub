using TeamcollborationHub.server.Services.Security;

namespace TeamcollaborationHub.server.UnitTest.Services;

[TestFixture]
[TestOf(typeof(PasswordHashing))]
public class PasswordHashingTest
{
    private PasswordHashing _passwordHashing = new();

    [Test]
    public void HashPassword()
    {
        var password = "password";
        Assert.That(_passwordHashing.Hash(password), Is.Not.Null);
        Assert.That(_passwordHashing.Hash(password), Is.Not.EqualTo(password));
    }
    [Test]
    public void VerifyHash()
    {
        var password = "password";
        var hash = _passwordHashing.Hash(password);
        Assert.That(hash, Is.Not.Null);
        Assert.That(hash, Is.Not.Null);
        Assert.That(hash, Is.Not.EqualTo(password));
        var verify = _passwordHashing.VerifyPassword(password, hash);
        Assert.That(verify, Is.True);
    }
    [Test]
    public void VerifyTwoIdenticalPasswordsDontHaveDifferentHash()
    {
        var password1 = "password";
        var password2 = "password";
        var hash1 = _passwordHashing.Hash(password1);
        var hash2 = _passwordHashing.Hash(password2);
        Assert.NotNull(hash1);
        Assert.NotNull(hash2);
        Assert.That(hash1, Is.Not.EqualTo(hash2));
    }
}