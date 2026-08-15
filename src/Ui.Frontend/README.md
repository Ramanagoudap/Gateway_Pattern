# Ui.Frontend (Angular) - Frontend for Gateway_Pattern

This folder contains a minimal Angular application that calls the ApiGateway. It is a small example app to demonstrate calling /api/v1/orders and /api/v1/notifications through the API Gateway.

Prerequisites
- Node.js (recommended 18+)
- npm (comes with Node.js)
- Optional: @angular/cli to run `ng serve` globally, but the npm scripts use a local CLI installation.

Install and run
1. cd src/Ui.Frontend
2. npm install
3. npm start

This will start the Angular dev server on http://localhost:4200 and proxy API requests starting with /api to the ApiGateway at http://localhost:5172 (see proxy.conf.json). Ensure ApiGateway is running at http://localhost:5172 before calling API endpoints.

Notes
- The proxy configuration forwards any request beginning with /api to the ApiGateway (applicationUrl in ApiGateway/Properties/launchSettings.json is http://localhost:5172).
- The sample component demonstrates GET and POST calls to the gateway.
