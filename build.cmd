git status --porcelain |findstr . && goto giterror

cd src/WTA.Web.UI/ &&^
npm install &&^
npm run build &&^
cd ../../ &&^
dotnet restore &&^
cd src/WTA.Web/ &&^
dotnet publish -c Release -o ../../publish -r win-x64 --self-contained true -p:PublishSingleFile=true &&^
cd ../../

:giterror
git status -s
