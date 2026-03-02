#!/bin/bash
# @name: example-hello
# @description: A simple example workflow that greets the user. Use this as a template for new workflows.
# @param name: The name to greet (default: World)
# @param greeting: Optional greeting prefix (default: Hello)
# @output: A greeting message printed to stdout

NAME="${name:-World}"
GREETING="${greeting:-Hello}"

echo "${GREETING}, ${NAME}!"
echo "Workflow completed successfully."
