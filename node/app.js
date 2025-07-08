const express = require('express');
const path = require('path');
const bodyParser = require('body-parser');
const cors = require('cors');
const helmet = require('helmet');
const compression = require('compression');
const expressLayouts = require('express-ejs-layouts');

const app = express();
const PORT = process.env.PORT || 3000;

// Middleware
app.use(helmet({
    contentSecurityPolicy: {
        directives: {
            defaultSrc: ["'self'"],
            styleSrc: ["'self'", "'unsafe-inline'", "https://cdn.jsdelivr.net", "https://cdnjs.cloudflare.com"],
            scriptSrc: ["'self'", "'unsafe-inline'", "https://cdn.jsdelivr.net"],
            scriptSrcElem: ["'self'", "'unsafe-inline'", "https://cdn.jsdelivr.net"],
            fontSrc: ["'self'", "https://cdn.jsdelivr.net", "https://cdnjs.cloudflare.com"],
            imgSrc: ["'self'", "data:", "https:"],
            connectSrc: ["'self'"],
            frameSrc: ["'self'"],
            objectSrc: ["'none'"],
            mediaSrc: ["'self'"],
            frameAncestors: ["'self'"]
        }
    }
}));
app.use(compression());
app.use(cors());
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));
app.use(expressLayouts);
app.set('layout', 'layout');

// Set view engine
app.set('view engine', 'ejs');
app.set('views', path.join(__dirname, 'views'));

// Static files
app.use(express.static(path.join(__dirname, 'public')));

// Global variables to track scenarios
global.activeThreads = new Set();
global.memoryConsumers = [];
global.connectionPool = [];
global.storageUsage = 0;

// Helper functions
const helpers = {
    // CPU intensive operations
    runCpuIntensiveOperations: () => {
        const cpuWorkers = [];
        const cpuCount = require('os').cpus().length;
        
        for (let i = 0; i < cpuCount * 2; i++) {
            const worker = setInterval(() => {
                let result = 0;
                for (let j = 0; j < 1000000; j++) {
                    result += Math.sqrt(j) * Math.sin(j) * Math.cos(j) * Math.tan(j);
                    result = Math.pow(result, 2) + Math.log(Math.abs(result) + 1);
                    if (result > 1000000) {
                        result = Math.sqrt(result);
                    }
                }
            }, 100);
            
            cpuWorkers.push(worker);
            global.activeThreads.add(worker);
        }
        
        // Stop after 60 seconds
        setTimeout(() => {
            cpuWorkers.forEach(worker => {
                clearInterval(worker);
                global.activeThreads.delete(worker);
            });
        }, 60000);
    },

    // Memory consumption
    consumeMemory: () => {
        const memoryChunks = [];
        try {
            while (true) {
                const chunk = Buffer.alloc(1024 * 1024); // 1MB chunks
                memoryChunks.push(chunk);
                global.memoryConsumers.push(chunk);
                // Small delay to prevent immediate crash
                if (memoryChunks.length % 10 === 0) {
                    // Force garbage collection if available
                    if (global.gc) {
                        global.gc();
                    }
                }
            }
        } catch (error) {
            console.log('Memory consumption stopped:', error.message);
        }
    },

    // Consume all memory (more aggressive)
    consumeAllMemory: () => {
        const memoryChunks = [];
        try {
            while (true) {
                const chunk = Buffer.alloc(1024 * 1024 * 100); // 100MB chunks
                memoryChunks.push(chunk);
                global.memoryConsumers.push(chunk);
            }
        } catch (error) {
            console.log('All memory consumed:', error.message);
        }
    },

    // Create deadlock simulation
    createDeadlock: () => {
        const lock1 = {};
        const lock2 = {};
        
        const thread1 = setInterval(() => {
            const lock1Acquired = Math.random() > 0.5;
            if (lock1Acquired) {
                setTimeout(() => {
                    const lock2Acquired = Math.random() > 0.5;
                    if (lock2Acquired) {
                        // Simulate deadlock
                        console.log('Deadlock condition created');
                    }
                }, 1000);
            }
        }, 100);
        
        const thread2 = setInterval(() => {
            const lock2Acquired = Math.random() > 0.5;
            if (lock2Acquired) {
                setTimeout(() => {
                    const lock1Acquired = Math.random() > 0.5;
                    if (lock1Acquired) {
                        // Simulate deadlock
                        console.log('Deadlock condition created');
                    }
                }, 1000);
            }
        }, 100);
        
        global.activeThreads.add(thread1);
        global.activeThreads.add(thread2);
    },

    // Create thread leak
    createThreadLeak: () => {
        for (let i = 0; i < 1000; i++) {
            const thread = setInterval(() => {
                // Thread never exits - this creates a leak
                console.log(`Leaked thread ${i} still running`);
            }, 1000);
            global.activeThreads.add(thread);
        }
    },

    // Simulate connection pool exhaustion
    exhaustConnectionPool: () => {
        for (let i = 0; i < 1000; i++) {
            const connection = {
                id: i,
                createdAt: new Date(),
                status: 'active'
            };
            global.connectionPool.push(connection);
        }
    },

    // Simulate storage quota exceeded
    exceedStorageQuota: () => {
        try {
            while (true) {
                global.storageUsage += 1024 * 1024; // 1MB increments
                // Simulate storage write
                const fs = require('fs');
                const tempFile = path.join(__dirname, 'temp', `storage_${Date.now()}.tmp`);
                fs.writeFileSync(tempFile, Buffer.alloc(1024 * 1024));
            }
        } catch (error) {
            console.log('Storage quota exceeded:', error.message);
        }
    }
};

