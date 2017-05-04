param([string]$scenarios="[default]", [string]$server="kestrel")

$scenarios = (-split $scenarios) -join ","

cd Benchmarks
dotnet restore
dotnet build -c Release
Start-Process -NoNewWindow dotnet -ArgumentList "bin\Release\netcoreapp2.0\Benchmarks.dll", "server.urls=http://*:8080", "server=$server", "threadCount=1", "NonInteractive=true", "scenarios=$scenarios"
