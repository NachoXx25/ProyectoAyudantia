# Social network for developers.

The objective of this project is to implement a REST API using ASP.NET Core 9 and PostgreSQL to create a social networking platform for developers. It includes user authentication with JWT, profile management, a post system,and real-time chat using SignalR and WebSockets. The Repository Pattern is implemented to ensure a clean architecture, separation of concerns, and easier maintainability. Cloudinary is used for external media storage, allowing efficient handling of images and other assets. Additionally, the platform integrates Stripe for payment functionalities, enabling users to make transactions. The system is designed for scalability, security, and high performance.

# Installation

For the execution of the project, the following must be installed:
-   [Visual Studio Code 1.89.1+](https://code.visualstudio.com/)
-   [ASP .Net Core 9](https://dotnet.microsoft.com/en-us/download)
-   [PG admin4](https://www.enterprisedb.com/downloads/postgres-postgresql-downloads)
-   [git 2.45.1+](https://git-scm.com/downloads)
-   [Postman](https://www.postman.com/downloads/)
-   [Docker Desktop](https://www.docker.com/get-started/)

Once the above is installed, clone the repository with the command:


# Quick Start
1. Clone this repository to your local machine using CMD:
```bash
    git clone https://github.com/NachoXx25/ProyectoAyudantia.git
```
2. Navigate to the project folder:
```bash
    cd ProyectoAyudantia
```
3. Open the proyect with Visual Studio Code:
```bash
    code .
```
4. Restore the project dependencies in the terminal:
```bash
    dotnet restore
```
5. To execute the proyect use the next command in the VSC terminal:
```bash
    dotnet run
```

# Important
This system requires certain credentials to run properly. These credentials are not included in the GitHub repository due to confidentiality reasons. If you are a stakeholder and need these credentials to run the program, please contact the repository owner or maintainers.

Additionally, you should take the .env.example file and fill in the relevant information for the database connection.

## Example Of Environment Variables 

To run this project, you will need to add the following environment variables to your .env file.

`PostgreSQLConnection`=your_db_connection_type_here

`JWT_SECRET`=your_db_host_here


# Usage
You can test the API using the Postman collection file included in this repository: Proyecto LinkedIn Devs.postman_collection.json.


## Authors Github
- [@Ignacio Valenzuela](https://github.com/NachoXx25)
