const { Client } = require('pg');
const fs = require('fs');
const path = require('path');

async function runMigrations() {
  const client = new Client({
    host: process.env.DB_HOST || 'localhost',
    port: process.env.DB_PORT || 5432,
    database: process.env.DB_NAME || 'piedb',
    user: process.env.DB_USER || 'pieuser',
    password: process.env.DB_PASSWORD || 'password123'
  });

  try {
    await client.connect();
    console.log('Connected to database');

    const migrationFile = path.join(__dirname, '001_initial_schema.sql');
    const sql = fs.readFileSync(migrationFile, 'utf8');

    await client.query(sql);
    console.log('Migration 001_initial_schema.sql completed successfully');
  } catch (err) {
    console.error('Migration failed:', err);
    process.exit(1);
  } finally {
    await client.end();
  }
}

runMigrations();
