.nuget\nuget install -o packages NUnit.Runners
.nuget\nuget install -o packages Grok.Numenta\packages.config
.nuget\nuget install -o packages HelloGrok\packages.config
.nuget\nuget install -o packages Tests\Integration\Grok.Numenta.IntegrationTests\packages.config
.nuget\nuget install -o packages Tests\Unit\Grok.Numenta.UnitTests\packages.config
msbuild grok-net.sln
packages\NUnit.Runners.2.6.2\tools\nunit-console /result:grok-net.xml /work:..\results Tests\Release\Grok.Numenta.UnitTests.dll