// Routes

// Home page
app.get('/', (req, res) => {
    res.render('index', {
        title: 'Diagnostic Scenarios - Node.js',
        scenarios: {
            crash: ['Crash1', 'Crash2', 'StackOverflow1', 'UnhandledException1'],
            highCpu: ['HighCpu1', 'HighCpu2', 'HighCpu3'],
            highMemory: ['HighMemory1', 'HighMemory2', 'HighMemory3'],
            highConnections: ['HighConnections1'],
            deadlock: ['Deadlock1'],
            http500: ['Http500_1', 'Http500_2', 'Http500_3', 'Http500_4'],
            slowResponse: ['SlowResponse1', 'SlowResponse2', 'SlowResponse3'],
            slowDatabase: ['SlowDatabase1', 'SlowDatabase2'],
            slowDependency: ['SlowDependency1', 'SlowDependency2'],
            missingDependency: ['MissingDependency1'],
            outOfMemory: ['OutOfMemory1'],
            threadLeak: ['ThreadLeak1'],
            connectionPool: ['ConnectionPool1', 'ConnectionPool2', 'ConnectionPool3'],
            storageQuota: ['StorageQuota1'],
            runtimeVersion: ['RuntimeVersion1']
        }
    });
});

// Process metrics
app.get('/process-metrics', (req, res) => {
    const os = require('os');
    const process = require('process');
    
    const metrics = {
        processId: process.pid,
        uptime: process.uptime(),
        memoryUsage: process.memoryUsage(),
        cpuUsage: process.cpuUsage(),
        platform: process.platform,
        nodeVersion: process.version,
        activeThreads: global.activeThreads.size,
        memoryConsumers: global.memoryConsumers.length,
        connectionPoolSize: global.connectionPool.length,
        storageUsage: global.storageUsage,
        systemMemory: {
            total: os.totalmem(),
            free: os.freemem(),
            used: os.totalmem() - os.freemem()
        },
        systemCpu: os.cpus(),
        loadAverage: os.loadavg()
    };
    
    res.render('process-metrics', { metrics });
});

// API Status endpoint
app.get('/api/status', (req, res) => {
    const process = require('process');
    
    const status = {
        activeThreads: global.activeThreads.size,
        memoryUsage: process.memoryUsage().rss,
        uptime: process.uptime(),
        storageUsage: global.storageUsage
    };
    
    res.json(status);
});

// Health check endpoint for debugging
app.get('/health', (req, res) => {
    res.status(200).json({
        status: 'OK',
        timestamp: new Date().toISOString(),
        uptime: process.uptime(),
        message: 'Application is running'
    });
});

// Restart web app
app.get('/restart-webapp', (req, res) => {
    res.render('restart-webapp');
});

