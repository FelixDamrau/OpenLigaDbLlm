# OpenLigaDbLlm

This project demonstrates the integration of a .NET ChatClient with an MCP (Model Context Protocol) Server that provides tools to interact with the OpenLigaDb API. The project includes a ChatClient, an MCP Server, and uses Docker Compose for setting up a monitoring stack (OpenTelemetry Collector, Loki, and Grafana).

## Project Structure

- `src/ChatWithTools`: Contains the .NET ChatClient application.
- `src/McpServer`: Contains the .NET MCP Server application with OpenLigaDb tools.
- `docker-compose.yml`: Defines the services for running the OpenTelemetry Collector, Loki, and Grafana using Docker.
- `otel-collector-config.yaml`: Configuration for the OpenTelemetry Collector.

## OpenLigaDB Swagger File

The `openligadb-swagger.json` file in the project root is a local copy of the OpenLigaDB API swagger definition, downloaded on 4/29/2025. This local file is used for generating the OpenLigaDB service client using NSwag.

A known issue regarding the `match.location` property being nullable in the swagger specification has been addressed in this local copy. For more details, refer to the GitHub issue: https://github.com/OpenLigaDB/OpenLigaDB-Samples/issues/96

## Prerequisites

- Docker and Docker Compose installed.
- .NET SDK (required to run the C# projects).

## Getting Started

This project involves running the C# applications and a monitoring stack using Docker Compose.

1. **Clone the repository:**

   ```bash
   git clone https://github.com/FelixDamrau/OpenLigaDbLlm.git
   cd OpenLigaDbLlm
   ```

2. **Start the monitoring stack with Docker Compose:**

   ```bash
   docker-compose up -d
   ```

   This command will start the OpenTelemetry Collector, Loki, and Grafana containers in detached mode.

3. **Run the .NET projects:**

   Before running the applications, ensure you have the following environment variables set:

   - `HELICONE_API_KEY`: Your Helicone API key. This is used for monitoring and logging API calls via Helicone.
   - `OPENROUTER_API_KEY`: Your OpenRouter API key. This is used for authenticating with the OpenRouter API.

   Navigate to the respective project directories and run the applications using the .NET CLI:

   ```bash
   cd src/McpServer
   dotnet run

   # In a new terminal:
   cd src/ChatWithTools
   dotnet run
   ```

   Ensure the McpServer is running before starting the ChatClient.

## MCP Server

The `McpServer` project implements an MCP server that exposes tools for interacting with the OpenLigaDb API. These tools can be used by an MCP-compatible client (like the included ChatClient) to retrieve data from OpenLigaDb.

### MCP Server Tools

The MCP server exposes the following tools:

- `getAvailableLeagues`: Gets all available leagues
- `getFilteredLeagues`: Gets all available leagues that match the given filter
- `getAllTeams`: Gets all teams for the given league in the given season
- `echo`: Echoes the given message. This tool is mainly for debugging purposes.

## ChatClient

The `ChatWithTools` project is a .NET console application that acts as a client to the MCP Server. It demonstrates how to connect to an MCP server, discover available tools, and use them to perform actions (in this case, interacting with the OpenLigaDb API).

Ensure you have set the `HELICONE_API_KEY` and `OPENROUTER_API_KEY` environment variables before running the ChatClient.

To use the MCP tools via the ChatClient, you will interact with the running `ChatWithTools` console application. The specific commands or interface will depend on the implementation within the `ChatWithTools` project. Refer to the ChatClient's output or documentation (if any) for details on how to interact with it and utilize the MCP tools.

## Testing the MCP Server

You can test the MCP server using the MCP Inspector tool.

1. Start the MCP Inspector from your terminal (e.g., PowerShell):

   ```powershell
   npx @modelcontextprotocol/inspector
   ```

2. In the MCP Inspector, connect to the MCP server. The default host is `http://127.0.0.1:6274/`.
3. Select `SSE` as the transport type and connect.

## Monitoring (OpenTelemetry)

The project includes a Docker Compose setup for a monitoring stack using OpenTelemetry:

- **OpenTelemetry Collector:** Collects telemetry data from the applications.
- **Loki:** A log aggregation system.
- **Grafana:** A data visualization and dashboarding tool.

You can access Grafana at `http://localhost:3000` after starting the Docker containers.

## Monitoring (Helicone)

The project also supports monitoring and logging API calls via Helicone. To enable this, ensure you have set the `HELICONE_API_KEY` environment variable.

## Links

- OpenLigaDB Website: [https://www.openligadb.de/](https://www.openligadb.de/)
- OpenLigaDB Samples GitHub: [https://github.com/OpenLigaDB/OpenLigaDB-Samples](https://github.com/OpenLigaDB/OpenLigaDB-Samples)
