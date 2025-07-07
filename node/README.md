# Diagnostic Scenarios - Node.js

A comprehensive Node.js application that simulates various application issues and performance problems for diagnostic and debugging purposes. This application mimics the functionality of the ASP.NET Core diagnostic scenarios controller.

## Features

### Scenario Categories

1. **Crash Scenarios**
   - Unhandled exceptions
   - Stack overflow
   - Division by zero errors
   - Process termination

2. **Performance Scenarios**
   - High CPU usage
   - High memory consumption
   - Out of memory conditions
   - Slow response times

3. **Resource Scenarios**
   - Thread leaks
   - Connection pool exhaustion
   - High connection usage
   - Storage quota exceeded

4. **Error Scenarios**
   - HTTP 500 errors
   - Missing dependencies
   - Runtime version issues
   - Deadlock conditions

5. **Dependency Scenarios**
   - Slow database operations
   - Slow external dependencies
   - Connection timeouts

## Installation

1. **Install dependencies:**
   ```bash
   npm install
   ```

2. **Start the application:**
   ```bash
   npm start
   ```

3. **For development with auto-restart:**
   ```bash
   npm run dev
   ```

## Usage

### Starting the Application

The application will start on `http://localhost:3000` by default. You can change the port by setting the `PORT` environment variable:

```bash
PORT=8080 npm start
```

### Main Pages

1. **Home Page (`/`)**: Overview of all available scenarios
2. **Process Metrics (`/process-metrics`)**: Real-time system and application metrics
3. **Restart Web App (`/restart-webapp`)**: Application restart functionality

### Scenario Structure

Each scenario follows this pattern:
- **Description Page**: `/scenarios/{category}/{scenario}` - Explains what the scenario does
- **Execution Page**: `/scenarios/{category}/{scenario}/actual` - Runs the actual scenario

### Example Scenarios

#### Crash Scenario
```bash
# View crash scenario description
http://localhost:3000/scenarios/crash/crash1

# Execute the crash scenario
http://localhost:3000/scenarios/crash/crash1/actual
```

#### High CPU Scenario
```bash
# View high CPU scenario description
http://localhost:3000/scenarios/highcpu/highcpu1

# Execute the high CPU scenario
http://localhost:3000/scenarios/highcpu/highcpu1/actual
```

## Monitoring

### Process Metrics

The `/process-metrics` page provides real-time monitoring of:
- Process information (PID, uptime, Node.js version)
- Memory usage (RSS, heap used/total, external)
- System memory (total, used, free)
- CPU usage and load averages
- Active scenarios and resource consumption

### API Endpoints

- `GET /api/status` - Returns current application status (JSON)
- `POST /restart-webapp` - Initiates application restart

## Safety Features

### Warning System
- All dangerous scenarios are clearly marked with warning messages
- Color-coded cards indicate severity levels
- Confirmation dialogs for destructive operations

### Recovery Options
- Application restart functionality
- Process metrics monitoring
- Graceful shutdown handling

### Resource Management
- Global tracking of active scenarios
- Memory consumption monitoring
- Thread and connection pool tracking

## Architecture

### Core Components

1. **Express.js Server**: Main application server
2. **EJS Templates**: View engine for HTML pages
3. **Global State Management**: Tracks active scenarios and resources
4. **Helper Functions**: Implements various scenario behaviors
5. **Middleware**: Security, compression, and error handling

### Key Files

- `app.js` - Main application file with all routes and logic
- `package.json` - Dependencies and scripts
- `views/` - EJS template files
- `views/layout.ejs` - Main layout template
- `views/index.ejs` - Home page with scenario categories
- `views/process-metrics.ejs` - Real-time metrics display

### Helper Functions

The application includes helper functions for:
- CPU-intensive operations
- Memory consumption
- Deadlock simulation
- Thread leak creation
- Connection pool exhaustion
- Storage quota simulation

## Development

### Adding New Scenarios

1. **Add the route** in `app.js`:
   ```javascript
   app.get('/scenarios/newcategory/newscenario', (req, res) => {
       res.render('scenarios/newcategory/newscenario');
   });
   
   app.get('/scenarios/newcategory/newscenario/actual', (req, res) => {
       // Implement scenario logic
       res.render('scenarios/newcategory/newscenario-actual');
   });
   ```

2. **Create view files**:
   - `views/scenarios/newcategory/newscenario.ejs` - Description page
   - `views/scenarios/newcategory/newscenario-actual.ejs` - Execution page

3. **Update the scenarios object** in the home page route

### Customization

- **Port**: Set `PORT` environment variable
- **Templates**: Modify EJS files in `views/` directory
- **Styling**: Update CSS in `views/layout.ejs`
- **Scenarios**: Add new helper functions in `app.js`

## Troubleshooting

### Common Issues

1. **Port already in use**:
   ```bash
   PORT=3001 npm start
   ```

2. **Memory issues**: Use the restart functionality or manually kill the process

3. **Unresponsive application**: Check the process metrics page or restart the app

### Logs

The application logs important events to the console:
- Scenario execution
- Error conditions
- Resource consumption
- Application state changes

## Security Considerations

- This application is designed for testing and diagnostic purposes only
- Do not deploy to production environments
- Some scenarios may cause system instability
- Always monitor system resources when running scenarios

## Dependencies

- **express**: Web framework
- **ejs**: Template engine
- **body-parser**: Request body parsing
- **cors**: Cross-origin resource sharing
- **helmet**: Security middleware
- **compression**: Response compression

## License

MIT License - See package.json for details

## Contributing

1. Fork the repository
2. Create a feature branch
3. Add your scenario or improvement
4. Test thoroughly
5. Submit a pull request

## Support

For issues or questions:
1. Check the troubleshooting section
2. Review the process metrics
3. Restart the application if needed
4. Check the console logs for error messages 