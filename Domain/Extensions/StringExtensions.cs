namespace Domain.Extensions;

public static class StringExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullEmptyOrWhiteSpace(this string? value) =>
        string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
}