app.post('/restart-webapp', (req, res) => {
    res.json({ message: 'Restart initiated', timestamp: new Date().toISOString() });
    
    // Simulate restart after 2 seconds
    setTimeout(() => {
        process.exit(0);
    }, 2000);
});

// Crash Scenarios
app.get('/scenarios/crash/crash1', (req, res) => {
    res.render('scenarios/crash/crash1');
});

app.get('/scenarios/crash/crash1-actual', (req, res) => {
    // Start a background process that will throw an unhandled exception
    setTimeout(() => {
        throw new Error('This is an unhandled exception that will crash the application.');
    }, 1000);
    
    res.render('scenarios/crash/crash1-actual');
});

app.get('/scenarios/crash/crash2', (req, res) => {
    res.render('scenarios/crash/crash2');
});

app.get('/scenarios/crash/crash2-actual', (req, res) => {
    setTimeout(() => {
        throw new Error('Division by zero crash simulation');
    }, 1000);
    
    res.render('scenarios/crash/crash2-actual');
});

app.get('/scenarios/crash/stackoverflow1', (req, res) => {
    res.render('scenarios/crash/stackoverflow1');
});

app.get('/scenarios/crash/stackoverflow1-actual', (req, res) => {
    // Simulate stack overflow with recursive function
    const causeStackOverflow = () => {
        causeStackOverflow();
    };
    
    setTimeout(() => {
        causeStackOverflow();
    }, 1000);
    
    res.render('scenarios/crash/stackoverflow1-actual');
});

app.get('/scenarios/crash/unhandledexception1', (req, res) => {
    res.render('scenarios/crash/unhandledexception1');
});

app.get('/scenarios/crash/unhandledexception1-actual', (req, res) => {
    setTimeout(() => {
        throw new Error('Unhandled exception simulation');
    }, 1000);
    
    res.render('scenarios/crash/unhandledexception1-actual');
});

// High CPU Scenarios
app.get('/scenarios/highcpu/highcpu1', (req, res) => {
    res.render('scenarios/highcpu/highcpu1');
});

app.get('/scenarios/highcpu/highcpu1-actual', (req, res) => {
    helpers.runCpuIntensiveOperations();
    res.render('scenarios/highcpu/highcpu1-actual');
});

app.get('/scenarios/highcpu/highcpu2', (req, res) => {
    res.render('scenarios/highcpu/highcpu2');
});

app.get('/scenarios/highcpu/highcpu2-actual', (req, res) => {
    helpers.runCpuIntensiveOperations();
    res.render('scenarios/highcpu/highcpu2-actual');
});

app.get('/scenarios/highcpu/highcpu3', (req, res) => {
    res.render('scenarios/highcpu/highcpu3');
});

app.get('/scenarios/highcpu/highcpu3-actual', (req, res) => {
    helpers.runCpuIntensiveOperations();
    res.render('scenarios/highcpu/highcpu3-actual');
});

// High Memory Scenarios
app.get('/scenarios/highmemory/highmemory1', (req, res) => {
    res.render('scenarios/highmemory/highmemory1');
});

app.get('/scenarios/highmemory/highmemory1-actual', (req, res) => {
    helpers.consumeMemory();
    res.render('scenarios/highmemory/highmemory1-actual');
});

app.get('/scenarios/highmemory/highmemory2', (req, res) => {
    res.render('scenarios/highmemory/highmemory2');
});

app.get('/scenarios/highmemory/highmemory2-actual', (req, res) => {
    helpers.consumeMemory();
    res.render('scenarios/highmemory/highmemory2-actual');
});

app.get('/scenarios/highmemory/highmemory3', (req, res) => {
    res.render('scenarios/highmemory/highmemory3');
});

app.get('/scenarios/highmemory/highmemory3-actual', (req, res) => {
    helpers.consumeMemory();
    res.render('scenarios/highmemory/highmemory3-actual');
});

// High Connections Scenarios
app.get('/scenarios/highconnections/highconnections1', (req, res) => {
    res.render('scenarios/highconnections/highconnections1');
});

app.get('/scenarios/highconnections/highconnections1-actual', (req, res) => {
    helpers.exhaustConnectionPool();
    res.render('scenarios/highconnections/highconnections1-actual');
});

// Deadlock Scenarios
app.get('/scenarios/deadlock/deadlock1', (req, res) => {
    res.render('scenarios/deadlock/deadlock1');
});

