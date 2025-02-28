# TogetherNotes API

**TogetherNotes** is the backend API developed using **.NET** and built with **Visual Studio 2022** to facilitate the connection between the **TogetherNotes** Android app, Windows Forms app, and the database. This API manages users, performances, contracts, ratings, and communication between musicians and venues like bars and restaurants, ensuring smooth interaction across platforms.

## Features

- **User Management**: Handles the registration, authentication, and management of various user types (musicians, venues, and administrators).
- **Performance Management**: Allows creation, modification, and scheduling of performances between musicians and venues.
- **Contract Management**: Manages contracts between musicians and venues, including performance details, ratings, and feedback.
- **Messaging System**: Provides real-time communication between musicians and venues via messaging.
- **Rating System**: Enables mutual ratings between musicians and venues, ensuring transparency and quality.
- **Admin Features**: Provides admin functionalities for managing users, performances, and platform oversight.

### Technologies Used

- **.NET Framework**: The framework used to build the API, ensuring performance and robustness.
- **C#**: The primary programming language for the API.
- **Entity Framework**: Used for managing database interactions.
- **JWT Authentication**: For secure user authentication using JSON Web Tokens.
- **SignalR**: For real-time communication (e.g., chat or notifications).

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/TogetherNotes/api.git
   ```

2. Open the project in **Visual Studio 2022**.

3. Restore the NuGet packages to set up dependencies.

4. Build the solution and run the API directly within **Visual Studio**.

### Contributing

Feel free to fork the repository, submit issues, or create pull requests to contribute to the project. We welcome any improvements or suggestions!

### License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
