using System.Reflection;

/* General Product Information */
[assembly: AssemblyProduct("App.Refoveo")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyCopyright("Copyright © 2014")]
[assembly: AssemblyTrademark("")]

/* Configuration Type */
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

/* Assembly Versioning: mixture of MSDN and SemVer approaches
 *   MSDN Versioning: http://msdn.microsoft.com/en-us/library/51ket42z%28v=vs.110%29.aspx
 *   SemVer:          http://semver.org/spec/v2.0.0.html 
 *   
 * Format: Major.Minor.Patch-ReleaseType.BuildNum
 *  - Major - for incompatible API changes (big changes)
 *  - Minor - for adding functionality in a backwards-compatible manner (small changes)
 *  - Patch - for bugfixes (increments only for "bugfix/" branch)
 *  - ReleaseType - additional information: either 
 *     -- "pre-alpha" - active phase on docs creation and specifications ("dev" branch)
 *     -- "alpha" - active development and adding new features ("dev" branch)
 *     -- "beta" - feature-freeze (feature-complete), active bugfixes ("staging-qa" branch)
 *     -- "rc" - stabilizing code: only very critical issues are to be resolved ("staging-qa" branch)
 *     -- "release" - release in production ("release" branch)
 *     -- "release.hotfix" - rapid bugfix for release ("hotfix" branch)
 *     
 *  AssemblyVersion - defines a version for the product itself 
 *   -- format: Major.Minor.0
 *  AssemblyFileVersion - defines a version for particular assembly 
 *   -- format: Major.Minor.Patch
 *  AssemblyInformationalVersion - defines detailed version for the product and assemblies 
 *   -- format: Major.Minor.Patch-ReleaseType.BuildNumber
 *   
 *  {develop} branch contains ONLY alpha version
 */
[assembly: AssemblyVersion("0.1.1")]
[assembly: AssemblyFileVersion("0.1.1")]
[assembly: AssemblyInformationalVersion("0.1.1-alpha.2")]
