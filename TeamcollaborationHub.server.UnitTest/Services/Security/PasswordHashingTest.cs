using TeamcollborationHub.server.Services.Security;

namespace TeamcollaborationHub.server.UnitTest.Services.Security;

[TestFixture]
[TestOf(typeof(PasswordHashing))]
public class PasswordHashingTest
{
    private PasswordHashing passwordHashing;

    public PasswordHashingTest()
    {
        passwordHashing = new();
    }

    [Test]
    public void HashPassword()
    {
        var password = "password";
        Assert.NotNull(passwordHashing.Hash(password));
        Assert.That(passwordHashing.Hash(password), Is.Not.EqualTo(password));
    }
    [Test]
    public void VerifyHash()
    {
        var password = "password";
        var hash = passwordHashing.Hash(password);
        Assert.NotNull(hash);
        Assert.That(hash, Is.Not.Null);
        Assert.That(hash, Is.Not.EqualTo(password));
        var verify = passwordHashing.VerifyPassword(password, hash);
        Assert.That(verify, Is.True);
    }
    [Test]
    public void VerifyTwoIdenticalPasswordsDontHaveDifferentHash()
    {
        var password1 = "password";
        var password2 = "password";
        var hash1 = passwordHashing.Hash(password1);
        var hash2 = passwordHashing.Hash(password2);
        Assert.NotNull(hash1);
        Assert.NotNull(hash2);
        Assert.That(hash1, Is.Not.EqualTo(hash2));
    }
}