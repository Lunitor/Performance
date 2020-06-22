'use strict';
var path = require('path');
var express = require('express');
const { createProxyMiddleware } = require('http-proxy-middleware');

var app = express();

app.use('/sensorreadings', createProxyMiddleware({ target: 'http://localhost:55800', changeOrigin: true }));

var staticPath = path.join(__dirname, '/');
app.use(express.static(staticPath));

// Allows you to set port in the project properties.
app.set('port', process.env.PORT || 3000);

var server = app.listen(app.get('port'), function () {
    console.log('listening');
});