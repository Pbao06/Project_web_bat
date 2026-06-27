FROM mcr.microsoft.com/dotnet/sdk:9.0 as build
WORKDIR /src 
# copy file project va cac goi framework
COPY ["Getdata1.csproj","./"]
RUN dotnet restore

# copy het source code vao image 
Copy . . 
# build publish ra thu muc app/ publish 
RUN dotnet publish "Getdata1.csproj" -c  Release -o /app/publish
# create runtime 
From mcr.microsoft.com/dotnet/aspnet:9.0 as final
WORKDIR /app
COPY --from=build /app/publish .

#render toi port 10000
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "Getdata1.dll"]