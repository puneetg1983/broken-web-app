const fs = require('fs');
const path = require('path');

// List of all scenario views (from app.js routes)
const scenarioViews = [
  // Crash
  'scenarios/crash/crash1',
  'scenarios/crash/crash1-actual',
  'scenarios/crash/crash2',
  'scenarios/crash/crash2-actual',
  'scenarios/crash/stackoverflow1',
  'scenarios/crash/stackoverflow1-actual',
  'scenarios/crash/unhandledexception1',
  'scenarios/crash/unhandledexception1-actual',
  // High CPU
  'scenarios/highcpu/highcpu1',
  'scenarios/highcpu/highcpu1-actual',
  'scenarios/highcpu/highcpu2',
  'scenarios/highcpu/highcpu2-actual',
  'scenarios/highcpu/highcpu3',
  'scenarios/highcpu/highcpu3-actual',
  // High Memory
  'scenarios/highmemory/highmemory1',
  'scenarios/highmemory/highmemory1-actual',
  'scenarios/highmemory/highmemory2',
  'scenarios/highmemory/highmemory2-actual',
  'scenarios/highmemory/highmemory3',
  'scenarios/highmemory/highmemory3-actual',
  // High Connections
  'scenarios/highconnections/highconnections1',
  'scenarios/highconnections/highconnections1-actual',
  // Deadlock
  'scenarios/deadlock/deadlock1',
  'scenarios/deadlock/deadlock1-actual',
  // HTTP 500
  'scenarios/http500/http500_1',
  'scenarios/http500/http500_1-actual',
  'scenarios/http500/http500_2',
  'scenarios/http500/http500_2-actual',
  'scenarios/http500/http500_3',
  'scenarios/http500/http500_3-actual',
  'scenarios/http500/http500_4',
  'scenarios/http500/http500_4-actual',
  // Slow Response
  'scenarios/slowresponse/slowresponse1',
  'scenarios/slowresponse/slowresponse1-actual',
  'scenarios/slowresponse/slowresponse2',
  'scenarios/slowresponse/slowresponse2-actual',
  'scenarios/slowresponse/slowresponse3',
  'scenarios/slowresponse/slowresponse3-actual',
  // Slow Database
  'scenarios/slowdatabase/slowdatabase1',
  'scenarios/slowdatabase/slowdatabase1-actual',
  'scenarios/slowdatabase/slowdatabase2',
  'scenarios/slowdatabase/slowdatabase2-actual',
  // Slow Dependency
  'scenarios/slowdependency/slowdependency1',
  'scenarios/slowdependency/slowdependency1-actual',
  'scenarios/slowdependency/slowdependency2',
  'scenarios/slowdependency/slowdependency2-actual',
  // Missing Dependency
  'scenarios/missingdependency/missingdependency1',
  // Out of Memory
  'scenarios/outofmemory/outofmemory1',
  'scenarios/outofmemory/outofmemory1-actual',
  // Thread Leak
  'scenarios/threadleak/threadleak1',
  'scenarios/threadleak/threadleak1-actual',
  // Connection Pool
  'scenarios/connectionpool/connectionpool1',
  'scenarios/connectionpool/connectionpool1-actual',
  'scenarios/connectionpool/connectionpool2',
  'scenarios/connectionpool/connectionpool2-actual',
  'scenarios/connectionpool/connectionpool3',
  'scenarios/connectionpool/connectionpool3-actual',
  // Storage Quota
  'scenarios/storagequota/storagequota1',
  'scenarios/storagequota/storagequota1-actual',
  'scenarios/storagequota/cleanup',
  // Runtime Version
  'scenarios/runtimeversion/runtimeversion1',
  'scenarios/runtimeversion/runtimeversion1-actual',
];

const viewsDir = path.join(__dirname, '../views');

scenarioViews.forEach(viewPath => {
  const filePath = path.join(viewsDir, `${viewPath}.ejs`);
  const dir = path.dirname(filePath);
  if (!fs.existsSync(dir)) {
    fs.mkdirSync(dir, { recursive: true });
  }
  let content = `<h1>${viewPath.replace(/scenarios\//, '').replace(/\//g, ' - ')}</h1>\n<p>This is a placeholder for the ${viewPath} page.</p>\n`;
  // If this is not an -actual page, add a link to the /actual page (not -actual)
  if (!viewPath.endsWith('-actual')) {
    const actualPath = viewPath + '-actual';
    if (scenarioViews.includes(actualPath)) {
      // The correct way: replace the last part after the last / with /actual
      const actualUrlFinal = '/' + viewPath.replace(/\//g, '/').replace(/([^/]+)$/, 'actual');
      content += `<p><a href="${actualUrlFinal}">Go to actual scenario &rarr;</a></p>\n`;
    }
  }
  if (!fs.existsSync(filePath)) {
    fs.writeFileSync(filePath, content);
    console.log('Created:', filePath);
  }
});

console.log('All missing scenario EJS files have been created (if they were missing).'); 