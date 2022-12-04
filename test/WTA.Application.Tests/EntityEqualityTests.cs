using WTA.Application.Domain.Users;

namespace WTA.Application.Tests
{
  [TestClass]
  public class EntityEqualityTests
  {
    [TestMethod]
    public void TwoTransientEntitiesShouldNotBeEqual()
    {
      var entity1 = new User();
      var entity2 = new User();

      Assert.AreNotEqual(entity1, entity2);
    }
  }
}
