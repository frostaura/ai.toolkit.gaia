---
name: start-projects
description: A skill for understanding how to run projects both locally and in production. This includes knowledge of the necessary tools, dependencies, and configurations required to set up and run the project successfully. The agent should be able to provide clear instructions for running the project in different environments, troubleshoot common issues that may arise during setup, and ensure that the project is running smoothly.
---

<skill>
  <name>start-projects</name>
  <description>
  A skill for understanding how to run projects both locally and in production. This includes knowledge of the necessary tools, dependencies, and configurations required to set up and run the project successfully. The agent should be able to provide clear instructions for running the project in different environments, troubleshoot common issues that may arise during setup, and ensure that the project is running smoothly.
  </description>
  <responsibilities>
    <responsibility>Provide clear instructions for setting up and running the project locally and in production.</responsibility>
    <responsibility>How to effetibvely use the docker-compose files for local development and production deployment.</responsibility>
    <responsibility>How to effectively spin up the backend and frontend services for testing and iterating on issues.</responsibility>
  </responsibilities>
  <hints>
    <hint>`npm run dev` to start the development server for the frontend. Do this in a background terminal or detatched mode so that the process lives beyond your response so the user may test.</hint>
    <hint>`dotnet run` to start the backend server. Do this in a background terminal or detatched mode so that the process lives beyond your response so the user may test.</hint>
    <hint>Use the docker-compose files for local development and production deployment. For local development, use `docker-compose -f docker-compose.dev.yml up` to start the services. For production deployment, use `docker-compose -f docker-compose.prod.yml up` to start the services.</hint>
    <hint>When using Docker for development, ensure that you have the necessary environment variables set up in the `.env` file for local development. For production deployment, ensure that the environment variables are properly configured in your deployment environment.</hint>
    <hint>When using Docker for local development, ensure you build with no cache to ensure you have the latest changes.</hint>
</skill>