app.get('/scenarios/deadlock/deadlock1-actual', (req, res) => {
    helpers.createDeadlock();
    res.render('scenarios/deadlock/deadlock1-actual');
});

// HTTP 500 Scenarios
app.get('/scenarios/http500/http500_1', (req, res) => {
    res.render('scenarios/http500/http500_1');
});

app.get('/scenarios/http500/http500_1-actual', (req, res) => {
    throw new Error('Simulated HTTP 500 error');
});

app.get('/scenarios/http500/http500_2', (req, res) => {
    res.render('scenarios/http500/http500_2');
});

app.get('/scenarios/http500/http500_2-actual', (req, res) => {
    throw new Error('Simulated HTTP 500 error - Argument Exception');
});

app.get('/scenarios/http500/http500_3', (req, res) => {
    res.render('scenarios/http500/http500_3');
});

app.get('/scenarios/http500/http500_3-actual', (req, res) => {
    throw new Error('Simulated HTTP 500 error - Null Reference');
});

app.get('/scenarios/http500/http500_4', (req, res) => {
    res.render('scenarios/http500/http500_4');
});

app.get('/scenarios/http500/http500_4-actual', (req, res) => {
    throw new Error('Simulated HTTP 500 error - Index Out of Range');
});

// Slow Response Scenarios
app.get('/scenarios/slowresponse/slowresponse1', (req, res) => {
    res.render('scenarios/slowresponse/slowresponse1');
});

app.get('/scenarios/slowresponse/slowresponse1-actual', (req, res) => {
    setTimeout(() => {
        res.render('scenarios/slowresponse/slowresponse1-actual');
    }, 30000); // 30 seconds
});

app.get('/scenarios/slowresponse/slowresponse2', (req, res) => {
    res.render('scenarios/slowresponse/slowresponse2');
});

app.get('/scenarios/slowresponse/slowresponse2-actual', (req, res) => {
    setTimeout(() => {
        res.render('scenarios/slowresponse/slowresponse2-actual');
    }, 60000); // 60 seconds
});

app.get('/scenarios/slowresponse/slowresponse3', (req, res) => {
    res.render('scenarios/slowresponse/slowresponse3');
});

app.get('/scenarios/slowresponse/slowresponse3-actual', (req, res) => {
    setTimeout(() => {
        res.render('scenarios/slowresponse/slowresponse3-actual');
    }, 120000); // 2 minutes
});

// Slow Database Scenarios
app.get('/scenarios/slowdatabase/slowdatabase1', (req, res) => {
    res.render('scenarios/slowdatabase/slowdatabase1');
});

app.get('/scenarios/slowdatabase/slowdatabase1-actual', (req, res) => {
    setTimeout(() => {
        res.render('scenarios/slowdatabase/slowdatabase1-actual');
    }, 45000); // 45 seconds
});

app.get('/scenarios/slowdatabase/slowdatabase2', (req, res) => {
    res.render('scenarios/slowdatabase/slowdatabase2');
});

app.get('/scenarios/slowdatabase/slowdatabase2-actual', (req, res) => {
    setTimeout(() => {
        res.render('scenarios/slowdatabase/slowdatabase2-actual');
    }, 90000); // 90 seconds
});

// Slow Dependency Scenarios
app.get('/scenarios/slowdependency/slowdependency1', (req, res) => {
    res.render('scenarios/slowdependency/slowdependency1');
});

app.get('/scenarios/slowdependency/slowdependency1-actual', (req, res) => {
    setTimeout(() => {
        res.render('scenarios/slowdependency/slowdependency1-actual');
    }, 40000); // 40 seconds
});

app.get('/scenarios/slowdependency/slowdependency2', (req, res) => {
    res.render('scenarios/slowdependency/slowdependency2');
});

app.get('/scenarios/slowdependency/slowdependency2-actual', (req, res) => {
    setTimeout(() => {
        res.render('scenarios/slowdependency/slowdependency2-actual');
    }, 80000); // 80 seconds
});

// Missing Dependency Scenarios
app.get('/scenarios/missingdependency/missingdependency1', (req, res) => {
    res.render('scenarios/missingdependency/missingdependency1');
});

