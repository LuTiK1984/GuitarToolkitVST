namespace GuitarToolkit.Core.Models;

/// <summary>
/// Определение гаммы/лада: название и набор интервалов от тоники в полутонах.
/// </summary>
public record ScaleDefinition(string Name, int[] Intervals);
