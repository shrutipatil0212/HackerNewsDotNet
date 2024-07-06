Hacker News API created in .NET

Using the Hacker News - https://github.com/HackerNews/API, created a API that allows users to fetch the newest stories from the
feed. https://hacker-news.firebaseio.com/v0/newstories.json?print=pretty Used C# .NET Core back-end and restful API.

## Features

1. Usage of dependency injection
2. Caching of the newest stories
3. Automated tests for the code

## Installation

Download the zip folder of the repository or kindly clone it using the URL - https://github.com/shrutipatil0212/HackerNewsDotNet.git

## Usage

Once the folder is available locally, to run a .NET solution using the command line, follow these steps:

1. Install .NET SDK
Ensure that the .NET SDK is installed on your machine. You can download it from the .NET download page.

2. Open Command Prompt or Terminal
Open your command prompt (Windows)

3. Navigate to Your Project Directory
Use the cd command to navigate to the directory where your .NET solution (.sln file) is located. For example: cd path\to\your\project

4. Restore Dependencies
Restore the dependencies for your project by running:dotnet restore

5. Build the Solution
Build your solution using the following command: dotnet build

6. Run the Application
To run your application, specify the project file (.csproj) to run. For example: dotnet run --project youpath\HackerNewsDotNet-master\HackerNewsAPI\HackerNewsAPI.csproj

Here’s a sequence of commands assuming your project is located in C:\Projects\MyApp and the main project file is MyApp.csproj:

cd C:\Projects\MyApp
dotnet restore
dotnet build
dotnet run --project youpath\HackerNewsDotNet-master\HackerNewsAPI\HackerNewsAPI.csproj

Running Tests
If you want to run tests, navigate to the test project directory and use the following command:
dotnet test

## Contributing

Once ran successfully, you will see Swagger window open up. Click on 'Try It Out' anf then Execute. You will get the result with newest Stories fetched
