@echo off
setlocal
cd /d "%~dp0"

echo Starting RabbitMQ...
docker compose up -d
if errorlevel 1 (
  echo Docker Compose failed. Is Docker running?
  exit /b 1
)

if not exist "src\Client\node_modules\" (
  echo Installing client dependencies...
  pushd src\Client
  call npm install
  if errorlevel 1 (
    popd
    echo npm install failed.
    exit /b 1
  )
  popd
)

start "Cities.Api" cmd /k "dotnet run --project src\Cities.Api --launch-profile http"
start "Weather.Api" cmd /k "dotnet run --project src\Weather.Api --launch-profile http"
start "Gateway.Api" cmd /k "dotnet run --project src\Gateway.Api --launch-profile http"
start "Client" cmd /k "cd /d src\Client && npm run dev"

echo.
echo RabbitMQ UI:  http://localhost:15672  (guest / guest)
echo Cities API:   http://localhost:5249
echo Weather API:  http://localhost:5250
echo Gateway:      http://localhost:5260
echo Client:       http://localhost:5173
echo.
echo Close the extra terminal windows to stop the apps.
echo RabbitMQ keeps running: docker compose down
