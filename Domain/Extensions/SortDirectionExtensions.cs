namespace Domain.Extensions;

public static class SortDirectionExtensions {
    public static SortDirection FromMudDirection(this MudBlazor.SortDirection direction) {
        return direction switch {
            MudBlazor.SortDirection.Ascending => SortDirection.Ascending,
            MudBlazor.SortDirection.Descending => SortDirection.Descending,
            MudBlazor.SortDirection.None => SortDirection.None,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
    
    public static MudBlazor.SortDirection ToMudDirection(this SortDirection direction) {
        return direction switch {
            SortDirection.Ascending => MudBlazor.SortDirection.Ascending,
            SortDirection.Descending => MudBlazor.SortDirection.Descending,
            SortDirection.None => MudBlazor.SortDirection.None,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}