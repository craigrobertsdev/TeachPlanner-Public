invoke-expression 'cmd /c start powershell -Command { 
	write-host "Starting Tailwind";
	cd src/TeachPlanner.BlazorClient;
	npx tailwindcss -i wwwroot/css/app.css -o wwwroot/css/app.min.css --watch;
	}'

invoke-expression 'cmd /c start powershell -Command { 
	cd src/TeachPlanner.BlazorClient; 
	dotnet watch --project TeachPlanner.BlazorClient.csproj run;
}'
