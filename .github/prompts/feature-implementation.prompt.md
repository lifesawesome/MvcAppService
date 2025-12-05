# Feature Implementation Prompt

You are an expert ASP.NET Core MVC developer. I need help implementing a new feature.

## Before You Start

Please gather the following information:
1. What is the feature requirement?
2. Which parts of the application will be affected?
3. Are there any security considerations?
4. What is the expected user flow?
5. Are there any performance requirements?

## Implementation Guidelines

When implementing the feature, please:

### 1. Design Phase
- [ ] Create or update data models with proper validation attributes
- [ ] Design database schema changes if needed
- [ ] Plan the controller actions and routes
- [ ] Design the view models
- [ ] Plan the views and UI components

### 2. Backend Implementation
- [ ] Create/update Entity Framework models
- [ ] Generate and review migrations
- [ ] Implement service layer if complex business logic
- [ ] Create controller actions with proper attributes:
  - [HttpGet], [HttpPost], [HttpPut], [HttpDelete]
  - [ValidateAntiForgeryToken] on state-changing operations
  - [Authorize] for protected endpoints
- [ ] Add proper error handling and logging
- [ ] Use async/await for database operations
- [ ] Implement input validation
- [ ] Add authorization checks

### 3. Frontend Implementation
- [ ] Create/update Razor views using Tag Helpers
- [ ] Implement client-side validation
- [ ] Add proper ARIA attributes for accessibility
- [ ] Ensure responsive design
- [ ] Create partial views for reusable components
- [ ] Add JavaScript for interactive features
- [ ] Style with Bootstrap and custom CSS

### 4. Security Implementation
- [ ] Validate all user inputs
- [ ] Encode all outputs
- [ ] Use parameterized queries (EF Core handles this)
- [ ] Implement proper authorization
- [ ] Add anti-forgery tokens to forms
- [ ] Sanitize file uploads if applicable
- [ ] Log security-relevant events

### 5. Testing
- [ ] Write unit tests for business logic
- [ ] Write integration tests for controllers
- [ ] Test edge cases and error conditions
- [ ] Test with invalid inputs
- [ ] Verify security measures

### 6. Documentation
- [ ] Add XML documentation to public methods
- [ ] Update README if needed
- [ ] Document any configuration changes
- [ ] Add inline comments for complex logic

## Code Quality Standards

Ensure all code follows:
- ✅ Naming conventions (see AspNetCoreInstructions.instructions.md)
- ✅ SOLID principles
- ✅ DRY (Don't Repeat Yourself)
- ✅ Security best practices (see SecurityInstructions.instructions.md)
- ✅ Error handling patterns
- ✅ Async/await patterns
- ✅ Dependency injection

## Example Feature Structure

For a typical CRUD feature, create:

```
Models/
  └── [Feature]Model.cs
ViewModels/
  └── [Feature]ViewModel.cs (if needed)
Controllers/
  └── [Feature]Controller.cs
Views/
  └── [Feature]/
      ├── Index.cshtml
      ├── Details.cshtml
      ├── Create.cshtml
      ├── Edit.cshtml
      └── Delete.cshtml
Services/
  └── [Feature]Service.cs (if business logic is complex)
Data/
  └── Migrations/
      └── [Timestamp]_[MigrationName].cs
```

## Deliverables

Please provide:
1. **Implementation Plan**: Step-by-step plan before coding
2. **Code**: All necessary files with complete implementation
3. **Migration Commands**: EF Core commands to run
4. **Testing Instructions**: How to test the feature
5. **Security Considerations**: Any security notes
6. **Documentation**: Update relevant docs

Let's build this feature the right way! 🚀
