using System;

namespace GeneratorTools;

/// <summary>
///     Attribute signaling the documentation generator not to include item in output.
/// </summary>
[OmitFromDocumentation]
[AttributeUsage(AttributeTargets.All)]
public sealed class OmitFromDocumentationAttribute : Attribute
{
}
