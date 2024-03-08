#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["WebApiNet5/WebApiNet5.csproj", "WebApiNet5/"]
RUN dotnet restore "WebApiNet5/WebApiNet5.csproj"
COPY . .
WORKDIR "/src/WebApiNet5"
RUN dotnet build "WebApiNet5.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApiNet5.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY ./WebApiNet5/procdump.deb .
#COPY ./WebApiNet5/start.sh .

#老的运行
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApiNet5.dll"]
#老的运行


# 1. 使用中科大镜像源
#RUN sed -i 's/deb.debian.org/mirrors.ustc.edu.cn/g' /etc/apt/sources.list

# 2. 安装 wget
#RUN apt-get update && apt-get install -y gdb

#RUN dpkg -i procdump.deb
#RUN mkdir /dumps
#RUN echo "#!/bin/bash \n\
#procdump -c 80 -n 2 -s 2 -w dotnet /dumps & \n\
#dotnet \$1 \n\
#" > /app/start.sh

#RUN chmod +x /app/start.sh
#ENTRYPOINT ["/app/start.sh", "WebApiNet5.dll"]