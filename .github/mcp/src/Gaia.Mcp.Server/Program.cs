// Gaia MCP Server (skeleton)
// NOTE: This repo ships a reference implementation layout. Wire it into your MCP runtime of choice.

using Gaia.Mcp.Server.Tools;

namespace Gaia.Mcp.Server;

public static class Program
{
    public static void Main(string[] args)
    {
        // Intentionally minimal.
        // Register tools with your MCP host / JSON-RPC bridge.
        // Tools are implemented as pure services so they can be hosted anywhere.
        Console.WriteLine("Gaia.Mcp.Server skeleton. Host integration required.");
    }
}