app.get('/scenarios/missingdependency/missingdependency1-actual', (req, res) => {
    throw new Error('Required dependency not found');
});

// Out of Memory Scenarios
app.get('/scenarios/outofmemory/outofmemory1', (req, res) => {
    res.render('scenarios/outofmemory/outofmemory1');
});

app.get('/scenarios/outofmemory/outofmemory1-actual', (req, res) => {
    helpers.consumeAllMemory();
    res.render('scenarios/outofmemory/outofmemory1-actual');
});

// Thread Leak Scenarios
app.get('/scenarios/threadleak/threadleak1', (req, res) => {
    res.render('scenarios/threadleak/threadleak1');
});

app.get('/scenarios/threadleak/threadleak1-actual', (req, res) => {
    helpers.createThreadLeak();
    res.render('scenarios/threadleak/threadleak1-actual');
});

// Connection Pool Scenarios
app.get('/scenarios/connectionpool/connectionpool1', (req, res) => {
    res.render('scenarios/connectionpool/connectionpool1');
});

app.get('/scenarios/connectionpool/connectionpool1-actual', (req, res) => {
    helpers.exhaustConnectionPool();
    res.render('scenarios/connectionpool/connectionpool1-actual');
});

app.get('/scenarios/connectionpool/connectionpool2', (req, res) => {
    res.render('scenarios/connectionpool/connectionpool2');
});

app.get('/scenarios/connectionpool/connectionpool2-actual', (req, res) => {
    helpers.exhaustConnectionPool();
    res.render('scenarios/connectionpool/connectionpool2-actual');
});

app.get('/scenarios/connectionpool/connectionpool3', (req, res) => {
    res.render('scenarios/connectionpool/connectionpool3');
});

app.get('/scenarios/connectionpool/connectionpool3-actual', (req, res) => {
    helpers.exhaustConnectionPool();
    res.render('scenarios/connectionpool/connectionpool3-actual');
});

// Storage Quota Scenarios
app.get('/scenarios/storagequota/storagequota1', (req, res) => {
    res.render('scenarios/storagequota/storagequota1');
});

app.get('/scenarios/storagequota/storagequota1-actual', (req, res) => {
    helpers.exceedStorageQuota();
    res.render('scenarios/storagequota/storagequota1-actual');
});

app.get('/scenarios/storagequota/cleanup', (req, res) => {
    // Clean up storage
    global.storageUsage = 0;
    const fs = require('fs');
    const tempDir = path.join(__dirname, 'temp');
    if (fs.existsSync(tempDir)) {
        fs.rmSync(tempDir, { recursive: true, force: true });
    }
    res.render('scenarios/storagequota/cleanup');
});

// Runtime Version Scenarios
app.get('/scenarios/runtimeversion/runtimeversion1', (req, res) => {
    res.render('scenarios/runtimeversion/runtimeversion1');
});

app.get('/scenarios/runtimeversion/runtimeversion1-actual', (req, res) => {
    res.render('scenarios/runtimeversion/runtimeversion1-actual', {
        nodeVersion: process.version,
        platform: process.platform,
        arch: process.arch
    });
});

// Error handling middleware
app.use((err, req, res, next) => {
    console.error('Error:', err);
    res.status(500).render('error', {
        error: err,
        message: err.message,
        stack: process.env.NODE_ENV === 'development' ? err.stack : undefined
    });
});

// 404 handler
app.use((req, res) => {
    res.status(404).render('error', {
        error: { status: 404 },
        message: 'Page not found'
    });
});

// Start server
// Only listen if not running under Azure App Service with IISNode
if (!process.env.WEBSITE_NODE_DEFAULT_VERSION) {
    app.listen(PORT, () => {
        console.log(`Diagnostic Scenarios Node.js app listening on port ${PORT}`);
        console.log(`Visit http://localhost:${PORT} to see the scenarios`);
    });
} else {
    console.log('Running under Azure App Service with IISNode, server startup handled by IIS');
}

// Graceful shutdown
process.on('SIGTERM', () => {
    console.log('SIGTERM received, shutting down gracefully');
    process.exit(0);
});

process.on('SIGINT', () => {
    console.log('SIGINT received, shutting down gracefully');
    process.exit(0);
});

// Export the app for IISNode
module.exports = app;