$root = Split-Path -Parent $MyInvocation.MyCommand.Path

Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$root\Jewelry\Jewelry.APIService'; dotnet run" -WindowStyle Normal
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$root\Jewelry\Jewelry.AuthService'; dotnet run" -WindowStyle Normal
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$root\WebClient'; npm run dev" -WindowStyle Normal

Write-Host "Da khoi dong: APIService (5001), AuthService (5002), Frontend (3000)"
