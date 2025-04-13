# VR Test Project

This project is a .NET 8 application that processes and saves data from files into a PostgreSQL database. It includes services for file monitoring, file parsing, data validation, and data saving.

## Prerequisites

- .NET 8 SDK
- PostgreSQL database v.17
- Visual Studio 2022

## Setup

1. **Clone the repository:**

```bash
git clone https://github.com/sashakoval/VR-Test.git
cd VR-TEST
```


2. **Configure the database connection:**

    Update the `appsettings.json` file with your PostgreSQL connection string:
   
```bash
{
  "ConnectionStrings": {
    "PostgreSQLConnectionStrings": "Host=your_host;Database=your_db;Username=your_user;Password=your_password"
  },
  "FileMonitorOptions": {
    "DirectoryPath": "path_to_monitor"
  }
}
```

3. **Run database migrations:**

    Open a Package Manager Console and run:

```bash
    Update-Database
```

4. **Build the project:**

    Open the solution in Visual Studio 2022 and build the project.

## Running the Application

1. **Start the application:**

    You can run the application from Visual Studio by pressing `F5` or using the following command in the terminal:
  dotnet run --project VR
    
    
2. **Generate test data:**

    You can generate test data using the `DataGenerator` project. Run the following command in the terminal:

```bash
    dotnet run --project DataGenerator
```

    This will create a `datafile.txt` in the `generated_data` directory.

3. **Process the generated data:**
    Move generated file to specified directory in `appsettings.json`
    The application will monitor the specified directory for new files and process them automatically.

    
