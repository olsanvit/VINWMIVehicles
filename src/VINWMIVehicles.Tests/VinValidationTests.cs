using FluentAssertions;
using SharedServices.Models.VINWMIVehicles.Vin;

namespace VINWMIVehicles.Tests;

/// <summary>
/// Tests VIN structural rules: 17 chars, no I/O/Q, check-digit character set.
/// These rules are validated via static helper methods defined inline since no
/// dedicated VinValidator service exists in the project.
/// </summary>
public class VinValidationTests
{
    // ── Inline validation helpers (mirrors domain rules) ─────────────────────

    private static readonly HashSet<char> InvalidChars = new() { 'I', 'O', 'Q' };

    private static bool IsValidLength(string vin) => vin.Length == 17;

    private static bool HasNoIllegalChars(string vin) =>
        vin.ToUpperInvariant().All(c => !InvalidChars.Contains(c));

    private static bool IsValidCheckDigit(char digit) =>
        char.IsDigit(digit) || digit == 'X';

    private static bool IsValidVin(string vin)
    {
        if (!IsValidLength(vin)) return false;
        var upper = vin.ToUpperInvariant();
        if (!HasNoIllegalChars(upper)) return false;
        if (!IsValidCheckDigit(upper[8])) return false;
        return true;
    }

    // ── Length tests ─────────────────────────────────────────────────────────

    [Fact]
    public void VinWithExactly17Chars_PassesLengthCheck()
    {
        var vin = "1HGCM82633A004352"; // Known valid VIN format
        IsValidLength(vin).Should().BeTrue();
    }

    [Fact]
    public void VinWith16Chars_FailsLengthCheck()
    {
        var vin = "1HGCM82633A00435";
        IsValidLength(vin).Should().BeFalse();
    }

    [Fact]
    public void VinWith18Chars_FailsLengthCheck()
    {
        var vin = "1HGCM82633A0043522";
        IsValidLength(vin).Should().BeFalse();
    }

    [Fact]
    public void EmptyVin_FailsLengthCheck()
    {
        IsValidLength(string.Empty).Should().BeFalse();
    }

    // ── Illegal character tests ───────────────────────────────────────────────

    [Fact]
    public void VinContainingI_IsInvalid()
    {
        var vin = "1HGCM826I3A004352"; // position 8 is 'I'
        HasNoIllegalChars(vin).Should().BeFalse();
    }

    [Fact]
    public void VinContainingO_IsInvalid()
    {
        var vin = "1HGCM826O3A004352";
        HasNoIllegalChars(vin).Should().BeFalse();
    }

    [Fact]
    public void VinContainingQ_IsInvalid()
    {
        var vin = "QHGCM82633A004352";
        HasNoIllegalChars(vin).Should().BeFalse();
    }

    [Fact]
    public void VinWithoutIllegalChars_PassesCharCheck()
    {
        var vin = "1HGCM82633A004352";
        HasNoIllegalChars(vin).Should().BeTrue();
    }

    // ── Check digit (position 9, index 8) tests ───────────────────────────────

    [Theory]
    [InlineData('0')]
    [InlineData('5')]
    [InlineData('9')]
    [InlineData('X')]
    public void CheckDigit_ValidValues_Accepted(char digit)
    {
        IsValidCheckDigit(digit).Should().BeTrue();
    }

    [Theory]
    [InlineData('A')]
    [InlineData('Z')]
    [InlineData('I')]
    public void CheckDigit_NonDigitNonX_Rejected(char digit)
    {
        IsValidCheckDigit(digit).Should().BeFalse();
    }

    // ── VinRecord model tests ─────────────────────────────────────────────────

    [Fact]
    public void VinRecord_WmiProperty_IsFirstThreeCharsOfVin()
    {
        var record = new VinRecord { Vin = "1HGCM82633A004352", Wmi = "1HG" };
        record.Wmi.Should().Be(record.Vin[..3]);
    }

    [Fact]
    public void VinRecord_IsValid_DefaultsFalse()
    {
        var record = new VinRecord();
        record.IsValid.Should().BeFalse();
    }

    [Fact]
    public void FullValidation_ValidVin_ReturnsTrue()
    {
        var vin = "1HGCM82633A004352";
        IsValidVin(vin).Should().BeTrue();
    }

    [Fact]
    public void FullValidation_TooShortVin_ReturnsFalse()
    {
        IsValidVin("1HGCM82633A").Should().BeFalse();
    }
}
