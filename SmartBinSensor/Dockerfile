# using .NET image that is debian based
FROM mcr.microsoft.com/dotnet/sdk:8.0

RUN apt-get update -y && apt-get upgrade -y

# Copy the application files
COPY . /app

# Set the working directory
WORKDIR /app

# Build the application
RUN dotnet publish -c Release -o out

ARG GENERATION_INTERVAL_IN_MS=1000
ARG INITIAL_FILL_PERCENTAGE=0.0
ARG VERBOSE=false

ENV GENERATION_INTERVAL_IN_MS=$GENERATION_INTERVAL_IN_MS
ENV INITIAL_FILL_PERCENTAGE=$INITIAL_FILL_PERCENTAGE
ENV VERBOSE=$VERBOSE

ENTRYPOINT ["sh", "-c", "dotnet /app/out/SmartBinSensor.dll --generation-interval-in-ms ${GENERATION_INTERVAL_IN_MS} --initial-fill-percentage ${INITIAL_FILL_PERCENTAGE} --verbose ${VERBOSE}"]

# Example call:
# $ podman run --rm -e VERBOSE=true -e GENERATION_INTERVAL_IN_MS=100 --name sensor localhost/sensor