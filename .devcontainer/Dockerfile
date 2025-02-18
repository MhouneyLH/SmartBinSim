FROM mcr.microsoft.com/dotnet/sdk:8.0

RUN apt-get update -y && apt-get upgrade -y \
    && apt-get clean

WORKDIR /workspace

COPY . .

RUN chmod +x install.sh && ./install.sh

CMD ["bash"]
