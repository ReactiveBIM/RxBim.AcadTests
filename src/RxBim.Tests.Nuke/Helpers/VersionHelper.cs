namespace RxBim.Tests.Nuke.Helpers;

/// <summary>
/// Version-related helper.
/// </summary>
public static class VersionHelper
{
    /// <summary>
    /// Extracts versions from the input string.
    /// </summary>
    /// <param name="input">A string in the format "version-version" or "version, version".
    /// Example: "2019, 2021, 2023-2025".</param>
    public static List<int> GetVersions(string input)
    {
        const char versionSplitChar = ',';
        const char versionRangeSplitChar = '-';

        var versions = new List<int>();

        var versionParts = input.Split(versionSplitChar);
        foreach (var versionPart in versionParts)
        {
            var versionRange = versionPart.Split(versionRangeSplitChar);
            switch (versionRange.Length)
            {
                case 1:
                    versions.Add(ParseVersion(versionRange[0]));
                    break;
                case 2:
                {
                    var start = ParseVersion(versionRange[0]);
                    var end = ParseVersion(versionRange[1]);
                    var includedVersions = Enumerable.Range(start, end - start + 1);
                    versions.AddRange(includedVersions);
                    break;
                }
            }
        }

        return versions;
    }

    private static int ParseVersion(string version)
    {
        if (int.TryParse(version, out var parsedVersion))
            return parsedVersion;

        throw new FormatException($"The \"{version}\" version format is invalid.");
    }
}