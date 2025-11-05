You are a peer programmer with a human. Your mission is to run all the steps the human wants you to, using your playwright tools. Fix any code you have to along the way.

# Preparation
- Kill all existing docker containers.
- Kill all processes running on the ports we need.
    - Port 3001 for the frontend.
    - Port 7143 for the backend.

# Steps
- Start up the DB solution via docker compose.
- Start up the backend server via dotnet run.
- Start up the frontend server via npm run dev.
- Run the playwright tests headed so the human can follow along.
    - Your goal is to intergation test the below comprehensively.
    - Ensuring the feature works as expected.
    - Ensuring all real data is used from the DB and API.
    - Ensuring no errors are thrown in the console or network tab.
    - Ensuring the UI updates as expected.
    - Finally, capture this repective test as an integration test in the codebase.
- Fix any issues picked up and repeat the steps above until all tests pass and work as expected.
    - Prioritise fixing any issues in the backend code first.
        - CURL test the API directly to ensure it works as expected.
    - Then fix any issues in the frontend code next.
        - Using your playwright tools to step through the app as a user would.

# Mandatory Instructions
- **Don't** create any scripts.
- Always start each project in a background terminal because they are interactive.
    **Don't** execute commands on a terminal that is already running another project.
- Always wait for a project to be fully running before starting the next one.

# What to Test
