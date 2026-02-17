# SekolahFixCRUD - School Registration System Documentation

## 1. Project Profile

- **Project Name**: SekolahFixCRUD
- **Description**: A comprehensive backend system for school registration, allowing students to register profiles, browse courses, and enroll, while administrators manage the academic catalog and approve registrations.
- **Objective**: To provide a clean, maintainable, and professionally structured backend implementation of a school registration process, suitable for educational management.
- **Target Users**: School Administrators and Students.
- **Technology Stack**:
  - **ASP.NET Core 10.0**: High-performance web framework.
  - **SQLite**: Lightweight, file-based relational database.
  - **Entity Framework Core**: ORM for database abstraction.
  - **ASP.NET Identity**: Security framework for users and roles.
  - **JWT (JSON Web Tokens)**: Secure stateless authentication.
  - **AutoMapper**: Object-to-object mapping for DTOs.
  - **Repository Pattern**: Abstraction of data access logic.
  - **ServiceResult Pattern**: Consistent service response handling.

## 2. Architecture & Design Decisions

### Single Project Architecture

The project follows a **Single Project Architecture** to keep deployment and development simple, yet it maintains clear separation of concerns through a logical folder structure. This approach is ideal for small to medium-sized applications.

### Folder Structure

- `Controllers/`: Entry points for API requests. Validates inputs and returns HTTP responses.
- `Entities/`: Domain models and database schema definitions.
- `DTOs/`: Used for data transfer between layers to avoid exposing sensitive entity data.
- `Repositories/`: Handles direct data operations and abstracts complexity from the service layer.
- `Interfaces/`: Defines contracts for services and repositories to ensure loosely coupled components.
- `Services/`: Contains core business logic, validation, and complex workflows.
- `Data/`: Manages database context, migrations, and Fluent API configurations.
- `Common/`: Shared utilities like `ServiceResult<T>` and constants.
- `Middleware/`: Global logic like custom exception handling.

## 3. Class & Method Documentation

### 🔹 AppDbContext

- **Purpose**: Bridges the application with the SQLite database.
- **Key Responsibility**: Configures relationships and handles audit/soft delete automation.
- **OnModelCreating**: Defines relationships (One-to-One, One-to-Many, Many-to-Many) using Fluent API.
- **SaveChangesAsync**: Overridden to automatically set `CreatedAt` and `UpdatedAt` timestamps.

### 🔹 AuthService

- **Purpose**: Manages user security and Identity.
- **RegisterAsync**: Handles student account creation and automatically seeds a pending profile.
- **LoginAsync**: Validates credentials and generates a signed JWT.

### 🔹 StudentService

- **EnrollCoursesAsync**: Business logic for course selection. It enforces a strict check: a student MUST be approved by an Admin before they can enroll in any courses.

### 🔹 AdminService

- **ApproveStudentAsync**: Updates the student status from 'Pending' to 'Approved', unlocking their ability to enroll in courses.
- **DeleteCourseAsync**: Implements soft delete – it marks a course as deleted without removing it from the database physically.

## 4. Database Documentation

### Entity Relationships

- **Student ↔ User (One-to-One)**: Each student profile is tied to a specific Identity user.
- **Teacher → Course (One-to-Many)**: A teacher can handle multiple courses.
- **Student ↔ Course (Many-to-Many)**: Students can enroll in multiple courses, and courses have many students. This is managed via the `StudentCourse` junction table.

### Soft Delete & Audit logic

- **IsDeleted**: Entities inheriting from `BaseEntity` use this flag. A global query filter in `AppDbContext` ensures that "deleted" records are hidden by default.
- **Audit Fields**: `CreatedAt` and `UpdatedAt` are managed by the `DbContext` interceptor, ensuring data integrity without manual code in every service.

## 5. Security & JWT

The system uses **Role-Based Authorization**.

1. **Admin**: Has full CRUD access to academic data and the power to approve/reject students.
2. **Student**: Limited to profile management and course enrollment.

**JWT Flow**:

1. User logs in.
2. Server validates and returns a token including the User's Role as a claim.
3. Client sends the token in the `Authorization: Bearer <Token>` header for subsequent requests.

## 6. API Documentation

### Auth Endpoints

- `POST /api/auth/register`: Create a new student account.
- `POST /api/auth/login`: Authenticate and receive a JWT.

### Student Endpoints

- `GET /api/student/profile`: Retrieve personal profile details.
- `POST /api/student/enroll`: Enroll in a list of course IDs (Requires Approval).

### Admin Endpoints

- `POST /api/admin/student/approve/{id}`: Approve a student's registration.
- `GET /api/admin/courses`: List all courses (Paginated).

---

> [!TIP]
> **Export to PDF**: You can export this documentation to a professional PDF using the "Markdown PDF" extension in VS Code or any standard Markdown-to-PDF tool.
