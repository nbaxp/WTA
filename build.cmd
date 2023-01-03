@echo off
git status --porcelain |findstr . && goto giterror

if output=$(git status --porcelain) && [ -z "$output" ]; then
  echo "Working directory clean"
else 
  echo "Uncommitted changes"
fi

cd src/WTA.Web.UI/ &&^
npm install &&^
npm run build &&^
cd ../../ &&^
dotnet restore &&^
cd src/WTA.Web/ &&^
dotnet publish -c Release -o ../../publish -r win-x64 --self-contained true -p:PublishSingleFile=true &&^
cd ../../

:giterror
@echo on
git status
