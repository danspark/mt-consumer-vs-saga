import http from 'k6/http';

export const options  = {
  vus: 10,
  duration: '5m'
};

export default function() {
    http.post('http://localhost:8082');
    http.post('http://localhost:8081');
}