﻿import http from 'k6/http';

export const options  = {
  vus: 10,
  duration: '30s'  
};

export default function() {
    http.post('http://localhost:8082');
    http.post('http://localhost:8081');
}