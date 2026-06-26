using task04;

namespace task04tests;

public class SpaceshipTests
{
    [Fact]
    public void Cruiser_ShouldHaveCorrectStats()
    {
        ISpaceship cruiser = new Cruiser();
        Assert.Equal(50, cruiser.Speed);
        Assert.Equal(100, cruiser.FirePower);
    }
    [Fact]
    public void Fighter_ShouldHaveCorrectStats()
    {
        ISpaceship fighter = new Fighter();
        Assert.Equal(100, fighter.Speed);
        Assert.Equal(50, fighter.FirePower);
    }
    [Fact]
    public void Fighter_ShouldBeFasterThanCruiser()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(fighter.Speed>cruiser.Speed);
    }
    [Fact]
    public void Cruiser_ShouldHaveMoreFirePowerThanFighter()
    {
        var cruiser = new Cruiser();
        var fighter = new Fighter();
        Assert.True(cruiser.FirePower > fighter.FirePower);
    }
    [Fact]
    public void Cruiser_MoveForward_CalledWithoutErrors()
    {
        var cruiser = new Cruiser();
        cruiser.MoveForward();
    }
    [Fact]
    public void Fighter_MoveForward_CalledWithoutErrors()
    {
        var figter = new Fighter();
        figter.MoveForward();
    }
    [Fact]
    public void Cruiser_Rotate_CalledWithoutErrors()
    {
        var cruiser = new Cruiser();
        cruiser.Rotate(90);
    }
    [Fact]
    public void Fighter_Rotate_CalledWithoutErrors()
    {
        var fighter = new Fighter();
        fighter.Rotate(90);
    }
    [Fact]
    public void Cruiser_Fire_CalledWithoutErrors()
    {
        var cruiser = new Cruiser();
        cruiser.Fire();
    }
    [Fact]
    public void Fighter_Fire_CalledWithoutErrors()
    {
        var fighter = new Fighter();
        fighter.Fire();
    }
}
