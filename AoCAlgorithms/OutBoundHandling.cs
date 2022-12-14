namespace AoCAlgorithms;


/// <summary>
/// Out of bounds policy
/// </summary>
public enum OutBoundHandling
{
    /// <summary>
    /// Throw when indices are outside of bonds (default mode)
    /// </summary>
    Throw,
    /// <summary>
    /// No constraint on indices, returns default value if out of bonds
    /// </summary>
    DefaultValue,
    /// <summary>
    /// Indices constraint to bounds. They are wrapped around if outside bounds
    /// <remarks>This will be reflected in returned coordinates</remarks>
    /// </summary>
    WrapAround,
    /// <summary>
    /// No constraint on indices, provided array is repeated along the axis.
    /// </summary>
    Repeat
}